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
    using System;
    using System.IO;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            if (commandData == null) {
                return Result.Failed;
            }

            if (!System.IO.Directory.Exists(Constants.DefaultExportDirectory)) {
                System.IO.Directory.CreateDirectory(Constants.DefaultExportDirectory);
            }

            if (string.IsNullOrEmpty(FileUtilities.GetCentralFileName(commandData.Application.ActiveUIDocument.Document))) {
                using (var fail = new TaskDialog("FAIL")) {
                    fail.MainContent = "Please save the file before continuing";
                    fail.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                    fail.Show();
                }
                return Result.Failed;
            }

            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null) {
                return Autodesk.Revit.UI.Result.Failed;
            }
            //using (var form = new MainForm(commandData.Application.ActiveUIDocument)) {
            //    form.ShowDialog();
            //}
            
            var window = new Window1(new ExportManager(uidoc.Document));
            window.ShowDialog();

            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
