// (C) Copyright 2020 by Andrew Nicholas
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
    using SCaddins.ExportManager;

    public static class Pin
    {
        public static void PinSheetContents(ICollection<ExportSheet> sheets, Document doc)
        {
            using (var t = new Transaction(doc, "Pin Sheet Contents"))
            {
                if (t.Start() == TransactionStatus.Started)
                {
                    foreach (var sheet in sheets)
                    {
                        PinTitle(sheet, doc);
                        PinViewsOnSheet(sheet.Sheet, doc);
                        ////PinSheetAnnotations(sheet.Sheet, doc);
                    }
                    t.Commit();
                }
                else
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("Error", "Could not pin sheet contents");
                    return;
                }
            }
        }

        private static void PinViewsOnSheet(ViewSheet sheet, Document doc)
        {
            var views = sheet.GetAllPlacedViews();
            foreach (var viewId in views)
            {
                var view = doc.GetElement(viewId);
                view.Pinned = true;
            }
        }

        ////private static void PinSheetAnnotations(ViewSheet sheet, Document doc)
        ////{
        ////   //todo
        ////}

        private static void PinTitle(ExportSheet sheet, Document doc)
        {
            Manager.TitleBlockInstanceFromSheetNumber(sheet.SheetNumber, doc).Pinned = true;
        }
    }
}
