// (C) Copyright 2012-2021 by Andrew Nicholas
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
    using Microsoft.Win32;
    using Properties;
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

        public static ACADVersion AcadVersion
        {
            get; set;
        }

        public static bool ConfirmOverwrite
        {
            get; set;
        }

        public ObservableCollection<ExportSheet> AllSheets { get; }

        public ObservableCollection<ViewSetItem> AllViewSheetSets { get; private set; }

        public Document Doc
        {
            get; set;
        }

        public bool ExportAdditionalViewports
        {
            get; set;
        }

        public bool ExportViewportsOnly
        {
            get; set;
        }

        public string ExportDirectory
        {
            get => exportDirectory;

            set
            {
                if (value != null)
                {
                    exportDirectory = value;
                    foreach (ExportSheet sheet in AllSheets)
                    {
                        sheet.ExportDirectory = value;
                    }
                }
            }
        }

        public SegmentedSheetName FileNameScheme
        {
            get => fileNameScheme;

            set
            {
                fileNameScheme = value;
                foreach (ExportSheet sheet in AllSheets)
                {
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

        public string PdfPrinterName
        {
            get; set;
        }

        public string PDF24PrinterName
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

#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
        /// <summary>
        /// Fallback options for pdf export (Revit versions 2022 and greater.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static PDFExportOptions CreateDefaultPDFExportOptions(string format, Document doc)
        {
            var scheme = NativeNamingRulesUtils.CreateNamingRuleFromFormatString(format, doc);
            var opts = new PDFExportOptions();
            opts.SetNamingRule(scheme);
            opts.ColorDepth = ColorDepthType.Color;
            opts.Combine = false;
            opts.ExportQuality = PDFExportQualityType.DPI1200;
            opts.HideCropBoundaries = true;
            opts.HideReferencePlane = true;
            opts.HideScopeBoxes = true;
            opts.HideUnreferencedViewTags = true;
            opts.MaskCoincidentLines = false;
            opts.RasterQuality = RasterQualityType.High;
            opts.OriginOffsetX = 0;
            opts.OriginOffsetY = 0;
            opts.PaperFormat = ExportPaperFormat.Default;
            opts.PaperOrientation = PageOrientationType.Auto;
            opts.PaperPlacement = PaperPlacementType.Center;
            return opts;
        }
#endif

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
            using (var fec = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ProjectInformation))
            {
                foreach (var element in fec)
                {
                    if (element is ProjectInfo)
                    {
                        var i = (ProjectInfo)element;
                        var p = i.LookupParameter("Project Config File");
                        if (p != null)
                        {
                            return p.AsString();
                        }
                    }
                }
                string central = FileUtilities.GetCentralFileName(doc);
                string s = Path.GetDirectoryName(central) + @"\SCexport.xml";
                return s;
            }
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

        public static void ToggleBooleanParameter(ICollection<ExportSheet> sheets, Document doc, bool turnOn, string paramName, bool isSheet)
        {
            if (sheets == null)
            {
                return;
            }
            using (Transaction t = new Transaction(doc))
            {
                if (t.Start("SCexport - Toggle Sheet Parameter") == TransactionStatus.Started)
                {
                    foreach (ExportSheet sheet in sheets)
                    {
                        if (isSheet)
                        {
                            sheet.ToggleParameterByName(turnOn, paramName);
                        }
                        else
                        {
                            sheet.ToggleTitleParameterByName(turnOn, paramName);
                        }
                    }
                    if (t.Commit() != TransactionStatus.Committed)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox("Failure", "Could not toggle north points");
                    }
                }
            }
        }

        // FIXME. Move this somewhere else and tidy it up.
        public static List<YesNoParameter> GetYesNoTitleblockParameters(ICollection<ExportSheet> sheets, Document doc)
        {
            var yesNoParameters = new List<YesNoParameter>();
            foreach (ExportSheet sheet in sheets)
            {
                var titleBlock = Manager.TitleBlockInstanceFromSheetNumber(sheet.SheetNumber, doc);
                var parameters = titleBlock.Parameters;
                foreach (var parameter in parameters)
                {
                    var p = parameter as Parameter;
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                    if (!yesNoParameters.Select(s => s.Name).Contains(p.Definition.Name)
                        && !(p.Element is ElementType)
                        && !p.IsReadOnly
                        && p.Definition.GetDataType() == SpecTypeId.Boolean.YesNo)
                    {
                        yesNoParameters.Add(new YesNoParameter(p.Definition.Name, null));
                    }
#else
                    if (!yesNoParameters.Select(s => s.Name).Contains(p.Definition.Name)
                        && !(p.Element is ElementType)
                        && !p.IsReadOnly
                        && p.Definition.ParameterType == ParameterType.YesNo)
                    {
                        yesNoParameters.Add(new YesNoParameter(p.Definition.Name, null));
                    }
#endif
                }
            }
            return yesNoParameters;
        }

        // FIXME. Move this somewhere else and tidy it up.
        public static List<YesNoParameter> GetYesNoSheetParameters(ICollection<ExportSheet> sheets, Document doc)
        {
            var yesNoParameters = new List<YesNoParameter>();
            foreach (ExportSheet sheet in sheets)
            {
                var parameters = sheet.Sheet.Parameters;
                foreach (var parameter in parameters)
                {
                    var p = parameter as Parameter;
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                    if (!yesNoParameters.Select(s => s.Name).Contains(p.Definition.Name)
                        && !(p.Element is ElementType)
                        && !p.IsReadOnly
                        && p.Definition.GetDataType() == SpecTypeId.Boolean.YesNo)
                    {
                        yesNoParameters.Add(new YesNoParameter(p.Definition.Name, null));
                    }
#else
                    if (!yesNoParameters.Select(s => s.Name).Contains(p.Definition.Name)
                        && !(p.Element is ElementType)
                        && !p.IsReadOnly
                        && p.Definition.ParameterType == ParameterType.YesNo)
                    {
                        yesNoParameters.Add(new YesNoParameter(p.Definition.Name, null));
                    }
#endif
                }
            }
            return yesNoParameters;
        }

        public static void ShowSheetsInSheetList(ICollection<ExportSheet> sheets, Document doc)
        {
            ChangeSheetVisiblityInSchedule(sheets, true, doc);
        }

        public static void HideSheetsInSheetList(ICollection<ExportSheet> sheets, Document doc)
        {
            ChangeSheetVisiblityInSchedule(sheets, false, doc);
        }

        public void AddExportOption(ExportOptions exportOptions)
        {
            exportFlags |= exportOptions;
        }

        [SecurityCritical]
        //[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
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

            if (!sheet.ValidExportName)
            {
                log.AddError(sheet.FullExportName, "Invalid file name, Export cancelled.");
                log.EndLoggingIndividualItem(startTime, null);
                return;
            }

            if (exportFlags.HasFlag(ExportOptions.DirectPDF))
            {
                ExportRevitPDF(sheet, log);
            }

#if !REVIT2022 && !REVIT2023 && !REVIT2024 && !REVIT2025
            if (sheet.SCPrintSetting != null)
            {
                if (exportFlags.HasFlag(ExportOptions.DWG))
                {
                    ExportDWG(sheet, log);
                }

                if (exportFlags.HasFlag(ExportOptions.PDF))
                {
                    ExportAdobePDF(sheet, log);
                }

                if (exportFlags.HasFlag(ExportOptions.PDF24))
                {
                    ExportPDF24(sheet, log);
                }
            }
            else
            {
                log.AddError(sheet.FullExportName, Resources.MessageNoPrintSettingAssigned);
            }
#else
            if (exportFlags.HasFlag(ExportOptions.DWG))
            {
                ExportDWG(sheet, log);
            }

            if (exportFlags.HasFlag(ExportOptions.PDF))
            {
                ExportAdobePDF(sheet, log);
            }

            if (exportFlags.HasFlag(ExportOptions.PDF24))
            {
                ExportPDF24(sheet, log);
            }
#endif
            log.EndLoggingIndividualItem(startTime, null);
        }

        public void SaveViewSet(string name, List<ExportSheet> selectedSheets)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var viewSetItem = new ViewSetItem(-1, name, selectedSheets.Select(s => s.Id.IntegerValue).ToList());
#pragma warning restore CS0618 // Type or member is obsolete
            AllViewSheetSets.Add(viewSetItem);
            using (Transaction t = new Transaction(Doc))
            {
                if (t.Start("Save view sheet set") == TransactionStatus.Started)
                {
                    var set = new ViewSet();
                    foreach (var s in selectedSheets)
                    {
                        set.Insert(s.Sheet);
                    }
                    var pm = Doc.PrintManager;
                    pm.PrintRange = PrintRange.Select;
                    pm.ViewSheetSetting.CurrentViewSheetSet = pm.ViewSheetSetting.InSession;
                    pm.ViewSheetSetting.InSession.Views = set;
                    pm.ViewSheetSetting.SaveAs(name);
                    if (t.Commit() != TransactionStatus.Committed)
                    {
                        t.RollBack();
                    }
                }
                else
                {
                    t.RollBack();
                }
            }
        }

        public bool HasExportOption(ExportOptions option)
        {
            return exportFlags.HasFlag(option);
        }

        public void LoadSettings()
        {
            PdfPrinterName = Settings1.Default.AdobePrinterDriver;
            PDF24PrinterName = Settings1.Default.PDF24PrinterDriver;
            PrinterNameA3 = Settings1.Default.A3PrinterDriver;
            PrinterNameLargeFormat = Settings1.Default.LargeFormatPrinterDriver;
            exportDirectory = Settings1.Default.ExportDir;
            AcadVersion = ACADVersion.Default;
            SaveHistory = Settings1.Default.SaveHistory;
            ShowExportLog = Settings1.Default.ShowExportLog;
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                ForceRevisionToDateString = false;
                UseDateForEmptyRevisions = false;
                Settings1.Default.ForceDateRevision = false;
                Settings1.Default.UseDateForEmptyRevisions = false;
#else
                ForceRevisionToDateString = Settings1.Default.ForceDateRevision;
                UseDateForEmptyRevisions = Settings1.Default.UseDateForEmptyRevisions;
#endif
            VerifyOnStartup = Settings1.Default.VerifyOnStartup;
            ExportAdditionalViewports = Settings1.Default.ExportAdditionalViewports;
            ExportViewportsOnly = Settings1.Default.ExportViewportsOnly;
        }

        public bool PDFSanityCheck()
        {
            var ps = new PrinterSettings();
            ps.PrinterName = PdfPrinterName;
            return ps.IsValid;
        }

        public bool PDF24SanityCheck()
        {
            var ps = new PrinterSettings();
            ps.PrinterName = PDF24PrinterName;
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
                SCaddinsApp.WindowManager.ShowMessageBox("Print Error", "print error");
            }
            log.EndLoggingIndividualItem(startTime, null);
        }

        public void RemoveExportOption(ExportOptions exportOptions)
        {
            exportFlags &= ~exportOptions;
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
            if (HasExportOption(option))
            {
                RemoveExportOption(option);
            }
            else
            {
                AddExportOption(option);
            }
        }

#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
        public bool TryGetExportPdfSettingsByName(string name, out PDFExportOptions options)
        {
            var collector = new FilteredElementCollector(Doc);
            collector.OfClass(typeof(ExportPDFSettings));
            foreach (var setting in collector)
            {
                if (setting.Name == name)
                {
                    var s = setting as ExportPDFSettings;
                    options = s.GetOptions();
                    return true;
                }
            }
            options = null;
            return false;
        }
#endif

        public void Update()
        {
            try
            {
                PrintManager pm = Doc.PrintManager;
                if (pm == null)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("PM is null");
                    return;
                }
                if (PrintSettings.SetPrinterByName(Doc, PdfPrinterName, pm))
                {
                    foreach (ExportSheet sc in AllSheets)
                    {
                        if (!sc.Verified)
                        {
                            sc.UpdateSheetInfo();
                        }
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Update error in ExportManager.");
            }
        }

        public void UpdateAllViewSheetSets()
        {
            AllViewSheetSets = GetAllViewSheetSets(Doc);
        }

        internal void LoadConfigFile()
        {
            FileNameTypes.Clear();

            //// Load config from project if it exists
            /// settings for xml file
            var settingsOne = Doc.ProjectInformation.LookupParameter("Primary SCexport Settings Name");
            var formatOne = Doc.ProjectInformation.LookupParameter("Primary SCexport Settings Format");
            var settingsTwo = Doc.ProjectInformation.LookupParameter("Secondary SCexport Settings Name");
            var formatTwo = Doc.ProjectInformation.LookupParameter("Secondary SCexport Settings Format");

            // If no args are set the script will run anyway without args.
            // arg will be auto set to the currently exported file. 
            var exportHookOne = Doc.ProjectInformation.LookupParameter("Primary Post Export Script");
            var exportHookOneArgs = Doc.ProjectInformation.LookupParameter("Primary Post Export Script Args");
            var exportHookOneFileExtensions = Doc.ProjectInformation.LookupParameter("Primary Post Export Extensions");
            var exportHookTwo = Doc.ProjectInformation.LookupParameter("Secondary Post Export Script");
            var exportHookTwoArgs = Doc.ProjectInformation.LookupParameter("Secondary Post Export Script Args");
            var exportHookTwoFileExtensions = Doc.ProjectInformation.LookupParameter("Secondary Post Export Extensions");

            // setting using Revit native naming (PDF export dialog)
            // naming scheme will be auto generated from these
            // export hooks above will also be used.
            var pdfSettingsOne = Doc.ProjectInformation.LookupParameter("Primary SCexport PDF Settings Name");
            var pdfSettingsTwo = Doc.ProjectInformation.LookupParameter("Secondary SCexport PDF Settings Name");

            var hook = new PostExportHookCommand();
            if (exportHookOne != null)
            {
                hook.SetCommand(exportHookOne.AsString());
                hook.SetName("ProjectConfigHookOne");

                if (exportHookOneFileExtensions != null)
                {
                    foreach (string s in exportHookOneFileExtensions.AsString().Split(';'))
                    {
                        hook.AddSupportedFilenameExtension(s);
                    }
                }

                if (exportHookOneArgs != null)
                {
                    var args = exportHookOneArgs.AsString();
                    hook.SetArguments(args);
                }
                else
                {
                    var args = @"$exportDir\$fullExportName$fileExtension"; // file extension includes the "."
                    hook.SetArguments(args);
                }
                postExportHooks.Add(hook.Name, hook);
            }

            var hook2 = new PostExportHookCommand();
            if (exportHookTwo != null)
            {
                hook2.SetCommand(exportHookTwo.AsString());
                hook2.SetName("ProjectConfigHookTwo");

                if (exportHookTwoFileExtensions != null)
                {
                    foreach (string s in exportHookTwoFileExtensions.AsString().Split(';'))
                    {
                        hook2.AddSupportedFilenameExtension(s);
                    }
                }

                if (exportHookTwoArgs != null)
                {
                    var args = exportHookTwoArgs.AsString();
                    hook2.SetArguments(args);
                }
                else
                {
                    var args = @"$exportDir\$fullExportName$fileExtension"; // file extension includes the "."
                    hook2.SetArguments(args);
                }
                postExportHooks.Add(hook2.Name, hook2);
            }

#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
            // Don't attempt to do this if not available (Revit < 2022)
            if (pdfSettingsOne != null)
            {
                PDFExportOptions opts;
                if (TryGetExportPdfSettingsByName(pdfSettingsOne.AsString(), out opts))
                {
                    var s = new SegmentedSheetName();
                    s.PDFExportOptions = opts;
                    s.Name = pdfSettingsOne.AsString();
                    if (exportHookOne != null)
                    {
                        s.Hooks.Add(hook.Name);
                    }
                    FileNameTypes.Add(s);
                }
            }

            if (pdfSettingsTwo != null)
            {
                PDFExportOptions opts;
                if (TryGetExportPdfSettingsByName(pdfSettingsTwo.AsString(), out opts))
                {
                    var s = new SegmentedSheetName();
                    s.PDFExportOptions = opts;
                    s.Name = pdfSettingsTwo.AsString();
                    if (exportHookTwo != null)
                    {
                        s.Hooks.Add(hook2.Name);
                    }
                    FileNameTypes.Add(s);
                }
            }

#endif

            if (settingsOne != null && formatOne != null)
            {
                var name = new SegmentedSheetName();
                name.Name = settingsOne.AsString();
                name.NameFormat = formatOne.AsString();
                if (exportHookOne != null)
                {
                    name.Hooks.Add(hook.Name);
                } 
                FileNameTypes.Add(name);
            } 

            if (settingsTwo != null && formatTwo != null)
            {
                var name = new SegmentedSheetName();
                name.Name = settingsTwo.AsString();
                name.NameFormat = formatTwo.AsString();
                if (exportHookTwo != null)
                {
                    name.Hooks.Add(hook2.Name);
                }
                FileNameTypes.Add(name);
            }

            // Load config settings from file if available
            var config = GetConfigFileName(Doc);
            var b = ImportXMLinfo(config);
            
            if (!b || FileNameTypes.Count <= 0)
            {
                var name = new SegmentedSheetName();
                name.Name = "YYYYMMDD-AD-NNN";
                name.NameFormat = "$projectNumber-$sheetNumber[$sheetRevision] - $sheetDescription";
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                name.PDFExportOptions = CreateDefaultPDFExportOptions(name.NameFormat, Doc);
#endif
                FileNameTypes.Add(name);
                FileNameScheme = name;
                FileNameScheme = FileNameTypes[0];
            }
            else
            {
                FileNameScheme = FileNameTypes[0];
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

        private static void ChangeSheetVisiblityInSchedule(ICollection<ExportSheet> sheets, bool showInSchedule, Document doc)
        {
            if (sheets == null)
            {
                return;
            }
            using (Transaction t = new Transaction(doc))
            {
                if (t.Start("SCexport - Change Sheet Visiblity In Schedules") == TransactionStatus.Started)
                {
                    foreach (ExportSheet sheet in sheets)
                    {
                        sheet.AppearsInSheetList = showInSchedule;
                    }
                    if (t.Commit() != TransactionStatus.Committed)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox("Failure", "Could not Change Sheet Visiblity In Schedules");
                    }
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
            using (FilteredElementCollector collector = new FilteredElementCollector(doc))
            {
                collector.OfClass(typeof(ViewSheetSet));
                foreach (var element in collector)
                {
                    var v = (ViewSheetSet)element;
#if REVIT2024 || REVIT2025
                    var viewIds = v.Views.Cast<View>()
                        .Where(vs => vs.ViewType == ViewType.DrawingSheet)
                        .Select(vs => (int)vs.Id.Value).ToList();
                    result.Add(new ViewSetItem((int)v.Id.Value, v.Name, viewIds));
#else
                    var viewIds = v.Views.Cast<View>()
                        .Where(vs => vs.ViewType == ViewType.DrawingSheet)
                        .Select(vs => vs.Id.IntegerValue).ToList();
                    result.Add(new ViewSetItem(v.Id.IntegerValue, v.Name, viewIds));
#endif
                }
            }
            return result;
        }

        /*private static void RemoveTitleBlock(
            ExportSheet vs,
            ICollection<ElementId> title,
            bool hide,
            Document doc)
        {
            if (!(doc.GetElement(vs.Id) is View view))
            {
                return;
            }
            var t = new Transaction(doc, "Hide Title");
            t.Start();
            try
            {
                if (hide)
                {
                    view.HideElements(title);
                }
                else
                {
                    view.UnhideElements(title);
                }
                t.Commit();
            }
            catch (ArgumentException e)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Revit", "cannot Hide Title: " + e.Message);
                t.RollBack();
            }
        }*/

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PrinterJobControl")]
        [SecurityCritical]
#if NET48
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
#endif
        private static bool SetAcrobatExportRegistryVal(string fileName, ExportLog log)
        {
            var exe = Process.GetCurrentProcess().MainModule.FileName;
            try
            {
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
            }
            catch (UnauthorizedAccessException ex)
            {
                log.AddError(fileName, ex.Message);
                return false;
            }
            catch (SecurityException ex)
            {
                log.AddError(fileName, ex.Message);
                return false;
            }
        }

        private static bool SetPDF24ExportRegistryVal(string dir, ExportLog log)
        {
            var exe = Process.GetCurrentProcess().MainModule.FileName;
            try
            {
                log.AddMessage("Attempting to set PDF24 Value with value");
                log.AddMessage("\t" + Constants.PDF24AutoSaveDir);
                log.AddMessage("\t" + exe);
                log.AddMessage("\t" + dir);
                Registry.SetValue(
                    Constants.PDF24AutoSaveDir,
                    "AutoSaveDir",
                    dir,
                    RegistryValueKind.String);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                log.AddError(dir, ex.Message);
                return false;
            }
            catch (SecurityException ex)
            {
                log.AddError(dir, ex.Message);
                return false;
            }
        }

        private void ExportRevitPDF(ExportSheet vs, ExportLog log)
        {
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025

            if (log != null)
            {
                log.AddMessage(Environment.NewLine + Resources.MessageStartingNativePDFExport);
            }
            else
            {
                return;
            }

            if (IsViewerMode())
            {
                log.AddError(vs.FullExportName, "Revit is in Viewer mode. Exporting is not allowed.");
                return;
            }

                List<ElementId> views;
            views = new List<ElementId>();
            views.Add(vs.Id);

            PDFExportOptions opts = vs.SegmentedFileName.PDFExportOptions;
            if (vs.ForceRasterPrint)
            {
                opts.AlwaysUseRaster = true;
            }

            if (FileUtilities.CanOverwriteFile(vs.FullExportPath(Resources.FileExtensionPDF)))
            {
                var name = vs.FullExportName + Resources.FileExtensionPDF;
                log.AddMessage(Resources.MessageExportingToDirectory + vs.ExportDirectory);
                log.AddMessage(Resources.MessageExportingToFileName + name);
                Doc.Export(vs.ExportDirectory, views, opts);

                if (vs.SegmentedFileName.Hooks.Count > 0)
                {
                    FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionPDF));
                    RunExportHooks(Resources.FileExtensionPDF, vs, log);
                }
            }
            else
            {
                log.AddError(vs.FullExportName, "File existts, not overwriting.");
            }
#endif
        }

        [SecurityCritical]
#if NET48
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
#endif
        private void ExportAdobePDF(ExportSheet vs, ExportLog log)
        {
#if !REVIT2022 && !REVIT2023 && !REVIT2024 && !REVIT2025
            ExportPDF(vs, log);
#else
                log.AddError(vs.FullExportName, "PDF export with Adobe Acrobat is not supported in Revit versions > 2021.");
                return;
#endif
        }

        [SecurityCritical]
#if NET48
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
#endif
        private void ExportPDF24(ExportSheet vs, ExportLog log)
        {
            ExportPDF(vs, log);
        }

        [SecurityCritical]
#if NET48
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
#endif
        private void ExportPDF(ExportSheet vs, ExportLog log)
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

            var printerName = string.Empty;
            if (exportFlags.HasFlag(ExportOptions.PDF))
            {
                printerName = PdfPrinterName;
            }

            if (exportFlags.HasFlag(ExportOptions.PDF24))
            {
                printerName = PDF24PrinterName;
            }

            if (!PrintSettings.PrintToFile(Doc, vs, pm, Resources.FileExtensionPDF, printerName))
            {
                log.AddError(vs.FullExportName, Resources.ErrorFailedToAssignPrintSetting + vs.PrintSettingName);
                return;
            }

            if (exportFlags.HasFlag(ExportOptions.PDF)) {
                if (!SetAcrobatExportRegistryVal(vs.FullExportPath(Resources.FileExtensionPDF), log))
                {
                    log.AddError(vs.FullExportName, "Unable to write to registry.");
                    return;
                }
            }

            if (exportFlags.HasFlag(ExportOptions.PDF24)) {
                if (!SetPDF24ExportRegistryVal(vs.ExportDirectory, log))
                {
                    log.AddError(vs.FullExportName, "Unable to write to registry.");
                    return;
                }
            }

            if (!vs.ValidExportName)
            {
                log.AddError(vs.FullExportName, "Filename contains invalid characters: " + vs.FullExportName);
                return;
            }

            if (!Directory.Exists(vs.ExportDirectory))
            {
                log.AddError(vs.FullExportName, "Directory does not exist: " + vs.FullExportName);
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

                if (vs.SegmentedFileName.Hooks.Count > 0)
                {
                    FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionPDF));
                    RunExportHooks(Resources.FileExtensionPDF, vs, log);
                }

                Common.SystemUtilities.KillAllProcesses("acrotray");
            } else
            {
                ////log.AddError(vs.FullExportName, Resources.ErrorCantOverwriteFile);
            }
        }

        // FIXME this is nasty
        private void ExportDWG(ExportSheet vs, ExportLog log)
        {
            if (log != null)
            {
                log.AddMessage(Environment.NewLine + Resources.MessageStartingDWGExport);
            }
            else
            {
                return;
            }

            if (IsViewerMode())
            {
                log.AddError(vs.FullExportName, "Revit is in Viewer mode. Exporting DWG files is not allowed");
                return;
            }

            if (!vs.ValidExportName)
            {
                log.AddError(vs.FullExportName, "Filename contains invalid characters: " + vs.FullExportName);
                return;
            }

            // List<ElementId> titleBlockHidden;
            // titleBlockHidden = new List<ElementId>();
            // var titleBlock = TitleBlockInstanceFromSheetNumber(vs.SheetNumber, Doc);
            // titleBlockHidden.Add(titleBlock.Id);

            // if (removeTitle)
            // {
            //     log.AddMessage(Resources.MessageAttemptingToHideTitleBlock);
            //     RemoveTitleBlock(vs, titleBlockHidden, true, Doc);
            // }
            List<ElementId> views;
            views = new List<ElementId>();

            if (ExportViewportsOnly || ExportAdditionalViewports)
            {
                foreach (var viewOnSheet in vs.Sheet.GetAllPlacedViews())
                {
                    var individualViewOnSheet = Doc.GetElement(viewOnSheet) as View;
                    if (individualViewOnSheet.ViewType == ViewType.FloorPlan || individualViewOnSheet.ViewType == ViewType.CeilingPlan)
                    {
                        views.Add(individualViewOnSheet.Id);
                    }
                }
            }

            if (views.Count == 0 || ExportAdditionalViewports)
            {
                views.Add(vs.Id);
            }

            using (var opts = GetDefaultDWGExportOptions())
            {
                log.AddMessage(Resources.MessageAssigningExportOptions + opts);
                foreach (var view in views)
                {
                    List<ElementId> viewList;
                    viewList = new List<ElementId>();
                    viewList.Add(view);
                    var name = vs.FullExportName + Resources.FileExtensionDWG;
                    if (views.Count > 1 && !(Doc.GetElement(view) is ViewSheet))
                    {
                        name += @" - " + ((View)Doc.GetElement(view)).Name;
                    }
                    log.AddMessage(Resources.MessageExportingToDirectory + vs.ExportDirectory);
                    log.AddMessage(Resources.MessageExportingToFileName + name);
                    Doc.Export(vs.ExportDirectory, name, viewList, opts);
                }
            }

            if (vs.SegmentedFileName.Hooks.Count > 0)
            {
                FileUtilities.WaitForFileAccess(vs.FullExportPath(Resources.FileExtensionDWG));

                // SCaddinsApp.WindowManager.ShowMessageBox("Running Export Hook");
                RunExportHooks(Resources.FileExtensionDWG, vs, log);
            }

            // if (removeTitle)
            // {
            //     log.AddMessage(Resources.MessageShowingTitleBlock);
            //     RemoveTitleBlock(vs, titleBlockHidden, false, Doc);
            // }
        }

        private DWGExportOptions GetDefaultDWGExportOptions()
        {
            var opts = new DWGExportOptions();
            opts.MergedViews = true;
            opts.FileVersion = AcadVersion;
            opts.HideScopeBox = true;
            opts.HideUnreferenceViewTags = true;
            if (ExportViewportsOnly || ExportAdditionalViewports)
            {
                opts.TargetUnit = ExportUnit.Meter;
                opts.SharedCoords = true;
            }
            return opts;
        }

        private bool ImportXMLinfo(string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }

            if (!ValidateXML(filename))
            {
                return false;
            }

#pragma warning disable CA3075 // Insecure DTD processing in XML
            using (var reader = new XmlTextReader(filename))
            {
#pragma warning restore CA3075 // Insecure DTD processing in XML
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "PostExportHook")
                    {
                        var hook = new PostExportHookCommand();
                        if (reader.AttributeCount > 0)
                        {
                            hook.SetName(reader.GetAttribute("name"));
                        }
                        do
                        {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
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

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "FilenameScheme")
                    {
                        var name = new SegmentedSheetName();
                        if (reader.AttributeCount > 0)
                        {
                            name.Name = reader.GetAttribute("name");
                        }
                        do
                        {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "Format":
                                        name.NameFormat = reader.ReadString();
                                        break;

                                    case "Hook":
                                        name.Hooks.Add(reader.ReadString());
                                        break;
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                                    case "PDFNamingRule":
                                        PDFExportOptions opts;
                                        if (TryGetExportPdfSettingsByName(reader.ReadString(), out opts))
                                        {
                                            name.PDFExportOptions = opts;
                                        }
                                        break;
#endif
                                }
                            }
                        } while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "FilenameScheme"));
                        FileNameTypes.Add(name);
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                        if (name.PDFExportOptions == null && name.NameFormat != null)
                        {
                            name.PDFExportOptions = CreateDefaultPDFExportOptions(name.NameFormat, Doc);
                        }
#endif
                    }
                }

                if (FileNameTypes.Count > 0)
                {
                    FileNameScheme = FileNameTypes[0];
                    foreach (ExportSheet sheet in AllSheets)
                    {
                        sheet.SetSegmentedSheetName(FileNameScheme);
                    }
                }
            }
            return true;
        }

        private void RunExportHooks(string extension, ExportSheet vs, ExportLog log)
        {
            // SCaddinsApp.WindowManager.ShowMessageBox(postExportHooks.Count.ToString());
            // SCaddinsApp.WindowManager.ShowMessageBox(postExportHooks.ElementAt(0).Value.GetCommand());
            for (int i = 0; i < postExportHooks.Count; i++)
            {
                if (postExportHooks.ElementAt(i).Value.HasExtension(extension))
                {
                    if (vs.SegmentedFileName.Hooks.Count < 1)
                    {
                        // SCaddinsApp.WindowManager.ShowMessageBox("Hook Extension Not Found");
                        return;
                    }

                    if (vs.SegmentedFileName.Hooks.Contains(postExportHooks.ElementAt(i).Key))
                    {
                        // SCaddinsApp.WindowManager.ShowMessageBox("Running Hook");
                        postExportHooks.ElementAt(i).Value.Run(vs, extension, log);
                    } 
                }
            }
        }

        private void SetDefaultFlags()
        {
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
            AddExportOption(ExportOptions.DirectPDF);
#else
             if (PDFSanityCheck()) {
                    AddExportOption(ExportOptions.PDF);
             }
             else if (PDF24SanityCheck())
             {
                AddExportOption(ExportOptions.PDF24);
            }
#endif
            if (Settings1.Default.HideTitleBlocks)
            {
                AddExportOption(ExportOptions.NoTitle);
            }

            forceDate |= Settings1.Default.ForceDateRevision;
        }

        private bool ValidateXML(string filename)
        {
            string errorMessage = string.Empty;
            if (filename == null || !File.Exists(filename))
            {
                return false;
            }
            try
            {
                var settings = new XmlReaderSettings();
#if !DEBUG
                settings.Schemas.Add(
                    null, SCaddins.Constants.InstallDirectory + @"\etc\SCexport.xsd");
#else
                    settings.Schemas.Add(null, @"C:\Code\cs\scaddins\etc\SCexport.xsd");
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
            }
            catch (XmlSchemaValidationException ex)
            {
                errorMessage += "Error reading xml file:" + filename + " - " + ex.Message;
            }
            catch (XmlException ex)
            {
                errorMessage += "Error reading xml file:" + filename + " - " + ex.Message;
            }
            catch (XmlSchemaException ex)
            {
                errorMessage += "Error reading xml file:" + filename + " - " + ex.Message;
            }
            catch (ArgumentNullException ex)
            {
                errorMessage += "Error reading xml file:" + filename + " - " + ex.Message;
            }
            catch (UriFormatException ex)
            {
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
