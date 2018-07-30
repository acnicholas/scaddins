using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Dynamic;

namespace SCaddins.ExportManager.ViewModels
{
    class OptionsViewModel : PropertyChangedBase
    {
        private ExportManager exportManager;
        private SCexportViewModel exportManagerViewModel;

        public OptionsViewModel(ExportManager exportManager, SCexportViewModel exportManagerViewModel)
        {
            this.exportManager = exportManager;
            this.exportManagerViewModel = exportManagerViewModel;
        }

        public bool ExportAdobePDF
        {
            get { return exportManager.HasExportOption(ExportOptions.PDF); }
            set
            {
                if (value) {
                    exportManager.AddExportOption(ExportOptions.PDF);
                } else {
                    exportManager.RemoveExportOption(ExportOptions.PDF);
                }
            }
        }

        public bool ExportPostscriptPDF
        {
            get { return exportManager.HasExportOption(ExportOptions.GhostscriptPDF); }
            set
            {
                if (value) {
                    exportManager.AddExportOption(ExportOptions.GhostscriptPDF);
                } else {
                    exportManager.RemoveExportOption(ExportOptions.GhostscriptPDF);
                }
            }
        }

        public bool ExportDWG
        {
            get { return exportManager.HasExportOption(ExportOptions.DWG); }
            set
            {
                if (value) {
                    exportManager.AddExportOption(ExportOptions.DWG);
                } else {
                    exportManager.RemoveExportOption(ExportOptions.DWG);
                }
            }
        }

        public bool ExportDGN
        {
            get; set;
        }

        public bool ExportDWF
        {
            get; set;
        }

        public string ExportDirectory
        {
            get { return exportManager.ExportDirectory; }
            set
            {
                if (value == exportManager.ExportDirectory) return;
                exportManager.ExportDirectory = value;
                NotifyOfPropertyChange(() => ExportDirectory);
            }
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

        public string ScaleBarScaleParameterName
        {
            get { return SCaddins.ExportManager.Settings1.Default.ScalebarScaleParameter; }
            set
            {
                if (value == SCaddins.ExportManager.Settings1.Default.ScalebarScaleParameter) return;
                SCaddins.ExportManager.Settings1.Default.ScalebarScaleParameter = value;
                SCaddins.ExportManager.Settings1.Default.Save();
            }
        }

        public string NorthPointVisibilityParameterName
        {
            get { return SCaddins.ExportManager.Settings1.Default.NorthPointVisibilityParameter; }
            set
            {
                if (value == SCaddins.ExportManager.Settings1.Default.NorthPointVisibilityParameter) return;
                SCaddins.ExportManager.Settings1.Default.NorthPointVisibilityParameter = value;
                SCaddins.ExportManager.Settings1.Default.Save();
            }
        }

        public List<Autodesk.Revit.DB.ACADVersion> AutoCADExportVersions
        {
            get
            {
                var versions = Enum.GetValues(typeof(Autodesk.Revit.DB.ACADVersion)).Cast<Autodesk.Revit.DB.ACADVersion>().ToList();
                return versions;
            }
        }

        public Autodesk.Revit.DB.ACADVersion SelectedAutoCADExportVersion
        {
            get { return exportManager.AcadVersion; }
            set
            {
                if (value == exportManager.AcadVersion) return;
                exportManager.AcadVersion = value;
                Settings1.Default.AcadExportVersion = value;
                Settings1.Default.Save();
            }
        }

        public List<SegmentedSheetName> FileNamingSchemes
        {
            get { return exportManager.FileNameTypes; }
        }

        public SegmentedSheetName SelectedFileNamingScheme
        {
            get { return exportManager.FileNameScheme; }
            set
            {
                if (value == exportManager.FileNameScheme) return;
                exportManager.FileNameScheme = value;
            }
        }

        public bool DateForEmptyRevisions
        {
            get { return exportManager.UseDateForEmptyRevisions;  }
            set { exportManager.UseDateForEmptyRevisions = value; }
        }

        public bool ForceDateForAllRevisions
        {
            get { return exportManager.ForceRevisionToDateString; }
            set { exportManager.ForceRevisionToDateString = value; }
        }

        public bool HideTitleBlocksForCadExports
        {
            get { return exportManager.HasExportOption(ExportOptions.NoTitle); }
            set
            {
                if (value == exportManager.HasExportOption(ExportOptions.NoTitle)) return;
                if (value) {
                    exportManager.AddExportOption(ExportOptions.NoTitle);
                } else {
                    exportManager.RemoveExportOption(ExportOptions.NoTitle);
                }
                Settings1.Default.HideTitleBlocks = value;
                Settings1.Default.Save();
            }
        }

        public void CreateProjectConfigFile()
        {
            FileUtilities.CreateConfigFile(exportManager.Doc);
        }

        public void EditProjectConfigFile()
        {
            FileUtilities.EditConfigFile(exportManager.Doc);
        }

        public string TextEditorBinPath
        {
            get { return SCaddins.ExportManager.Settings1.Default.TextEditor; }
            set
            {
                if (value == SCaddins.ExportManager.Settings1.Default.TextEditor) return;
                SCaddins.ExportManager.Settings1.Default.TextEditor = value;
                SCaddins.ExportManager.Settings1.Default.Save();
            }
        }

        public string AdobePDFPrintDriverName
        {
            get { return exportManager.PdfPrinterName; }
            set
            {
                if (value == exportManager.PdfPrinterName) return;
                exportManager.PdfPrinterName = value;
                Settings1.Default.AdobePrinterDriver = value;
                Settings1.Default.Save();
                NotifyOfPropertyChange(() => AdobePDFPrintDriverName);
            }
        }

        public string SelectPrinter(string printerToSelect, string currentPrinter)
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
            if (result.HasValue)
            {
                return result.Value == true ? printerViewModel.SelectedPrinter : currentPrinter;
            }
            return currentPrinter;
        }

        public void SelectAdobePrinter()
        {
            AdobePDFPrintDriverName = SelectPrinter("Select Adobe Printer", AdobePDFPrintDriverName);
        }

        public void SelectPostscriptPrinter()
        {
            PostscriptPrintDriverName = SelectPrinter("Select Postscript Printer", PostscriptPrintDriverName);
        }

        public void SelectA3Printer()
        {
            A3PrinterName = SelectPrinter("Select A3 Printer", A3PrinterName);
        }

        public void SelectLargeFormatPrinter()
        {
            LargeFormatPrinterName = SelectPrinter("Select Large Format Printer", LargeFormatPrinterName);
        }

        public string PostscriptPrintDriverName
        {
            get { return exportManager.PostscriptPrinterName; }
            set
            {
                if (value == exportManager.PostscriptPrinterName) return;
                exportManager.PostscriptPrinterName = value;
                SCaddins.ExportManager.Settings1.Default.PSPrinterDriver = value;
                SCaddins.ExportManager.Settings1.Default.Save();
                NotifyOfPropertyChange(() => PostscriptPrintDriverName);
            }
        }

        public string A3PrinterName
        {
            get { return exportManager.PrinterNameA3; }
            set
            {
                if (value == exportManager.PrinterNameA3) return;
                exportManager.PrinterNameA3 = value;
                SCaddins.ExportManager.Settings1.Default.A3PrinterDriver = value;
                SCaddins.ExportManager.Settings1.Default.Save();
                NotifyOfPropertyChange(() => A3PrinterName);
            }
        }

        public string LargeFormatPrinterName
        {
            get { return exportManager.PrinterNameLargeFormat; }
            set
            {
                if (value == exportManager.PrinterNameLargeFormat) return;
                exportManager.PrinterNameLargeFormat = value;
                SCaddins.ExportManager.Settings1.Default.LargeFormatPrinterDriver = value;
                SCaddins.ExportManager.Settings1.Default.Save();
                NotifyOfPropertyChange(() => LargeFormatPrinterName);
            }
        }

        public string GhostscriptBinLocation
        {
            get { return exportManager.GhostscriptBinDirectory; }
            set
            {
                if (value == exportManager.GhostscriptBinDirectory) return;
                exportManager.GhostscriptBinDirectory = value;
                Settings1.Default.GSBinDirectory = value;
                Settings1.Default.Save();
                NotifyOfPropertyChange(() => GhostscriptBinLocation);
            }
        }

        public void SelectGhostscriptBinLocation()
        {
            string path;
            var result = SCaddinsApp.WindowManager.ShowDirectorySelectionDialog(GhostscriptBinLocation, out path);
            if(result.HasValue && result.Value == true)
            {
                GhostscriptBinLocation = path;
            }
        }

        public string GhostscriptLibLocation
        {
            get { return exportManager.GhostscriptLibDirectory; }
            set
            {
                if (value == exportManager.GhostscriptLibDirectory) return;
                exportManager.GhostscriptLibDirectory = value;
                Settings1.Default.GSLibDirectory = value;
                Settings1.Default.Save();
                NotifyOfPropertyChange(() => GhostscriptLibLocation);
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

        public bool ShowSummaryLog
        {
            get { return SCaddins.ExportManager.Settings1.Default.ShowExportLog; }
            set
            {
                if (value == SCaddins.ExportManager.Settings1.Default.ShowExportLog) return;
                SCaddins.ExportManager.Settings1.Default.ShowExportLog = value;
                SCaddins.ExportManager.Settings1.Default.Save();
            }
        }
    }
}
