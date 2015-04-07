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

namespace SCaddins.SCwm
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    public static class SCwm
    {
        public static void ListActiveWindowNames(UIDocument document)
        {
            var views = document.GetOpenUIViews();
            var list = String.Empty;
            foreach (UIView view in views){
                View v = (View)document.Document.GetElement(view.ViewId);
                list += v.Name;
                list += System.Environment.NewLine;
            }
            TaskDialog.Show("Open Views", list);
        }
        
        public static void ListDrawingAreaDimensions(UIApplication app)
        {
            var rect = app.DrawingAreaExtents;
            var width = rect.Right - rect.Left;
            var height = rect.Bottom - rect.Top;
            TaskDialog.Show("Drawing Area", rect.Left + "," + rect.Top + "," + width + "," + height);
        }
            
    }
}