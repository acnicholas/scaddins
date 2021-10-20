﻿// (C) Copyright 2018-2021 by Andrew Nicholas
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
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq;
    using Caliburn.Micro;

    internal class OptionsViewModel : PropertyChangedBase
    {
        private Manager exportManager;
        private BindableCollection<string> fileNamingSchemes;
        private string selectedFileNamingScheme;

        public OptionsViewModel(Manager exportManager)
        {
            this.exportManager = exportManager;
            fileNamingSchemes = new BindableCollection<string>();
            foreach (var s in exportManager.FileNameTypes)
            {
                FileNamingSchemes.Add(s.Name);
            }
            selectedFileNamingScheme = exportManager.FileNameScheme.Name;
        }

        public static List<Autodesk.Revit.DB.ACADVersion> AutoCADExportVersions {
            get
            {
                var versions = Enum.GetValues(typeof(Autodesk.Revit.DB.ACADVersion)).Cast<Autodesk.Revit.DB.ACADVersion>().ToList();
                return versions;
            }
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 480;
                settings.Title = "SCexport - Options";
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                      new Uri("pack://application:,,,/SCaddins;component/Assets/scexport.png"));
                settings.ShowInTaskbar = false;
                settings.ResizeMode = System.Windows.ResizeMode.NoResize;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                return settings;
            }
        }

        public static string ForceRasterPrintParameterName
        {
            get
            {
                return Settings1.Default.UseRasterPrinterParameter;
            }

            set
            {
                if (value == Settings1.Default.UseRasterPrinterParameter) {
                    return;
                }
                Settings1.Default.UseRasterPrinterParameter = value;
                Settings1.Default.Save();
            }
        }

        public static string NorthPointVisibilityParameterName
        {
            get
            {
                return Settings1.Default.NorthPointVisibilityParameter;
            }

            set
            {
                if (value == Settings1.Default.NorthPointVisibilityParameter)
                {
                    return;
                }
                Settings1.Default.NorthPointVisibilityParameter = value;
                Settings1.Default.Save();
            }
        }

        public static bool SaveHistory
        {
            get
            {
                return Settings1.Default.SaveHistory;
            }

            set
            {
                if (value == Settings1.Default.SaveHistory)
                {
                    return;
                }
                Settings1.Default.SaveHistory = value;
                Settings1.Default.Save();
            }
        }

        public static string ScaleBarScaleParameterName
        {
            get
            {
                return Settings1.Default.ScalebarScaleParameter;
            }

            set
            {
                if (value == Settings1.Default.ScalebarScaleParameter)
                {
                    return;
                }
                Settings1.Default.ScalebarScaleParameter = value;
                Settings1.Default.Save();
            }
        }

        public static Autodesk.Revit.DB.ACADVersion SelectedAutoCADExportVersion
        {
            get
            {
                return Manager.AcadVersion;
            }

            set
            {
                if (value == Manager.AcadVersion)
                {
                    return;
                }
                Manager.AcadVersion = value;
            }
        }

        public static bool ShowSummaryLog
        {
            get
            {
                return Settings1.Default.ShowExportLog;
            }

            set
            {
                if (value == Settings1.Default.ShowExportLog)
                {
                    return;
                }
                Settings1.Default.ShowExportLog = value;
                Settings1.Default.Save();
            }
        }

        public static string TextEditorBinPath
        {
            get
            {
                return Settings1.Default.TextEditor;
            }

            set
            {
                if (value == Settings1.Default.TextEditor)
                {
                    return;
                }
                Settings1.Default.TextEditor = value;
                Settings1.Default.Save();
            }
        }

        public static string TitleBlockStampName
        {
            get
            {
                return Settings1.Default.TitleBlockStampFamilyName;
            }

            set
            {
                if (value == Settings1.Default.TitleBlockStampFamilyName)
                {
                    return;
                }
                Settings1.Default.TitleBlockStampFamilyName = value;
                Settings1.Default.Save();
            }
        }

        public string A3PrinterName
        {
            get
            {
                return exportManager.PrinterNameA3;
            }

            set
            {
                if (value == exportManager.PrinterNameA3)
                {
                    return;
                }
                exportManager.PrinterNameA3 = value;
                Settings1.Default.A3PrinterDriver = value;
                Settings1.Default.Save();
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
                if (value == exportManager.PdfPrinterName)
                {
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
                if (exportManager.UseDateForEmptyRevisions != value)
                {
                    exportManager.UseDateForEmptyRevisions = value;
                    Settings1.Default.UseDateForEmptyRevisions = value;
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
                if (value)
                {
                    exportManager.AddExportOption(ExportOptions.PDF);
                }
                else
                {
                    exportManager.RemoveExportOption(ExportOptions.PDF);
                }
            }
        }

        public bool ExportAdobePDFEnabled
        {
#if REVIT2022
            get { return false; }
#else
            get { return true; }
#endif
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
                if (value == exportManager.ExportDirectory)
                {
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
                if (value)
                {
                    exportManager.AddExportOption(ExportOptions.DWG);
                }
                else
                {
                    exportManager.RemoveExportOption(ExportOptions.DWG);
                }
            }
        }

        public bool ExportRevitPDFEnabled
        {
#if REVIT2022
            get { return true; }
#else
            get { return false; }
#endif
        }

        public bool ExportRevitPDF
        {
            get
            {
                return exportManager.HasExportOption(ExportOptions.DirectPDF);
            }

            set
            {
                if (value)
                {
                    exportManager.AddExportOption(ExportOptions.DirectPDF);
                }
                else
                {
                    exportManager.RemoveExportOption(ExportOptions.DirectPDF);
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

        public BindableCollection<string> FileNamingSchemes
        {
            get
            {
                return fileNamingSchemes;
            }

            set
            {
                fileNamingSchemes = value;
                NotifyOfPropertyChange(() => FileNamingSchemes);
                NotifyOfPropertyChange(() => SelectedFileNamingScheme);
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
                if (exportManager.ForceRevisionToDateString != value)
                {
                    exportManager.ForceRevisionToDateString = value;
                    Settings1.Default.ForceDateRevision = value;
                    Settings1.Default.Save();
                }
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
                if (value == exportManager.HasExportOption(ExportOptions.NoTitle))
                {
                    return;
                }
                if (value)
                {
                    exportManager.AddExportOption(ExportOptions.NoTitle);
                }
                else
                {
                    exportManager.RemoveExportOption(ExportOptions.NoTitle);
                }
                Settings1.Default.HideTitleBlocks = value;
                Settings1.Default.Save();
            }
        }

        public string InvalidSheetNameCharsAsString
        {
            get
            {
                return Settings1.Default.InvalidSheetNameChars;
            }

            set
            {
                if (value == Settings1.Default.InvalidSheetNameChars)
                {
                    return;
                }
                Settings1.Default.InvalidSheetNameChars = value;
                Settings1.Default.Save();
                foreach (var sheet in exportManager.AllSheets)
                {
                    sheet.Refresh();
                }
            }
        }

        public string InvalidSheetNumberCharsAsString
        {
            get
            {
                return Settings1.Default.InvalidSheetNumberChars;
            }

            set
            {
                if (value == Settings1.Default.InvalidSheetNumberChars)
                {
                    return;
                }
                Settings1.Default.InvalidSheetNumberChars = value;
                Settings1.Default.Save();
                foreach (var sheet in exportManager.AllSheets)
                {
                    sheet.Refresh();
                }
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
                Settings1.Default.LargeFormatPrinterDriver = value;
                Settings1.Default.Save();
                NotifyOfPropertyChange(() => LargeFormatPrinterName);
            }
        }

        public string SelectedFileNamingScheme
        {
            get
            {
                return selectedFileNamingScheme;
            }

            set
            {
                if (value == selectedFileNamingScheme) {
                    return;
                }
                if (value == null)
                {
                    return;
                }
                selectedFileNamingScheme = value;
                exportManager.SetFileNameScheme(value);
                NotifyOfPropertyChange(() => SelectedFileNamingScheme);
            }
        }

        public bool VerifyOnStartup
        {
            get
            {
                return exportManager.VerifyOnStartup;
            }

            set
            {
                if (value == exportManager.VerifyOnStartup)
                {
                    return;
                }
                exportManager.VerifyOnStartup = value;
                Settings1.Default.VerifyOnStartup = value;
                Settings1.Default.Save();
                NotifyOfPropertyChange(() => VerifyOnStartup);
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
                return result.Value ? printerViewModel.SelectedPrinter : currentPrinter;
            }
            return currentPrinter;
        }

        public void CreateProjectConfigFile()
        {
            FileUtilities.CreateConfigFile(exportManager.Doc);
        }

        public void EditProjectConfigFile()
        {
            FileUtilities.EditConfigFileModal(exportManager.Doc);
            exportManager.LoadConfigFile();
            FileNamingSchemes.Clear();
            foreach (var s in exportManager.FileNameTypes)
            {
                FileNamingSchemes.Add(s.Name);
            }
            SelectedFileNamingScheme = exportManager.FileNameScheme.Name;
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
            var result = SCaddinsApp.WindowManager.ShowDirectorySelectionDialog(ExportDirectory, out dir);
            if (result.HasValue && result.Value)
            {
                ExportDirectory = dir;
            }
        }

        public void SelectLargeFormatPrinter()
        {
            LargeFormatPrinterName = SelectPrinter("Select Large Format Printer", LargeFormatPrinterName);
        }
    }
}