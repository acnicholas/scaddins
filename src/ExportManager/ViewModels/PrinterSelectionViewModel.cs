// (C) Copyright 2018-2020 by Andrew Nicholas
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
    using System.Collections.Generic;
    using Caliburn.Micro;

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
            get
            {
                return selectedPrinter;
            }

            set
            {
                selectedPrinter = value;
                NotifyOfPropertyChange(() => SelectedPrinter);
            }
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void OK()
        {
            TryCloseAsync(true);
        }
    }
}