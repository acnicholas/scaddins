// (C) Copyright 2019-2020 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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
    using SCaddins;
    using System;
    using System.Diagnostics;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SyncViewsHandler : IExternalCommand
    {

        private static Stopwatch stopwatch;
        private static TimeSpan idleTimeout;
        public static readonly EventHandler<IdlingEventArgs> Handler = OnIdling;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (SCaddinsApp.handlerAttached == false)
            {
                SCaddinsApp.handlerAttached = true;
                AttachIdleEventHandler(commandData);
                SCaddinsApp.WindowManager.ShowMessageBox("Sync Views Started");
            }
            else
            {
                SCaddinsApp.handlerAttached = false;
                RemoveIdleEventHandler(commandData.Application);
                SCaddinsApp.WindowManager.ShowMessageBox("Sync Views Ended");
            }
            idleTimeout = new TimeSpan(1000000); // only full run every .1 seconds
            return Result.Succeeded;
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
                if (activeView.ViewType != ViewType.FloorPlan
                    && activeView.ViewType != ViewType.AreaPlan
                    && activeView.ViewType != ViewType.CeilingPlan)
                {
                    return;
                }

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
                    uiv2.ZoomAndCenterRectangle(zoomCorner[0], zoomCorner[1]);
                }
            }
        }
    }
}
