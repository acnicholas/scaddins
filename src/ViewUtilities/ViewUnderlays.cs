// (C) Copyright 2015-2017 by Andrew Nicholas
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
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    public static class ViewUnderlays
    {
        public static void RemoveUnderlays(
            ICollection<ExportManager.ExportSheet> sheets, Document doc)
        {
            if (doc == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Failure", "Could not remove underlays(doc)");
                return;
            }
            if (sheets == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Failure", "Could not remove underlays(sheets)");
                return;
            }
            using (Transaction t = new Transaction(doc))
            {
                if (t.Start("Remove Underlays") == TransactionStatus.Started)
                {
                    foreach (ExportManager.ExportSheet sheet in sheets)
                    {
                        foreach (ElementId id in sheet.Sheet.GetAllPlacedViews())
                        {
                            var v = (View)doc.GetElement(id);
                            RemoveUnderlay(v);
                        }
                    }
                    if (t.Commit() != TransactionStatus.Committed)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox("Failure", "Could not remove underlays");
                    }
                }
            }
        }

        public static void RemoveUnderlays(UIDocument uidoc)
        {
            if (uidoc == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Remove Underlays", "Could not remove underlays");
                return;
            }
            var selection = uidoc.Selection;
            if (selection.GetElementIds().Count < 1)
            {
                return;
            }
            using (Transaction t = new Transaction(uidoc.Document))
            {
                if (t.Start("Remove Underlays") == TransactionStatus.Started)
                {
                    foreach (ElementId id in selection.GetElementIds())
                    {
                        RemoveUnderlay(uidoc.Document.GetElement(id));
                    }
                    if (t.Commit() != TransactionStatus.Committed)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox("Failure", "Could not remove underlays");
                    }
                }
            }
        }

        private static void RemoveUnderlay(Element element)
        {
#if REVIT2024 || REVIT2025
            if (element.Category.Id.Value != (long)BuiltInCategory.OST_Views)
            {
                return;
            }
#else
            if (element.Category.Id.IntegerValue != (int)BuiltInCategory.OST_Views)
            {
                return;
            }
#endif
            var param = element.get_Parameter(BuiltInParameter.VIEW_UNDERLAY_BOTTOM_ID);
            param?.Set(ElementId.InvalidElementId);
        }
    }
}