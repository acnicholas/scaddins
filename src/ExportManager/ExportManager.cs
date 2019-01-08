// (C) Copyright 2012-2018 by Andrew Nicholas
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using System.Xml;
    using System.Xml.Schema;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using SCaddins.Common;
    using SCaddins.Properties;

    public class Manager
    {
        private static string activeDoc;
        private static Dictionary<string, FamilyInstance> titleBlocks;
        private ObservableCollection<ExportSheet> allSheets;
        private ObservableCollection<ViewSetItem> allViewSheetSets;
        private bool dateForEmptyRevisions;
        private string exportDirectory;
        private ExportOptions exportFlags;
        private SegmentedSheetName fileNameScheme;
        private List<SegmentedSheetName> fileNameTypes;
        private bool forceDate;
        private Dictionary<string, PostExportHookCommand> postExportHooks;

        public Manager(UIDocument uidoc)
        {
            Doc = uidoc.Document;
            UIDoc = uidoc;
            this.fileNameScheme = null;
            this.exportDirectory = Constants.DefaultExportDirectory;
            Manager.ConfirmOverwrite = true;
            Manager.activeDoc = null;
            this.allViewSheetSets = GetAllViewSheetSets(Doc);
            this.allSheets = new ObservableCollection<ExportSheet>();
            this.fileNameTypes = new List<SegmentedSheetName>();
            this.postExportHooks = new Dictionary<string, PostExportHookCommand>();
            this.exportFlags = ExportOptions.None;
            this.LoadSettings();
            this.SetDefaultFlags();
            this.PopulateSheets(this.allSheets);
            Manager.FixAcrotrayHang();
        }

        public static ACADVersion AcadVersion {
            get
            {
                return Settings1.Default.AcadExportVersion;
            }

            set
            {
                if (value == Settings1.Default.AcadExportVersion) {
                    return;
                }
                Settings1.Default.AcadExportVersion = value;
            }
        }

        public static bool ConfirmOverwrite {
            get; set;
        }

        public static string ForceRasterPrintParameterName
        {
            get
            {
                return Settings1.Default.UseRasterPrinterParameter;
            }
        }

        public ObservableCollection<ExportSheet> AllSheets {
            get { return this.allSheets; }
        }

        public ObservableCollection<ViewSetItem> AllViewSheetSets {
            get { return this.allViewSheetSets; }
        }

        public Document Doc {
            get; set;
        }

        public bool ExportViewportsOnly
        {
            get; set;
        }

        public string ExportDirectory {
            get
            {
                return this.exportDirectory;
            }

            set
            {
                if (value != null) {
                    this.exportDirectory = value;
                    foreach (ExportSheet sheet in this.allSheets) {
                        sheet.ExportDirectory = value;
                    }
                }
            }
        }

        public SegmentedSheetName FileNameScheme {
            get
            {
                return this.fileNameScheme;
            }

            set
            {
                this.fileNameScheme = value;
                foreach (ExportSheet sheet in allSheets) {
                    sheet.SetSegmentedSheetName(fileNameScheme);
                }
            }
        }

        public List<SegmentedSheetName> FileNameTypes {
            get { return this.fileNameTypes; }
        }

        public bool ForceRevisionToDateString
        {
            get
            {
                return this.forceDate;
            }

            set
            {
                this.forceDate = value;
                foreach (ExportSheet sheet in this.allSheets)
                {
                    sheet.ForceDate = value;
                }
            }
        }

        public string GhostscriptBinDirectory
        {
            get; set;
        }

        public string GhostscriptLibDirectory
        {
            get; set;
        }

        public string PdfPrinterName
        {
            get; set;
        }

        public string PostscriptPrinterName
        {
            get; set;
        }

        public string PrinterNameA3
        {
            get; set;
        }

        public string PrinterNameLargeFormat
        {
            get; set;
        }

        public bool ShowExportLog
        {
            get; set;
        }

        public UIDocument UIDoc
        {
            get; set;
        }

        public bool UseDateForEmptyRevisions
        {
            get
            {
                return dateForEmptyRevisions;
            }

            set
            {
                this.dateForEmptyRevisions = value;
                foreach (ExportSheet sheet in this.allSheets)
                {
                    sheet.UseDateForEmptyRevisions = value;
                }
            }
        }

        public bool VerifyOnStartup
        {
            get; set;
        }

        public static void AddRevisions(ICollection<ExportSheet> sheets, ElementId revisionId, Document doc)
        {
            if (sheets == null || revisionId == null || revisionId == null)
            {
                return;
            }

            using (var t = new Transaction(doc, "SCexport: Add new revisions"))
            {
                if (t.Start() == TransactionStatus.Started)
                {
                    foreach (ExportSheet sheet in sheets)
                    {
                        ICollection<ElementId> il = sheet.Sheet.GetAdditionalRevisionIds();
                        il.Add(revisionId);
                        sheet.Sheet.SetAdditionalRevisionIds(il);
                        sheet.UpdateRevision(true);
                    }
                    doc.Regenerate();
                    t.Commit();
                }
                else
                {
                    TaskDialog.Show("Error", "SCexport: error adding revisions, could not start transaction.");
                }
            }

            foreach (ExportSheet sheet in sheets)
            {
                sheet.UpdateRevision(true);
            }
        }

        public static string CreateSCexportConfig(Document doc)
        {
            string s = GetConfigFileName(doc);
            return File.Exists(s) ? s : null;
        }

        public static string CurrentViewNumber(Document doc)
        {
            View v = doc.ActiveView;
            if (v.ViewType == ViewType.DrawingSheet)
            {
                return v.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString();
            }
            else
            {
                return null;
            }
        }

        public static void FixScaleBars(ICollection<ExportSheet> sheets, Document doc)
        {
            if (sheets == null)
            {
                TaskDialog.Show("Error", "Please select sheets before attempting to add revisions");
                return;
            }
            using (Transaction t = new Autodesk.Revit.DB.Transaction(doc))
            {
                if (t.Start("SCexport - Fix Scale Bars") == TransactionStatus.Started)
                {
                    foreach (ExportSheet sheet in sheets)
                    {
                        if (!sheet.ValidScaleBar)
                        {
                            sheet.UpdateScaleBarScale();
                        }
                    }
                    if (t.Commit() != TransactionStatus.Committed)
                    {
                        TaskDialog.Show("Failure", "Could not fix scale bars");
                    }
                }
            }
        }

        public static string GetConfigFileName(Document doc)
        {
#if DEBUG
                        Debug.WriteLine("getting config file for " + doc.Title);
                        string s = @"C:\Andrew\code\cs\scaddins\share\SCexport-example-conf.xml";
#else
            string central = FileUtilities.GetCentralFileName(doc);
            string s = Path.GetDirectoryName(central) + @"\SCexport.xml";
#endif
            return s;
        }

        public static string GetOldConfigFileName(Document doc)
        {
            string central = FileUtilities.GetCentralFileName(doc);
            string s = Path.GetDirectoryName(central) + Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(central) + Resources.FileExtensionXML;
            return s;
        }

        public static string LatestRevisionDate(Document doc)
        {
            string s = string.Empty;
            int i = -1;
            using (FilteredElementCollector collector = new FilteredElementCollector(doc))
            {
                collector.OfCategory(BuiltInCategory.OST_Revisions);
                foreach (Element e in collector)
                {
                    int j = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger();
                    if (j > i)
                    {
                        i = j;
                        s = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString();
                    }
                }
            }
            return (s.Length > 1) ? s : string.Empty;
        }

        public static void OpenNextSheet(UIDocument udoc, ViewSheet view)
        {
            OpenSheet.OpenPreviousSheet(udoc, view);
        }

        public static void OpenPreviousSheet(UIDocument udoc, ViewSheet view)
        {
            OpenSheet.OpenNextSheet(udoc, view);
        }

        public static FamilyInstance TitleBlockInstanceFromSheetNumber(string sheetNumber, Document doc)
        {
            if (doc == null)
            {
                return null;
            }

            FamilyInstance result;
            if ((titleBlocks == null) || (activeDoc != FileUtilities.GetCentralFileName(doc)))
            {
                activeDoc = FileUtilities.GetCentralFileName(doc);
                titleBlocks = AllTitleBlocks(doc);
            }

            if (titleBlocks.TryGetValue(sheetNumber, out result))
            {
                return result;
            }
            else
            {
                titleBlocks = AllTitleBlocks(doc);
            }

            return titleBlocks.TryGetValue(sheetNumber, out result) ? result : null;
        }

        public static void ToggleNorthPoints(ICollection<ExportSheet> sheets, Document doc, bool turnOn)
        {
            if (sheets == null)
            {
                return;
            }
            using (Transaction t = new Autodesk.Revit.DB.Transaction(doc))
            {
                if (t.Start("SCexport - Toggle North Points") == TransactionStatus.Started)
                {
                    foreach (ExportSheet sheet in sheets)
                    {
                        sheet.ToggleNorthPoint(turnOn);
                    }
                    if (t.Commit() != TransactionStatus.Committed)
                    {
                        TaskDialog.Show("Failure", "Could not toggle north points");
                    }
                }
            }
        }

        public void AddExportOption(ExportOptions exportOptions)
        {
            this.exportFlags |= exportOptions;
        }

        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public void ExportSheet(ExportSheet sheet, ExportLog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            var startTime = log.StartLoggingIndividualItem(null);
            log.AddMessage(sheet.ToString());

            if (!sheet.Verified)
            {
                sheet.UpdateSheetInfo();
            }

            if (sheet.SCPrintSetting != null)
            {
                if (this.exportFlags.HasFlag(ExportOptions.DWG))
                {
                    this.ExportDWG(sheet, this.exportFlags.HasFlag(ExportOptions.NoTitle), log);
                }

                if (this.exportFlags.HasFlag(ExportOptions.PDF))
                {
                    this.ExportAdobePDF(sheet, log);
                }

                if (this.exportFlags.HasFlag(ExportOptions.GhostscriptPDF))
                {
                    this.ExportGSPDF(sheet, log);
                }
            }
            else
            {
                log.AddError(sheet.FullExportName, Resources.MessageNoPrintSettingAssigned);
            }
            log.EndLoggingIndividualItem(startTime, null);
        }

        public void SaveViewSet(string name, List<ExportSheet> selectedSheets)
        {
            using (Transaction t = new Transaction(Doc)) 
            {
                if (t.Start("Save view sheet set") == TransactionStatus.Started)
                {
                    Doc.PrintManager.PrintRange = PrintRange.Select;
                    Doc.PrintManager.Apply();
                    var set = new ViewSet();

                    foreach (ExportSheet exportSheet in selectedSheets)
                    {
                       set.Insert(exportSheet.Sheet);
                    }
                    Doc.PrintManager.ViewSheetSetting.CurrentViewSheetSet = Doc.PrintManager.ViewSheetSetting.InSession;
                    Doc.PrintManager.PrintRange = PrintRange.Select;
                    Doc.PrintManager.ViewSheetSetting.CurrentViewSheetSet.Views = set;
                    Doc.PrintManager.ViewSheetSetting.SaveAs(name);
                    if (t.Commit() != TransactionStatus.Committed)
                    {
                        t.RollBack();
                    }
                } else
                {
                    t.RollBack();
                }
            }
        }

        public bool GSSanityCheck()
        {
            if (!Directory.Exists(this.GhostscriptBinDirectory) || !Directory.Exists(this.GhostscriptLibDirectory))
            {
                return false;
            }
            var ps = new System.Drawing.Printing.PrinterSettings();
            ps.PrinterName = this.PostscriptPrinterName;
            return ps.IsValid;
        }

        public bool HasExportOption(ExportOptions option)
        {
            return this.exportFlags.HasFlag(option);
        }

        public void LoadSettings()
        {
            this.GhostscriptBinDirectory = SCaddins.ExportManager.Settings1.Default.GSBinDirectory;
            this.PdfPrinterName = SCaddins.ExportManager.Settings1.Default.AdobePrinterDriver;
            this.PrinterNameA3 = SCaddins.ExportManager.Settings1.Default.A3PrinterDriver;
            this.PrinterNameLargeFormat = SCaddins.ExportManager.Settings1.Default.LargeFormatPrinterDriver;
            this.PostscriptPrinterName = SCaddins.ExportManager.Settings1.Default.PSPrinterDriver;
            this.GhostscriptLibDirectory = SCaddins.ExportManager.Settings1.Default.GSLibDirectory;
            this.exportDirectory = SCaddins.ExportManager.Settings1.Default.ExportDir;
            AcadVersion = SCaddins.ExportManager.Settings1.Default.AcadExportVersion;
            this.ShowExportLog = SCaddins.ExportManager.Settings1.Default.ShowExportLog;
            this.ForceRevisionToDateString = SCaddins.ExportManager.Settings1.Default.ForceDateRevision;
            this.UseDateForEmptyRevisions = SCaddins.ExportManager.Settings1.Default.UseDateForEmptyRevisions;
            this.VerifyOnStartup = SCaddins.ExportManager.Settings1.Default.VerifyOnStartup;
        }

        public bool PDFSanityCheck()
        {
            var ps = new System.Drawing.Printing.PrinterSettings();
            ps.PrinterName = this.PdfPrinterName;
            return ps.IsValid;
        }

        public void Print(
            ExportSheet sheet,
            string printerName,
            int scale)
        {
            Print(sheet, printerName, scale, null);
        }

        public void Print(
        ExportSheet sheet,
        string printerName,
        int scale,
        ExportLog log)
        {
            PrintManager pm = Doc.PrintManager;
            bool printSetttingsValid;
            if (!sheet.Verified)
            {
                sheet.UpdateSheetInfo();
            }
            printSetttingsValid = false;

            switch (scale)
            {
                case 3:
                    printSetttingsValid |= PrintSettings.PrintToDevice(Doc, "A3-FIT", pm, printerName, sheet.ForceRasterPrint, log);
                    break;

                case 2:
                    printSetttingsValid |= PrintSettings.PrintToDevice(Doc, "A2-FIT", pm, printerName, sheet.ForceRasterPrint, log);
                    break;

                default:
                    int i = int.Parse(sheet.PageSize.Substring(1, 1), CultureInfo.InvariantCulture);
                    string printerNameTmp = i > 2 ? this.PrinterNameA3 : this.PrinterNameLargeFormat;
                    printSetttingsValid |= PrintSettings.PrintToDevice(Doc, sheet.PageSize, pm, printerNameTmp, sheet.ForceRasterPrint, log);
                    break;
            }
            if (printSetttingsValid)
            {
                pm.SubmitPrint(sheet.Sheet);
            }
            else
            {
                TaskDialog.Show("test", "print error");
            }
        }

        public void RemoveExportOption(ExportOptions exportOptions)
        {
            this.exportFlags = this.exportFlags & ~exportOptions;
        }

        public void SetFileNameScheme(string newScheme)
        {
            foreach (SegmentedSheetName scheme in this.fileNameTypes)
            {
                if (newScheme == scheme.Name)
                {
                    this.fileNameScheme = scheme;
                    foreach (ExportSheet sheet in this.allSheets)
                    {
                        sheet.SetSegmentedSheetName(this.fileNameScheme);
                    }
                }
            }
        }

        public void ToggleExportOption(ExportOptions option)
        {
            if (HasExportOption(option)) {
                RemoveExportOption(option);
            } else {
                AddExportOption(option);
            }
        }

        public void Update()
        {
            try
            {
                PrintManager pm = Doc.PrintManager;
                if (pm == null) {
                    SCaddinsApp.WindowManager.ShowMessageBox("PM is null");
                    return;
                }
                if (PrintSettings.SetPrinterByName(Doc, this.PdfPrinterName, pm)) {
                    foreach (ExportSheet sc in this.allSheets) {
                        if (!sc.Verified) {
                            sc.UpdateSheetInfo();
                        }
                    }
                }
            }
            catch {
                System.Diagnostics.Debug.WriteLine("Update error in ExportManager.");
            }
        }

        private static Dictionary<string, FamilyInstance> AllTitleBlocks(Document document)
        {
            var result = new Dictionary<string, FamilyInstance>();

            using (var collector = new FilteredElementCollector(document))
            {
                collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
                collector.OfClass(typeof(FamilyInstance));
                foreach (FamilyInstance e in collector)
                {
                    var s = e.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString();
                    if (!result.ContainsKey(s))
                    {
                        result.Add(s, e);
                    }
                }
            }

            return result;
        }

        private static void FixAcrotrayHang()
        {
            Microsoft.Win32.Registry.SetValue(
                Constants.HungAppTimeout,
                "HungAppTimeout",
                "30000",
                Microsoft.Win32.RegistryValueKind.String);
        }

        private static bool IsViewerMode()
        {
            var mainWindowTitle = System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle;
            return mainWindowTitle.Contains("VIEWER");
        }

        private static string PercentageSting(int n, int total)
        {
            var result = "Exporting " + n + " of " + total +
                " (" + (int)(((double)n / (double)total) * 100) + @"%)";
            return result;
        }

        private static ObservableCollection<ViewSetItem> GetAllViewSheetSets(Document doc)
        {
            var result = new ObservableCollection<ViewSetItem>();
            using (FilteredElementCollector collector = new FilteredElementCollector(doc)) {
                collector.OfClass(typeof(ViewSheetSet));
                foreach (ViewSheetSet v in collector) {
                    var viewIds = v.Views.Cast<View>()
                        .Where(vs => vs.ViewType == ViewType.DrawingSheet)
                        .Select(vs => vs.Id.IntegerValue).ToList();
                    result.Add(new ViewSetItem(v.Id.IntegerValue, v.Name, viewIds));
                }
            }
            return result;
        }

        private static void RemoveTitleBlock(
            ExportSheet vs,
            ICollection<ElementId> title,
            bool hide,
            Document doc)
        {
            var view = doc.GetElement(vs.Id) as View;
            var t = new Transaction(doc, "Hide Title");
            t.Start();
            try {
                if (hide) {
                    view.HideElements(title);
                } else {
                    view.UnhideElements(title);
                }
                t.Commit();
            } catch (Autodesk.Revit.Exceptions.ArgumentException e) {
                TaskDialog.Show("Revit", "cannot Hide Title: " + e.Message);
                t.RollBack();
            }
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PrinterJobControl")]
        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private static bool SetAcrobatExportRegistryVal(string fileName, ExportLog log)
        {
            string exe = Process.GetCurrentProcess().MainModule.FileName;
            try {
                log.AddMessage("Attempting to set Acrobat Registry Value with value");
                log.AddMessage("\t" + Constants.AcrobatPrinterJobControl);
                log.AddMessage("\t" + exe);
                log.AddMessage("\t" + fileName);
                Microsoft.Win32.Registry.SetValue(
                    Constants.AcrobatPrinterJobControl,
                    exe,
                    fileName,
                    Microsoft.Win32.RegistryValueKind.String);
                return true;
            } catch (UnauthorizedAccessException ex) {
                log.AddError(fileName, ex.Message);
                return false;
            } catch (SecurityException ex) {
                log.AddError(fileName, ex.Message);
                return false;
            }
        }

        private static string TimeSpanAsString(TimeSpan time)
        {
            var result = "Elapsed Time: " +
                MiscUtilities.PadLeftZeros(time.Hours.ToString(CultureInfo.CurrentCulture), 2) + "h:" +
                MiscUtilities.PadLeftZeros(time.Minutes.ToString(CultureInfo.CurrentCulture), 2) + "m:" +
                MiscUtilities.PadLeftZeros(time.Seconds.ToString(CultureInfo.CurrentCulture), 2) + "s";
            return result;
        }

        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private bool ExportAdobePDF(ExportSheet vs, ExportLog log)
        {
            if (log != null) {
                log.AddMessage(Environment.NewLine + Resources.MessageStartingPDFExport);
            } else {
                return false;
            }

            if (IsViewerMode()) {
                log.AddError(vs.FullExportName, "Revit is in Viewer mode. Printing is not allowed");
                return false;
            }

            PrintManager pm = Doc.PrintManager;

            log.AddMessage(Resources.MessageApplyingPrintSetting + vs.PrintSettingName);

            if (!PrintSettings.PrintToFile(Doc, vs, pm, Resources.FileExtensionPDF, this.PdfPrinterName)) {
                log.AddError(vs.FullExportName, Resources.ErrorFailedToAssignPrintSetting + vs.PrintSettingName);
                return false;
            }

            if (!SetAcrobatExportRegistryVal(vs.FullExportPath(Resources.FileExtensionPDF), log)) {
                log.AddError(vs.FullExportName, "Unable to write to registry");
                return false;
            }

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(Resources.FileExtensionPDF))) {
                if (File.Exists(vs.FullExportPath(Resources.FileExtensionPDF))) {
                    File.Delete(vs.FullExportPath(Resources.FileExtensionPDF));
                }
                log.AddMessage(Resources.MessageSubmittingPrint);
                
                if (pm.SubmitPrint(vs.Sheet)) {
                    log.AddMessage(Resources.MessageApparentlyCompletedSuccessfully);
                } else {
                    log.AddError(vs.FullExportName, Resources.ErrorFailedToPrint);
                }
                FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionPDF));

                this.RunExportHooks(Resources.FileExtensionPDF, vs);

                SCaddins.Common.SystemUtilities.KillAllProcesses("acrotray");
            } else {
                ////log.AddError(vs.FullExportName, Resources.ErrorCantOverwriteFile);
                return false;
            }

            return true;
        }

        // FIXME this is nasty
        private void ExportDWG(ExportSheet vs, bool removeTitle, ExportLog log)
        {
            if (log != null) {
                log.AddMessage(Environment.NewLine + Resources.MessageStartingDWGExport);
            } else {
                return;
            }

            if (IsViewerMode()) {
                log.AddError(vs.FullExportName, "Revit is in Viewer mode. Exporting DWG files is not allowed");
                return;
            }

            List<ElementId> titleBlockHidden;
            titleBlockHidden = new List<ElementId>();
            var titleBlock = Manager.TitleBlockInstanceFromSheetNumber(vs.SheetNumber, Doc);
            titleBlockHidden.Add(titleBlock.Id);

            if (removeTitle) {
                if (log != null) {
                    log.AddMessage(Resources.MessageAttemptingToHideTitleBlock);
                }
                Manager.RemoveTitleBlock(vs, titleBlockHidden, true, Doc);
            }

            PrintManager pm = Doc.PrintManager;

            using (var t = new Transaction(Doc, Resources.ApplyPrintSettings)) {
                if (t.Start() == TransactionStatus.Started) {
                    try {
                        pm.PrintToFile.Equals(true);
                        pm.PrintRange = PrintRange.Select;
                        pm.Apply();
                        t.Commit();
                    } catch (InvalidOperationException) {
                        if (log != null) {
                            log.AddWarning(null, Resources.MessageCouldNotApplyPrintSettings);
                        }
                        t.RollBack();
                    }
                }
            }

            List<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);

            if (ExportViewportsOnly)
            {
                foreach (var viewOnSheet in vs.Sheet.GetAllPlacedViews()) {
                    View individualViewOnSheet = Doc.GetElement(viewOnSheet) as View;
                    if (individualViewOnSheet.ViewType == ViewType.FloorPlan || individualViewOnSheet.ViewType == ViewType.CeilingPlan)
                    {
                        views.Add(individualViewOnSheet.Id);
                    }
                }
            }

            using (var opts = GetDefaultDWGExportOptions()) {
                if (log != null) {
                    log.AddMessage(Resources.MessageAssigningExportOptions + opts);
                }
                pm.PrintRange = PrintRange.Select;
                var name = vs.FullExportName + Resources.FileExtensionDWG;
                if (log != null) {
                    log.AddMessage(Resources.MessageExportingToDirectory + vs.ExportDirectory);
                    log.AddMessage(Resources.MessageExportingToFileName + name);
                }
                Doc.Export(vs.ExportDirectory, name, views, opts);
            }

            FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionDWG));
            this.RunExportHooks("dwg", vs);

            if (removeTitle) {
                if (log != null) {
                    log.AddMessage(Resources.MessageShowingTitleBlock);
                }
                Manager.RemoveTitleBlock(vs, titleBlockHidden, false, Doc);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Parameter name required by ps2pdf", MessageId = "sPAPERSIZE")]
        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private bool ExportGSPDF(ExportSheet vs, ExportLog log)
        {
            log.AddMessage(Environment.NewLine + Resources.MessageStartingGhostscriptPDFExport);
            log.AddMessage(vs.ToString());

            PrintManager pm = Doc.PrintManager;

            log.AddMessage(Resources.MessageApplyingPrintSetting + vs.PrintSettingName);

            if (!PrintSettings.PrintToFile(Doc, vs, pm, Resources.FileExtensionPS, this.PostscriptPrinterName)) {
                log.AddError(vs.FullExportName, Resources.ErrorFailedToAssignPrintSetting + vs.PrintSettingName);
                return false;
            }

            log.AddMessage(Resources.MessageSubmittingPrint);

            try {
                pm.SubmitPrint(vs.Sheet);
            } catch (InvalidOperationException) {
                File.Delete(vs.FullExportPath(Resources.FileExtensionPS));
                pm.SubmitPrint(vs.Sheet);
            }

            log.AddMessage(Resources.StartingPrint + vs.FullExportPath(Resources.FileExtensionPS));

            FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionPS));

            log.AddMessage(Resources.OK);

            string prog = "\"" + this.GhostscriptLibDirectory + @"\ps2pdf" + "\"";
            string size = vs.PageSize.ToLower(CultureInfo.CurrentCulture);
            string sizeFix = size.ToLower(CultureInfo.CurrentCulture).Replace("p", string.Empty);
            string args =
                "-sPAPERSIZE#" +
                sizeFix + " \"" + vs.FullExportPath(Resources.FileExtensionPS) +
                "\" \"" + vs.FullExportPath(Resources.FileExtensionPDF) + "\"";

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(Resources.FileExtensionPDF))) {
                log.AddMessage("Converting to PDF with: " + prog + " " + args);
                SCaddins.Common.ConsoleUtilities.StartHiddenConsoleProg(prog, args);
                FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionPDF));
                this.RunExportHooks("pdf", vs);
            } else {
                log.AddWarning(vs.FullExportName, Resources.MessageUnableToOverwriteExistingFile + vs.FullExportPath(Resources.FileExtensionPDF));
            }

            return true;
        }

        private DWGExportOptions GetDefaultDWGExportOptions()
        {
            var opts = new DWGExportOptions();
            opts.MergedViews = true;
            opts.FileVersion = AcadVersion;
            opts.HideScopeBox = true;
            opts.HideUnreferenceViewTags = true;
            if (ExportViewportsOnly) {
                opts.SharedCoords = true;
            }
            return opts;
        }

        private bool ImportXMLinfo(string filename)
        {
            if (!System.IO.File.Exists(filename)) {
                return false;
            }
            if (!this.ValidateXML(filename)) {
                return false;
            }

#pragma warning disable CA3075 // Insecure DTD processing in XML
            using (var reader = new XmlTextReader(filename)) {
#pragma warning restore CA3075 // Insecure DTD processing in XML
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "PostExportHook") {
                        var hook = new PostExportHookCommand();
                        if (reader.AttributeCount > 0) {
                            hook.SetName(reader.GetAttribute("name"));
                        }
                        do {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Element) {
                                switch (reader.Name) {
                                    case "Command":
                                    hook.SetCommand(reader.ReadString());
                                    break;

                                    case "Args":
                                    hook.SetArguments(reader.ReadString());
                                    break;

                                    case "SupportedFileExtension":
                                    hook.AddSupportedFilenameExtension(reader.ReadString());
                                    break;
                                }
                            }
                        } while (!(reader.NodeType == XmlNodeType.EndElement &&
                                 reader.Name == "PostExportHook"));
                        this.postExportHooks.Add(hook.Name, hook);
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "FilenameScheme") {
                        var name = new SegmentedSheetName();
                        if (reader.AttributeCount > 0) {
                            name.Name = reader.GetAttribute("name");
                        }
                        do {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Element) {
                                switch (reader.Name) {
                                    case "Format":
                                    name.NameFormat = reader.ReadString();
                                    break;

                                    case "Hook":
                                    name.Hooks.Add(reader.ReadString());
                                    break;
                                }
                            }
                        } while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "FilenameScheme"));
                        this.fileNameTypes.Add(name);
                    }
                }

                if (this.fileNameTypes.Count > 0) {
                    this.fileNameScheme = this.fileNameTypes[0];
                    foreach (ExportSheet sheet in this.allSheets) {
                        sheet.SetSegmentedSheetName(this.fileNameScheme);
                    }
                }
            }
            return true;
        }

        private void PopulateSheets(ObservableCollection<ExportSheet> s)
        {
            string config = GetConfigFileName(Doc);
            bool b = this.ImportXMLinfo(config);
            if (!b || this.fileNameTypes.Count <= 0) {
                var name = new SegmentedSheetName();
                name.Name = "YYYYMMDD-AD-NNN";
                name.NameFormat = "$projectNumber-$sheetNumber[$sheetRevision]";
                this.fileNameTypes.Add(name);
                this.fileNameScheme = name;
            }

            s.Clear();
            using (FilteredElementCollector collector = new FilteredElementCollector(Doc)) {
                collector.OfCategory(BuiltInCategory.OST_Sheets);
                collector.OfClass(typeof(ViewSheet));
                foreach (ViewSheet v in collector) {
                    var scxSheet = new ExportSheet(v, Doc, this.fileNameTypes[0], VerifyOnStartup, this);
                    s.Add(scxSheet);
                }
            }
        }

        private void RunExportHooks(string extension, ExportSheet vs)
        {
            for (int i = 0; i < this.postExportHooks.Count; i++) {
                if (this.postExportHooks.ElementAt(i).Value.HasExtension(extension)) {
                    if (this.fileNameScheme.Hooks.Count < 1) {
                        return;
                    } else {
                        if (this.fileNameScheme.Hooks.Contains(this.postExportHooks.ElementAt(i).Key)) {
                            this.postExportHooks.ElementAt(i).Value.Run(vs, extension);
                        }
                    }
                }
            }
        }

        private void SetDefaultFlags()
        {
            if (SCaddins.ExportManager.Settings1.Default.AdobePDFMode && this.PDFSanityCheck()) {
                this.AddExportOption(ExportOptions.PDF);
            } else if (!SCaddins.ExportManager.Settings1.Default.AdobePDFMode && this.GSSanityCheck()) {
                this.AddExportOption(ExportOptions.GhostscriptPDF);
            } else {
                if (this.PDFSanityCheck()) {
                    this.AddExportOption(ExportOptions.PDF);
                }
                this.AddExportOption(ExportOptions.DWG);
            }

            if (SCaddins.ExportManager.Settings1.Default.HideTitleBlocks) {
                this.AddExportOption(ExportOptions.NoTitle);
            }

            this.forceDate |= SCaddins.ExportManager.Settings1.Default.ForceDateRevision;
        }

        private bool ValidateXML(string filename)
        {
            string errorMessage = string.Empty;
            if (filename == null || !File.Exists(filename)) {
                return false;
            }
            try {
                var settings = new XmlReaderSettings();
#if !DEBUG
                settings.Schemas.Add(
                    null, SCaddins.Constants.InstallDirectory + @"\etc\SCexport.xsd");
#else
                                settings.Schemas.Add(null, @"C:\Andrew\code\cs\scaddins\etc\SCexport.xsd");
#endif
                settings.ValidationType = ValidationType.Schema;
                XmlReader reader = XmlReader.Create(filename, settings);
#pragma warning disable CA3075 // Insecure DTD processing in XML
                var document = new XmlDocument();
#pragma warning restore CA3075 // Insecure DTD processing in XML
                document.Load(reader);
                var eventHandler =
                    new ValidationEventHandler(this.ValidationEventHandler);
                document.Validate(eventHandler);
                return true;
            } catch (XmlSchemaValidationException ex) {
                errorMessage += "Error reading xml file:" + filename + " - " + ex.Message;
            } catch (XmlException ex) {
                errorMessage += "Error reading xml file:" + filename + " - " + ex.Message;
            } catch (XmlSchemaException ex) {
                errorMessage += "Error reading xml file:" + filename + " - " + ex.Message;
            } catch (ArgumentNullException ex) {
                errorMessage += "Error reading xml file:" + filename + " - " + ex.Message;
            } catch (UriFormatException ex) {
                errorMessage += "Error reading xml file:" + filename + " - " + ex.Message;
            }
            using (var td = new TaskDialog("SCexport - XML Config error")) {
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                td.MainInstruction = "SCexport - XML Config error";
                td.MainContent = errorMessage;
                td.Show();
            }
            return false;
        }

        private void ValidationEventHandler(
                    object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    TaskDialog.Show("Error: {0}", e.Message);
                    break;

                case XmlSeverityType.Warning:
                    TaskDialog.Show("Warning {0}", e.Message);
                    break;
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */