// (C) Copyright 2014-2017 by Andrew Nicholas
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

namespace SCaddins.ViewUtilities
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class CreateUserViewCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null) {
                return Result.Failed;
            }

            Document doc = commandData.Application.ActiveUIDocument.Document;
            View view = doc.ActiveView;
            View newView = null;

            using (Transaction t = new Transaction(doc)) {
                if (t.Start("SCuv Copies User View") == TransactionStatus.Started) {
                    newView = UserView.Create(view, doc);
                    if (newView == null) {
                        UserView.ShowErrorDialog(view);    
                    }
                    if (t.Commit() != TransactionStatus.Committed) {
                        TaskDialog.Show("Failed", "Could not create user view[s]");
                    }
                }
            }
            UIApplication uiapp = new UIApplication(doc.Application);
            uiapp.ActiveUIDocument.ActiveView = newView;
            return Result.Succeeded;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
