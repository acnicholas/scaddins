// (C) Copyright 2013-2014 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCloudSChed
{
    using System.Collections.Generic;
    using System.Reflection;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Microsoft.Office.Interop.Excel;

    public static class SCloudScheduler
    {     
        public static void ExportCloudInfo(Document doc, Dictionary<string, RevisionItem> dictionary)
        {
            const string ExportFilename = @"C:\Temp\SClouds";
            Microsoft.Office.Interop.Excel.Application excelApp;
            Worksheet excelWorksheet;
            Workbook excelWorkbook;

            excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = false;
            excelWorkbook = (Workbook)excelApp.Workbooks.Add(Missing.Value);
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
                string description = GetParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_DESCRIPTION);
                string date = GetParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_DATE);
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
                        } catch (Autodesk.Revit.Exceptions.ArgumentNullException e) {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                        }
                        data[cloudNumber, 1] = GetParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_NUM);
                        data[cloudNumber, 2] = viewName;
                        data[cloudNumber, 3] = GetParamaterAsString(revCloud, BuiltInParameter.ALL_MODEL_MARK);
                        data[cloudNumber, 4] = GetParamaterAsString(revCloud, BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                        data[cloudNumber, 5] = GetParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_DATE);
                        data[cloudNumber, 6] = GetParamaterAsString(revCloud, BuiltInParameter.REVISION_CLOUD_REVISION_DESCRIPTION);
                    }
                }
            }

            if (cloudNumber < 1) {
                TaskDialog.Show("WARNING", "no clouds found to export");
            } else {
                WriteArray(data, cloudNumber, 7, excelWorksheet);
                TaskDialog.Show("Finished", cloudNumber + @" revision clouds sheduled in the file " + ExportFilename);
                excelWorkbook.SaveAs(ExportFilename, XlFileFormat.xlWorkbookNormal);
                excelWorkbook.Close();
            }
        }
              
        private static string GetParamaterAsString(Element revCloud, BuiltInParameter b)
        {
            string result = string.Empty;
            try {
                result += revCloud.get_Parameter(b).AsString();
            } catch {
            }
            return result;
        }
   
        /// <summary>
        /// Write to an excel worksheet
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
    }    
}