// (C) Copyright 2012-2013 by Andrew Nicholas
//
// This file is part of SCexport.
//
// SCexport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCexport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCexport.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCexport
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Schema;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using SCaddins.Common;

    /// <summary>
    /// Export files to comply with SC standard.
    /// </summary>
    public class SCexport
    {
        private static Dictionary<string, FamilyInstance> titleBlocks;
        private static Document doc;
        private static string activeDoc;
        private static string author;
        private static string nonIssueTag;
        private ACADVersion acadVersion;
        private ExportFlags flags;
        private List<SheetName> filenameTypes;
        private List<ViewSheetSetCombo> allViewSheetSets;
        private SheetName filenameScheme;
        private SortableBindingList<SCexportSheet> allSheets;
        private bool forceDate;
        private string exportDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="SCexport" /> class.
        /// </summary>
        /// <param name="doc">
        /// The active Revit doc.
        /// </param>
        public SCexport(Document doc)
        {
            SCexport.doc = doc;
            this.acadVersion = ACADVersion.R2004;
            this.filenameScheme = null;
            this.exportDir = Constants.DefaultExportDir;
            SCexport.ConfirmOverwrite = true;
            SCexport.activeDoc = null;
            this.allViewSheetSets = new List<ViewSheetSetCombo>();
            this.allSheets = new SortableBindingList<SCexportSheet>();
            this.filenameTypes = new List<SheetName>();
            this.flags = ExportFlags.None;
            this.LoadSettings();
            this.SetDefaultFlags();
            this.PopulateViewSheetSets(ref this.allViewSheetSets);
            this.PopulateSheets(ref this.allSheets);
            this.FixAcrotrayHang();
        }

        /// <summary>
        /// Type of export.
        /// </summary>
        [Flags]
        public enum ExportFlags
        {
            /// <summary>Export Nothing.</summary>
            None = 0,

            /// <summary>Export files using Adobe Acrobat.</summary>
            PDF = 1,

            /// <summary>Export a AutoCAD file.</summary>
            DWG = 2,

            /// <summary>Export A Microstation file.</summary>
            DGN = 4,

            /// <summary>Export a Autodesk dwf file.</summary>
            DWF = 8,

            /// <summary>
            /// Export files using Ghostscript to vreate pdf's.
            /// </summary>
            GS_PDF = 16,

            /// <summary>Remove titleblock from sheet before exporting.
            /// </summary>
            NO_TITLE = 32,

            /// <summary>Tag pdf files with metadata.</summary>
            TAG_PDF = 64,
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ask the user
        /// <see cref="SCexport"/> to confirm overwrite.
        /// </summary>
        /// <value>
        /// <c>true</c> if confirm overwrite; otherwise, <c>false</c>.
        /// </value>
        public static bool ConfirmOverwrite
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the author of the document to be exported.
        /// </summary>
        /// <value>The author.</value>
        public static string Author {
            get { return author; }
        }

        public static Version SCexportVersion
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
        }
        
        public string PrinterNameA3
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

        public string GhostsciptLibDir
        {
            get; set;
        }

        public string GhostsciptBinDir
        {
            get; set;
        }

        /// <summary>
        /// Gets the available filename schemes.
        /// </summary>
        /// <value>Gets the value of the filenameTypes field.</value>
        public List<SheetName> FilenameTypes
        {
            get { return this.filenameTypes; }
        }
        
        /// <summary>
        /// Gets all sheets.
        /// </summary>
        /// <value>All sheets.</value>
        public SortableBindingList<SCexportSheet> AllSheets
        {
            get { return this.allSheets; }
        }

        /// <summary>
        /// Gets all view sheet sets in the active Revit document.
        /// </summary>
        /// <value>All view sheet sets.</value>
        public List<ViewSheetSetCombo> AllViewSheetSets
        {
            get { return this.allViewSheetSets; }
        }

        /// <summary>
        /// Gets or sets the export flags.
        /// </summary>
        /// <value>The flags.</value>
        public ExportFlags Flags
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the acad version to export to.
        /// </summary>
        /// <value>The acad version.</value>
        public ACADVersion AcadVersion
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force the date
        /// as the current revision.
        /// <see cref="SCexport"/>.
        /// </summary>
        /// <value><c>true</c> if force date; otherwise, <c>false</c>.</value>
        public bool ForceDate
        {
            get { 
                return this.forceDate;
            }
            
            set {
                this.forceDate = value;
                foreach (SCexportSheet sheet in this.allSheets) {
                    sheet.ForceDate = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the export dir.
        /// </summary>
        /// <value>The export dir.</value>
        public string ExportDir {
            get {
                return this.exportDir;
            }
            
            set {
                if (value != null) {
                    this.exportDir = value;
                    foreach (SCexportSheet sheet in this.allSheets) {
                        sheet.ExportDir = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the filename sheme.
        /// </summary>
        /// <value>The filename sheme.</value>
        public SheetName FilenameScheme {
            get { 
                return this.filenameScheme;
            }
            
            set {
                this.filenameScheme = value;
                foreach (SCexportSheet sheet in this.allSheets) {
                    sheet.SegmentedFileName = value;
                }
            }
        }

        public static void CheckForUpdates()
        {
            string url = @"https://bitbucket.org/anicholas/SCexport/downloads/";
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            string html = string.Empty;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                html = reader.ReadToEnd();
            }

            var r = new Regex("href=\"(.*)\">.*SCexport-win64-(.*).msi</a>");
            Match m = r.Match(html);
            Version latestVersion = SCexportVersion;
            while (m.Success)
            {
                var v = new Version(m.Groups[2].Value);
                if (v > latestVersion) {
                    latestVersion = v;
                }
                m = m.NextMatch();
            }

            if (latestVersion > SCexportVersion) {
                var td = new TaskDialog("New Version");
                td.MainInstruction = "New version: " + latestVersion.ToString() + " is available" + System.Environment.NewLine +
                    "Do you want to download it now?";
                td.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.No;
                TaskDialogResult result = td.Show();
                if (result == TaskDialogResult.Ok) {
                    // download new verison (goto download page)
                    System.Diagnostics.Process.Start(url);
                }
            } else {
                TaskDialog.Show("Version Check", "SCexport is up to date.");
            }
        }

        /// <summary>
        /// Gets the title block family.
        /// </summary>
        /// <returns> The title block family. </returns>
        /// <param name="sheetNumber">
        /// The sheet number the contains the titleblock.
        /// </param>
        /// <param name="doc"> The Revit Document. </param>
        public static FamilyInstance GetTitleBlockFamily(
            string sheetNumber, Document doc)
        {
            FamilyInstance result;
            if ((titleBlocks == null) ||
                (activeDoc != FileUtils.GetCentralFilename(doc))) {
                activeDoc = FileUtils.GetCentralFilename(doc);
                titleBlocks = AllTitleBlocks(doc);
            }

            if (titleBlocks.TryGetValue(sheetNumber, out result)) {
                return result;
            } else {
                titleBlocks = AllTitleBlocks(doc);
            }

            if (titleBlocks.TryGetValue(sheetNumber, out result)) {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Add a header at the start of an export to the current log file.
        /// </summary>
        /// <param name="logFile">The log file.</param>
        public static void LogHeader(string logFile)
        {
            string header = "SCexport: " +
                System.DateTime.Now.ToLongDateString() +
                " " + System.DateTime.Now.ToLongTimeString();
            string columnHeaders = "Full Export Name" + "\t" +
                "Sheet Number" + "\t" + "Sheet Title" + "\t" + "Revision" +
                "\t" + "Revision Description" + "\t" + "Revision Date";
            System.IO.File.AppendAllText(
                logFile, header + System.Environment.NewLine);
            System.IO.File.AppendAllText(
                logFile, columnHeaders + System.Environment.NewLine);
        }

        /// <summary>
        /// Get a date string in the format of YYYYMMDD.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A date string in the format of YYYYMMDD.</returns>
        public static DateTime ToDateTime(string s)
        {
            s.Trim();
            string delims = @"-.\/_ ";
            char[] c = delims.ToCharArray();
            int d2 = s.LastIndexOfAny(c);
            int d1 = s.IndexOfAny(c);
            int len = s.Length;

            // FIXME clean this up.
            try {
                string year = string.Empty;
                string month = string.Empty;
                string day = string.Empty;
                if (s.Length > d2 + 1) {
                    year = s.Substring(d2 + 1);
                }
                if (s.Length > (d1 + 1) && (d2 - d1 - 1) < s.Length - (d1 + 1)) {
                    month = s.Substring(d1 + 1, d2 - d1 - 1);
                }
                if (s.Length > 0 && d1 <= s.Length) {
                    day = s.Substring(0, d1);
                }
                return new DateTime(
                    Convert.ToInt32(year),
                    Convert.ToInt32(month),
                    Convert.ToInt32(day));
            } catch {
                return new DateTime();
            }
        }

        /// <summary>
        /// Start a console program - Hidden.
        /// </summary>
        /// <param name="exePath">The program to start.</param>
        /// <param name="args">The args to send to the program.</param>
        public static void StartHiddenConsoleProg(string exePath, string args)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.FileName = exePath;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.Arguments = args;
            var p = new Process();
            p = System.Diagnostics.Process.Start(startInfo);
            p.WaitForExit();
        }

        /// <summary>
        /// Gets the current date.
        /// </summary>
        /// <returns>
        /// The date in the format YYYYMMDD.
        /// </returns>
        public static string GetDateString()
        {
            DateTime moment = DateTime.Now;
            string syear = moment.Year.ToString();
            string smonth = SCexport.PadLeftZero(moment.Month.ToString(), 2);
            string sday = SCexport.PadLeftZero(moment.Day.ToString(), 2);
            return syear + smonth + sday;
        }

        /// <summary>
        /// Creates a custom XML config file in the same folder as the active
        /// document.
        /// </summary>
        /// <param name="doc">The current revit document.</param>
        /// <returns>
        /// The full path to the project config file for
        /// the active Revit document.
        /// </returns>
        public static string CreateSCexportConfig(Document doc)
        {
            string s = GetConfigFilename(ref doc);
            if (File.Exists(s)) {
                return s;
            } else {
                return null;
            }
        }

        /// <summary>
        /// The config file name that SCexport expects to find.
        /// This will be an xml file with same location and name as the active
        /// Revit model.
        /// </summary>
        /// <param name="doc">The active revit document.</param>
        /// <returns>The name of the config file.</returns>
        public static string GetConfigFilename(ref Document doc)
        {
            string central = FileUtils.GetCentralFilename(doc);
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
        
        public static void RenameSheets(ICollection<SCexportSheet> sheets)
        {
            var r = new RenameSheetForm(sheets, doc);
            var result = r.ShowDialog();
            foreach (SCexportSheet sheet in sheets) {
                    sheet.UpdateNumber();
            }
        }

        public static void AddRevisions(ICollection<SCexportSheet> sheets)
        {
            #if REVIT2014
            var r = new RevisionSelectionDialog(doc);
            var result = r.ShowDialog();
            if ((r.ID != null) && (result == System.Windows.Forms.DialogResult.OK)) {
                var t = new Transaction(doc, "SCexport: Add new revisions");
                t.Start();
                foreach (SCexportSheet sheet in sheets) {
                    ICollection<ElementId> il = sheet.Sheet.GetAdditionalProjectRevisionIds();
                    il.Add(r.ID);
                    sheet.Sheet.SetAdditionalProjectRevisionIds(il);
                }
                t.Commit();
                foreach (SCexportSheet sheet in sheets) {
                    sheet.UpdateRevision();
                }
            }
            #endif
        }
        
        /// <summary>
        /// Gets the name of the current view of the active document.
        /// </summary>
        /// <returns> The name of the view. </returns>
        public static string CurrentView()
        {
            View v = doc.ActiveView;
            if (v.ViewType == ViewType.DrawingSheet) {
                return v.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER).AsString();
            } else {
                return null;
            }
        }

        /// <summary>
        /// The date of the latest revision in the current doc.
        /// </summary>
        /// <returns>The revision date.</returns>
        public static string LatestRevisionDate()
        {
            string s = string.Empty;
            int i = -1;
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc);
            a.OfCategory(BuiltInCategory.OST_Revisions);
            foreach (Element e in a) {
                int j = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger();
                if (j > i) {
                    i = j;
                    s = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString();
                }
            }
            if (s.Length > 1) {
                return s;
            } else {
                return string.Empty;
            }
        }

        /// <summary>
        /// Update sheet information.
        /// </summary>
        public void Update()
        {
            foreach (SCexportSheet sc in this.allSheets) {
                if (!sc.Verified) {
                    sc.UpdateSheetInfo();
                }
            }
        }

        /// <summary>
        /// Adds an export flag to the current set of flags.
        /// </summary>
        /// <param name="f"> The Flag to add. </param>
        public void AddExportFlag(ExportFlags f)
        {
            this.flags |= f;
        }

        /// <summary>
        /// Removes an export flag from the current set of flags.
        /// </summary>
        /// <param name="f"> The flag to remove. </param>
        public void RemoveExportFlag(ExportFlags f)
        {
            this.flags = this.flags & ~f;
        }

        /// <summary>
        /// Determines whether this instance has contains a certain export flag.
        /// </summary>
        /// <returns><c>true</c> if this instance has flag the specified f; otherwise, <c>false</c>.</returns>
        /// <param name="f"> The flag to evaluate. </param>
        public bool HasFlag(ExportFlags f)
        {
            return this.flags.HasFlag(f);
        }

        /// <summary>
        /// Gets or sets the filename scheme to use with exports.
        /// </summary>
        /// <param name="s"> The filename scheme. </param>
        public void SetFilenameScheme(string s)
        {
            foreach (SheetName scheme in this.filenameTypes) {
                if (s == scheme.Name) {
                    this.FilenameScheme = scheme;
                }
            }
        }

        /// <summary>
        /// Export some view sheets..
        /// </summary>
        /// <param name="sheets">The sheets to export.</param>
        /// <param name="pbar">The progress bar to update.</param>
        /// <param name="info">Information text to update with progress.</param>
        /// <param name="strip">
        /// The title strip that contains the progress bar in info.
        /// </param>
        public void Export(
            ICollection<SCexportSheet> sheets,
            System.Windows.Forms.ToolStripProgressBar pbar,
            System.Windows.Forms.ToolStripLabel info,
            System.Windows.Forms.StatusStrip strip)
        {
            DateTime startTime = DateTime.Now;
            TimeSpan elapsedTime = DateTime.Now - startTime;
            string logFile = this.exportDir + "\\" + "SCexport.log";
            LogHeader(logFile);
            foreach (SCexportSheet sheet in sheets) {
                pbar.PerformStep();
                elapsedTime = DateTime.Now - startTime;
                info.Text = this.PercentageSting(pbar.Value, pbar.Maximum) +
                    " - " + this.TimeSpanAsString(elapsedTime);
                strip.Update();
                this.Export(sheet);
                this.LogIndividualFile(sheet, logFile);
            }

            // Tag file
            if (this.flags.HasFlag(SCexport.ExportFlags.TAG_PDF) &&
                (this.flags.HasFlag(SCexport.ExportFlags.PDF) ||
                 this.flags.HasFlag(SCexport.ExportFlags.GS_PDF)))
            {
                foreach (SCexportSheet sheet in sheets) {
                    if (sheet.SheetRevisionDate.Length < 1) {
                        PdfTools.TagPDF(
                            sheet.FullExportPath(".pdf"),
                            sheet.SheetDescription,
                            nonIssueTag);
                    } else {
                        string s = "[" + sheet.SheetRevisionDate + "] " +
                            sheet.SheetRevisionDescription;
                        PdfTools.TagPDF(
                            sheet.FullExportPath(".pdf"),
                            sheet.SheetDescription,
                            s);
                    }
                }
            }

            LogFooter(logFile, pbar.Maximum, this.TimeSpanAsString(elapsedTime));
        }

        /// <summary>
        /// Print the specified sheets.
        /// </summary>
        /// <param name="sheets"> All the sheets to export. </param>
        public void PrintA3(
            ICollection<SCexportSheet> sheets,
            string printerName)
        {
            PrintManager pm = doc.PrintManager;

            // FIXME need more than a3
            if (!PrintSettings.ApplyPrintSettings(
                ref doc, "A3-FIT", ref pm, printerName)) {
                return;
            }

            TaskDialog td = new TaskDialog("SCexport - Print Warning");
            td.MainInstruction = "Warning";
            td.MainContent = "The print feature is experimental, please only export a " +
                "small selection of sheets until you are sure it is working correctly." +
                System.Environment.NewLine + System.Environment.NewLine +
                "Press ok to continue.";
            td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            td.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.No;
            TaskDialogResult tdr = td.Show();

            if (tdr == TaskDialogResult.Ok) {
                foreach (SCexportSheet sheet in sheets) {
                    pm.SubmitPrint(sheet.Sheet);
                }
            }
        }

        public bool GSSanityCheck()
        {
            if (!Directory.Exists(this.GhostsciptBinDir) || !Directory.Exists(this.GhostsciptLibDir)) {
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
            if (!ps.IsValid) {
                return false;
            }
            return true;
        }
        
        public void LoadSettings()
        {
            this.GhostsciptBinDir = SCaddins.SCexport.Settings1.Default.GSBinDirectory;           
            this.PdfPrinterName = SCaddins.SCexport.Settings1.Default.AdobePrinterDriver; 
            this.PrinterNameA3 = SCaddins.SCexport.Settings1.Default.A3PrinterDriver; 
            this.PostscriptPrinterName = SCaddins.SCexport.Settings1.Default.PSPrinterDriver; 
            this.GhostsciptLibDir = SCaddins.SCexport.Settings1.Default.GSLibDirectory; 
            this.exportDir = SCaddins.SCexport.Settings1.Default.ExportDir;
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
                    // TaskDialog.Show("TEST",sortedSheets[i + inc].SheetNumber);
                    var dh = new DialogHandler(
                        new UIApplication(udoc.Application.Application));
                    Autodesk.Revit.DB.FamilyInstance result =
                        SCexport.GetTitleBlockFamily(
                            sortedSheets[i + inc].SheetNumber, udoc.Document);
                    if (result != null) {
                        udoc.ShowElements(result);
                    }
                    return;
                }
            }
            TaskDialog.Show("TEST", "stuffed");
        }

        private static void LogFooter(
            string logFile, int count, string timeElapsed)
        {
            string s = "TOTAL: " + count.ToString() +
                " files exported in" + timeElapsed +
                System.Environment.NewLine + System.Environment.NewLine;
            System.IO.File.AppendAllText(logFile, s);
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

        private static string PadLeftZero(string s, int desiredLength)
        {
            if (s.Length == desiredLength - 1) {
                return "0" + s;
            } else {
                return s;
            }
        }

        private void SetDefaultFlags()
        {      
            if (SCaddins.SCexport.Settings1.Default.AdobePDFMode && this.PDFSanityCheck()) {
                this.AddExportFlag(SCexport.ExportFlags.PDF);
            } else if (!SCaddins.SCexport.Settings1.Default.AdobePDFMode && this.GSSanityCheck()) {
                this.AddExportFlag(ExportFlags.GS_PDF);
            } else {
                if (this.PDFSanityCheck()) {
                     this.AddExportFlag(SCexport.ExportFlags.PDF);   
                }
                this.AddExportFlag(ExportFlags.DWG);
            }
            if (SCaddins.SCexport.Settings1.Default.TagPDFExports) {
                this.AddExportFlag(ExportFlags.TAG_PDF);
            }
            if (SCaddins.SCexport.Settings1.Default.HideTitleBlocks) {
                this.AddExportFlag(ExportFlags.NO_TITLE);
            }
            if (SCaddins.SCexport.Settings1.Default.ForceDateRevision) {
                this.forceDate = true;
            }
        }

        private string PercentageSting(int n, int total)
        {
            var result = "Exporting " + n + " of " + total +
                " (" + (int)(((double)n / (double)total) * 100) + @"%)";
            return result;
        }

        private string TimeSpanAsString(TimeSpan time)
        {
            var result = "Elapsed Time: " +
                PadLeftZero(time.Hours.ToString(), 2) + "h:" +
                PadLeftZero(time.Minutes.ToString(), 2) + "m:" +
                PadLeftZero(time.Seconds.ToString(), 2) + "s";
            return result;
        }

        private void LogIndividualFile(SCexportSheet sheet, string logFile)
        {
            string s = sheet.FullExportName + "\t" + sheet.SheetNumber +
                "\t" + sheet.SheetDescription + "\t" + sheet.SheetRevision +
                "\t" + sheet.SheetRevisionDescription + "\t" +
                sheet.SheetRevisionDate + System.Environment.NewLine;
            System.IO.File.AppendAllText(logFile, s);
        }

        private void FixAcrotrayHang()
        {
            Microsoft.Win32.Registry.SetValue(
                Constants.HungAppTimeout,
                "HungAppTimeout",
                "30000",
                Microsoft.Win32.RegistryValueKind.String);
        }

        private void KillAcrotray()
        {
            try {
                foreach (Process proc in Process.GetProcessesByName("acrotray")) {
                    proc.Kill();
                }
            } catch (Exception ex) {
                TaskDialog.Show("Error", ex.Message);
            }
        }

        private void SetRegistryVal(string fileName)
        {
            string exe =
                Process.GetCurrentProcess().MainModule.FileName;
            #if DEBUG
            string msg = "Reg set as: " + System.Environment.NewLine + exe;
            Debug.WriteLine(msg);
            #endif
            Microsoft.Win32.Registry.SetValue(
                Constants.AcrobatPJC,
                exe,
                fileName,
                Microsoft.Win32.RegistryValueKind.String);
        }

        /// <summary>
        /// Create an array of all the title block in the current document.
        /// </summary>
        /// <param name="s">Array of SCexportSheets.</param>
        private void PopulateSheets(ref SortableBindingList<SCexportSheet> s)
        {
            string config = GetConfigFilename(ref doc);
            bool b = this.ImportXMLinfo(config);
            if (!b) {
                var name =
                    new SheetName(SheetName.Scheme.SC_STANDARD);
                this.filenameTypes.Add(name);
                this.filenameScheme = name;
            }
            if (this.filenameTypes.Count <= 0) {
                var name =
                    new SheetName(SheetName.Scheme.SC_STANDARD);
                this.filenameTypes.Add(name);
                this.filenameScheme = name;
            }

            s.Clear();
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc);
            a.OfCategory(BuiltInCategory.OST_Sheets);
            a.OfClass(typeof(ViewSheet));
            foreach (ViewSheet v in a) {
                var scxSheet =
                    new SCexportSheet(v, doc, this.filenameTypes[0], this);
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
                    null, Constants.InstallDir + @"\Data\SCexport.xsd");
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
            nonIssueTag = Constants.PdfNonIssueTag;

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
                                case "NonIssueTag":
                                    nonIssueTag = reader.ReadString();
                                    break;
                            }
                        }
                    } while (!(reader.NodeType == XmlNodeType.EndElement &&
                               reader.Name == "PDFTagSettings"));
                }

                if (reader.NodeType == XmlNodeType.Element &&
                    reader.Name == "FilenameScheme") {
                    var name = new SheetName();
                    if (reader.AttributeCount > 0) {
                        name.Name = reader.GetAttribute("label");
                    }
                    name.AddNodesFromXML(ref reader);
                    this.filenameTypes.Add(name);
                }
            }
            
            if (this.filenameTypes.Count > 0) {
                this.FilenameScheme = this.filenameTypes[0];
            }

            reader.Close();
            return true;
        }

        private void PopulateViewSheetSets(ref List<ViewSheetSetCombo> vss)
        {
            vss.Clear();
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc);
            a.OfClass(typeof(ViewSheetSet));
            foreach (ViewSheetSet v in a) {
                vss.Add(new ViewSheetSetCombo(v));
            }
        }

        private void Export(SCexportSheet r)
        {
            if (!r.Verified) {
                r.UpdateSheetInfo();
            }

            if (r.SCPrintSetting != null) {
                if (this.flags.HasFlag(SCexport.ExportFlags.DWG)) {
                    this.ExportDWG(r, this.flags.HasFlag(ExportFlags.NO_TITLE));
                }

                if (this.flags.HasFlag(SCexport.ExportFlags.PDF)) {
                    if (!this.ExportAdobePDF(r)) {
                        TaskDialog.Show("SCexport", "Could not print pdf");
                        return;
                    }
                }

                if (this.flags.HasFlag(SCexport.ExportFlags.GS_PDF)) {
                    if (!this.ExportGSPDF(r)) {
                        TaskDialog.Show(
                            "SCexport", "Could not export postscript pdf");
                        return;
                    }
                }

                if (this.flags.HasFlag(SCexport.ExportFlags.DGN)) {
                    this.ExportDGN(r);
                }

                if (this.flags.HasFlag(SCexport.ExportFlags.DWF)) {
                    this.ExportDWF(r);
                }
            }
        }

        private void RemoveTitleBlock(
            SCexportSheet vs,
            ref View view,
            ICollection<ElementId> title,
            bool hide)
        {
            #if (!REVIT2012)
            view = doc.GetElement(vs.Id) as View;
            Transaction t = new Transaction(doc, "Hide Title");
            t.Start();
            try {
                if (hide) {
                    view.HideElements(title);
                } else {
                    view.UnhideElements(title);
                }
                t.Commit();
            } catch {
                TaskDialog.Show("Revit", "cannot Hide Title");
                t.RollBack();
            }
            #endif
        }

        // FIXME
        // this could do with a lot of cleaning up!
        // ...even more than some other things
        private void ExportDWG(SCexportSheet vs, bool removeTitle)
        {
            View view = null;
            ICollection<ElementId> titleBlockHidden;
            titleBlockHidden = new List<ElementId>();
            var titleBlock = SCexport.GetTitleBlockFamily(vs.SheetNumber, doc);
            titleBlockHidden.Add(titleBlock.Id);

            if (removeTitle) {
                this.RemoveTitleBlock(vs, ref view, titleBlockHidden, true);
            }

            PrintManager pm = doc.PrintManager;

            var t = new Transaction(doc, "Apply print settings");
            t.Start();

            try {
                pm.PrintToFile.Equals(true);
                pm.PrintRange = PrintRange.Select;
                pm.Apply();
                t.Commit();
            } catch {
                TaskDialog.Show("Revit", "cannot apply print settings");
                t.RollBack();
            }

            ICollection<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);
            var opts = new DWGExportOptions();
            opts.MergedViews = true;
            opts.FileVersion = this.acadVersion;

            #if (!REVIT2012)
            pm.PrintRange = PrintRange.Select;
            opts.HideScopeBox = true;
            opts.HideUnreferenceViewTags = true;
            #endif

            bool r;
            var name = vs.FullExportName + ".dwg";
            r = doc.Export(vs.ExportDir, name, views, opts);
            System.Threading.Thread.Sleep(1000);

            if (removeTitle && view != null) {
                this.RemoveTitleBlock(vs, ref view, titleBlockHidden, false);
            }
        }

        private void ExportDWF(SCexportSheet vs)
        {
            var views = new ViewSet();
            views.Insert(vs.Sheet);
            var opts = new DWFExportOptions();
            #if REVIT2014
            opts.CropBoxVisible = false;
            #endif
            #if REVIT2015
            opts.CropBoxVisible = false;
            #endif
            opts.ExportingAreas = true;
            var t = new Transaction(doc, "Export DWF");
            t.Start();
            try {
                string tmp = vs.FullExportName + ".dwf";
                bool r = doc.Export(vs.ExportDir, tmp, views, opts);
                t.Commit();
            } catch {
                TaskDialog.Show("SCexport", "cannot export dwf");
                t.RollBack();
            }
        }

        private void ExportDGN(SCexportSheet vs)
        {
            var opts = new DGNExportOptions();
            #if REVIT2012
            ViewSet views = new ViewSet();
            views.Insert(vs.Sheet);
            doc.Export(vs.ExportDir, vs.FullExportName + ".dgn", views, opts);
            #else
            ICollection<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);
            doc.Export(vs.ExportDir, vs.FullExportName, views, opts);
            #endif
        }

        private bool ExportGSPDF(SCexportSheet vs)
        {
            PrintManager pm = doc.PrintManager;

            if (!PrintSettings.ApplyPrintSettings(
                ref doc, vs, ref pm, ".ps", this.PostscriptPrinterName)) {
                return false;
            }

            try {
                pm.SubmitPrint(vs.Sheet);
            } catch (Exception) {
                File.Delete(vs.FullExportPath(".ps"));
                pm.SubmitPrint(vs.Sheet);
            }

            FileUtils.WaitForFileAccess(vs.FullExportPath(".ps"));

            string args = @"/c ps2pdf -sPAPERSIZE#" +
                vs.PageSize.ToLower() + " \"" + vs.FullExportPath(".ps") +
                "\" \"" + vs.FullExportPath(".pdf") + "\"";

            if (FileUtils.CanOverwriteFile(vs.FullExportPath(".pdf"))) {
                SCexport.StartHiddenConsoleProg("cmd.exe", args);
            }
            return true;
        }
        
        private bool ExportAdobePDF(SCexportSheet vs)
        {
            PrintManager pm = doc.PrintManager;

            if (!PrintSettings.ApplyPrintSettings(
                ref doc, vs, ref pm, ".pdf", this.PdfPrinterName)) {
                return false;
            }

            this.SetRegistryVal(vs.FullExportPath(".pdf"));

            if (FileUtils.CanOverwriteFile(vs.FullExportPath(".pdf"))) {
                if (File.Exists(vs.FullExportPath(".pdf"))) {
                    File.Delete(vs.FullExportPath(".pdf"));
                }
                pm.SubmitPrint(vs.Sheet);
                this.KillAcrotray();
            }
            FileUtils.WaitForFileAccess(vs.FullExportPath(".pdf"));
            return true;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
