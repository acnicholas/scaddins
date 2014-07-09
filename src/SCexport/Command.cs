// (C) Copyright 2012-2013 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCexport.
// SCexport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCexport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCexport.  If not, see <http://www.gnu.org/licenses/>.

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

    /// <summary>
    /// The add-in command..
    /// </summary>
    public class Command : IExternalCommand
    {
        /// <summary>
        /// Execute the specified command.
        /// </summary>
        /// <param name="commandData">Command data.</param>
        /// <param name="message">Message for something.</param>
        /// <param name="elements">Set of selected elements.</param>
        /// <returns> The result. </returns>
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            string exportDir = Constants.DefaultExportDir;
            if (!System.IO.Directory.Exists(exportDir)) {
                System.IO.Directory.CreateDirectory(exportDir);
            }
            
            // make sure teh file has been saved before continuing
            if (FileUtils.GetCentralFilename(
                commandData.Application.ActiveUIDocument.Document) == string.Empty) {
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
