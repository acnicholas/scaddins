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
        public static void TileWindows(UIApplication app, int mainWidthPercentage)
        {
            const string cmd = @"C:\Andrew\code\cs\scaddins\etc\SCwm.exe";
            var activeView = app.ActiveUIDocument.ActiveView;
            var mainWidth = GetDrawingAreaWidth(app) * mainWidthPercentage / 100;
            var mainHeight = GetDrawingAreaHeight(app) - 4;
            var minorWidth = GetDrawingAreaWidth(app) - mainWidth;
            
            //set main window locations
            var args = "\"" + activeView.Name + "\"" + " 0 0 " + mainWidth + " " + mainHeight;
            SCexport.SCexport.StartHiddenConsoleProg(cmd, args);
            
            //set secondary window locations
            var docs = app.Application.Documents;
            var views = app.ActiveUIDocument.GetOpenUIViews();
            var numberOfViews = views.Count;
            var th = GetDrawingAreaHeight(app) / (numberOfViews - 1);
            if (numberOfViews == 1){
                return;
            }
            int i = 0;
            foreach (UIView view in views){
                View v = (View)app.ActiveUIDocument.Document.GetElement(view.ViewId);
                if (v.Name != activeView.Name) {
                    var args2 = "\"" + v.Name + "\" " + mainWidth + " " + th*i + " "  + minorWidth + " " + th;
                    SCexport.SCexport.StartHiddenConsoleProg(cmd, args2);
                    i++;
                }
            }
        }
           
        public static int GetDrawingAreaWidth(UIApplication app) {
            var rect = app.DrawingAreaExtents;
            return rect.Right - rect.Left;
        }
        
        public static int GetDrawingAreaHeight(UIApplication app) {
            var rect = app.DrawingAreaExtents;
            return rect.Bottom - rect.Top;
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