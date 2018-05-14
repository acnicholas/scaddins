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
        private WindowManager windowManager;

        public OptionsViewModel(ExportManager exportManager, WindowManager windowManager)
        {
            this.exportManager = exportManager;
            this.windowManager = windowManager;
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
                value = exportManager.AcadVersion;
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
            get { return exportManager.ForceRevisionToDateString;  }
            set { exportManager.ForceRevisionToDateString = value; }
        }

        public bool ForceDateForAllRevisions
        {
            get; set;
        }

        public bool HideTitleBlocksForCadExports
        {
            get; set;
        }

        //Advanced Options

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
            get; set;
        }

        //Print Options
        public string AdobePDFPrintDriverName
        {
            get { return exportManager.PdfPrinterName; }
            set
            {
                if (value == exportManager.PdfPrinterName) return;
                exportManager.PdfPrinterName = value;
            }
        }


        public void SelectPrinter(string printerToSelect)
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 200;
            settings.Width = 400;
            settings.Title = printerToSelect;
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            var printerViewModel = new PrinterSelectionViewModel("Adobe PDF");
            windowManager.ShowDialog(printerViewModel, null, settings);
        }

        public void SelectAdobePrinter()
        {
            SelectPrinter("Select Adobe Printer");
        }

        public void SelectPostscriptPrinter()
        {
            SelectPrinter("Select Postscript Printer");
        }

        public void SelectA3Printer()
        {
            SelectPrinter("Select A3 Printer");
        }

        public void SelectLargeFormatPrinter()
        {
            SelectPrinter("Select Large Format Printer");
        }

        public string PostscriptPrintDriverName
        {
            get { return exportManager.PostscriptPrinterName; }
            set
            {
                if (value == exportManager.PostscriptPrinterName) return;
                exportManager.PostscriptPrinterName = value;
            }
        }

        public string A3PrinterName
        {
            get { return exportManager.PrinterNameA3; }
            set
            {
                if (value == exportManager.PrinterNameA3) return;
                exportManager.PrinterNameA3 = value;
            }
        }

        public string LargeFormatPrinterName
        {
            get { return exportManager.PrinterNameLargeFormat; }
            set
            {
                if (value == exportManager.PrinterNameLargeFormat) return;
                exportManager.PrinterNameLargeFormat = value;
            }
        }

        //Ghostscript
        public string GhostscriptBinLocation
        {
            get { return exportManager.GhostscriptBinDirectory; }
            set
            {
                if (value == exportManager.GhostscriptBinDirectory) return;
                exportManager.GhostscriptBinDirectory = value;
            }
        }

        public string GhostscriptLibLocation
        {
            get { return exportManager.GhostscriptLibDirectory; }
            set
            {
                if (value == exportManager.GhostscriptLibDirectory) return;
                exportManager.GhostscriptLibDirectory = value;
            }
        }

        public bool AlwaysShowExportLog
        {
            get; set;
        }
    }
}
