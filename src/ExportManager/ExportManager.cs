// (C) Copyright 2012-2016 by Andrew Nicholas
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

    public class ExportManager
    {
        private static Dictionary<string, FamilyInstance> titleBlocks;
        private static string activeDoc;
        private ExportOptions exportFlags;
        private ExportLog log;
        private List<SegmentedSheetName> fileNameTypes;
        private ObservableCollection<ViewSheetSetCombo> allViewSheetSets;
        private Dictionary<string, PostExportHookCommand> postExportHooks;
        private SegmentedSheetName fileNameScheme;
        private ObservableCollection<ExportSheet> allSheets;
        private bool forceDate;
        private bool dateForEmptyRevisions;
        private string exportDirectory;

        public ExportManager(UIDocument uidoc)
        {
            Doc = uidoc.Document;
            UIDoc = uidoc;
            this.fileNameScheme = null;
            this.exportDirectory = Constants.DefaultExportDirectory;
            ExportManager.ConfirmOverwrite = true;
            ExportManager.activeDoc = null;
            this.log = new ExportLog();
            this.allViewSheetSets = new ObservableCollection<ViewSheetSetCombo>();
            this.allSheets = new ObservableCollection<ExportSheet>();
            this.fileNameTypes = new List<SegmentedSheetName>();
            this.postExportHooks = new Dictionary<string, PostExportHookCommand>();
            this.exportFlags = ExportOptions.None;
            this.LoadSettings();
            this.SetDefaultFlags();
            ExportManager.PopulateViewSheetSets(this.allViewSheetSets, Doc);
            this.PopulateSheets(this.allSheets);
            ExportManager.FixAcrotrayHang();
        }

        public static bool ConfirmOverwrite
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

        public string PdfPrinterName
        {
            get; set;
        }

        public string PostscriptPrinterName
        {
            get; set;
        }

        public string GhostscriptLibDirectory
        {
            get; set;
        }

        public string GhostscriptBinDirectory
        {
            get; set;
        }

        public List<SegmentedSheetName> FileNameTypes
        {
            get { return this.fileNameTypes; }
        }

        public ObservableCollection<ExportSheet> AllSheets
        {
            get { return this.allSheets; }
        }

        public ObservableCollection<ViewSheetSetCombo> AllViewSheetSets
        {
            get { return this.allViewSheetSets; }
        }

        public ACADVersion AcadVersion
        {
            get
            {
                return Settings1.Default.AcadExportVersion;
            }
            set
            {
                if (value == Settings1.Default.AcadExportVersion) return;
                Settings1.Default.AcadExportVersion = value;
            }
        }

        public bool UseDateForEmptyRevisions
        {
            get { return dateForEmptyRevisions; }
            set
            {
                this.dateForEmptyRevisions = value;
                foreach (ExportSheet sheet in this.allSheets) {
                    sheet.UseDateForEmptyRevisions = value;
                }
            }
        }

        public UIDocument UIDoc
        {
            get; set;
        }

        public Document Doc
        {
            get; set;
        }

        public bool ForceRevisionToDateString
        {
            get {
                return this.forceDate;
            }

            set {
                this.forceDate = value;
                foreach (ExportSheet sheet in this.allSheets) {
                    sheet.ForceDate = value;
                }
            }
        }

        public string ExportDirectory {
            get {
                return this.exportDirectory;
            }

            set {
                if (value != null) {
                    this.exportDirectory = value;
                    foreach (ExportSheet sheet in this.allSheets) {
                        sheet.ExportDirectory = value;
                    }
                }
            }
        }

        public SegmentedSheetName FileNameScheme
        {
            get
            {
                return this.fileNameScheme;
            }
            set
            {
                this.fileNameScheme = value;
                foreach (ExportSheet sheet in allSheets)
                {
                    sheet.SetSegmentedSheetName(fileNameScheme);
                }
            }
        }

        public bool ShowExportLog {
            get; set;
        }

        public static FamilyInstance TitleBlockInstanceFromSheetNumber(
            string sheetNumber, Document doc)
        {
            if (doc == null) {
                return null;
            }

            FamilyInstance result;
            if ((titleBlocks == null) || (activeDoc != FileUtilities.GetCentralFileName(doc))) {
                activeDoc = FileUtilities.GetCentralFileName(doc);
                titleBlocks = AllTitleBlocks(doc);
            }

            if (titleBlocks.TryGetValue(sheetNumber, out result)) {
                return result;
            } else {
                titleBlocks = AllTitleBlocks(doc);
            }

            return titleBlocks.TryGetValue(sheetNumber, out result) ? result : null;
        }

        public static string CreateSCexportConfig(Document doc)
        {
            string s = GetConfigFileName(doc);
            return File.Exists(s) ? s : null;
        }

        public static string GetOldConfigFileName(Document doc)
        {
            string central = FileUtilities.GetCentralFileName(doc);
            string s = Path.GetDirectoryName(central) + Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(central) + Resources.FileExtensionXML;
            return s;
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

        public static void OpenPreviousSheet(UIDocument udoc, ViewSheet view)
        {
            OpenSheet.OpenNextSheet(udoc, view);
        }

        public static void OpenNextSheet(UIDocument udoc, ViewSheet view)
        {
            OpenSheet.OpenPreviousSheet(udoc, view);
        }

        public static void FixScaleBars(ICollection<ExportSheet> sheets, Document doc)
        {
            if (sheets == null) {
                TaskDialog.Show("Error", "Please select sheets before attempting to add revisions");
                return;
            }
            using (Transaction t = new Autodesk.Revit.DB.Transaction(doc)) {
                if (t.Start("SCexport - Fix Scale Bars") == TransactionStatus.Started) {
                    foreach (ExportSheet sheet in sheets) {
                        if (!sheet.ValidScaleBar) {
                            sheet.UpdateScaleBarScale();
                        }
                    }
                    if (t.Commit() != TransactionStatus.Committed) {
                        TaskDialog.Show("Failure", "Could not fix scale bars");
                    }
                }
            }
        }

        public static void ToggleNorthPoints(ICollection<ExportSheet> sheets, Document doc, bool turnOn)
        {
            if (sheets == null) {
                return;
            }
            using (Transaction t = new Autodesk.Revit.DB.Transaction(doc)) {
                if (t.Start("SCexport - Toggle North Points") == TransactionStatus.Started) {
                    foreach (ExportSheet sheet in sheets) {
                        sheet.ToggleNorthPoint(turnOn);
                    }
                    if (t.Commit() != TransactionStatus.Committed) {
                        TaskDialog.Show("Failure", "Could not toggle north points");
                    }
                }
            }
        }

        public static string LatestRevisionDate(Document doc)
        {
            string s = string.Empty;
            int i = -1;
            using (FilteredElementCollector collector = new FilteredElementCollector(doc)) {
                collector.OfCategory(BuiltInCategory.OST_Revisions);
                foreach (Element e in collector) {
                    int j = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger();
                    if (j > i) {
                        i = j;
                        s = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString();
                    }
                }
            }
            return (s.Length > 1) ? s : string.Empty;
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

        public void Print(
            ExportSheet sheet,
            string printerName,
            int scale)
        {
            PrintManager pm = Doc.PrintManager;
            bool printSetttingsValid;
            if (!sheet.Verified) {
                sheet.UpdateSheetInfo();
            }
            printSetttingsValid = false;

            switch (scale) {
                case 3:
                printSetttingsValid |= PrintSettings.PrintToDevice(Doc, "A3-FIT", pm, printerName, this.log);
                break;
                case 2:
                printSetttingsValid |= PrintSettings.PrintToDevice(Doc, "A2-FIT", pm, printerName, this.log);
                break;
                default:
                int i = int.Parse(sheet.PageSize.Substring(1, 1), CultureInfo.InvariantCulture);
                string printerNameTmp = i > 2 ? this.PrinterNameA3 : this.PrinterNameLargeFormat;
                printSetttingsValid |= PrintSettings.PrintToDevice(Doc, sheet.PageSize, pm, printerNameTmp, this.log);
                break;
            }
            if (printSetttingsValid) {
                pm.SubmitPrint(sheet.Sheet);
            } else {
                TaskDialog.Show("test", "print error");
            }
        }

        public void Update()
        {
            try {
                PrintManager pm = Doc.PrintManager;
                if (pm == null) return;
                if (PrintSettings.SetPrinterByName(Doc, this.PdfPrinterName, pm)) {
                    foreach (ExportSheet sc in this.allSheets) {
                        if (!sc.Verified) {
                            sc.UpdateSheetInfo();
                        }
                    }
                }
            } catch { }
        }

        public void AddExportOption(ExportOptions exportOptions)
        {
            this.exportFlags |= exportOptions;
        }

        public void RemoveExportOption(ExportOptions exportOptions)
        {
            this.exportFlags = this.exportFlags & ~exportOptions;
        }

        public bool HasExportOption(ExportOptions option)
        {
            return this.exportFlags.HasFlag(option);
        }

        public void SetFileNameScheme(string newScheme)
        {
            foreach (SegmentedSheetName scheme in this.fileNameTypes) {
                if (newScheme == scheme.Name) {
                    this.fileNameScheme = scheme;
                    foreach (ExportSheet sheet in this.allSheets) {
                        sheet.SetSegmentedSheetName(this.fileNameScheme);
                    }
                }
            }
        }

        public bool GSSanityCheck()
        {
            if (!Directory.Exists(this.GhostscriptBinDirectory) || !Directory.Exists(this.GhostscriptLibDirectory)) {
                return false;
            }
            var ps = new System.Drawing.Printing.PrinterSettings();
            ps.PrinterName = this.PostscriptPrinterName;
            return ps.IsValid;
        }

        public bool PDFSanityCheck()
        {
            var ps = new System.Drawing.Printing.PrinterSettings();
            ps.PrinterName = this.PdfPrinterName;
            return ps.IsValid;
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
            this.AcadVersion = SCaddins.ExportManager.Settings1.Default.AcadExportVersion;
            this.ShowExportLog = SCaddins.ExportManager.Settings1.Default.ShowExportLog;
            this.ForceRevisionToDateString = SCaddins.ExportManager.Settings1.Default.ForceDateRevision;
            this.UseDateForEmptyRevisions = SCaddins.ExportManager.Settings1.Default.UseDateForEmptyRevisions;
        }

        //[SecurityCritical]
        //internal void ExportSheets(
        // ICollection<ExportSheet> sheets) {
        //    if (sheets == null) {
        //        return;
        //    }

        //    DateTime startTime = DateTime.Now;
        //    TimeSpan elapsedTime = DateTime.Now - startTime;
        //    PrintManager pm = Doc.PrintManager;
        //    PrintSettings.SetPrinterByName(Doc, this.PdfPrinterName, pm);
        //    this.log.Clear();
        //    this.log.Start(Resources.ExportStarted);

        //    foreach (ExportSheet sheet in sheets) {
        //        elapsedTime = DateTime.Now - startTime;
        //        this.ExportSheet(sheet);
        //    }

        //    this.log.Stop(Resources.ExportComplete);

        //    #if DEBUG
        //    this.log.ShowSummaryDialog();
        //    #else
        //    if (this.log.Errors > 0 || this.ShowExportLog) {
        //        this.log.ShowSummaryDialog();
        //    }
        //    #endif
        //}

        private static Dictionary<string, FamilyInstance> AllTitleBlocks(Document document)
        {
            var result = new Dictionary<string, FamilyInstance>();

            using (var collector = new FilteredElementCollector(document)) {
                collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
                collector.OfClass(typeof(FamilyInstance));
                foreach (FamilyInstance e in collector) {
                    var s = e.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString();
                    if (!result.ContainsKey(s)) {
                        result.Add(s, e);
                    }
                }
            }

            return result;
        }

        private static string PercentageSting(int n, int total)
        {
            var result = "Exporting " + n + " of " + total +
                " (" + (int)(((double)n / (double)total) * 100) + @"%)";
            return result;
        }

        private static string TimeSpanAsString(TimeSpan time)
        {
            var result = "Elapsed Time: " +
                MiscUtilities.PadLeftZeros(time.Hours.ToString(CultureInfo.CurrentCulture), 2) + "h:" +
                MiscUtilities.PadLeftZeros(time.Minutes.ToString(CultureInfo.CurrentCulture), 2) + "m:" +
                MiscUtilities.PadLeftZeros(time.Seconds.ToString(CultureInfo.CurrentCulture), 2) + "s";
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PrinterJobControl")]
        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private static void SetAcrobatExportRegistryVal(string fileName, ExportLog log)
        {
            string exe =
                Process.GetCurrentProcess().MainModule.FileName;
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
            } catch (UnauthorizedAccessException ex) {
                log.AddError(fileName, ex.Message);
            } catch (SecurityException ex) {
                log.AddError(fileName, ex.Message);
            }
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

        private static void PopulateViewSheetSets(ObservableCollection<ViewSheetSetCombo> vss, Document doc)
        {
            vss.Clear();
            vss.Add(new ViewSheetSetCombo("<All Sheets in Model>"));
            using (FilteredElementCollector collector = new FilteredElementCollector(doc)) {
                collector.OfClass(typeof(ViewSheetSet));
                foreach (ViewSheetSet v in collector) {
                    vss.Add(new ViewSheetSetCombo(v));
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


        public static void AddRevisions(ICollection<ExportSheet> sheets, ElementId revisionId, Document doc)
        {
            if (sheets == null || revisionId == null || revisionId == null) {
                return;
            }

            using (var t = new Transaction(doc, "SCexport: Add new revisions"))
            {
                if (t.Start() == TransactionStatus.Started) {
                    foreach (ExportSheet sheet in sheets) {
                        ICollection<ElementId> il = sheet.Sheet.GetAdditionalRevisionIds();
                        il.Add(revisionId);
                        sheet.Sheet.SetAdditionalRevisionIds(il);
                        sheet.UpdateRevision(true);
                    }
                    doc.Regenerate();
                    t.Commit();
                } else {
                    TaskDialog.Show("Error", "SCexport: error adding revisions, could not start transaction.");
                }
            }
            
            foreach (ExportSheet sheet in sheets) {
                sheet.UpdateRevision(true);
            }
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
                    var scxSheet = new ExportSheet(v, Doc, this.fileNameTypes[0], this);
                    s.Add(scxSheet);
                }
            }
        }

        private void ValidationEventHandler(
            object sender, ValidationEventArgs e)
        {
            switch (e.Severity) {
                case XmlSeverityType.Error:
                    TaskDialog.Show("Error: {0}", e.Message);
                    break;
                case XmlSeverityType.Warning:
                    TaskDialog.Show("Warning {0}", e.Message);
                    break;
            }
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
                var document = new XmlDocument();
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

        private bool ImportXMLinfo(string filename)
        {
            if (!System.IO.File.Exists(filename)) {
                return false;
            }
            if (!this.ValidateXML(filename)) {
                return false;
            }

            using (var reader = new XmlTextReader(filename)) {
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

        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public void ExportSheet(ExportSheet sheet)
        {
            if (!sheet.Verified) {
                sheet.UpdateSheetInfo();
            }

            if (sheet.SCPrintSetting != null) {
                var startTime = DateTime.Now;
                if (this.exportFlags.HasFlag(ExportOptions.DWG)) {             
                    this.ExportDWG(sheet, this.exportFlags.HasFlag(ExportOptions.NoTitle));
                }

                if (this.exportFlags.HasFlag(ExportOptions.PDF)) {
                    this.ExportAdobePDF(sheet);
                }

                if (this.exportFlags.HasFlag(ExportOptions.GhostscriptPDF)) {
                    this.ExportGSPDF(sheet);
                }

                //if (this.exportFlags.HasFlag(ExportOptions.DGN)) {
                //    //ExportManager.ExportDGN(sheet, Doc);
                //}

                //if (this.exportFlags.HasFlag(ExportOptions.DWF)) {
                //    //ExportManager.ExportDWF(sheet, Doc);
                //}
                var elapsedTime = DateTime.Now - startTime;
                this.log.AddMessage(Resources.MessageElapsedTimeForLastExport + elapsedTime.ToString());
            } else {
                this.log.AddError(sheet.FullExportName, Resources.MessageNoPrintSettingAssigned);
            }
        }

        private DWGExportOptions GetDefaultDWGExportOptions()
        {
            var opts = new DWGExportOptions();
            opts.MergedViews = true;
            opts.FileVersion = this.AcadVersion;
            opts.HideScopeBox = true;
            opts.HideUnreferenceViewTags = true;
            return opts;
        }

        // FIXME this is nasty
        private void ExportDWG(ExportSheet vs, bool removeTitle)
        {
            this.log.AddMessage(Environment.NewLine + Resources.MessageStartingDWGExport);
            this.log.AddMessage(vs.ToString());
            
            List<ElementId> titleBlockHidden;
            titleBlockHidden = new List<ElementId>();
            var titleBlock = ExportManager.TitleBlockInstanceFromSheetNumber(vs.SheetNumber, Doc);
            titleBlockHidden.Add(titleBlock.Id);

            if (removeTitle) {
                this.log.AddMessage(Resources.MessageAttemptingToHideTitleBlock);
                ExportManager.RemoveTitleBlock(vs, titleBlockHidden, true, Doc);
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
                        this.log.AddWarning(null, Resources.MessageCouldNotApplyPrintSettings);
                        t.RollBack();
                    }
                } 
            }

            List<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);

            using (var opts = GetDefaultDWGExportOptions()) {
                this.log.AddMessage(Resources.MessageAssigningExportOptions + opts);
                pm.PrintRange = PrintRange.Select;
                var name = vs.FullExportName + Resources.FileExtensionDWG;
                this.log.AddMessage(Resources.MessageExportingToDirectory + vs.ExportDirectory);
                this.log.AddMessage(Resources.MessageExportingToFileName + name);
                Doc.Export(vs.ExportDirectory, name, views, opts);
            }
           
            FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionDWG));
            this.RunExportHooks("dwg", vs);

            if (removeTitle) {
                this.log.AddMessage(Resources.MessageShowingTitleBlock);
                ExportManager.RemoveTitleBlock(vs, titleBlockHidden, false, Doc);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "sPAPERSIZE")]
        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private bool ExportGSPDF(ExportSheet vs)
        {
            this.log.AddMessage(Environment.NewLine + Resources.MessageStartingGhostscriptPDFExport);
            this.log.AddMessage(vs.ToString());
            
            PrintManager pm = Doc.PrintManager;
            
            this.log.AddMessage(Resources.MessageApplyingPrintSetting + vs.PrintSettingName);

            if (!PrintSettings.PrintToFile(Doc, vs, pm, Resources.FileExtensionPS, this.PostscriptPrinterName)) {
                this.log.AddError(vs.FullExportName, Resources.ErrorFailedToAssignPrintSetting + vs.PrintSettingName);
                return false;
            }
            
            this.log.AddMessage(Resources.MessageSubmittingPrint);

            try {
                pm.SubmitPrint(vs.Sheet);
            } catch (InvalidOperationException) {
                File.Delete(vs.FullExportPath(Resources.FileExtensionPS));
                pm.SubmitPrint(vs.Sheet);
            }
            
            this.log.AddMessage(Resources.StartingPrint + vs.FullExportPath(Resources.FileExtensionPS));

            FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionPS));
            
            this.log.AddMessage(Resources.OK);
            
            string prog = "\"" + this.GhostscriptLibDirectory  + @"\ps2pdf" + "\"";
            string size = vs.PageSize.ToLower(CultureInfo.CurrentCulture);
            string sizeFix = size.ToLower(CultureInfo.CurrentCulture).Replace("p", string.Empty);   
            string args = 
                "-sPAPERSIZE#" +
                sizeFix + " \"" + vs.FullExportPath(Resources.FileExtensionPS) +
                "\" \"" + vs.FullExportPath(Resources.FileExtensionPDF) + "\"";

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(Resources.FileExtensionPDF))) {
                this.log.AddMessage("Converting to PDF with: " + prog + " " + args);
                SCaddins.Common.ConsoleUtilities.StartHiddenConsoleProg(prog, args);
                FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionPDF));
                this.RunExportHooks("pdf", vs);
            } else {
                this.log.AddWarning(vs.FullExportName, Resources.MessageUnableToOverwriteExistingFile + vs.FullExportPath(Resources.FileExtensionPDF));    
            }

            return true;
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

        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private bool ExportAdobePDF(ExportSheet vs)
        {
            PrintManager pm = Doc.PrintManager;
            
            this.log.AddMessage(Resources.MessageApplyingPrintSetting + vs.PrintSettingName);

            if (!PrintSettings.PrintToFile(Doc, vs, pm, Resources.FileExtensionPDF, this.PdfPrinterName)) {
                this.log.AddError(vs.FullExportName, Resources.ErrorFailedToAssignPrintSetting + vs.PrintSettingName);
                return false;
            }

            SetAcrobatExportRegistryVal(vs.FullExportPath(Resources.FileExtensionPDF), this.log);

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(Resources.FileExtensionPDF))) {
                if (File.Exists(vs.FullExportPath(Resources.FileExtensionPDF))) {
                    File.Delete(vs.FullExportPath(Resources.FileExtensionPDF));
                }
                this.log.AddMessage(Resources.MessageSubmittingPrint);
                if (pm.SubmitPrint(vs.Sheet)) {
                    this.log.AddMessage(Resources.MessageApparentlyCompletedSuccessfully);
                } else {
                    this.log.AddError(vs.FullExportName, Resources.ErrorFailedToPrint);    
                }
                FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionPDF));
                
                this.RunExportHooks(Resources.FileExtensionPDF, vs);
                             
                SCaddins.Common.SystemUtilities.KillAllProcesses("acrotray");
            } else {
                this.log.AddError(vs.FullExportName, Resources.ErrorCantOverwriteFile);
                return false;
            }
            
            return true;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
