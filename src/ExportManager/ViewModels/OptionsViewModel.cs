// (C) Copyright 2018 by Andrew Nicholas
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

namespace SCaddins.ExportManager.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Caliburn.Micro;

    internal class OptionsViewModel : PropertyChangedBase
    {
        private Manager exportManager;
        private SCexportViewModel exportManagerViewModel;

        public OptionsViewModel(Manager exportManager, SCexportViewModel exportManagerViewModel)
        {
            this.exportManager = exportManager;
            this.exportManagerViewModel = exportManagerViewModel;
        }

        public static List<Autodesk.Revit.DB.ACADVersion> AutoCADExportVersions {
            get
            {
                var versions = Enum.GetValues(typeof(Autodesk.Revit.DB.ACADVersion)).Cast<Autodesk.Revit.DB.ACADVersion>().ToList();
                return versions;
            }
        }

        public static string ForceRasterPrintParameterName {
            get
            {
                return SCaddins.ExportManager.Settings1.Default.UseRasterPrinterParameter;
            }

            set
            {
                if (value == SCaddins.ExportManager.Settings1.Default.UseRasterPrinterParameter) {
                    return;
                }
                SCaddins.ExportManager.Settings1.Default.UseRasterPrinterParameter = value;
                SCaddins.ExportManager.Settings1.Default.Save();
            }
        }

        public static string NorthPointVisibilityParameterName {
            get
            {
                return SCaddins.ExportManager.Settings1.Default.NorthPointVisibilityParameter;
            }

            set
            {
                if (value == SCaddins.ExportManager.Settings1.Default.NorthPointVisibilityParameter) {
                    return;
                }
                SCaddins.ExportManager.Settings1.Default.NorthPointVisibilityParameter = value;
                SCaddins.ExportManager.Settings1.Default.Save();
            }
        }

        public static string ScaleBarScaleParameterName {
            get
            {
                return SCaddins.ExportManager.Settings1.Default.ScalebarScaleParameter;
            }

            set
            {
                if (value == SCaddins.ExportManager.Settings1.Default.ScalebarScaleParameter) {
                    return;
                }
                SCaddins.ExportManager.Settings1.Default.ScalebarScaleParameter = value;
                SCaddins.ExportManager.Settings1.Default.Save();
            }
        }

        public static Autodesk.Revit.DB.ACADVersion SelectedAutoCADExportVersion {
            get
            {
                return Manager.AcadVersion;
            }

            set
            {
                if (value == Manager.AcadVersion) {
                    return;
                }
                Manager.AcadVersion = value;
                Settings1.Default.AcadExportVersion = value;
                Settings1.Default.Save();
            }
        }

        public static bool ShowSummaryLog {
            get
            {
                return SCaddins.ExportManager.Settings1.Default.ShowExportLog;
            }

            set
            {
                if (value == SCaddins.ExportManager.Settings1.Default.ShowExportLog) {
                    return;
                }
                SCaddins.ExportManager.Settings1.Default.ShowExportLog = value;
                SCaddins.ExportManager.Settings1.Default.Save();
            }
        }

        public static string TextEditorBinPath {
            get
            {
                return SCaddins.ExportManager.Settings1.Default.TextEditor;
            }

            set
            {
                if (value == SCaddins.ExportManager.Settings1.Default.TextEditor) {
                    return;
                }
                SCaddins.ExportManager.Settings1.Default.TextEditor = value;
                SCaddins.ExportManager.Settings1.Default.Save();
            }
        }

        public string A3PrinterName {
            get
            {
                return exportManager.PrinterNameA3;
            }

            set
            {
                if (value == exportManager.PrinterNameA3) {
                    return;
                }
                exportManager.PrinterNameA3 = value;
                SCaddins.ExportManager.Settings1.Default.A3PrinterDriver = value;
                SCaddins.ExportManager.Settings1.Default.Save();
                NotifyOfPropertyChange(() => A3PrinterName);
            }
        }

        public string AdobePDFPrintDriverName
        {
            get
            {
                return exportManager.PdfPrinterName;
            }

            set
            {
                if (value == exportManager.PdfPrinterName) {
                    return;
                }
                exportManager.PdfPrinterName = value;
                Settings1.Default.AdobePrinterDriver = value;
                Settings1.Default.Save();
                NotifyOfPropertyChange(() => AdobePDFPrintDriverName);
            }
        }

        public bool DateForEmptyRevisions
        {
            get
            {
                return exportManager.UseDateForEmptyRevisions;
            }

            set
            {
                if (exportManager.UseDateForEmptyRevisions != value) {
                    exportManager.UseDateForEmptyRevisions = value;
                    Settings1.Default.UseDateForEmptyRevisions = value;
                    Settings1.Default.Save();
                }
            }
        }

        public bool ExportViewportsOnly
        {
            get
            {
                return exportManager.ExportViewportsOnly;
            }

            set
            {
                if (exportManager.ExportViewportsOnly != value)
                {
                    exportManager.ExportViewportsOnly = value;
                    Settings1.Default.ExportViewportsOnly = value;
                    Settings1.Default.Save();
                }
            }
        }

        public bool ExportAdobePDF
        {
            get
            {
                return exportManager.HasExportOption(ExportOptions.PDF);
            }

            set
            {
                if (value) {
                    exportManager.AddExportOption(ExportOptions.PDF);
                } else {
                    exportManager.RemoveExportOption(ExportOptions.PDF);
                }
            }
        }

        public bool ExportDGN
        {
            get; set;
        }

        public string ExportDirectory
        {
            get
            {
                return exportManager.ExportDirectory;
            }

            set
            {
                if (value == exportManager.ExportDirectory) {
                    return;
                }
                exportManager.ExportDirectory = value;
                Settings1.Default.ExportDir = value;
                Settings1.Default.Save();
                NotifyOfPropertyChange(() => ExportDirectory);
            }
        }

        public bool ExportDWF
        {
            get; set;
        }

        public bool ExportDWG
        {
            get
            {
                return exportManager.HasExportOption(ExportOptions.DWG);
            }

            set
            {
                if (value) {
                    exportManager.AddExportOption(ExportOptions.DWG);
                } else {
                    exportManager.RemoveExportOption(ExportOptions.DWG);
                }
            }
        }

        public bool ExportPostscriptPDF
        {
            get
            {
                return exportManager.HasExportOption(ExportOptions.GhostscriptPDF);
            }

            set
            {
                if (value) {
                    exportManager.AddExportOption(ExportOptions.GhostscriptPDF);
                } else {
                    exportManager.RemoveExportOption(ExportOptions.GhostscriptPDF);
                }
            }
        }

        public List<SegmentedSheetName> FileNamingSchemes
        {
            get
            {
                return exportManager.FileNameTypes;
            }
        }

        public bool ForceDateForAllRevisions
        {
            get
            {
                return exportManager.ForceRevisionToDateString;
            }

            set
            {
                if (exportManager.ForceRevisionToDateString != value) {
                    exportManager.ForceRevisionToDateString = value;
                    Settings1.Default.ForceDateRevision = value;
                    Settings1.Default.Save();
                }
            }
        }

        public string GhostscriptBinLocation
        {
            get
            {
                return exportManager.GhostscriptBinDirectory;
            }

            set
            {
                if (value == exportManager.GhostscriptBinDirectory) {
                    return;
                }
                exportManager.GhostscriptBinDirectory = value;
                Settings1.Default.GSBinDirectory = value;
                Settings1.Default.Save();
                NotifyOfPropertyChange(() => GhostscriptBinLocation);
            }
        }

        public string GhostscriptLibLocation
        {
            get
            {
                return exportManager.GhostscriptLibDirectory;
            }

            set
            {
                if (value == exportManager.GhostscriptLibDirectory) {
                    return;
                }
                exportManager.GhostscriptLibDirectory = value;
                Settings1.Default.GSLibDirectory = value;
                Settings1.Default.Save();
                NotifyOfPropertyChange(() => GhostscriptLibLocation);
            }
        }

        public bool HideTitleBlocksForCadExports
        {
            get
            {
                return exportManager.HasExportOption(ExportOptions.NoTitle);
            }

            set
            {
                if (value == exportManager.HasExportOption(ExportOptions.NoTitle)) {
                    return;
                }
                if (value) {
                    exportManager.AddExportOption(ExportOptions.NoTitle);
                } else {
                    exportManager.RemoveExportOption(ExportOptions.NoTitle);
                }
                Settings1.Default.HideTitleBlocks = value;
                Settings1.Default.Save();
            }
        }

        public string LargeFormatPrinterName
        {
            get
            {
                return exportManager.PrinterNameLargeFormat;
            }

            set
            {
                if (value == exportManager.PrinterNameLargeFormat) {
                    return;
                }
                exportManager.PrinterNameLargeFormat = value;
                SCaddins.ExportManager.Settings1.Default.LargeFormatPrinterDriver = value;
                SCaddins.ExportManager.Settings1.Default.Save();
                NotifyOfPropertyChange(() => LargeFormatPrinterName);
            }
        }

        public string PostscriptPrintDriverName
        {
            get
            {
                return exportManager.PostscriptPrinterName;
            }

            set
            {
                if (value == exportManager.PostscriptPrinterName) {
                    return;
                }
                exportManager.PostscriptPrinterName = value;
                SCaddins.ExportManager.Settings1.Default.PSPrinterDriver = value;
                SCaddins.ExportManager.Settings1.Default.Save();
                NotifyOfPropertyChange(() => PostscriptPrintDriverName);
            }
        }

        public SegmentedSheetName SelectedFileNamingScheme
        {
            get
            {
                return exportManager.FileNameScheme;
            }

            set
            {
                if (value == exportManager.FileNameScheme) {
                    return;
                }
                exportManager.FileNameScheme = value;
            }
        }

        public static string SelectPrinter(string printerToSelect, string currentPrinter)
        {
            dynamic settings = new ExpandoObject();
            settings.MinHeight = 150;
            settings.MinWidth = 300;
            settings.Title = printerToSelect;
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            var printerViewModel = new PrinterSelectionViewModel(currentPrinter);
            bool? result = SCaddinsApp.WindowManager.ShowDialog(printerViewModel, null, settings);
            if (result.HasValue) {
                return result.Value == true ? printerViewModel.SelectedPrinter : currentPrinter;
            }
            return currentPrinter;
        }

        public void CreateProjectConfigFile()
        {
            FileUtilities.CreateConfigFile(exportManager.Doc);
        }

        public void EditProjectConfigFile()
        {
            FileUtilities.EditConfigFile(exportManager.Doc);
        }

        public void SelectA3Printer()
        {
            A3PrinterName = SelectPrinter("Select A3 Printer", A3PrinterName);
        }

        public void SelectAdobePrinter()
        {
            AdobePDFPrintDriverName = SelectPrinter("Select Adobe Printer", AdobePDFPrintDriverName);
        }

        public void SelectExportDirectory()
        {
            string dir;
            var result = SCaddinsApp.WindowManager.ShowDirectorySelectionDialog(GhostscriptLibLocation, out dir);
            if (result.HasValue && result.Value == true)
            {
                ExportDirectory = dir;
            }
        }

        public void SelectGhostscriptBinLocation()
        {
            string path;
            var result = SCaddinsApp.WindowManager.ShowDirectorySelectionDialog(GhostscriptBinLocation, out path);
            if (result.HasValue && result.Value == true)
            {
                GhostscriptBinLocation = path;
            }
        }

        public void SelectGhostscriptLibLocation()
        {
            string path;
            var result = SCaddinsApp.WindowManager.ShowDirectorySelectionDialog(GhostscriptLibLocation, out path);
            if (result.HasValue && result.Value == true)
            {
                GhostscriptLibLocation = path;
            }
        }

        public void SelectLargeFormatPrinter()
        {
            LargeFormatPrinterName = SelectPrinter("Select Large Format Printer", LargeFormatPrinterName);
        }

        public void SelectPostscriptPrinter()
        {
            PostscriptPrintDriverName = SelectPrinter("Select Postscript Printer", PostscriptPrintDriverName);
        }
    }
}