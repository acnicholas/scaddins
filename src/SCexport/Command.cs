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

namespace SCaddins.SCexport
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
            const string exportDir = Constants.DefaultExportDir;
            if (!System.IO.Directory.Exists(exportDir)) {
                System.IO.Directory.CreateDirectory(exportDir);
            }
            
            if (string.IsNullOrEmpty(FileUtilities.GetCentralFileName(
                    commandData.Application.ActiveUIDocument.Document))) {
                var fail = new TaskDialog("FAIL");
                fail.MainContent = "Please save the file before continuing";
                fail.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                fail.Show();
                return Result.Failed;
            }
            
            MainForm form = null;
            var transaction = new TransactionGroup(
                    commandData.Application.ActiveUIDocument.Document);
            try {
                transaction.Start("SCexport");
                form = new MainForm(
                        commandData.Application.ActiveUIDocument);

                if (null != form && false == form.IsDisposed) {
                    form.ShowDialog();
                }

                transaction.Commit();
            } catch (System.Exception ex) {
                transaction.RollBack();
                message = ex.Message;
                return Result.Failed;
            } finally {
                if (null != form && false == form.IsDisposed) {
                    form.Dispose();
                }
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
