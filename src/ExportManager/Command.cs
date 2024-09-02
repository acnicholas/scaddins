// (C) Copyright 2012-2020 by Andrew Nicholas
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
    using System.Collections.Generic;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using static SCaddinsApp;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
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
                WindowManager.ShowMessageBox("FAIL", "Please save the file before continuing");
                return Result.Failed;
            }

            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                return Result.Failed;
            }

            var views = new List<ViewSheet>();

            if (uidoc.Document.ActiveView.ViewType == ViewType.ProjectBrowser)
            {
                var s = uidoc.Selection.GetElementIds();
                foreach (var id in s)
                {
                    var projectBrowserView = uidoc.Document.GetElement(id);
                    if (projectBrowserView is View)
                    {
                        var v = (View)projectBrowserView;
                        if (v.ViewType == ViewType.ProjectBrowser)
                        {
                            continue;
                        }
                        if (v is ViewSheet)
                        {
                            views.Add((ViewSheet)v);
                        }
                    }
                }
            }

            // Deselect all elements before continuing so they don't appear incorrectly
            uidoc.Selection.SetElementIds(new List<ElementId>());

            var manager = new Manager(uidoc);
            var log = new ExportLog();
            if (views == null)
            {
                views = new List<ViewSheet>();
            }
            var vm = new ViewModels.SCexportViewModel(manager, views);
            var wm = WindowManager;
            wm.ShowDialogAsync(vm, null, ViewModels.SCexportViewModel.DefaultWindowSettings);

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
                }

                var progressVm = new ViewModels.ProgressMonitorViewModel
                {
                    MaximumValue = vm.SelectedSheets.Count,
                    Value = 0
                };

                log.Clear();
                log.Start(exportType + " Started.");

                WindowManager.ShowModalWindowAsync(progressVm, null, ViewModels.ProgressMonitorViewModel.DefaultWindowSettings);

                if (manager.SaveHistory)
                {
                    RecentExport.Save(manager, vm.SelectedSheets);
                }

                foreach (var sheet in vm.SelectedSheets)
                {
                    progressVm.ProgressSummary += @" --> " + exportType + @" " + sheet.FullExportName + "...";

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
                            return Result.Succeeded;
                    }

                    progressVm.Value++;
                    string niceTime = string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        "OK  [ time {0:hh\\.mm\\:ss}  total {1:hh\\.mm\\:ss}  ~remaining {2:hh\\.mm\\:ss}]",
                        log.LastItemElapsedTime,
                        log.TimeSinceStart,
                        System.TimeSpan.FromTicks(log.TimeSinceStart.Ticks / progressVm.Value * (progressVm.MaximumValue - progressVm.Value)));
                    progressVm.ProgressSummary += niceTime + System.Environment.NewLine;

                    if (progressVm.CancelPressed)
                    {
                        break;
                    }
                }

                log.Stop("Finished");
                progressVm.Stop(log);
                progressVm.ProcessComplete = true;
            }

            if (manager.ShowExportLog || log.Errors > 0)
            {
                var exportLogViewModel = new ViewModels.ExportLogViewModel(log);
                WindowManager.ShowDialogAsync(exportLogViewModel, null, ViewModels.ExportLogViewModel.DefaultWindowSettings);
            }

            return Result.Succeeded;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
