using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.ExportManager.ViewModels
{
    class OptionsViewModel
    {
        private ExportManager exportManager;

        public OptionsViewModel(ExportManager exportManager)
        {
            this.exportManager = exportManager;    
        }

        public bool ExportPDF
        {
            get; set;
        }

        public bool ExportGhostscriptPDF
        {
            get; set;
        }

        public bool ExportDWG
        {
            get; set;
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
            get; set;
        }

        public string ScaleBarScaleParameterName
        {
            get; set;
        }

        public string NorthPointVisibilityParameterName
        {
            get; set;
        }

        public string SelectedAutoCADExportVersion
        {
            get; set;
        }

        public string SelectedFileNamingScheme
        {
            get; set;
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
            get; set;
        }

        public void SelectAdobePDFPrintDriver()
        {

        }

        public string PostscriptPrintDriverName
        {
            get; set;
        }

        public void SelectPostscriptPrintDriver()
        {

        }

        public string A3PrinterName
        {
            get; set;
        }

        public void SelectA3Printer()
        {

        }

        public string LargeFormatPrinterName
        {
            get; set;
        }

        public void SelectLargeFormatPrinter()
        {

        }

        //Ghostscript
        public string GhostscriptBinLocation
        {
            get; set;
        }

        public string GhostscriptLibLocation
        {
            get; set;
        }

        public bool AlwaysShowExportLog
        {
            get; set;
        }
    }
}
