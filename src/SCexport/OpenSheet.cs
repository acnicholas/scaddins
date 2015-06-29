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

    /// <summary>
    /// Open a selected sheet in the current Revit doc.
    /// </summary>
    public class OpenSheet : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            DialogHandler.AddRevitDialogHandler(commandData.Application);
            var osd = new OpenSheetDialog();
            System.Windows.Forms.DialogResult tdr = osd.ShowDialog();
            if (tdr == System.Windows.Forms.DialogResult.OK) {
                Autodesk.Revit.DB.FamilyInstance result =
                    ExportManager.GetTitleBlockFamily(osd.Value, doc);
                if (result == null) {
                    result = ExportManager.GetTitleBlockFamily("CD" + osd.Value, doc);    
                }
                if (result == null) {
                    result = ExportManager.GetTitleBlockFamily("DA" + osd.Value, doc);    
                }
                if (result == null) {
                    result = ExportManager.GetTitleBlockFamily("SK" + osd.Value, doc);    
                }
                if (result == null) {
                    result = ExportManager.GetTitleBlockFamily("AD-CD" + osd.Value, doc);    
                }
                if (result == null) {
                    result = ExportManager.GetTitleBlockFamily("AD-DA" + osd.Value, doc);    
                }
                if (result == null) {
                    result = ExportManager.GetTitleBlockFamily("AD-SK" + osd.Value, doc);    
                }
                if (result != null) {
                    commandData.Application.ActiveUIDocument.ShowElements(result);
                    return Autodesk.Revit.UI.Result.Succeeded;
                } else {
                    TaskDialog.Show("SCexport", "Sheet: " + osd.Value + " cannot be found.");
                    return Autodesk.Revit.UI.Result.Failed;
                }
            } else {
                TaskDialog.Show("SCexport", "Sheet: " + osd.Value + " cannot be found.");
                return Autodesk.Revit.UI.Result.Failed;
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
