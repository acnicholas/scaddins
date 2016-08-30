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
    
    public static class Utilities
    {
        public static void RemoveUnderlays(
            ICollection<SCaddins.SCexport.ExportSheet> sheets, Document doc)
        {
            var t = new Transaction(doc, "Remove Underlays");
            t.Start();
            foreach (SCaddins.SCexport.ExportSheet sheet in sheets) {
                #if REVIT2014
                foreach (View v in sheet.Sheet.Views) {
                    RemoveUnderlay(v);
                }  
                #else
                foreach (ElementId id in sheet.Sheet.GetAllPlacedViews()) {
                    var v = (View)doc.GetElement(id);
                    RemoveUnderlay(v);
                } 
                #endif
            }
            t.Commit();            
        }
        
        public static void RemoveUnderlays(UIDocument uidoc)
        {
            var selection = uidoc.Selection;
            #if REVIT2014
            if (selection.Elements.Size < 1) {
                return;
            }
            var t = new Transaction(uidoc.Document);
            t.Start("Remove Underlays");
            foreach (Element element in selection.Elements) {
                RemoveUnderlay(element);
            }
            t.Commit();
            #else
            if (selection.GetElementIds().Count < 1) {
                return;
            }
            var t = new Transaction(uidoc.Document);
            t.Start("Remove Underlays");
            foreach (ElementId id in selection.GetElementIds()) {
                RemoveUnderlay(uidoc.Document.GetElement(id));
            }
            t.Commit();
            #endif
        }
        
        private static void RemoveUnderlay(Element element)
        {
            if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Views) {
                #if REVIT2016 || REVIT2017
                var param = element.get_Parameter(BuiltInParameter.VIEW_UNDERLAY_BOTTOM_ID);
                if (param != null) {
                    param.Set(ElementId.InvalidElementId);
                }
                #else
                var param = element.get_Parameter(BuiltInParameter.VIEW_UNDERLAY_ID);
                if (param != null) {
                    param.Set(ElementId.InvalidElementId);
                }
                #endif
            }   
        }     
    }
}