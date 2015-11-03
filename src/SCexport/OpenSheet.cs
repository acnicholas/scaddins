// (C) Copyright 2013-2014 by Andrew Nicholas
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
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

   [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class OpenSheet : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            DialogHandler.AddRevitDialogHandler(commandData.Application);
            
            var openSheetDialog = new OpenSheetDialog();
            System.Windows.Forms.DialogResult openSheetDialogResult = openSheetDialog.ShowDialog();
            
            if (openSheetDialogResult != System.Windows.Forms.DialogResult.OK) {
                return Autodesk.Revit.UI.Result.Failed;
            }
            
            FamilyInstance titleBlockInstance = null;
            string[] possiblePrefixes = { string.Empty, "CD", "DA", "SK", "AD-CD", "AD-DA", "AD-SK" };
            foreach (string s in possiblePrefixes) {
                titleBlockInstance = 
                    ExportManager.TitleBlockInstanceFromSheetNumber(s + openSheetDialog.Value, doc);
                if (titleBlockInstance != null) {
                    commandData.Application.ActiveUIDocument.ShowElements(titleBlockInstance);
                    return Autodesk.Revit.UI.Result.Succeeded;
                }
            }
            
            TaskDialog.Show("SCexport", "Sheet: " + openSheetDialog.Value + " cannot be found.");
            return Autodesk.Revit.UI.Result.Failed;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
