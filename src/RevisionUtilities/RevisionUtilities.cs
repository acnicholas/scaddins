// (C) Copyright 2013-2016 by Andrew Nicholas
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

namespace SCaddins.RevisionUtilities
{
    // using System.Collections.Generic;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Microsoft.Office.Interop.Excel;
    using SCaddins.Common;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public static class RevisionUtilities
    {
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Justification = "Because.")]        
        public static void ExportCloudInfo(Document doc, Dictionary<string, RevisionItem> dictionary, string exportFilename)
        {
            string ExportFilename = string.IsNullOrEmpty(exportFilename) ? @"C:\Temp\SClouds" : exportFilename;
            Application excelApp;
            Worksheet excelWorksheet;
            Workbook excelWorkbook;

            excelApp = new Application();
            excelApp.Visible = false;
            excelWorkbook = (Workbook)excelApp.Workbooks.Add(Missing.Value);
            excelWorksheet = (Worksheet)excelWorkbook.ActiveSheet;

            int cloudNumber = 0;
            
            SortableBindingListCollection<RevisionCloudItem> allRevisionClouds = GetRevisionClouds(doc);

            string[,] data = new string[allRevisionClouds.Count + 1, 8]; 
            data[0, 0] = "Sheet Number";
            data[0, 1] = "Revision Number";
            data[0, 2] = "Sheet Name";
            data[0, 3] = "Revision Mark";
            data[0, 4] = "Revision Comment";
            data[0, 5] = "Revision Date";
            data[0, 6] = "Revision Description";
            data[0, 7] = "GUID";

            foreach (RevisionCloudItem revCloud in allRevisionClouds) {
                if (dictionary.ContainsKey(revCloud.Date + revCloud.Description)) {     
                    cloudNumber++; 
                    data[cloudNumber, 0] = revCloud.SheetNumber;
                    data[cloudNumber, 1] = revCloud.Revision;
                    data[cloudNumber, 2] = revCloud.SheetName;
                    data[cloudNumber, 3] = revCloud.Mark;
                    data[cloudNumber, 4] = revCloud.Comments;
                    data[cloudNumber, 5] = revCloud.Date;
                    data[cloudNumber, 6] = revCloud.Description;
                    data[cloudNumber, 7] = revCloud.Id.IntegerValue.ToString();
                }
            }

            if (cloudNumber < 1) {
                TaskDialog.Show("WARNING", "no clouds found to export");
            } else {
                WriteArray(data, cloudNumber, 8, excelWorksheet);
                TaskDialog.Show("Finished", cloudNumber + @" revision clouds sheduled in the file " + ExportFilename);
                excelWorkbook.SaveAs(ExportFilename, XlFileFormat.xlWorkbookNormal);
                excelWorkbook.Close();
            }
        }
        
        
        public static SortableBindingListCollection<RevisionCloudItem> GetRevisionClouds(Document doc)
        {
            var revisionClouds = new SortableBindingListCollection<RevisionCloudItem>();
            if (doc != null) {
                using (FilteredElementCollector a = new FilteredElementCollector(doc)) {
                    a.OfCategory(BuiltInCategory.OST_RevisionClouds);
                    a.OfClass(typeof(RevisionCloud));
                    foreach (RevisionCloud e in a) {
                        revisionClouds.Add(new RevisionCloudItem(doc, e));
                    }
                }
            }
            return revisionClouds;
        }
        
        public static SortableBindingListCollection<RevisionItem> GetRevisions(Document doc)
        {
            var revisions = new SortableBindingListCollection<RevisionItem>();
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc);
            a.OfCategory(BuiltInCategory.OST_Revisions);
            foreach (Revision e in a) {  
                if (e.IsValidObject) {
                    revisions.Add(new RevisionItem(e));
                }
            }
            return revisions;
        }
        
        public static void AssignRevisionToClouds(Document doc, Collection<RevisionCloudItem> revisionClouds)
        {
            var r = new SCaddins.ExportManager.RevisionSelectionDialog(doc);
            var result = r.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK) {      
                return;
            }
            if (r.Id == null) {
                TaskDialog.Show("test", "id is null"); 
                return;
            }
            using (var t = new Transaction(doc, "Assign Revisions to Clouds")) {
                t.Start();
                foreach (RevisionCloudItem rc in revisionClouds) { 
                    if (rc != null) {
                        rc.SetCloudId(r.Id);
                    } 
                }
                t.Commit();
            }
        }
        
         public static void DeleteRevisionClouds(Document doc, Collection<RevisionCloudItem> revisionClouds)
        {
            using (var t = new Transaction(doc, "Deleting Revision Clouds")) {
                t.Start();
                foreach (RevisionCloudItem rc in revisionClouds) { 
                    if (rc != null) {
                        doc.Delete(rc.Id);
                    }
                }
                t.Commit();
            }
        }
        

        public static string GetParameterAsString(Element revCloud, BuiltInParameter builtInParameter)
        {
            var parameter = revCloud.get_Parameter(builtInParameter);
            if (parameter == null) {
                return string.Empty;
            }
            string result = parameter.AsString();
            return string.IsNullOrEmpty(result) ? string.Empty : result;
        }

        /// <summary>
        /// Write to an excel worksheet
        /// from here:
        /// http://www.clear-lines.com/blog/post/Write-data-to-an-Excel-worksheet-with-C-fast.aspx
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="worksheet"></param>
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Justification = "Because.")]
        private static void WriteArray(string[,] data, int rows, int columns, Worksheet worksheet)
        {
            if (worksheet != null) {
                var startCell = worksheet.Cells[1, 1] as Range;
                var endCell = worksheet.Cells[rows + 1, columns] as Range;
                var writeRange = worksheet.Range[startCell, endCell];
                writeRange.Value2 = data;
            }
        }
    }
}
