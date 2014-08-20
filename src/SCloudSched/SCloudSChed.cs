// (C) Copyright 2013 by Andrew Nicholas
//
// This file is part of SCloudSChed.
//
// SCloudSChed is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCloudSChed is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCloudSChed.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCloudSChed
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Microsoft.Office.Interop.Excel;
    using System.Reflection;
    using System.Collections.Generic;
    using SCaddins.Common;
    
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private SortableBindingList<RevisionItem> revisions;

        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
          ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            this.revisions = new SortableBindingList<RevisionItem>();
            this.GetRevisions(doc, ref this.revisions);
            Form1 form = new Form1(doc, this.revisions);
            form.Show();
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        private void GetRevisions(Document doc, ref SortableBindingList<RevisionItem> revisions)
        {
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc);
            a.OfCategory(BuiltInCategory.OST_Revisions);
            foreach (Element e in a) {
                int seq = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger();
                string date = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString();
                int issued = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_ISSUED).AsInteger();
                string description = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DESCRIPTION).AsString();
                revisions.Add(new RevisionItem(date, description, issued == 1, seq));
            }
        }
    }

    public class RevisionItem
    {
        private bool export;
        
        public bool Export
        {
            get { return this.export; }
            set { this.export = value; }
        }

        private string description;
        
        public string Description
        {
            get { return this.description; }
        }

        private string date;
        
        public string Date
        {
            get { return this.date; }
        }

        private bool isIssued;
        
        public bool IsIssued
        {
            get { return this.isIssued; }
        }

        private int sequence;
        
        public int Sequence
        {
            get { return this.sequence; }
        }

        public RevisionItem(string date, string description, bool isIssued, int sequence)
        {
            this.description = description;
            this.date = date;
            this.isIssued = isIssued;
            this.sequence = sequence;
        }
    }

    public class SCloudSched
    {
        private static string getParamaterAsString(Element revCloud, BuiltInParameter b)
        {
            string result = string.Empty;
            try {
                result += revCloud.get_Parameter(b).AsString();
            } catch {
            }
            return result;
        }

        /// <summary>
        /// Write to an excell worksheet
        /// from here:
        /// http://www.clear-lines.com/blog/post/Write-data-to-an-Excel-worksheet-with-C-fast.aspx
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="worksheet"></param>
        private static void WriteArray(string[,] data, int rows, int columns, Worksheet worksheet)
        {
            if (worksheet != null) {
                var startCell = worksheet.Cells[1, 1] as Range;
                var endCell = worksheet.Cells[rows, columns] as Range;
                var writeRange = worksheet.Range[startCell, endCell];
                writeRange.Value2 = data;
            }
        }

        public static void exportCloudInfo(Document doc, Dictionary<string, RevisionItem> dictionary)
        {
            const string exportFilename = @"C:\Temp\SClouds";
            Microsoft.Office.Interop.Excel.Application excelApp;
            Worksheet excelWorksheet;
            Workbook excelWorkbook;

            excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = false;
            excelWorkbook = (Workbook)(excelApp.Workbooks.Add(Missing.Value));
            excelWorksheet = (Worksheet)excelWorkbook.ActiveSheet;

            int cloudNumber = 0;
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc);
            a.OfCategory(BuiltInCategory.OST_RevisionClouds);

            string[,] data = new string[a.ToElements().Count + 1, 7];
            data[0, 0] = "Sheet Number";
            data[0, 1] = "Revision Number";
            data[0, 2] = "Sheet Name";
            data[0, 3] = "Revision Mark";
            data[0, 4] = "Revision Comment";
            data[0, 5] = "Revision Date";
            data[0, 6] = "Revision Description";

            foreach (Element revCloud in a) {
                string description = getParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_DESCRIPTION);
                string date = getParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_DATE);
                RevisionItem revItem;
                if (dictionary.TryGetValue(date + description, out revItem)) {
                    if (revItem.Export) {
                        cloudNumber++;
                        string viewName = string.Empty;
                        try {
                            View view = (View)doc.GetElement(revCloud.OwnerViewId);
                            viewName = view.ViewName;
                            if (view.ViewType == ViewType.DrawingSheet) {
                                data[cloudNumber, 0] = ((ViewSheet)view).SheetNumber.ToString();
                            } else {
                                data[cloudNumber, 0] = view.get_Parameter(BuiltInParameter.VIEWPORT_SHEET_NUMBER).AsString();
                            }
                        } catch {
                        }
                        data[cloudNumber, 1] = getParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_NUM);
                        data[cloudNumber, 2] = viewName;
                        data[cloudNumber, 3] = getParamaterAsString(revCloud, BuiltInParameter.ALL_MODEL_MARK);
                        data[cloudNumber, 4] = getParamaterAsString(revCloud, BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                        data[cloudNumber, 5] = getParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_DATE);
                        data[cloudNumber, 6] = getParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_DESCRIPTION);
                    }
                }
            }

            if (cloudNumber < 1) {
                TaskDialog.Show("WARNING", "no clouds found to export");
            } else {
                WriteArray(data, cloudNumber, 7, excelWorksheet);
                TaskDialog.Show("Finished", cloudNumber + @" revision clouds sheduled in the file " + exportFilename);
                excelWorkbook.SaveAs(exportFilename, XlFileFormat.xlWorkbookNormal);
                excelWorkbook.Close();
            }
        }
    }
}