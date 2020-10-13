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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Printing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using System.Xml;
    using System.Xml.Schema;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Common;
    using Microsoft.Win32;
    using Properties;
    using ArgumentException = Autodesk.Revit.Exceptions.ArgumentException;
    using PrintRange = Autodesk.Revit.DB.PrintRange;

    public class Manager
    {
        // ReSharper disable once InconsistentNaming
        private static string activeDoc;
        // ReSharper disable once InconsistentNaming
        private static Dictionary<string, FamilyInstance> titleBlocks;
        private readonly Dictionary<string, PostExportHookCommand> postExportHooks;
        private bool dateForEmptyRevisions;
        private string exportDirectory;
        private ExportOptions exportFlags;
        private SegmentedSheetName fileNameScheme;
        private bool forceDate;
        
        public Manager(UIDocument uidoc)
        {
            Doc = uidoc.Document;
            UIDoc = uidoc;
            fileNameScheme = null;
            exportDirectory = Constants.DefaultExportDirectory;
            ConfirmOverwrite = true;
            activeDoc = null;
            UpdateAllViewSheetSets();
            AllSheets = new ObservableCollection<ExportSheet>();
            FileNameTypes = new ObservableCollection<SegmentedSheetName>();
            postExportHooks = new Dictionary<string, PostExportHookCommand>();
            exportFlags = ExportOptions.None;
            LoadSettings();
            SetDefaultFlags();
            LoadConfigFile();
            PopulateSheets(AllSheets);
            FixAcrotrayHang();
        }

        public static ACADVersion AcadVersion {
            get; set;
        }

        public static bool ConfirmOverwrite {
            get; set;
        }

        public static string ForceRasterPrintParameterName => Settings1.Default.UseRasterPrinterParameter;

        public ObservableCollection<ExportSheet> AllSheets { get; }

        public ObservableCollection<ViewSetItem> AllViewSheetSets { get; private set; }

        public Document Doc {
            get; set;
        }

        public bool ExportViewportsOnly
        {
            get; set;
        }

        public string ExportDirectory {
            get => exportDirectory;

            set
            {
                if (value != null) {
                    exportDirectory = value;
                    foreach (ExportSheet sheet in AllSheets) {
                        sheet.ExportDirectory = value;
                    }
                }
            }
        }

        public SegmentedSheetName FileNameScheme {
            get => fileNameScheme;

            set
            {
                fileNameScheme = value;
                foreach (ExportSheet sheet in AllSheets) {
                    sheet.SetSegmentedSheetName(fileNameScheme);
                }
            }
        }

        public ObservableCollection<SegmentedSheetName> FileNameTypes { get; }

        public bool ForceRevisionToDateString
        {
            get
            {
                return forceDate;
            }

            set
            {
                forceDate = value;
                foreach (ExportSheet sheet in AllSheets)
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

        public bool SaveHistory
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
                dateForEmptyRevisions = value;
                foreach (ExportSheet sheet in AllSheets)
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
            if (sheets == null || revisionId == null)
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
                    SCaddinsApp.WindowManager.ShowErrorMessageBox("Error", "SCexport: error adding revisions, could not start transaction.");
                }
            }

            foreach (ExportSheet sheet in sheets)
            {
                sheet.UpdateRevision(true);
            }
        }

        public static string CreateSCexportConfig(Document doc)
        {
            var s = GetConfigFileName(doc);
            return File.Exists(s) ? s : null;
        }

        public static string CurrentViewNumber(Document doc)
        {
            var v = doc.ActiveView;
            if (v.ViewType == ViewType.DrawingSheet)
            {
                return v.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString();
            }

            return null;
        }

        public static void FixScaleBars(ICollection<ExportSheet> sheets, Document doc)
        {
            if (sheets == null)
            {
                SCaddinsApp.WindowManager.ShowErrorMessageBox("Error", "Please select sheets before attempting to fix scalebars");
                return;
            }
            using (Transaction t = new Transaction(doc))
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
                        SCaddinsApp.WindowManager.ShowMessageBox("Failure", "Could not fix scale bars");
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
            //// SCaddinsApp.WindowManager.ShowMessageBox("OK");
            var fec = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ProjectInformation);
            foreach (var element in fec) {
                if (element is ProjectInfo) {
                    //// SCaddinsApp.WindowManager.ShowMessageBox("OK");
                    var i = (ProjectInfo)element;
                    var p = i.LookupParameter("Project Config File");
                    if (p != null) {
                        return p.AsString();
                        //// SCaddinsApp.WindowManager.ShowMessageBox(p.AsString());
                    }
                }
            }
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

            titleBlocks = AllTitleBlocks(doc);

            return titleBlocks.TryGetValue(sheetNumber, out result) ? result : null;
        }

        public static void ToggleNorthPoints(ICollection<ExportSheet> sheets, Document doc, bool turnOn)
        {
            if (sheets == null)
            {
                return;
            }
            using (Transaction t = new Transaction(doc))
            {
                if (t.Start("SCexport - Toggle North Points") == TransactionStatus.Started)
                {
                    foreach (ExportSheet sheet in sheets)
                    {
                        sheet.ToggleNorthPoint(turnOn);
                    }
                    if (t.Commit() != TransactionStatus.Committed)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox("Failure", "Could not toggle north points");
                    }
                }
            }
        }

        public void AddExportOption(ExportOptions exportOptions)
        {
            exportFlags |= exportOptions;
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
                if (exportFlags.HasFlag(ExportOptions.DWG))
                {
                    ExportDWG(sheet, exportFlags.HasFlag(ExportOptions.NoTitle), log);
                }

                if (exportFlags.HasFlag(ExportOptions.PDF))
                {
                    ExportAdobePDF(sheet, log);
                }

////                if (exportFlags.HasFlag(ExportOptions.GhostscriptPDF))
////                {
////                    PostscriptPrinterName = "Microsoft Print to PDF";
////                    PdfPrinterName = "Microsoft Print to PDF";
////                    ExportMSPDF(sheet, log);
////                }
            }
            else
            {
                log.AddError(sheet.FullExportName, Resources.MessageNoPrintSettingAssigned);
            }
            log.EndLoggingIndividualItem(startTime, null);
        }

        public void SaveViewSet(string name, List<ExportSheet> selectedSheets)
        {
            var viewSetItem = new ViewSetItem(-1, name, selectedSheets.Select(s => s.Id.IntegerValue).ToList());
            AllViewSheetSets.Add(viewSetItem);
            using (Transaction t = new Transaction(Doc)) {
                if (t.Start("Save view sheet set") == TransactionStatus.Started) {
                    var set = new ViewSet();
                    foreach (var s in selectedSheets) {
                        set.Insert(s.Sheet);
                    }
                    var pm = Doc.PrintManager;
                    ////pm.ViewSheetSetting.
                    pm.PrintRange = PrintRange.Select;
                    pm.ViewSheetSetting.CurrentViewSheetSet = pm.ViewSheetSetting.InSession;
                    pm.ViewSheetSetting.InSession.Views = set;
                    ////pm.Apply();
                    pm.ViewSheetSetting.SaveAs(name);
                    if (t.Commit() != TransactionStatus.Committed) {
                        t.RollBack();
                    }
                } else {
                    t.RollBack();
                }
            }
        }

        public bool GSSanityCheck()
        {
            if (!Directory.Exists(GhostscriptBinDirectory) || !Directory.Exists(GhostscriptLibDirectory))
            {
                return false;
            }
            var ps = new PrinterSettings();
            ps.PrinterName = PostscriptPrinterName;
            return ps.IsValid;
        }

        public bool HasExportOption(ExportOptions option)
        {
            return exportFlags.HasFlag(option);
        }

        public void LoadSettings()
        {
            GhostscriptBinDirectory = Settings1.Default.GSBinDirectory;
            PdfPrinterName = Settings1.Default.AdobePrinterDriver;
            PrinterNameA3 = Settings1.Default.A3PrinterDriver;
            PrinterNameLargeFormat = Settings1.Default.LargeFormatPrinterDriver;
            PostscriptPrinterName = Settings1.Default.PSPrinterDriver;
            GhostscriptLibDirectory = Settings1.Default.GSLibDirectory;
            exportDirectory = Settings1.Default.ExportDir;
            AcadVersion = ACADVersion.Default;
            SaveHistory = Settings1.Default.SaveHistory;
            ShowExportLog = Settings1.Default.ShowExportLog;
            ForceRevisionToDateString = Settings1.Default.ForceDateRevision;
            UseDateForEmptyRevisions = Settings1.Default.UseDateForEmptyRevisions;
            VerifyOnStartup = Settings1.Default.VerifyOnStartup;
        }

        public bool PDFSanityCheck()
        {
            var ps = new PrinterSettings();
            ps.PrinterName = PdfPrinterName;
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
            var startTime = log.StartLoggingIndividualItem(null);
            log.AddMessage(sheet.ToString());

            PrintManager pm = Doc.PrintManager;
            bool printSettingsValid;
            if (!sheet.Verified)
            {
                sheet.UpdateSheetInfo();
            }
            printSettingsValid = false;

            switch (scale)
            {
                case 3:
                    printSettingsValid |= PrintSettings.PrintToDevice(Doc, "A3-FIT", pm, printerName, sheet.ForceRasterPrint, log);
                    break;

                case 2:
                    printSettingsValid |= PrintSettings.PrintToDevice(Doc, "A2-FIT", pm, printerName, sheet.ForceRasterPrint, log);
                    break;

                default:
                    int i = int.Parse(sheet.PageSize.Substring(1, 1), CultureInfo.InvariantCulture);
                    string printerNameTmp = i > 2 ? PrinterNameA3 : PrinterNameLargeFormat;
                    printSettingsValid |= PrintSettings.PrintToDevice(Doc, sheet.PageSize, pm, printerNameTmp, sheet.ForceRasterPrint, log);
                    break;
            }
            if (printSettingsValid)
            {
                pm.SubmitPrint(sheet.Sheet);
            }
            else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("test", "print error");
            }
            log.EndLoggingIndividualItem(startTime, null);
        }

        public void RemoveExportOption(ExportOptions exportOptions)
        {
            exportFlags = exportFlags & ~exportOptions;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Caliburn.Micro")]
        public void SetFileNameScheme(string newScheme)
        {
            foreach (SegmentedSheetName scheme in FileNameTypes)
            {
                if (newScheme == scheme.Name)
                {
                    FileNameScheme = scheme;
                    foreach (ExportSheet sheet in AllSheets)
                    {
                        sheet.SetSegmentedSheetName(FileNameScheme);
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
                if (PrintSettings.SetPrinterByName(Doc, PdfPrinterName, pm)) {
                    foreach (ExportSheet sc in AllSheets) {
                        if (!sc.Verified) {
                            sc.UpdateSheetInfo();
                        }
                    }
                }
            }
            catch {
                Debug.WriteLine("Update error in ExportManager.");
            }
        }

        internal void LoadConfigFile()
        {
            FileNameTypes.Clear();
            var config = GetConfigFileName(Doc);
            var b = ImportXMLinfo(config);
            if (!b || FileNameTypes.Count <= 0)
            {
                var name = new SegmentedSheetName();
                name.Name = "YYYYMMDD-AD-NNN";
                name.NameFormat = "$projectNumber-$sheetNumber[$sheetRevision]";
                FileNameTypes.Add(name);
                FileNameScheme = name;
            }
        }

        internal void PopulateSheets(ObservableCollection<ExportSheet> s)
        {
            s.Clear();
            using (var collector = new FilteredElementCollector(Doc))
            {
                collector.OfCategory(BuiltInCategory.OST_Sheets);
                collector.OfClass(typeof(ViewSheet));
                foreach (var element in collector)
                {
                    var v = (ViewSheet)element;
                    var scxSheet = new ExportSheet(v, Doc, FileNameTypes[0], VerifyOnStartup, this);
                    s.Add(scxSheet);
                }
            }
        }

        private static Dictionary<string, FamilyInstance> AllTitleBlocks(Document document)
        {
            var result = new Dictionary<string, FamilyInstance>();

            using (var collector = new FilteredElementCollector(document))
            {
                collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
                collector.OfClass(typeof(FamilyInstance));
                foreach (var element in collector)
                {
                    var e = (FamilyInstance)element;
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
            Registry.SetValue(
                Constants.HungAppTimeout,
                "HungAppTimeout",
                "30000",
                RegistryValueKind.String);
        }

        private static bool IsViewerMode()
        {
            var mainWindowTitle = Process.GetCurrentProcess().MainWindowTitle;
            return mainWindowTitle.Contains("VIEWER");
        }

        private static ObservableCollection<ViewSetItem> GetAllViewSheetSets(Document doc)
        {
            var result = new ObservableCollection<ViewSetItem>();
            using (FilteredElementCollector collector = new FilteredElementCollector(doc)) {
                collector.OfClass(typeof(ViewSheetSet));
                foreach (var element in collector) {
                    var v = (ViewSheetSet)element;
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
            if (!(doc.GetElement(vs.Id) is View view)) {
                return;
            }
            var t = new Transaction(doc, "Hide Title");
            t.Start();
            try {
                if (hide) {
                    view.HideElements(title);
                } else {
                    view.UnhideElements(title);
                }
                t.Commit();
            } catch (ArgumentException e) {
                SCaddinsApp.WindowManager.ShowMessageBox("Revit", "cannot Hide Title: " + e.Message);
                t.RollBack();
            }
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PrinterJobControl")]
        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private static bool SetAcrobatExportRegistryVal(string fileName, ExportLog log)
        {
            var exe = Process.GetCurrentProcess().MainModule.FileName;
            try {
                log.AddMessage("Attempting to set Acrobat Registry Value with value");
                log.AddMessage("\t" + Constants.AcrobatPrinterJobControl);
                log.AddMessage("\t" + exe);
                log.AddMessage("\t" + fileName);
                Registry.SetValue(
                    Constants.AcrobatPrinterJobControl,
                    exe,
                    fileName,
                    RegistryValueKind.String);
                return true;
            } catch (UnauthorizedAccessException ex) {
                log.AddError(fileName, ex.Message);
                return false;
            } catch (SecurityException ex) {
                log.AddError(fileName, ex.Message);
                return false;
            }
        }

        public void UpdateAllViewSheetSets()
        {
            AllViewSheetSets = GetAllViewSheetSets(Doc);
        }

        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private void ExportAdobePDF(ExportSheet vs, ExportLog log)
        {
            if (log != null) {
                log.AddMessage(Environment.NewLine + Resources.MessageStartingPDFExport);
            } else
            {
                return;
            }

            if (IsViewerMode())
            {
                log.AddError(vs.FullExportName, "Revit is in Viewer mode. Printing is not allowed.");
                return;
            }

            PrintManager pm = Doc.PrintManager;

            log.AddMessage(Resources.MessageApplyingPrintSetting + vs.PrintSettingName);

            if (!PrintSettings.PrintToFile(Doc, vs, pm, Resources.FileExtensionPDF, PdfPrinterName))
            {
                log.AddError(vs.FullExportName, Resources.ErrorFailedToAssignPrintSetting + vs.PrintSettingName);
                return;
            }

            if (!SetAcrobatExportRegistryVal(vs.FullExportPath(Resources.FileExtensionPDF), log))
            {
                log.AddError(vs.FullExportName, "Unable to write to registry.");
                return;
            }

            if (!FileUtilities.IsValidFileName(vs.FullExportName))
            {
                log.AddError(vs.FullExportName, "Filename contains invalid characters: " + vs.FullExportName);
                return;
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

                RunExportHooks(Resources.FileExtensionPDF, vs);

                SystemUtilities.KillAllProcesses("acrotray");
            } else
            {
                ////log.AddError(vs.FullExportName, Resources.ErrorCantOverwriteFile);
            }
        }

/*        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private bool ExportMSPDF(ExportSheet vs, ExportLog log)
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

            if (!PrintSettings.PrintToFile(Doc, vs, pm, Resources.FileExtensionPDF, PdfPrinterName)) {
                log.AddError(vs.FullExportName, Resources.ErrorFailedToAssignPrintSetting + vs.PrintSettingName);
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

                RunExportHooks(Resources.FileExtensionPDF, vs);

                ////SCaddins.Common.SystemUtilities.KillAllProcesses("acrotray");
            } else {
                ////log.AddError(vs.FullExportName, Resources.ErrorCantOverwriteFile);
                return false;
            }

            return true;
        }*/

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

            if (!FileUtilities.IsValidFileName(vs.FullExportName))
            {
                log.AddError(vs.FullExportName, "Filename contains invalid characters: " + vs.FullExportName);
                return;
            }

            List<ElementId> titleBlockHidden;
            titleBlockHidden = new List<ElementId>();
            var titleBlock = TitleBlockInstanceFromSheetNumber(vs.SheetNumber, Doc);
            titleBlockHidden.Add(titleBlock.Id);

            if (removeTitle) {
                log.AddMessage(Resources.MessageAttemptingToHideTitleBlock);
                RemoveTitleBlock(vs, titleBlockHidden, true, Doc);
            }

            List<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);

            if (ExportViewportsOnly)
            {
                foreach (var viewOnSheet in vs.Sheet.GetAllPlacedViews()) {
                    var individualViewOnSheet = Doc.GetElement(viewOnSheet) as View;
                    if (individualViewOnSheet.ViewType == ViewType.FloorPlan || individualViewOnSheet.ViewType == ViewType.CeilingPlan)
                    {
                        views.Add(individualViewOnSheet.Id);
                    }
                }
            }

            using (var opts = GetDefaultDWGExportOptions()) {
                log.AddMessage(Resources.MessageAssigningExportOptions + opts);
                var name = vs.FullExportName + Resources.FileExtensionDWG;
                log.AddMessage(Resources.MessageExportingToDirectory + vs.ExportDirectory);
                log.AddMessage(Resources.MessageExportingToFileName + name);
                Doc.Export(vs.ExportDirectory, name, views, opts);
            }

            FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionDWG));
            RunExportHooks("dwg", vs);

            if (removeTitle) {
                log.AddMessage(Resources.MessageShowingTitleBlock);
                RemoveTitleBlock(vs, titleBlockHidden, false, Doc);
            }
        }

/*        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Parameter name required by ps2pdf", MessageId = "sPAPERSIZE")]
        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private bool ExportGSPDF(ExportSheet vs, ExportLog log)
        {
            log.AddMessage(Environment.NewLine + Resources.MessageStartingGhostscriptPDFExport);
            log.AddMessage(vs.ToString());

            PrintManager pm = Doc.PrintManager;

            log.AddMessage(Resources.MessageApplyingPrintSetting + vs.PrintSettingName);

            if (!PrintSettings.PrintToFile(Doc, vs, pm, Resources.FileExtensionPS, PostscriptPrinterName)) {
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

            string prog = "\"" + GhostscriptLibDirectory + @"\ps2pdf" + "\"";
            string size = vs.PageSize.ToLower(CultureInfo.CurrentCulture);
            string sizeFix = size.ToLower(CultureInfo.CurrentCulture).Replace("p", string.Empty);
            string args =
                "-sPAPERSIZE#" +
                sizeFix + " \"" + vs.FullExportPath(Resources.FileExtensionPS) +
                "\" \"" + vs.FullExportPath(Resources.FileExtensionPDF) + "\"";

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(Resources.FileExtensionPDF))) {
                log.AddMessage("Converting to PDF with: " + prog + " " + args);
                ConsoleUtilities.StartHiddenConsoleProg(prog, args);
                FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionPDF));
                RunExportHooks("pdf", vs);
            } else {
                log.AddWarning(vs.FullExportName, Resources.MessageUnableToOverwriteExistingFile + vs.FullExportPath(Resources.FileExtensionPDF));
            }

            return true;
        }*/

        private DWGExportOptions GetDefaultDWGExportOptions()
        {
            var opts = new DWGExportOptions();
            opts.MergedViews = true;
            opts.FileVersion = AcadVersion;
            opts.HideScopeBox = true;
            opts.HideUnreferenceViewTags = true;
            if (ExportViewportsOnly) {
                opts.TargetUnit = ExportUnit.Meter;
                opts.SharedCoords = true;
            }
            return opts;
        }

        private bool ImportXMLinfo(string filename)
        {
            if (!File.Exists(filename)) {
                return false;
            }
            ////return true;
            if (!ValidateXML(filename)) {
                return false;
            }

            postExportHooks.Clear();

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
                        postExportHooks.Add(hook.Name, hook);
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
                        FileNameTypes.Add(name);
                    }
                }

                if (FileNameTypes.Count > 0) {
                    FileNameScheme = FileNameTypes[0];
                    foreach (ExportSheet sheet in AllSheets) {
                        sheet.SetSegmentedSheetName(FileNameScheme);
                    }
                }
            }
            return true;
        }

        private void RunExportHooks(string extension, ExportSheet vs)
        {
            for (int i = 0; i < postExportHooks.Count; i++) {
                if (postExportHooks.ElementAt(i).Value.HasExtension(extension))
                {
                    if (FileNameScheme.Hooks.Count < 1) {
                        return;
                    }

                    if (FileNameScheme.Hooks.Contains(postExportHooks.ElementAt(i).Key)) {
                        postExportHooks.ElementAt(i).Value.Run(vs, extension);
                    }
                }
            }
        }

        private void SetDefaultFlags()
        {
            if (Settings1.Default.AdobePDFMode && PDFSanityCheck()) {
                AddExportOption(ExportOptions.PDF);
            } else if (!Settings1.Default.AdobePDFMode && GSSanityCheck()) {
                AddExportOption(ExportOptions.GhostscriptPDF);
            } else {
                if (PDFSanityCheck()) {
                    AddExportOption(ExportOptions.PDF);
                }
                AddExportOption(ExportOptions.DWG);
            }

            if (Settings1.Default.HideTitleBlocks) {
                AddExportOption(ExportOptions.NoTitle);
            }

            forceDate |= Settings1.Default.ForceDateRevision;
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
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
#pragma warning disable CA3075 // Insecure DTD processing in XML
                    var document = new XmlDocument();
#pragma warning restore CA3075 // Insecure DTD processing in XML
                    document.Load(reader);
                    var eventHandler =
                        new ValidationEventHandler(ValidationEventHandler);
                    document.Validate(eventHandler);
                }
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
            SCaddinsApp.WindowManager.ShowMessageBox("SCexport - XML Config error", errorMessage);
            return false;
        }

        private void ValidationEventHandler(
                    object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    SCaddinsApp.WindowManager.ShowMessageBox("Error: {0}", e.Message);
                    break;

                case XmlSeverityType.Warning:
                    SCaddinsApp.WindowManager.ShowMessageBox("Warning {0}", e.Message);
                    break;
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */