// (C) Copyright 2017-2020 by Andrew Nicholas
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

namespace SCaddins.RenameUtilities
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using System;
    using System.Collections.Generic;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RenameUtilitiesCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            System.Diagnostics.Trace.WriteLine("Starting RenameUtilitiesCommand");

            if (commandData == null)
            {
                System.Diagnostics.Trace.WriteLine("commandData == null");
                throw new ArgumentNullException(nameof(commandData));
            }

            Document doc = commandData.Application.ActiveUIDocument.Document;
            if (doc == null)
            {
                System.Diagnostics.Trace.WriteLine("doc == null");
                return Result.Failed;
            }

            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                System.Diagnostics.Trace.WriteLine("uidoc == null");
                return Result.Failed;
            }

            //collect items from project browser if any are selected.
            var pbElements = new List<ElementId>();

            System.Diagnostics.Trace.WriteLine("Selecion check starting");

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
                        pbElements.Add(((Element)v).Id);
                    }
                }
            }

            System.Diagnostics.Trace.WriteLine("Selecion check complete");

            // Deselect all elements before continuing so they don't appear incorrectly
            uidoc.Selection.SetElementIds(new List<ElementId>());

            RenameManager manager;

            //if project browser has a selection rename only those
            if (pbElements.Count > 0)
            {
                System.Diagnostics.Trace.WriteLine("Starting rename with Project Browser selecion");
                manager = new RenameManager(doc, pbElements);
            } else
            {
                System.Diagnostics.Trace.WriteLine("Starting rename with no selecion");
                manager = new RenameManager(doc);
            }

            var vm = new ViewModels.RenameUtilitiesViewModel(manager);
            SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, ViewModels.RenameUtilitiesViewModel.DefaultWindowSettings);

            return Result.Succeeded;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
