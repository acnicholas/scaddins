// (C) Copyright 2012-2020 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System;
    using System.Globalization;
    using Autodesk.Revit.DB;
    using Properties;

    public static class PrintSettings
    {
        public static bool CheckSheetSize(
                    double width, double height, double tw, double th)
        {
            double w = Math.Round(width);
            double h = Math.Round(height);

            // use a tolerance of 2mm.
            return tw + 2 > w && tw - 2 < w && th + 2 > h && th - 2 < h;
        }

        public static bool CreatePrintSetting(Document doc, string isoSheetSize, bool forceRaster)
        {
            if (doc == null || string.IsNullOrEmpty(isoSheetSize))
            {
                return false;
            }
            PrintManager pm = doc.PrintManager;
            bool success = false;
            foreach (PaperSize paperSize in pm.PaperSizes)
            {
                if (paperSize.Name.Substring(0, 2) == isoSheetSize.Substring(0, 2))
                {
                    var t = new Transaction(doc, "Apply print settings");
                    t.Start();
                    var ips = pm.PrintSetup.CurrentPrintSetting;
                    if (ips.PrintParameters.IsReadOnly)
                    {
                        t.RollBack();
                        return false;
                    }
                    try
                    {
                        ips.PrintParameters.PaperSize = paperSize;
                        ips.PrintParameters.HideCropBoundaries = true;
                        if (isoSheetSize.Length > 2 && !isoSheetSize.Contains("FIT"))
                        {
                            ips.PrintParameters.PageOrientation =
                            PageOrientationType.Portrait;
                        }
                        else
                        {
                            ips.PrintParameters.PageOrientation =
                            PageOrientationType.Landscape;
                        }

                        ips.PrintParameters.HideScopeBoxes = true;
                        ips.PrintParameters.HideReforWorkPlanes = true;
                        ips.PrintParameters.HideUnreferencedViewTags = true;
                        if (isoSheetSize.Contains("FIT"))
                        {
                            ips.PrintParameters.ZoomType = ZoomType.FitToPage;
                            ips.PrintParameters.PaperPlacement = PaperPlacementType.Margins;
                            ips.PrintParameters.MarginType = MarginType.NoMargin;
                        }
                        else
                        {
                            ips.PrintParameters.ZoomType = ZoomType.Zoom;
                            ips.PrintParameters.Zoom = 100;
                            ips.PrintParameters.PaperPlacement = PaperPlacementType.Margins;
                            ips.PrintParameters.MarginType = MarginType.NoMargin;
                        }

                        if (!forceRaster)
                        {
                            if (!TrySavePrintSetup(pm, "SCX-" + isoSheetSize, 1))
                            {
                                t.RollBack();
                                return false;
                            }
                        }
                        else
                        {
                            ips.PrintParameters.HiddenLineViews = HiddenLineViewsType.RasterProcessing;
                            if (!TrySavePrintSetup(pm, "SCX-" + @"(Raster)" + isoSheetSize, 1))
                            {
                                t.RollBack();
                                return false;
                            }
                        }

                        t.Commit();
                        success = true;
                    }
                    catch (InvalidOperationException ex)
                    {
                        System.Diagnostics.Debug.Print(ex.Message);
                        SCaddinsApp.WindowManager.ShowMessageBox(
                            "SCexport",
                            "Unable to create print setting: " + "SCX-" + isoSheetSize);
                        t.RollBack();
                    }
                }
            }
            return success;
        }

        public static bool TrySavePrintSetup(PrintManager pm, string name, int attempts)
        {
            try
            {
                return pm.PrintSetup.SaveAs(name);
            }
            catch
            {
                if (attempts > 0)
                {
                    return TrySavePrintSetup(pm, name, --attempts);
                }
                return false;
            }
        }

        /// <summary>
        /// Retrieve the print setting for this sheet.
        /// If it is not found, attempt to create a new one.
        /// ...if this still fails return null.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="printSetting">The Name of the print setting</param>
        /// <param name="forceRaster"></param>
        /// <returns></returns>
        public static PrintSetting GetPrintSettingByName(Document doc, string printSetting, bool forceRaster)
        {
            if (doc == null || string.IsNullOrEmpty(printSetting))
            {
                return null;
            }

            foreach (ElementId id in doc.GetPrintSettingIds())
            {
                var ps2 = doc.GetElement(id) as PrintSetting;
                if (!forceRaster)
                {
                    if (ps2 != null && ps2.Name.ToString(CultureInfo.CurrentCulture).Equals("SCX-" + printSetting, StringComparison.CurrentCulture))
                    {
                        return ps2;
                    }
                }
                else
                {
                    if (ps2 != null && ps2.Name.ToString(CultureInfo.CurrentCulture).Equals("SCX-" + printSetting + @"(Raster)", StringComparison.CurrentCulture))
                    {
                        return ps2;
                    }
                }
            }

            if (!CreatePrintSetting(doc, printSetting, forceRaster))
            {
                return null;
            }

            // FIXME. put this in a separate method. it's the same as above.
            foreach (ElementId id in doc.GetPrintSettingIds())
            {
                var ps2 = doc.GetElement(id) as PrintSetting;
                if (!forceRaster)
                {
                    if (ps2 != null && ps2.Name.ToString(CultureInfo.CurrentCulture).Equals("SCX-" + printSetting, StringComparison.CurrentCulture))
                    {
                        return ps2;
                    }
                }
                else
                {
                    if (ps2 != null && ps2.Name.ToString(CultureInfo.CurrentCulture).Equals("SCX-" + printSetting + @"(Raster)", StringComparison.CurrentCulture))
                    {
                        return ps2;
                    }
                }
            }

            var msg = "SCX-" + printSetting + " could not be created!";
            SCaddinsApp.WindowManager.ShowMessageBox("Creating Papersize", msg);
            return null;
        }

        /// <summary>
        /// Return the name of the paper size of the current sheet
        /// FIXME add sizes other that iso A*
        /// </summary>
        public static string GetSheetSizeAsString(ExportSheet sheet)
        {
            if (sheet == null)
            {
                return string.Empty;
            }

            double[] p = { 1189, 841, 594, 420, 297, 210, 297, 420, 594, 841, 1189 };
            string[] s = { "A0", "A1", "A2", "A3", "A4", "A4P", "A3P", "A2P", "A1P", "A0P" };

            for (int i = 0; i < s.Length; i++)
            {
                if (CheckSheetSize(sheet.Width, sheet.Height, p[i], p[i + 1]))
                {
                    return s[i];
                }
            }

            if (CheckSheetSize(sheet.Width, sheet.Height, 1000, 707))
            {
                return "B1";
            }

            return Math.Round(sheet.Width).ToString(CultureInfo.InvariantCulture) + "x" +
                Math.Round(sheet.Height).ToString(CultureInfo.InvariantCulture);
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static bool PrintToDevice(
                Document doc,
                string size,
                PrintManager pm,
                string printerName,
                bool forceRaster,
                ExportLog log)
        {
            if (pm == null)
            {
                return false;
            }

            PrintSetting ps = LoadRevitPrintSetting(doc, size, pm, printerName, forceRaster, log);

            if (ps == null)
            {
                return false;
            }

            var t = new Transaction(doc, Resources.ApplyPrintSettings);
            t.Start();
            try
            {
                if (ps.IsValidObject)
                {
                    pm.PrintSetup.CurrentPrintSetting = ps;
                }
                else
                {
                    if (log != null)
                    {
                        log.AddWarning(null, Resources.WarningPrintSetupReadOnly);
                    }
                }
                pm.PrintRange = PrintRange.Current;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.PaperPlacement = PaperPlacementType.Margins;
                pm.PrintSetup.InSession.PrintParameters.PaperPlacement = PaperPlacementType.Margins;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.MarginType = MarginType.UserDefined;
                pm.PrintSetup.InSession.PrintParameters.MarginType = MarginType.UserDefined;
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.OriginOffsetX = 0;
                pm.PrintSetup.InSession.PrintParameters.OriginOffsetY = 0;
#else
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.UserDefinedMarginX = 0;
                pm.PrintSetup.InSession.PrintParameters.UserDefinedMarginX = 0;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.UserDefinedMarginY = 0;
                pm.PrintSetup.InSession.PrintParameters.UserDefinedMarginY = 0;
#endif
                pm.PrintToFile = false;
                pm.Apply();
                t.Commit();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                if (log != null)
                {
                    log.AddError(null, ex.ToString());
                }
                t.RollBack();
                return false;
            }
        }

        public static bool PrintToFile(
                Document doc,
                ExportSheet vs,
                PrintManager pm,
                string ext,
                string printerName)
        {
            if (pm == null || vs == null) {
                return false;
            }
            if (vs.SCPrintSetting == null) {
                vs.UpdateSheetInfo();
                return false;
            }

            if (!SetPrinterByName(doc, printerName, pm)) {
                return false;
            }

            using (var t = new Transaction(doc, "Print Pdf"))
            {
                if (t.Start() == TransactionStatus.Started)
                {
                    try {
                        if (!pm.PrintSetup.IsReadOnly)
                        {
                            pm.PrintSetup.CurrentPrintSetting = vs.SCPrintSetting;
                            pm.PrintRange = PrintRange.Current;
                            pm.PrintToFile = true;
                            pm.PrintToFileName = vs.FullExportPath(ext);
                            pm.Apply();
                            t.Commit();
                            return true;
                        }
                        t.Commit();
                        return false;
                    } catch (InvalidOperationException ex) {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        t.RollBack();
                        return false;
                    }
                } else {
                    t.RollBack();
                    return false;
                }
            }
        }

        public static bool SetPrinterByName(
                Document doc, string name, PrintManager pm)
        {
            if (string.IsNullOrEmpty(name) || pm == null)
            {
                return false;
            }
            var t = new Transaction(doc, Resources.SetPrinter);
            t.Start();
            try
            {
                pm.SelectNewPrintDriver(name);
                t.Commit();
                return true;
            }
            catch (InvalidOperationException e)
            {
                var msg = "Print driver " + name + " not found.  Exiting now. Message: " + e.Message;
                SCaddinsApp.WindowManager.ShowMessageBox("SCexport", msg);
                t.RollBack();
                return false;
            }
        }

        private static PrintSetting LoadRevitPrintSetting(
                Document doc,
                string size,
                PrintManager pm,
                string printerName,
                bool forceRaster,
                ExportLog log)
        {
            log.AddMessage(Resources.MessageAttemptingToLoadRevitPrintSettings + size);
            PrintSetting ps = GetPrintSettingByName(doc, size, forceRaster);

            if (ps == null)
            {
                log.AddError(null, Resources.ErrorRetrievingRevitPrintSettingsFAILED);
                return null;
            }

            log.AddMessage(Resources.MessageUsingPrinter + printerName);
            if (!SetPrinterByName(doc, printerName, pm))
            {
                log.AddError(null, Resources.MessageCannotSetPrinter + printerName);
                return null;
            }

            return ps;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
