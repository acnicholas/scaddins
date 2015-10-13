// (C) Copyright 2012-2015 by Andrew Nicholas
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

    public class ExportManager
    {
        private static Dictionary<string, FamilyInstance> titleBlocks;
        private static Document doc;
        private static string activeDoc;
        private static string author;
        private ExportOptions exportFlags;
        private Collection<SegmentedSheetName> fileNameTypes;
        private Collection<ViewSheetSetCombo> allViewSheetSets;
        private SegmentedSheetName fileNameScheme;
        private SortableBindingListCollection<ExportSheet> allSheets;
        private bool forceDate;
        private string exportDir;

        public ExportManager(Document doc)
        {
            ExportManager.doc = doc;
            this.fileNameScheme = null;
            this.exportDir = Constants.DefaultExportDir;
            ExportManager.ConfirmOverwrite = true;
            ExportManager.activeDoc = null;
            this.allViewSheetSets = new Collection<ViewSheetSetCombo>();
            this.allSheets = new SortableBindingListCollection<ExportSheet>();
            this.fileNameTypes = new Collection<SegmentedSheetName>();
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

        public static string Author {
            get { return author; }
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

        public string GhostscriptLibDir
        {
            get; set;
        }

        public string GhostscriptBinDir
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

        public string ExportDir {
            get {
                return this.exportDir;
            }

            set {
                if (value != null) {
                    this.exportDir = value;
                    foreach (ExportSheet sheet in this.allSheets) {
                        sheet.ExportDir = value;
                    }
                }
            }
        }

        public SegmentedSheetName FileNameScheme {
            get {
                return this.fileNameScheme;
            }
        }

        public static FamilyInstance TitleBlockInstanceFromSheetNumber(
            string sheetNumber, Document doc)
        {
            FamilyInstance result;
            if ((titleBlocks == null) ||
                (activeDoc != FileUtilities.GetCentralFileName(doc))) {
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

        public static string GetConfigFileName(Document doc)
        {
            string central = FileUtilities.GetCentralFileName(doc);
            string s = Path.GetDirectoryName(central) + @"\" +
                Path.GetFileNameWithoutExtension(central) + ".xml";
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
            var renameSheetDialog = new RenameSheetForm(sheets, doc);
            var result = renameSheetDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                foreach (ExportSheet sheet in sheets) {
                    sheet.UpdateNumber();
                    sheet.UpdateName();
                }
            }
        }

        public static void FixScaleBars(ICollection<ExportSheet> sheets)
        {
            var t = new Autodesk.Revit.DB.Transaction(doc);
            t.Start("SCexport - Fix Scale Bars");
            foreach (ExportSheet sheet in sheets) {
                if (!sheet.ValidScaleBar) {
                    sheet.UpdateScaleBarScale();
                }
            }
            t.Commit();
        }

        public static void AddRevisions(ICollection<ExportSheet> sheets)
        {
            var r = new RevisionSelectionDialog(doc);
            var result = r.ShowDialog();
            if ((r.Id != null) && (result == System.Windows.Forms.DialogResult.OK)) {
                var t = new Transaction(doc, "SCexport: Add new revisions");
                t.Start();
                foreach (ExportSheet sheet in sheets) {
                    #if REVIT2014
                    ICollection<ElementId> il = sheet.Sheet.GetAdditionalProjectRevisionIds();
                    il.Add(r.Id);
                    sheet.Sheet.SetAdditionalProjectRevisionIds(il);
                    #else
                    ICollection<ElementId> il = sheet.Sheet.GetAdditionalRevisionIds();
                    il.Add(r.Id);
                    sheet.Sheet.SetAdditionalRevisionIds(il);
                    #endif
                }
                t.Commit();
                foreach (ExportSheet sheet in sheets) {
                    sheet.UpdateRevision(true);
                }
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
            #if (!REVIT2016)
            if (version == "R2000") {
                return ACADVersion.R2000;
            }
            if (version == "R2004") {
                return ACADVersion.R2004;
            }
            #endif
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
                #if (!REVIT2016)
                case ACADVersion.R2000:
                    return "R2000";
                case ACADVersion.R2004:
                    return "R2004";
                #endif
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
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Revisions);
            foreach (Element e in a) {
                int j = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger();
                if (j > i) {
                    i = j;
                    s = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString();
                }
            }
            return (s.Length > 1) ? s : string.Empty;
        }

        public void Print(
            ICollection<ExportSheet> sheets,
            string printerName,
            int scale)
        {
            PrintManager pm = doc.PrintManager;
            TaskDialogResult tdr = ShowPrintWarning();

            if (tdr == TaskDialogResult.Ok) { 
                bool printSetttingsValid;
                foreach (ExportSheet sheet in sheets.OrderBy(x => x.SheetNumber).ToList()) {
                    if (!sheet.Verified) {
                        sheet.UpdateSheetInfo();
                    }
                    printSetttingsValid = false;

                    switch (scale) {
                    case 3:
                        printSetttingsValid |= PrintSettings.ApplyPrintSettings(doc, "A3-FIT", pm, printerName);
                        break;
                    case 2:
                        printSetttingsValid |= PrintSettings.ApplyPrintSettings(doc, "A2-FIT", pm, printerName);
                        break;
                    default:
                        int i = int.Parse(sheet.PageSize.Substring(1, 1));
                        string printerNameTmp = i > 2 ? "this.PrinterNameA3" : this.PrinterNameLargeFormat;
                        printSetttingsValid |= PrintSettings.ApplyPrintSettings(doc, sheet.PageSize, pm, printerNameTmp);
                        break;
                    }
                    if (printSetttingsValid) {
                        pm.SubmitPrint(sheet.Sheet);
                    }
                }
            }
        }

        public void Update()
        {
            PrintManager pm = doc.PrintManager;
            PrintSettings.SetPrinter(doc, this.PdfPrinterName, pm);

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
                        sheet.SegmentedFileName = this.fileNameScheme;
                    } 
                }
            }
        }

        public int GetTotalNumberOfExports(ICollection<ExportSheet> sheets)
        {
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
            DateTime startTime = DateTime.Now;
            TimeSpan elapsedTime = DateTime.Now - startTime;

            var exportLog = new ExportLog(startTime, this.GetTotalNumberOfExports(sheets));

            PrintManager pm = doc.PrintManager;
            PrintSettings.SetPrinter(doc, this.PdfPrinterName, pm);

            foreach (ExportSheet sheet in sheets) {
                progressBar.PerformStep();
                elapsedTime = DateTime.Now - startTime;
                info.Text = ExportManager.PercentageSting(progressBar.Value, progressBar.Maximum) +
                    " - " + ExportManager.TimeSpanAsString(elapsedTime);
                strip.Update();
                this.ExportSheet(sheet, exportLog);
            }

            if (exportLog.Errors > 0) {
                exportLog.ShowSummaryDialog();
            }
        }

        public bool GSSanityCheck()
        {
            if (!Directory.Exists(this.GhostscriptBinDir) || !Directory.Exists(this.GhostscriptLibDir)) {
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
            this.GhostscriptBinDir = SCaddins.SCexport.Settings1.Default.GSBinDirectory;
            this.PdfPrinterName = SCaddins.SCexport.Settings1.Default.AdobePrinterDriver; 
            this.PrinterNameA3 = SCaddins.SCexport.Settings1.Default.A3PrinterDriver; 
            this.PrinterNameLargeFormat = SCaddins.SCexport.Settings1.Default.LargeFormatPrinterDriver;
            this.PostscriptPrinterName = SCaddins.SCexport.Settings1.Default.PSPrinterDriver; 
            this.GhostscriptLibDir = SCaddins.SCexport.Settings1.Default.GSLibDirectory; 
            this.exportDir = SCaddins.SCexport.Settings1.Default.ExportDir;
            this.AcadVersion = AcadVersionFromString(SCaddins.SCexport.Settings1.Default.AcadExportVersion);
        }

        private static void OpenSheet(UIDocument udoc, ViewSheet view, int inc)
        {
            FilteredElementCollector a;
            a = new FilteredElementCollector(udoc.Document)
                .OfCategory(BuiltInCategory.OST_Sheets)
                .OfClass(typeof(ViewSheet));
            IList<ViewSheet> sheets = new List<ViewSheet>();
            foreach (ViewSheet v in a) {
                sheets.Add(v);
            }
            IEnumerable<ViewSheet> sortedEnum = sheets.OrderBy(f => f.SheetNumber);
            IList<ViewSheet> sortedSheets = sortedEnum.ToList();

            // FIXME don't allow overflow.
            for (int i = 0; i < sortedSheets.Count; i++) {
                if (sortedSheets[i].SheetNumber == view.SheetNumber) {
                    DialogHandler.AddRevitDialogHandler(new UIApplication(udoc.Application.Application));
                    Autodesk.Revit.DB.FamilyInstance result =
                        ExportManager.TitleBlockInstanceFromSheetNumber(
                            sortedSheets[i + inc].SheetNumber, udoc.Document);
                    if (result != null) {
                        udoc.ShowElements(result);
                    }
                    return;
                }
            }
        }

        private static Dictionary<string, FamilyInstance> AllTitleBlocks(
            Document document)
        {
            var result = new Dictionary<string, FamilyInstance>();

            FilteredElementCollector a = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .OfClass(typeof(FamilyInstance));

            foreach (FamilyInstance e in a) {
                var s = e.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString();
                if (!result.ContainsKey(s)) {
                    result.Add(s, e);
                }
            }

            return result;
        }
        
        private static TaskDialogResult ShowPrintWarning()
        {
            var td = new TaskDialog("SCexport - Print Warning");
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

        [SecurityCritical]
        private static void SetAcrobatExportRegistryVal(string fileName)
        {
            string exe =
                Process.GetCurrentProcess().MainModule.FileName;
            Microsoft.Win32.Registry.SetValue(
                Constants.AcrobatPrinterJobControl,
                exe,
                fileName,
                Microsoft.Win32.RegistryValueKind.String);
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
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc).OfClass(typeof(ViewSheetSet));
            foreach (ViewSheetSet v in a) {
                vss.Add(new ViewSheetSetCombo(v));
            }
        }

        private static void ExportDWF(ExportSheet vs)
        {
            var views = new ViewSet();
            views.Insert(vs.Sheet);
            var opts = new DWFExportOptions();
            opts.CropBoxVisible = false;
            opts.ExportingAreas = true;
            var t = new Transaction(doc, "Export DWF");
            t.Start();
            try {
                string tmp = vs.FullExportName + ".dwf";
                doc.Export(vs.ExportDir, tmp, views, opts);
                t.Commit();
            } catch (ArgumentException e) {
                TaskDialog.Show("SCexport", "cannot export dwf: " + e.Message);
                t.RollBack();
            } catch (InvalidOperationException e) {
                TaskDialog.Show("SCexport", "cannot export dwf: " + e.Message);
                t.RollBack();
            }
        }

        private static void ExportDGN(ExportSheet vs)
        {
            var opts = new DGNExportOptions();
            ICollection<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);
            doc.Export(vs.ExportDir, vs.FullExportName, views, opts);
        }
 
        private void SetDefaultFlags()
        {
            if (SCaddins.SCexport.Settings1.Default.AdobePDFMode && this.PDFSanityCheck()) {
                this.AddExportOption(ExportOptions.PDF);
            } else if (!SCaddins.SCexport.Settings1.Default.AdobePDFMode && this.GSSanityCheck()) {
                this.AddExportOption(ExportOptions.GhostscriptPDF);
            } else {
                if (this.PDFSanityCheck()) {
                     this.AddExportOption(ExportOptions.PDF);   
                }
                this.AddExportOption(ExportOptions.DWG);
            }
            if (SCaddins.SCexport.Settings1.Default.TagPDFExports) {
                this.AddExportOption(ExportOptions.TagPDFExports);
            }
            if (SCaddins.SCexport.Settings1.Default.HideTitleBlocks) {
                this.AddExportOption(ExportOptions.NoTitle);
            }
            this.forceDate |= SCaddins.SCexport.Settings1.Default.ForceDateRevision;
        }

        private void PopulateSheets(SortableBindingListCollection<ExportSheet> s)
        {
            string config = GetConfigFileName(doc);
            bool b = this.ImportXMLinfo(config);
            if (!b) {
                var name =
                    new SegmentedSheetName(FilenameScheme.Standard);
                this.fileNameTypes.Add(name);
                this.fileNameScheme = name;
            }
            if (this.fileNameTypes.Count <= 0) {
                var name =
                    new SegmentedSheetName(FilenameScheme.Standard);
                this.fileNameTypes.Add(name);
                this.fileNameScheme = name;
            }

            s.Clear();
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc);
            a.OfCategory(BuiltInCategory.OST_Sheets);
            a.OfClass(typeof(ViewSheet));
            foreach (ViewSheet v in a) {
                var scxSheet =
                    new ExportSheet(v, doc, this.fileNameTypes[0], this);
                s.Add(scxSheet);
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
            try {
                var settings = new XmlReaderSettings();
                settings.Schemas.Add(
                    null, SCaddins.Constants.InstallDir + @"\etc\SCexport.xsd");
                settings.ValidationType = ValidationType.Schema;
                XmlReader reader = XmlReader.Create(filename, settings);
                var document = new XmlDocument();
                document.Load(reader);
                var eventHandler =
                    new ValidationEventHandler(this.ValidationEventHandler);
                document.Validate(eventHandler);
                return true;
            } catch (Exception ex) {
                TaskDialog.Show(
                    "SCexport", "Error reading xml file: " + ex.Message);
                return false;
            }
        }

        private bool ImportXMLinfo(string filename)
        {
            if (!System.IO.File.Exists(filename)) {
                return false;
            }
            if (!this.ValidateXML(filename)) {
                return false;
            }

            author = "SCexport";

            var reader = new XmlTextReader(filename);

            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element &&
                    reader.Name == "PDFTagSettings") {
                    do {
                        reader.Read();
                        if (reader.NodeType == XmlNodeType.Element) {
                            switch (reader.Name) {
                                case "Author":
                                    author = reader.ReadString();
                                    break;
                            }
                        }
                    } while (!(reader.NodeType == XmlNodeType.EndElement &&
                               reader.Name == "PDFTagSettings"));
                }

                if (reader.NodeType == XmlNodeType.Element &&
                    reader.Name == "FilenameScheme") {
                    var name = new SegmentedSheetName();
                    if (reader.AttributeCount > 0) {
                        name.Name = reader.GetAttribute("label");
                    }
                    name.AddNodesFromXML(reader);
                    this.fileNameTypes.Add(name);
                }
            }

            if (this.fileNameTypes.Count > 0) {
                this.fileNameScheme = this.fileNameTypes[0];
                foreach (ExportSheet sheet in this.allSheets) {
                    sheet.SegmentedFileName = this.fileNameScheme;
                }
            }

            reader.Close();
            return true;
        }

        [SecurityCritical]
        private void ExportSheet(ExportSheet sheet, ExportLog log)
        {
            if (!sheet.Verified) {
                sheet.UpdateSheetInfo();
            }

            if (sheet.SCPrintSetting != null) {
                if (this.exportFlags.HasFlag(ExportOptions.DWG)) {
                    this.ExportDWG(sheet, this.exportFlags.HasFlag(ExportOptions.NoTitle));
                }

                if (this.exportFlags.HasFlag(ExportOptions.PDF)) {
                    this.ExportAdobePDF(sheet, log);
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
            } else {
                log.AddError(sheet.FullExportName, "no print setting assigned.");
            }
        }

        private void ApplyDefaultDWGExportOptions(ref DWGExportOptions opts)
        {
            opts.MergedViews = true;
            opts.FileVersion = this.AcadVersion;
            opts.HideScopeBox = true;
            opts.HideUnreferenceViewTags = true;
        }

        private void ExportDWG(ExportSheet vs, bool removeTitle)
        {
            ICollection<ElementId> titleBlockHidden;
            titleBlockHidden = new List<ElementId>();
            var titleBlock = ExportManager.TitleBlockInstanceFromSheetNumber(vs.SheetNumber, doc);
            titleBlockHidden.Add(titleBlock.Id);

            if (removeTitle) {
                ExportManager.RemoveTitleBlock(vs, titleBlockHidden, true);
            }

            PrintManager pm = doc.PrintManager;

            var t = new Transaction(doc, "Apply print settings");
            t.Start();

            try {
                pm.PrintToFile.Equals(true);
                pm.PrintRange = PrintRange.Select;
                pm.Apply();
                t.Commit();
            } catch (InvalidOperationException) {
                TaskDialog.Show("Revit", "cannot apply print settings");
                t.RollBack();
            }

            ICollection<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);

            var opts = new DWGExportOptions();
            this.ApplyDefaultDWGExportOptions(ref opts);

            pm.PrintRange = PrintRange.Select;

            var name = vs.FullExportName + ".dwg";
            doc.Export(vs.ExportDir, name, views, opts);
            System.Threading.Thread.Sleep(1000);

            if (removeTitle) {
                ExportManager.RemoveTitleBlock(vs, titleBlockHidden, false);
            }
        }

        [SecurityCritical]
        private bool ExportGSPDF(ExportSheet vs)
        {
            PrintManager pm = doc.PrintManager;

            if (!PrintSettings.ApplyPrintSettings(
                doc, vs, pm, ".ps", this.PostscriptPrinterName)) {
                return false;
            }

            try {
                pm.SubmitPrint(vs.Sheet);
            } catch (InvalidOperationException) {
                File.Delete(vs.FullExportPath(".ps"));
                pm.SubmitPrint(vs.Sheet);
            }

            FileUtilities.WaitForFileAccess(vs.FullExportPath(".ps"));

            string args = 
                @"/c " + this.GhostscriptLibDir  + @"\ps2pdf -sPAPERSIZE#" +
                vs.PageSize.ToLower(CultureInfo.CurrentCulture) + " \"" + vs.FullExportPath(".ps") +
                "\" \"" + vs.FullExportPath(".pdf") + "\"";

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(".pdf"))) {
                SCaddins.Common.ConsoleUtilities.StartHiddenConsoleProg("cmd.exe", args);
            }
            return true;
        }

        [SecurityCritical]
        private bool ExportAdobePDF(ExportSheet vs, ExportLog log)
        {
            PrintManager pm = doc.PrintManager;

            if (!PrintSettings.ApplyPrintSettings(
                doc, vs, pm, ".pdf", this.PdfPrinterName)) {
                log.AddError(vs.FullExportName, "failed to assign print setting: " + vs.PrintSettingName);
                return false;
            }

            SetAcrobatExportRegistryVal(vs.FullExportPath(".pdf"));

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(".pdf"))) {
                if (File.Exists(vs.FullExportPath(".pdf"))) {
                    File.Delete(vs.FullExportPath(".pdf"));
                }
                if (pm.SubmitPrint(vs.Sheet)) {
                    log.AddSuccess(vs.FullExportName, "(apparently) completed succesfully");
                } else {
                    log.AddError(vs.FullExportName, "failed to print");    
                }
                SCaddins.Common.SystemUtilities.KillAllProcesses("acrotray");
            } else {
                log.AddError(vs.FullExportName, "could not overwrite file, maybe check permissions?");
                return false;
            }
            FileUtilities.WaitForFileAccess(vs.FullExportPath(".pdf"));
            return true;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
