// (C) Copyright 2012-2014 by Andrew Nicholas
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
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using System.Runtime.InteropServices;

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(System.IntPtr hWnd, int nCmdShow);

        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            if (commandData == null)
            {
                return Result.Failed;
            }

            if (!System.IO.Directory.Exists(Constants.DefaultExportDirectory))
            {
                System.IO.Directory.CreateDirectory(Constants.DefaultExportDirectory);
            }

            if (string.IsNullOrEmpty(FileUtilities.GetCentralFileName(commandData.Application.ActiveUIDocument.Document)))
            {
                using (var fail = new TaskDialog("FAIL"))
                {
                    fail.MainContent = "Please save the file before continuing";
                    fail.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                    fail.Show();
                }
                return Result.Failed;
            }

            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                return Autodesk.Revit.UI.Result.Failed;
            }

            var manager = new ExportManager(uidoc);
            var log = new ExportLog();
            var vm = new ViewModels.SCexportViewModel(manager);
            SCaddinsApp.WindowManager.ShowDialog(vm, null, ViewModels.SCexportViewModel.DefaultWindowSettings);

            var closeMode = vm.CloseStatus;

            if (vm.CloseStatus != ViewModels.SCexportViewModel.CloseMode.Exit)
            {
                string exportType = string.Empty;

                switch (vm.CloseStatus)
                {
                    case ViewModels.SCexportViewModel.CloseMode.Export:
                        exportType = "Exporting";
                        break;
                    case ViewModels.SCexportViewModel.CloseMode.Print:
                    case ViewModels.SCexportViewModel.CloseMode.PrintA3:
                    case ViewModels.SCexportViewModel.CloseMode.PrintA2:
                        exportType = "Printing";
                        break;
                    default:
                        break;
                }

                var progressVm = new ViewModels.ProgressMonitorViewModel();
                progressVm.MaximumValue = vm.SelectedSheets.Count;
                progressVm.Value = 0;

                log.Clear();
                log.Start(exportType + " Started.");

                SCaddinsApp.WindowManager.ShowWindow(progressVm, null, ViewModels.ProgressMonitorViewModel.DefaultWindowSettings);

                foreach (var sheet in vm.SelectedSheets)
                {
                    progressVm.ProgressSummary += exportType + @" " + sheet.FullExportName + "...";

                    switch (vm.CloseStatus)
                    {
                        case ViewModels.SCexportViewModel.CloseMode.Export:
                            manager.ExportSheet(sheet, log);
                            break;
                        case ViewModels.SCexportViewModel.CloseMode.Print:
                            manager.Print(sheet, manager.PrinterNameLargeFormat, 1, log);
                            break;
                        case ViewModels.SCexportViewModel.CloseMode.PrintA3:
                            manager.Print(sheet, manager.PrinterNameA3, 3, log);
                            break;
                        case ViewModels.SCexportViewModel.CloseMode.PrintA2:
                            manager.Print(sheet, manager.PrinterNameLargeFormat, 2, log);
                            break;
                        default:
                            break;
                    }

                    progressVm.ProgressSummary += "OK" + System.Environment.NewLine;
                    progressVm.Value++;
                    if (progressVm.CancelPressed)
                    {
                        break;
                    }
                }

                log.Stop("Finished");
                progressVm.ProcessComplete = true;

            }

            ////if (manager.ShowExportLog && log != null)
            ////{
            ////    var logVM = new ViewModels.ExportLogViewModel(log);
            ////    SCaddinsApp.WindowManager.ShowDialog(logVM, null, ViewModels.ExportLogViewModel.DefaultWindowSettings);
            ////}


            return Autodesk.Revit.UI.Result.Succeeded;
        }

        ////private void TryHideAcrobatProgress()
        ////{
        ////    System.IntPtr hWnd = FindWindow(null, @"Creating Adobe PDF");
        ////    if ((int)hWnd > 0)
        ////    {
        ////        ShowWindow(hWnd, 0);
        ////    } else
        ////    {
        ////        ////TaskDialog.Show("TEST", "nope");
        ////    }
        ////}
    }
}


/* vim: set ts=4 sw=4 nu expandtab: */
