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
    using System.Xml;
    using System.Xml.Schema;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using SCaddins.Common;
    using SCaddins.Properties;

    public class ExportManager
    {
        private static Dictionary<string, FamilyInstance> titleBlocks;
        private static Document doc;
        private static string activeDoc;
        private ExportOptions exportFlags;
        private ExportLog log;
        private Collection<SegmentedSheetName> fileNameTypes;
        private Collection<ViewSheetSetCombo> allViewSheetSets;
        private Dictionary<string, PostExportHookCommand> postExportHooks;
        private SegmentedSheetName fileNameScheme;
        private SortableBindingListCollection<ExportSheet> allSheets;
        private bool forceDate;
        private string exportDirectory;

        public ExportManager(Document doc)
        {
            ExportManager.doc = doc;
            this.fileNameScheme = null;
            this.exportDirectory = Constants.DefaultExportDirectory;
            ExportManager.ConfirmOverwrite = true;
            ExportManager.activeDoc = null;
            this.log = new ExportLog();
            this.allViewSheetSets = new Collection<ViewSheetSetCombo>();
            this.allSheets = new SortableBindingListCollection<ExportSheet>();
            this.fileNameTypes = new Collection<SegmentedSheetName>();
            this.postExportHooks = new Dictionary<string, PostExportHookCommand>();
            this.exportFlags = ExportOptions.None;
            this.LoadSettings();
            this.SetDefaultFlags();
            ExportManager.PopulateViewSheetSets(this.allViewSheetSets);
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

        public Collection<SegmentedSheetName> FileNameTypes
        {
            get { return this.fileNameTypes; }
        }

        public SortableBindingListCollection<ExportSheet> AllSheets
        {
            get { return this.allSheets; }
        }

        public Collection<ViewSheetSetCombo> AllViewSheetSets
        {
            get { return this.allViewSheetSets; }
        }

        public ExportOptions ExportOptions
        {
            get; set;
        }

        public ACADVersion AcadVersion
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

        public SegmentedSheetName FileNameScheme {
            get {
                return this.fileNameScheme;
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
            string s = Path.GetDirectoryName(central) + @"\" +
                Path.GetFileNameWithoutExtension(central) + ".xml";
            return s;
        }
        
        public static string GetConfigFileName(Document doc)
        {
            // if (File.Exists(GetOldConfigFileName(doc))) {
            //     TaskDialog.Show("Old config found");
            // }            
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
            OpenSheet(udoc, view, -1);
        }

        public static void OpenNextSheet(UIDocument udoc, ViewSheet view)
        {
            OpenSheet(udoc, view, 1);
        }

        public static void RenameSheets(ICollection<ExportSheet> sheets)
        {
            using (var renameSheetDialog = new RenameSheetForm(sheets, doc)) {
                var result = renameSheetDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    foreach (ExportSheet sheet in sheets) {
                        sheet.UpdateNumber();
                        sheet.UpdateName();
                    }
                }
            }
        }

        public static void FixScaleBars(ICollection<ExportSheet> sheets)
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
        
        public static void ToggleNorthPoints(ICollection<ExportSheet> sheets)
        {
            if (sheets == null) {
                return;
            }
            using (Transaction t = new Autodesk.Revit.DB.Transaction(doc)) {
                if (t.Start("SCexport - Toggle North Points") == TransactionStatus.Started) {
                    foreach (ExportSheet sheet in sheets) {
                        sheet.ToggleNorthPoint();
                    }
                    if (t.Commit() != TransactionStatus.Committed) {
                        TaskDialog.Show("Failure", "Could not toggle north points");
                    }
                }
            }
        }

        public static void AddRevisions(ICollection<ExportSheet> sheets)
        {
            if (sheets == null) {
                TaskDialog.Show("Error", "Please select sheets before attempting to add revisions");
                return;
            }
            using (var r = new RevisionSelectionDialog(doc)) {
                var result = r.ShowDialog();
                if ((r.Id != null) && (result == System.Windows.Forms.DialogResult.OK)) {
                    using (var t = new Transaction(doc, "SCexport: Add new revisions")) {
                        if (t.Start() == TransactionStatus.Started) {
                            foreach (ExportSheet sheet in sheets) {
                                ICollection<ElementId> il = sheet.Sheet.GetAdditionalRevisionIds();
                                il.Add(r.Id);
                                sheet.Sheet.SetAdditionalRevisionIds(il);
                            }
                            t.Commit();
                        } else {
                            TaskDialog.Show("Error", "SCexport: error adding revisions, could not start transaction.");
                        }
                    }
                }
            }
            foreach (ExportSheet sheet in sheets) {
                sheet.UpdateRevision(true);
            }
        }

        public static string CurrentViewName()
        {
            View v = doc.ActiveView;
            if (v.ViewType == ViewType.DrawingSheet) {
                return v.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString();
            } else {
                return null;
            }
        }

        public static ACADVersion AcadVersionFromString(string version)
        {
            if (version == "R2007") {
                return ACADVersion.R2007;
            }
            if (version == "R2010") {
                return ACADVersion.R2010;
            }
            return (version == "R2013") ? ACADVersion.R2013 : ACADVersion.Default;
        }

        public static string AcadVersionToString(ACADVersion version)
        {
            switch (version) {
                case ACADVersion.R2007:
                    return "R2007";
                case ACADVersion.R2010:
                    return "R2010";
                case ACADVersion.R2013:
                    return "R2013";
                default:
                    return "Default";
            }
        }

        public static string LatestRevisionDate()
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

        public void Print(
            ICollection<ExportSheet> sheets,
            string printerName,
            int scale,
            System.Windows.Forms.ToolStripProgressBar progressBar,
            System.Windows.Forms.ToolStripItem info,
            System.Windows.Forms.Control strip)
        {
            if (info == null || progressBar == null || strip == null) {
                return;
            }

            PrintManager pm = doc.PrintManager;
            TaskDialogResult tdr = ShowPrintWarning();
            DateTime startTime = DateTime.Now;
            TimeSpan elapsedTime = DateTime.Now - startTime;

            if (tdr == TaskDialogResult.Ok) { 
                bool printSetttingsValid;
                this.log.Clear();
                this.log.Start(Resources.StartingPrint);
                foreach (ExportSheet sheet in sheets.OrderBy(x => x.SheetNumber).ToList()) {
                    
                    progressBar.PerformStep();
                    elapsedTime = DateTime.Now - startTime;
                    info.Text = ExportManager.PercentageSting(progressBar.Value, progressBar.Maximum) +
                    " - " + ExportManager.TimeSpanAsString(elapsedTime);
                    strip.Update();
                    
                    if (!sheet.Verified) {
                        sheet.UpdateSheetInfo();
                    }
                    printSetttingsValid = false;

                    switch (scale) {
                    case 3:
                        printSetttingsValid |= PrintSettings.PrintToDevice(doc, "A3-FIT", pm, printerName, this.log);
                        break;
                    case 2:
                        printSetttingsValid |= PrintSettings.PrintToDevice(doc, "A2-FIT", pm, printerName, this.log);
                        break;
                    default:
                        int i = int.Parse(sheet.PageSize.Substring(1, 1), CultureInfo.InvariantCulture);
                        string printerNameTmp = i > 2 ? "this.PrinterNameA3" : this.PrinterNameLargeFormat;
                        printSetttingsValid |= PrintSettings.PrintToDevice(doc, sheet.PageSize, pm, printerNameTmp, this.log);
                        break;
                    }
                    if (printSetttingsValid) {
                        pm.SubmitPrint(sheet.Sheet);
                    }
                }
                this.log.Stop(Resources.FinishedPrint);
                #if DEBUG
                this.log.ShowSummaryDialog();
                #else
                if (this.log.Errors > 0) {
                    this.log.ShowSummaryDialog();
                }
                #endif
            }
        }

        public void Update()
        {
            PrintManager pm = doc.PrintManager;
            PrintSettings.SetPrinterByName(doc, this.PdfPrinterName, pm);

            foreach (ExportSheet sc in this.allSheets) {
                if (!sc.Verified) {
                    sc.UpdateSheetInfo();
                }
            }
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

        public int GetTotalNumberOfExports(ICollection<ExportSheet> sheets)
        {
            if (sheets == null) {
                TaskDialog.Show("Error", "Please select sheets before attempting to add revisions");
                return 0;
            }
            int i = 0;
            if (this.HasExportOption(ExportOptions.DGN)) {
                i++;
            }
            if (this.HasExportOption(ExportOptions.DWF)) {
                i++;
            }
            if (this.HasExportOption(ExportOptions.DWG)) {
                i++;
            }
            if (this.HasExportOption(ExportOptions.GhostscriptPDF)) {
                i++;
            }
            if (this.HasExportOption(ExportOptions.PDF)) {
                i++;
            }
            return i * sheets.Count;
        }

        [SecurityCritical]
        public void ExportSheets(
            ICollection<ExportSheet> sheets,
            System.Windows.Forms.ToolStripProgressBar progressBar,
            System.Windows.Forms.ToolStripItem info,
            System.Windows.Forms.Control strip)
        {
            if (progressBar == null || info == null || strip == null || sheets == null) {
                return;
            }

            DateTime startTime = DateTime.Now;
            TimeSpan elapsedTime = DateTime.Now - startTime;
            PrintManager pm = doc.PrintManager;
            PrintSettings.SetPrinterByName(doc, this.PdfPrinterName, pm);
            this.log.Clear();
            this.log.TotalExports = progressBar.Maximum;
            this.log.Start(Resources.ExportStarted);

            foreach (ExportSheet sheet in sheets) {
                progressBar.PerformStep();
                elapsedTime = DateTime.Now - startTime;
                info.Text = ExportManager.PercentageSting(progressBar.Value, progressBar.Maximum) +
                    " - " + ExportManager.TimeSpanAsString(elapsedTime);
                strip.Update();
                this.ExportSheet(sheet);
            }
            
            this.log.Stop(Resources.ExportComplete);

            #if DEBUG
            this.log.ShowSummaryDialog();
            #else
            if (this.log.Errors > 0 || this.ShowExportLog) {
                this.log.ShowSummaryDialog();
            }
            #endif
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
            this.AcadVersion = AcadVersionFromString(SCaddins.ExportManager.Settings1.Default.AcadExportVersion);
            this.ShowExportLog = SCaddins.ExportManager.Settings1.Default.ShowExportLog;
        }

        private static void OpenSheet(UIDocument udoc, ViewSheet view, int inc)
        {
            IList<ViewSheet> sheets = new List<ViewSheet>();
            using (var collector = new FilteredElementCollector(udoc.Document)) {
                collector.OfCategory(BuiltInCategory.OST_Sheets);
                collector.OfClass(typeof(ViewSheet)); 
                foreach (ViewSheet v in collector) {
                    sheets.Add(v);
                }
            }
            IEnumerable<ViewSheet> sortedEnum = sheets.OrderBy(f => f.SheetNumber);
            IList<ViewSheet> sortedSheets = sortedEnum.ToList();

            // FIXME don't allow overflow.
            for (int i = 0; i < sortedSheets.Count; i++) {
                if (sortedSheets[i].SheetNumber == view.SheetNumber) {
                    DialogHandler.AddRevitDialogHandler(new UIApplication(udoc.Application.Application));
                    FamilyInstance result = ExportManager.TitleBlockInstanceFromSheetNumber(sortedSheets[i + inc].SheetNumber, udoc.Document);
                    if (result != null) {
                        udoc.ShowElements(result);
                    }
                    return;
                }
            }
        }

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

        private static TaskDialogResult ShowPrintWarning()
        {
            using (var td = new TaskDialog("SCexport - Print Warning")) {
                td.MainInstruction = "Warning";
                td.MainContent = "The print feature is experimental, please only export a " +
                    "small selection of sheets until you are sure it is working correctly." +
                    System.Environment.NewLine + System.Environment.NewLine +
                    "Press ok to continue.";
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                td.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.No;
                TaskDialogResult tdr = td.Show();
                return tdr;
            }
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
        private static void SetAcrobatExportRegistryVal(string fileName, ExportLog log)
        {
            string exe =
                Process.GetCurrentProcess().MainModule.FileName;
            try {
                log.AddMessage("Attempting to set Acrobat Registry Value with value");
                log.AddMessage(@"   Reg: " + Constants.AcrobatPrinterJobControl);
                log.AddMessage(@"   Exe: " + exe);
                log.AddMessage(@"   Filename: " + fileName);
                Microsoft.Win32.Registry.SetValue(
                    Constants.AcrobatPrinterJobControl,
                    exe,
                    fileName,
                    Microsoft.Win32.RegistryValueKind.String);
            } catch (UnauthorizedAccessException ex) {
                log.AddError(fileName, @"Unauthorized Access Exception: cannot write to windows registry");
                log.AddError(fileName, ex.Message);
            } catch (SecurityException ex) {
                log.AddError(fileName, @"Security Exception: cannot write to windows registry");
                log.AddError(fileName, ex.Message);
            }
        }

        private static void RemoveTitleBlock(
            ExportSheet vs,
            ICollection<ElementId> title,
            bool hide)
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

        private static void PopulateViewSheetSets(Collection<ViewSheetSetCombo> vss)
        {
            vss.Clear();
            using (FilteredElementCollector collector = new FilteredElementCollector(doc)) {
                collector.OfClass(typeof(ViewSheetSet));
                foreach (ViewSheetSet v in collector) {
                    vss.Add(new ViewSheetSetCombo(v));
                }
            }
        }

        private static void ExportDWF(ExportSheet vs)
        {
            var views = new ViewSet();
            views.Insert(vs.Sheet);
            var opts = new DWFExportOptions();
            opts.CropBoxVisible = false;
            opts.ExportingAreas = true;
            using (var t = new Transaction(doc)) {
                if (t.Start("Export DWF") == TransactionStatus.Started) {
                    try {
                        string tmp = vs.FullExportName + ".dwf";
                        doc.Export(vs.ExportDirectory, tmp, views, opts);
                        t.Commit();
                    } catch (ArgumentException e) {
                        TaskDialog.Show("SCexport", "cannot export dwf: " + e.Message);
                        t.RollBack();
                    } catch (InvalidOperationException e) {
                        TaskDialog.Show("SCexport", "cannot export dwf: " + e.Message);
                        t.RollBack();
                    }
                }
            }
        }

        private static void ExportDGN(ExportSheet vs)
        {
            var opts = new DGNExportOptions();
            ICollection<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);
            doc.Export(vs.ExportDirectory, vs.FullExportName, views, opts);
            opts.Dispose();
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

        private void PopulateSheets(SortableBindingListCollection<ExportSheet> s)
        {
            string config = GetConfigFileName(doc);
            bool b = this.ImportXMLinfo(config);
            if (!b) {
                var name = new SegmentedSheetName();
                name.Name = "YYYYMMDD-AD-NNN";
                name.NameFormat = "$projectNumber-$sheetNumber[$sheetRevision]";
                this.fileNameTypes.Add(name);
                this.fileNameScheme = name;
            }
            if (this.fileNameTypes.Count <= 0) {
                var name = new SegmentedSheetName();
                name.Name = "YYYYMMDD-AD-NNN";
                name.NameFormat = "$projectNumber-$sheetNumber[$sheetRevision]";
                this.fileNameTypes.Add(name);
                this.fileNameScheme = name;
            }

            s.Clear();
            using (FilteredElementCollector collector = new FilteredElementCollector(doc)) {
                collector.OfCategory(BuiltInCategory.OST_Sheets);
                collector.OfClass(typeof(ViewSheet));
                foreach (ViewSheet v in collector) {
                    var scxSheet = new ExportSheet(v, doc, this.fileNameTypes[0], this);
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
            TaskDialog td = new TaskDialog("SCexport - XML Config error");
            td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            td.MainInstruction = "SCexport - XML Config error";
            td.MainContent = errorMessage;
            td.Show();
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
        private void ExportSheet(ExportSheet sheet)
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

                if (this.exportFlags.HasFlag(ExportOptions.DGN)) {
                    ExportManager.ExportDGN(sheet);
                }

                if (this.exportFlags.HasFlag(ExportOptions.DWF)) {
                    ExportManager.ExportDWF(sheet);
                }
                var elapsedTime = DateTime.Now - startTime;
                this.log.AddMessage("Elapsed Time for last export: " + elapsedTime.ToString());
            } else {
                this.log.AddError(sheet.FullExportName, "No print setting assigned.");
            }
        }

        private void ApplyDefaultDWGExportOptions(ref DWGExportOptions opts)
        {
            opts.MergedViews = true;
            opts.FileVersion = this.AcadVersion;
            opts.HideScopeBox = true;
            opts.HideUnreferenceViewTags = true;
        }

        //FIXME this is nasty
        private void ExportDWG(ExportSheet vs, bool removeTitle)
        {
            this.log.AddMessage(Environment.NewLine + "### Starting DWG Export ###");
            this.log.AddMessage(vs.ToString());
            
            ICollection<ElementId> titleBlockHidden;
            titleBlockHidden = new List<ElementId>();
            var titleBlock = ExportManager.TitleBlockInstanceFromSheetNumber(vs.SheetNumber, doc);
            titleBlockHidden.Add(titleBlock.Id);

            if (removeTitle) {
                this.log.AddMessage("Attempting to hide Title Block (temporarily)");
                ExportManager.RemoveTitleBlock(vs, titleBlockHidden, true);
            }

            PrintManager pm = doc.PrintManager;

            using (var t = new Transaction(doc, "Apply print settings")) {
                if (t.Start() == TransactionStatus.Started) {
                    try {
                        pm.PrintToFile.Equals(true);
                        pm.PrintRange = PrintRange.Select;
                        pm.Apply();
                        t.Commit();
                    } catch (InvalidOperationException) {
                        this.log.AddWarning(null, "Could not apply print settings");
                        t.RollBack();
                    }
                } 
            }

            ICollection<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);

            var opts = new DWGExportOptions();
            this.log.AddMessage("Assigning export options: " + opts);
            this.ApplyDefaultDWGExportOptions(ref opts);
            pm.PrintRange = PrintRange.Select;
            var name = vs.FullExportName + ".dwg";
            this.log.AddMessage("Exporting to directory: " + vs.ExportDirectory);
            this.log.AddMessage("Exporting to file name: " + name);
            doc.Export(vs.ExportDirectory, name, views, opts);
            opts.Dispose();
           
            FileUtilities.WaitForFileAccess(vs.FullExportPath(".dwg"));
            this.RunExportHooks("dwg", vs);

            if (removeTitle) {
                this.log.AddMessage("Showing Title Block");
                ExportManager.RemoveTitleBlock(vs, titleBlockHidden, false);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "sPAPERSIZE")]
        [SecurityCritical]
        private bool ExportGSPDF(ExportSheet vs)
        {
            this.log.AddMessage(Environment.NewLine + "### Starting Ghostscript PDF Export ###");
            this.log.AddMessage(vs.ToString());
            
            PrintManager pm = doc.PrintManager;
            
            this.log.AddMessage("Applying print setting: " + vs.PrintSettingName);

            if (!PrintSettings.PrintToFile(doc, vs, pm, ".ps", this.PostscriptPrinterName)) {
                this.log.AddError(vs.FullExportName, "failed to assign print setting: " + vs.PrintSettingName);
                return false;
            }
            
            this.log.AddMessage("Submitting Postscript print.");

            try {
                pm.SubmitPrint(vs.Sheet);
            } catch (InvalidOperationException) {
                File.Delete(vs.FullExportPath(".ps"));
                pm.SubmitPrint(vs.Sheet);
            }
            
            this.log.AddMessage("Printing: " + vs.FullExportPath(".ps"));

            FileUtilities.WaitForFileAccess(vs.FullExportPath(".ps"));
            
            this.log.AddMessage("...OK");
            
            string prog = "\"" + this.GhostscriptLibDirectory  + @"\ps2pdf" + "\"";
            string size = vs.PageSize.ToLower(CultureInfo.CurrentCulture);
            string sizeFix = size.ToLower(CultureInfo.CurrentCulture).Replace("p", string.Empty);   
            string args = 
                "-sPAPERSIZE#" +
                sizeFix + " \"" + vs.FullExportPath(".ps") +
                "\" \"" + vs.FullExportPath(".pdf") + "\"";

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(".pdf"))) {
                this.log.AddMessage("Converting to PDF with: " + prog + " " + args);
                SCaddins.Common.ConsoleUtilities.StartHiddenConsoleProg(prog, args);
                FileUtilities.WaitForFileAccess(vs.FullExportPath(".pdf"));
                this.RunExportHooks("pdf", vs);
            } else {
                this.log.AddWarning(vs.FullExportName, "Unable to overwrite existing file: " + vs.FullExportPath(".pdf"));    
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
        private bool ExportAdobePDF(ExportSheet vs)
        {
            PrintManager pm = doc.PrintManager;
            
            this.log.AddMessage(Resources.ApplyingPrintSetting + @": " + vs.PrintSettingName);

            if (!PrintSettings.PrintToFile(doc, vs, pm, ".pdf", this.PdfPrinterName)) {
                this.log.AddError(vs.FullExportName, "failed to assign print setting: " + vs.PrintSettingName);
                return false;
            }

            SetAcrobatExportRegistryVal(vs.FullExportPath(".pdf"), this.log);

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(".pdf"))) {
                if (File.Exists(vs.FullExportPath(".pdf"))) {
                    File.Delete(vs.FullExportPath(".pdf"));
                }
                this.log.AddMessage("Submitting print...");
                if (pm.SubmitPrint(vs.Sheet)) {
                    this.log.AddMessage("(apparently) completed successfully");
                } else {
                    this.log.AddError(vs.FullExportName, "Failed to print");    
                }
                FileUtilities.WaitForFileAccess(vs.FullExportPath(".pdf"));
                
                this.RunExportHooks("pdf", vs);
                             
                SCaddins.Common.SystemUtilities.KillAllProcesses("acrotray");
            } else {
                this.log.AddError(vs.FullExportName, "Could not overwrite file, maybe check permissions?");
                return false;
            }
            
            return true;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
