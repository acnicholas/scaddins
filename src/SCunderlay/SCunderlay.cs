// (C) Copyright 2015 by Andrew Nicholas
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

namespace SCaddins.SCunderlay
{
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
            var uidoc = commandData.Application.ActiveUIDocument;
            RemoveUnderlay(uidoc);
            return Result.Succeeded;
        }
        
        private void RemoveUnderlay(UIDocument uidoc)
        {
            var selection = uidoc.Selection;
            TaskDialog.Show("Test", selection.Elements.Size.ToString());
            if(selection.Elements.Size < 1) 
                return;
            var t = new Transaction(uidoc.Document);
            t.Start("Remove Underlays");
            foreach (Element element in selection.Elements) {
                if (element.Category.Id.IntegerValue == (int)(BuiltInCategory.OST_Views)) {
                    var param = element.get_Parameter(BuiltInParameter.VIEW_UNDERLAY_ID);
                    if (param != null) {
                        param.Set(ElementId.InvalidElementId);
                    }
                }
            }
            t.Commit();
        }
        
    }
}

