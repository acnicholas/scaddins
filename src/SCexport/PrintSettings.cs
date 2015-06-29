// (C) Copyright 2012-2014 by Andrew Nicholas
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

namespace SCaddins.SCexport
{
    using System;
    using System.Globalization;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    /// <summary>
    /// Description of PrintSettings.
    /// </summary>
    public static class PrintSettings
    {
        /// <summary>
        /// Return the sheet size as a String.
        /// from http://en.wikipedia.org/wiki/Paper_size
        /// i.e A1, A2....A4
        /// FIXME need to add other sizes.
        /// </summary>
        /// <param name="sheet">The SCexport sheet to query.</param>
        /// <returns>The sheet size as a String.</returns>    
        public static string GetSheetSizeAsString(ExportSheet sheet)
        {
            double[] p = { 1189, 841, 594, 420, 297, 210, 297, 420, 594, 841, 1189 };
            string[] s = { "A0", "A1", "A2", "A3", "A4", "A4P", "A3P", "A2P", "A1P", "A0P" };

            for (int i = 0; i < s.Length; i++) {
                if (CheckSheetSize(sheet.Width, sheet.Height, p[i], p[i + 1])) {
                    return s[i];
                }
            }

            if (CheckSheetSize(sheet.Width, sheet.Height, 1000, 707)) {
                return "B1";
            }

            return Math.Round(sheet.Width).ToString(CultureInfo.InvariantCulture) + "x" +
                Math.Round(sheet.Height).ToString(CultureInfo.InvariantCulture);
        }
        
        /// <summary>
        /// Create a print setting and add it to the current document.
        /// </summary>
        /// <param name="doc">The Revit document to create the print setting in.</param>
        /// <param name="s">The name of the sheet size - A4,A3,A1...</param>
        public static void CreatePrintSetting(Document doc, string s)
        {
            PrintManager pm = doc.PrintManager;
            foreach (PaperSize paperSize in pm.PaperSizes) {
                if (paperSize.Name.Substring(0, 2) == s.Substring(0, 2)) {
                    var t = new Transaction(doc, "Apply print settings");
                    t.Start();
                    var ips = pm.PrintSetup.CurrentPrintSetting;
                    try {
                        ips.PrintParameters.PaperSize = paperSize;
                        ips.PrintParameters.HideCropBoundaries = true;
                    if (s.Length > 2 && !s.Contains("FIT")) {
                        ips.PrintParameters.PageOrientation =
                            PageOrientationType.Portrait;
                    } else {
                        ips.PrintParameters.PageOrientation =
                            PageOrientationType.Landscape;
                    }

                    ips.PrintParameters.HideScopeBoxes = true;
                    ips.PrintParameters.HideReforWorkPlanes = true;
                    #if REVIT2014
                    ips.PrintParameters.HideUnreferencedViewTags = true;
                    #else
                    ips.PrintParameters.HideUnreferencedViewTags = true;
                    #endif
                    if (s.Contains("FIT")) {
                        ips.PrintParameters.ZoomType = ZoomType.FitToPage;
                        ips.PrintParameters.MarginType = MarginType.NoMargin;
                    } else {
                        ips.PrintParameters.ZoomType = ZoomType.Zoom;
                        ips.PrintParameters.Zoom = 100;
                        ips.PrintParameters.PaperPlacement =
                            PaperPlacementType.Margins;
                        ips.PrintParameters.MarginType = MarginType.UserDefined;
                        ips.PrintParameters.UserDefinedMarginX = 0;
                        ips.PrintParameters.UserDefinedMarginY = 0;
                    }

                    pm.PrintSetup.SaveAs("SCX-" + s);
                    t.Commit();
                    } catch {
                        TaskDialog.Show(
                            "SCexport",
                            "Unable to create print setting: " + "SCX-" + s);
                        t.RollBack();
                    }
                }
            }
        }
            
        public static bool ApplyPrintSettings(
                Document doc,
                string size,
                PrintManager pm,
                string printerName)
        {
            PrintSetting ps = PrintSettings.AssignPrintSetting(doc, size);
            
            if (ps == null) {
                return false;
            }

            if (!PrintSettings.SetPrinter(doc, printerName, pm)) {
                return false;
            }

            var t = new Transaction(doc, "Apply print settings");
            t.Start();
            try {
                pm.PrintSetup.CurrentPrintSetting = ps;
                pm.PrintRange = PrintRange.Current;                
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.MarginType = MarginType.NoMargin;
                pm.PrintSetup.InSession.PrintParameters.MarginType = MarginType.NoMargin;
                pm.PrintToFile = false;
                pm.Apply();
                t.Commit();
                return true;
            } catch {
                TaskDialog.Show("SCexport", "cannot apply print settings");
                t.RollBack();
                return false;
            }  
        }
        
        /// <summary>
        /// Apply a print setting to the current document.
        /// </summary>
        /// <param name="doc">The Revit document to create the print setting in.</param>
        /// <param name="vs">The Sheet containing the print setting.</param>
        /// <param name="pm">The Current print manager.</param>
        /// <param name="ext">The file extension to append on the exported file.</param>
        /// <param name="printerName">The name of the printer to print to.</param>
        /// <returns>True if successful.</returns>
        public static bool ApplyPrintSettings(
                Document doc,
                ExportSheet vs,
                PrintManager pm,
                string ext,
                string printerName)
        {
            if (vs.SCPrintSetting == null) {
                return false;
            }

            if (!PrintSettings.SetPrinter(doc, printerName, pm)) {
                return false;
            }

            var t = new Transaction(doc, "Print Pdf");
            t.Start();
            try {
                pm.PrintSetup.CurrentPrintSetting = vs.SCPrintSetting;
                pm.PrintRange = PrintRange.Current;
                pm.PrintToFile = true;
                pm.PrintToFileName = vs.FullExportPath(ext);
                pm.Apply();
                t.Commit();
                return true;
            } catch {
                t.RollBack();
                return false;
            }
        }

        /// <summary>
        /// Try to create a print setting to the current Revit document.
        /// </summary>
        /// <param name="doc">The Revit doc containing the printsettings.</param>
        /// <param name="ps">The search string.</param>
        /// <returns>The matching print setting, or null.</returns>
        public static PrintSetting AssignPrintSetting(Document doc, string printSetting)
        {
            foreach (ElementId id in doc.GetPrintSettingIds()) {
                var ps2 = doc.GetElement(id) as PrintSetting;
                if (ps2.Name.ToString().Equals("SCX-" + printSetting)) {
                    return ps2;
                }
            }

            try {
                CreatePrintSetting(doc, printSetting);
                foreach (ElementId id in doc.GetPrintSettingIds()) {
                    var ps2 = doc.GetElement(id) as PrintSetting;
                    if (ps2.Name.ToString().Equals("SCX-" + printSetting)) {
                        return ps2;
                    }
                }
            } catch {
                var msg = "SCX-" + printSetting + " could not be created!";
                TaskDialog.Show("Creating Papersize", msg);
            }
            return null;
        }
        
        public static bool SetPrinter(
                Document doc, string name, PrintManager pm)
        {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }
            var t = new Transaction(doc, "Set printer");
            t.Start();
            try {
                pm.SelectNewPrintDriver(name);
                t.Commit();
                return true;
            } catch (InvalidOperationException e) {
                var msg = "Print driver " + name + " not found.  Exiting now. Message: " + e.Message;
                TaskDialog.Show("SCexport", msg);
                t.RollBack();
                return false;
            }
        }
        
        private static bool CheckSheetSize(
            double width, double height, double tw, double th)
        {
            double w = Math.Round(width);
            double h = Math.Round(height);
            return tw + 2 > w && tw - 2 < w && th + 2 > h && th - 2 < h;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
