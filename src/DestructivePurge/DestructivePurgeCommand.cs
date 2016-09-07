// (C) Copyright 2013-2014 by Andrew Nicholas andrewnicholas@iinet.net.au
//
// This file is part of SCaddins
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

namespace SCaddins.SCwash
{
    using System;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            string warning = "SCwash can be a weapon of mass destruction!" + System.Environment.NewLine +
                System.Environment.NewLine +
                "make sure you dettach your model from central before running this Add-in." + System.Environment.NewLine +
                System.Environment.NewLine +
                "continue?";

            UIDocument udoc = commandData.Application.ActiveUIDocument;
            var td = new TaskDialog("SCwash WARNING!");
            td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            td.MainInstruction = "WARNING!";
            td.MainContent = warning;
            td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
            TaskDialogResult tr = td.Show();
            if (tr == TaskDialogResult.No) {
                return Autodesk.Revit.UI.Result.Succeeded;
            }

            SCwashForm form = null;
            var transaction = new TransactionGroup(commandData.Application.ActiveUIDocument.Document);
            try {
                transaction.Start("SCwash");
                form = new SCwashForm(udoc.Document, udoc);
                if (null != form && false == form.IsDisposed) {
                    form.ShowDialog();
                }
                transaction.Commit();
            }
            catch (InvalidOperationException ex) {
                transaction.RollBack();
                message = ex.Message;
                return Result.Failed;
            }
            finally {
                if (null != form && false == form.IsDisposed)
                {
                    form.Dispose();
                }
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
