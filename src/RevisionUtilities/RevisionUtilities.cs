// (C) Copyright 2013-2020 by Andrew Nicholas
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using Autodesk.Revit.DB;
    using Microsoft.Office.Interop.Excel;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public static class Manager
    {
        public static void AssignRevisionToClouds(Document doc, List<RevisionCloudItem> revisionClouds, ElementId cloudId)
        {
            if (doc == null || revisionClouds == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("ERROR", "Could not assign revisions to clouds");
                return;
            }
            if (cloudId == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("ERROR", "Selected cloud is not valid...for some reason");
                return;
            }
            using (var t = new Transaction(doc, "Assign Revisions to Clouds"))
            {
                t.Start();
                foreach (var rc in revisionClouds)
                {
                    rc?.SetCloudId(cloudId);
                }
                t.Commit();
            }
        }

        public static void DeleteRevisionClouds(Document doc, List<RevisionCloudItem> revisionClouds)
        {
            if (doc == null || revisionClouds == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("ERROR", "Could not delete revision clouds");
                return;
            }
            using (var t = new Transaction(doc, "Deleting Revision Clouds"))
            {
                t.Start();
                foreach (RevisionCloudItem rc in revisionClouds)
                {
                    if (rc != null)
                    {
                        doc.Delete(rc.Id);
                    }
                }
                t.Commit();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Justification = "Because.")]
        public static void ExportCloudInfo(Document doc, List<RevisionItem> revisions, string exportFilename)
        {
            if (doc == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("ERROR", "could not export cloud information");
                return;
            }

            var dictionary = new Dictionary<string, RevisionItem>();
            foreach (RevisionItem rev in revisions)
            {
                if (rev != null)
                {
                    string s = rev.Date + rev.Description;
                    if (!dictionary.ContainsKey(s))
                    {
                        dictionary.Add(s, rev);
                    }
                }
            }

            exportFilename = string.IsNullOrEmpty(exportFilename) ? @"C:\Temp\SClouds" : exportFilename;

            var excelApp = new Application { Visible = false };
            var excelWorkbook = excelApp.Workbooks.Add(Missing.Value);
            var excelWorksheet = (Worksheet)excelWorkbook.ActiveSheet;

            int cloudNumber = 0;

            List<RevisionCloudItem> allRevisionClouds = GetRevisionClouds(doc);

            string[,] data = new string[allRevisionClouds.Count + 1, 8];
            data[0, 0] = "Sheet Number";
            data[0, 1] = "Revision Number";
            data[0, 2] = "Sheet Name";
            data[0, 3] = "Revision Mark";
            data[0, 4] = "Revision Comment";
            data[0, 5] = "Revision Date";
            data[0, 6] = "Revision Description";
            data[0, 7] = "GUID";

            foreach (RevisionCloudItem revCloud in allRevisionClouds)
            {
                if (dictionary.ContainsKey(revCloud.Date + revCloud.Description))
                {
                    cloudNumber++;
                    data[cloudNumber, 0] = revCloud.SheetNumber;
                    data[cloudNumber, 1] = revCloud.Revision;
                    data[cloudNumber, 2] = revCloud.SheetName;
                    data[cloudNumber, 3] = revCloud.Mark;
                    data[cloudNumber, 4] = revCloud.Comments;
                    data[cloudNumber, 5] = revCloud.Date;
                    data[cloudNumber, 6] = revCloud.Description;
#if REVIT2024 || REVIT2025
                    data[cloudNumber, 7] = revCloud.Id.Value.ToString(CultureInfo.InvariantCulture);
#else
                    data[cloudNumber, 7] = revCloud.Id.IntegerValue.ToString(CultureInfo.InvariantCulture);
#endif
                }
            }

            if (cloudNumber < 1)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("WARNING", "no clouds found to export");
            }
            else
            {
                WriteArray(data, cloudNumber, 8, excelWorksheet);
                SCaddinsApp.WindowManager.ShowMessageBox("Finished", cloudNumber + @" revision clouds scheduled in the file " + exportFilename);
                excelWorkbook.SaveAs(exportFilename, XlFileFormat.xlWorkbookNormal);
                excelWorkbook.Close();
            }
        }

        public static List<RevisionCloudItem> GetRevisionClouds(Document doc)
        {
            var revisionClouds = new List<RevisionCloudItem>();
            if (doc != null)
            {
                using (FilteredElementCollector a = new FilteredElementCollector(doc))
                {
                    a.OfCategory(BuiltInCategory.OST_RevisionClouds);
                    a.OfClass(typeof(RevisionCloud));
                    foreach (var element in a)
                    {
                        var e = (RevisionCloud)element;
                        revisionClouds.Add(new RevisionCloudItem(doc, e));
                    }
                }
            }
            return revisionClouds;
        }

        public static List<RevisionItem> GetRevisions(Document doc)
        {
            var revisions = new List<RevisionItem>();
            using (var a = new FilteredElementCollector(doc))
            {
                a.OfCategory(BuiltInCategory.OST_Revisions);
                foreach (var element in a)
                {
                    var e = (Revision)element;
                    if (e.IsValidObject)
                    {
                        revisions.Add(new RevisionItem(e));
                    }
                }
            }
            return revisions;
        }

        /// <summary>
        /// Write to an excel worksheet
        /// from here:
        /// http://www.clear-lines.com/blog/post/Write-data-to-an-Excel-worksheet-with-C-fast.aspx
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="worksheet"></param>
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Justification = "Because.")]
        private static void WriteArray(string[,] data, int rows, int columns, Worksheet worksheet)
        {
            if (worksheet == null)
            {
                return;
            }
            var startCell = worksheet.Cells[1, 1] as Range;
            var endCell = worksheet.Cells[rows + 1, columns] as Range;
            var writeRange = worksheet.Range[startCell, endCell];
            writeRange.Value2 = data;
        }
    }
}