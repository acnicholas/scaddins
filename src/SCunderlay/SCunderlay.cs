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
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    
    public static class Utils
    {
        public static void RemoveUnderlays(
            ICollection<SCaddins.SCexport.SCexportSheet> sheets, Document doc)
        {
            var t = new Transaction(doc, "Remove Underlays");
            t.Start();
            foreach (SCaddins.SCexport.SCexportSheet sheet in sheets) {
                foreach (View v in sheet.Sheet.Views) {
                    RemoveUnderlay(v);
                }  
            }
            t.Commit();            
        }
        
        public static void RemoveUnderlays(UIDocument uidoc)
        {
            var selection = uidoc.Selection;
            if (selection.Elements.Size < 1)
                return;
            var t = new Transaction(uidoc.Document);
            t.Start("Remove Underlays");
            foreach (Element element in selection.Elements) {
                RemoveUnderlay(element);
            }
            t.Commit();
        }
        
        private static void RemoveUnderlay(Element element)
        {
            if (element.Category.Id.IntegerValue == (int)(BuiltInCategory.OST_Views)) {
                var param = element.get_Parameter(BuiltInParameter.VIEW_UNDERLAY_ID);
                if (param != null) {
                    param.Set(ElementId.InvalidElementId);
                }
            }   
        }
        
    }
}

