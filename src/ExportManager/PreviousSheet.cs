// (C) Copyright 2013-2020 by Andrew Nicholas
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

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]

    public class PreviousSheet : IExternalCommand
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

            Document doc = commandData.Application.ActiveUIDocument.Document;
            View currentView = doc.ActiveView;
            if (currentView.ViewType != ViewType.DrawingSheet)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("SCexport", "PreviousSheet can only be run if the active view is a sheet");
                return Result.Failed;
            }
            else
            {
                var vs = currentView as ViewSheet;
                Manager.OpenPreviousSheet(commandData.Application.ActiveUIDocument, vs);
                return Result.Succeeded;
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
