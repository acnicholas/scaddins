using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SCaddins.ExportManager.ViewModels
{
    class OptionsViewModel : PropertyChangedBase
    {
        private ExportManager exportManager;

        public OptionsViewModel(ExportManager exportManager)
        {
            this.exportManager = exportManager;    
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

        public void Save()
        {

        }

        public void Cancel()
        {

        }

        //Advanced Options

        public void CreateProjectConfigFile()
        {

        }

        public void EditProjectConfigFile()
        {

        }

        public string TExtEditorBinPath
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

        public void SelectAdobePDFPrintDriver()
        {

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

        public void SelectPostscriptPrintDriver()
        {

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

        public void SelectA3Printer()
        {

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

        public void SelectLargeFormatPrinter()
        {

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
