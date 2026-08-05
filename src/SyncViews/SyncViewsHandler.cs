// (C) Copyright 2026 by Andrew Nicholas (andrewcnicholas@gmail.com)
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

namespace SCaddins.SyncViews
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Events;
    using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
    using SCaddins;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SyncViewsHandler : IExternalCommand
    {

        private static Stopwatch stopwatch;
        private static TimeSpan idleTimeout;
        public static readonly EventHandler<IdlingEventArgs> Handler = OnIdling;
        //private static Dictionary<ElementId, XYZ> viewPortLocationDict;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //if (viewPortLocationDict == null)
            //{
            //    viewPortLocationDict = new Dictionary<ElementId, XYZ>();
            //}

            //load Dictionary
            //viewPortLocationDict.Clear();
            //var fec = new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document);
            //fec.OfCategory(BuiltInCategory.OST_Viewports);
            //foreach (var element in fec)
            //{
            //    var vp = element as Viewport;
            //    viewPortLocationDict.Add(vp.ViewId, vp.GetBoxCenter());
            //}

            if (SCaddinsApp.handlerAttached == false)
            {
                SCaddinsApp.handlerAttached = true;
                AttachIdleEventHandler(commandData);
                ToggleRIbbonText(commandData.Application, true);
                SCaddinsApp.WindowManager.ShowMessageBox("Sync Views Started");
                
            }
            else
            {
                SCaddinsApp.handlerAttached = false;
                RemoveIdleEventHandler(commandData.Application);
                ToggleRIbbonText(commandData.Application, false);
                SCaddinsApp.WindowManager.ShowMessageBox("Sync Views Ended");
                
            }
            idleTimeout = new TimeSpan(1000000); // only full run every .1 seconds
            return Result.Succeeded;
        }

        private static void ToggleRIbbonText(UIApplication app, bool running)
        {
            List<RibbonPanel> rPanels = new List<RibbonPanel>();
            rPanels = app.GetRibbonPanels("Studio.SC");
            foreach (RibbonPanel rp in rPanels)
            {
                if (rp.Name == "View")
                { 
                    foreach (RibbonItem ris in rp.GetItems())
                    {
                        if (ris.ItemText.Contains("Sync Views"))
                        {
                            if (running) ris.ItemText = "Sync Views" + System.Environment.NewLine +  "[Running]";
                            if (!running) ris.ItemText = "Sync Views" + System.Environment.NewLine + "[Stopped]";
                            //ris.ToolTip = "This is a test This is a test";
                        }
                    }
                }
            }
        }

        public static void AttachIdleEventHandler(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;
            if (stopwatch == null)
            {
                stopwatch = new Stopwatch();
            } 
            stopwatch.Restart();
            uiApp.Idling -= SyncViewsHandler.Handler; //dettach first just in case
            uiApp.Idling += SyncViewsHandler.Handler;
        }

        public static TimeSpan GetIdleElapsedTime()
        {
            return stopwatch != null ? stopwatch.Elapsed : TimeSpan.Zero;
        }

        public static void RemoveIdleEventHandler(UIApplication uiApp)
        {
            if (stopwatch != null)
            {
                stopwatch.Stop();
            }
            uiApp.Idling -= SyncViewsHandler.Handler;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        public static string GetMainWindowTitle(UIApplication uiapp)
        {
            IntPtr hwnd = uiapp.MainWindowHandle; // Revit main window handle
            var buffer = new System.Text.StringBuilder(512);
            GetWindowText(hwnd, buffer, buffer.Capacity);
            return buffer.ToString();
        }

        /// <summary>
        /// Run this if the parent view is a plan
        /// </summary>
        private static void SyncPlans(UIApplication uiApp, View activeView)
        {
            var uiViews = uiApp.ActiveUIDocument.GetOpenUIViews();

            // check if view has been accessed through a viewport.
            // if it has, then allow for view offset.
            // TODO
            // string mainWindowTitle = GetMainWindowTitle(uiApp);
            string activeViewTitle = activeView.Name;

            //defualt is no offset (we're not accessing through a viewport
            // XYZ offset = new XYZ(0, 0, 0);


            //if (mainWindowTitle.Contains(@"Sheet:"))
            //{
            // Debug.WriteLine(mainWindowTitle + " av:" + activeViewTitle);
            // SCaddinsApp.WindowManager.ShowMessageBox(mainWindowTitle + " av:" + activeViewTitle);
            //    viewPortLocationDict.TryGetValue(activeView.Id, out offset);
            //}

            Autodesk.Revit.UI.UIView uiView = null;
            foreach (UIView uiv in uiViews)
            {
                if (uiv.ViewId == activeView.Id)
                {
                    uiView = uiv;
                    break;
                }
            }

            if (uiView == null) return;

            var zoomCorner = uiView.GetZoomCorners();

            //if(offset.X != 0 && offset.Y != 0){
            //    var centZoom = (zoomCorner[1] + zoomCorner[1]) / 2;
            //    var movement = offset - centZoom;
            //    zoomCorner[0] = movement + zoomCorner[0];
            //    zoomCorner[1] = movement + zoomCorner[1];
            //}

            foreach (UIView uiv2 in uiViews)
            {
                if (uiv2.ViewId == uiView.ViewId) continue; // dont redraw the active view

                // dont redraw if the view is not a plan
                var v = activeView.Document.GetElement(uiv2.ViewId) as View;
                if (v.ViewType == ViewType.AreaPlan ||
                    v.ViewType == ViewType.FloorPlan ||
                    v.ViewType == ViewType.CeilingPlan)
                    {
                        uiv2.ZoomAndCenterRectangle(zoomCorner[0], zoomCorner[1]);
                    }
            }
        }

        /// <summary>
        /// Run this if the parenet view is a sheet.
        /// </summary>
        private static void SyncSheets(UIApplication uiApp, View activeView)
        {
            var uiViews = uiApp.ActiveUIDocument.GetOpenUIViews();

            Autodesk.Revit.UI.UIView uiView = null;
            foreach (UIView uiv in uiViews)
            {
                if (uiv.ViewId == activeView.Id)
                {
                    uiView = uiv;
                    break;
                }
            }

            if (uiView == null) return;

            var zoomCorner = uiView.GetZoomCorners();

            foreach (UIView uiv2 in uiViews)
            {
                if (uiv2.ViewId == uiView.ViewId) continue; // dont redraw the active view

                // dont redraw if the view is not a plan
                var v = activeView.Document.GetElement(uiv2.ViewId) as View;
                if (v.ViewType == ViewType.DrawingSheet)
                {
                    uiv2.ZoomAndCenterRectangle(zoomCorner[0], zoomCorner[1]);
                }
            }
        }

        public static void OnIdling(object sender, IdlingEventArgs e)
        {
            UIApplication uiApp = sender as UIApplication;
            Document doc = uiApp.ActiveUIDocument.Document;
            {
                //MAKE sure this is super fast as it will run ALL THE TIME!!!
                //lets only run every n times a seconds.
                if (GetIdleElapsedTime() < idleTimeout) return;
                stopwatch.Restart();

                var activeView = uiApp.ActiveUIDocument.ActiveView;

                if (activeView == null) return;

                switch(activeView.ViewType)
                {
                    case ViewType.AreaPlan:
                    case ViewType.FloorPlan:
                    case ViewType.CeilingPlan:
                        SyncPlans(uiApp, activeView);
                        break;
                    case ViewType.DrawingSheet:
                        SyncSheets(uiApp, activeView);
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
