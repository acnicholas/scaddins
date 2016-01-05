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
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    public static class SCwm
    {
        public static void MaximizeWindow(UIApplication app)
        {
            const string Cmd = @"C:\Andrew\code\cs\scaddins\etc\SCwmMaximizeWindow.exe";
            SCaddins.Common.ConsoleUtilities.StartHiddenConsoleProg(Cmd, string.Empty);       
        }
        
        public static void TileWindows(UIApplication app, int mainWidthPercentage)
        {
            const string Cmd = @"C:\Andrew\code\cs\scaddins\etc\SCwm.exe";
            
            var activeView = app.ActiveUIDocument.ActiveView;
            var activeFileName = System.IO.Path.GetFileName(app.ActiveUIDocument.Document.PathName);
            var mainWidth = GetDrawingAreaWidth(app) * mainWidthPercentage / 100;
            if(GetNumberOfPhysicalScreens() > 1) {
                System.Windows.Forms.Screen mainScreen = GetScreen(0);
                mainWidth = mainScreen.Bounds.Width - GetDrawingAreaX(app);
            }
            var mainHeight = GetDrawingAreaHeight(app) - 4;
            var minorWidth = GetDrawingAreaWidth(app) - mainWidth;
            
            // set main window location
            var args = "\"" + activeView.Name + "\"" + " 0 0 " + mainWidth + " " + mainHeight;
            SCaddins.Common.ConsoleUtilities.StartHiddenConsoleProg(Cmd, args);
            
            // set secondary window locations
            var numberOfViews = GetNumberOfOpenViews(app);
            var th = GetDrawingAreaHeight(app) / (numberOfViews - 1);
            
            if (numberOfViews == 1) {
                return;
            }
            
            int i = 0;
            foreach (Document doc in app.Application.Documents) {
                UIDocument udoc = new UIDocument(doc);   
                foreach (UIView view in udoc.GetOpenUIViews()) {
                    View v = (View)doc.GetElement(view.ViewId);
                    var viewName = v.Name + " - " + System.IO.Path.GetFileName(doc.PathName);
                    if (viewName != activeView.Name + " - " + activeFileName) {
                        var args2 = "\"" + v.Name + "\" " + mainWidth + " " + (th * i) + " " + minorWidth + " " + th;
                        SCaddins.Common.ConsoleUtilities.StartHiddenConsoleProg(Cmd, args2);
                        i++;
                    }
                }
            }
        }
        
        public static int GetNumberOfPhysicalScreens()
        {
            return System.Windows.Forms.Screen.AllScreens.Length;
        }
        
        public static System.Windows.Forms.Screen GetScreen(int screenNumber)
        {
            return System.Windows.Forms.Screen.AllScreens[screenNumber];
        }
           
        public static int GetDrawingAreaWidth(UIApplication app) {
            var rect = app.DrawingAreaExtents;
            return rect.Right - rect.Left;
        }
        
        public static int GetDrawingAreaHeight(UIApplication app) {
            var rect = app.DrawingAreaExtents;
            return rect.Bottom - rect.Top;
        }
        
        public static int GetDrawingAreaX(UIApplication app) {
            var rect = app.DrawingAreaExtents;
            return rect.Left;
        }
        
        public static int GetNumberOfOpenViews(UIApplication app)
        {
            int result = 0;
            var docs = app.Application.Documents;  
            foreach (Document doc in docs) {
                UIDocument udoc = new UIDocument(doc);
                result += udoc.GetOpenUIViews().Count; 
            }
            return result;
        }         
    }
}