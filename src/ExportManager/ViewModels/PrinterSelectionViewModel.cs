
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Caliburn.Micro;

namespace SCaddins.ExportManager.ViewModels
{

    public class PrinterSelectionViewModel : Screen
    {
        private List<string> printers;
        private string selectedPrinter;

        public PrinterSelectionViewModel(string desiredPrinterName)
        {   
            printers = new List<string>();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            SelectedPrinter = desiredPrinterName;
        }

        public List<string> Printers
        {
            get { return printers; }
        }

        public string SelectedPrinter
        {
            get { return selectedPrinter; }
            set
            {
                    selectedPrinter = value;
                    NotifyOfPropertyChange(() => SelectedPrinter);
            }
        }

        public void OK()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}
