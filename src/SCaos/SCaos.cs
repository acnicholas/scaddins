// (C) Copyright 2013-2014 by Andrew Nicholas
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

namespace SCaddins.SCaos
{
    using System;
    using System.Globalization;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private ProjectLocation projectLocation;
        private ProjectPosition position;
        private bool currentViewIsIso;

        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            UIDocument udoc = commandData.Application.ActiveUIDocument;
            Document doc = udoc.Document;
            this.projectLocation = doc.ActiveProjectLocation;
            this.position = this.projectLocation.get_ProjectPosition(XYZ.Zero);
            this.currentViewIsIso = false;

            View view = doc.ActiveView;
            string[] s = this.GetViewInfo(view);
            var form = new SCaosForm(s, this.currentViewIsIso);
            System.Windows.Forms.DialogResult result = form.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                if (form.radioButtonRotateCurrent.Checked) {
                    this.RotateView(doc.ActiveView, doc, udoc);
                }
                if (form.radioButtonWinterViews.Checked) {
                    this.CreateWinterViews(
                        doc,
                        udoc,
                        (DateTime)form.startTime.SelectedItem,
                        (DateTime)form.endTime.SelectedItem,
                        (TimeSpan)form.interval.SelectedItem);
                }
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
        
        private static bool ViewNameIsAvailable(Document doc, string name)
        {
            var c = new FilteredElementCollector(doc);
            c.OfClass(typeof(Autodesk.Revit.DB.View));
            foreach (View view in c) {
                var v = view as View;
                if (v.ViewName == name) {
                    return false;
                }
            }
            return true;
        }

        private string[] GetViewInfo(View view)
        {
            if (view.ViewType != ViewType.ThreeD) {
                var info = new string[4];
                info[0] = "Not a 3d view...";
                info[1] = string.Empty;
                info[2] = "Please select a 3d view to rotate";
                info[3] = "or use the create winter views feature";
                this.currentViewIsIso = false;
                return info;
            } else {
                this.currentViewIsIso = true;
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                double frame = sunSettings.ActiveFrame;
                double azimuth = sunSettings.GetFrameAzimuth(frame);
                double altitude = sunSettings.GetFrameAltitude(frame);
                azimuth += this.position.Angle;
                double azdeg = azimuth * 180 / System.Math.PI;
                double altdeg = altitude * 180 / System.Math.PI;
                var info = new string[7];
                info[0] = view.Name;
                info[1] = "Date - " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongDateString();
                info[2] = "Time - " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongTimeString();
                info[3] = "Sunrise - " + sunSettings.GetSunrise(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString();
                info[4] = "Sunset - " + sunSettings.GetSunset(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString();
                info[5] = "Sun Altitude - " + altdeg.ToString(CultureInfo.InvariantCulture);
                info[6] = "Sun Azimuth - " + azdeg.ToString(CultureInfo.InvariantCulture);
                return info;
            }
        }

        private void CreateWinterViews(
            Document doc,
            UIDocument udoc,
            DateTime startTime,
            DateTime endTime,
            TimeSpan interval)
        {
            ElementId id = null;

            // get the viewid
            var collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ViewFamilyType));
            foreach (Element e in collector) {
                var vft = (ViewFamilyType)e;
                if (vft.ViewFamily == ViewFamily.ThreeDimensional) {
                    id = vft.Id;
                    break;
                }
            }

            // FIXME add error message here
            if (id == null) {
                return;
            }

            while (startTime <= endTime) {
                var t = new Transaction(doc);
                t.Start("Create Solar View");
                View view = View3D.CreateIsometric(doc, id);
                var niceMinutes = "00";
                if (startTime.Minute > 0) {
                    niceMinutes = startTime.Minute.ToString(CultureInfo.CurrentCulture);
                }    
                var vname = "SOLAR ACCESS - " + startTime.ToShortDateString() + "-" + startTime.Hour + "." + niceMinutes;  
                if (ViewNameIsAvailable(doc, vname)) {
                    view.Name = vname;
                } else {
                    view.Name = vname + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) + @")";    
                }
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                sunSettings.StartDateAndTime = startTime;
                sunSettings.SunAndShadowType = SunAndShadowType.StillImage;
                t.Commit();
                this.RotateView(view, doc, udoc);
                startTime = startTime.Add(interval);
            }
        }

        private void RotateView(View view, Document doc, UIDocument udoc)
        {
            if (view.ViewType == ViewType.ThreeD) {
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                double frame = sunSettings.ActiveFrame;
                double azimuth = sunSettings.GetFrameAzimuth(frame);
                double altitude = sunSettings.GetFrameAltitude(frame);
                azimuth += this.position.Angle;
                BoundingBoxXYZ viewBounds = view.get_BoundingBox(view);
                XYZ max = viewBounds.Max;
                XYZ min = viewBounds.Min;
                var eye = new XYZ(min.X + ((max.X - min.X) / 2), min.Y + ((max.Y - min.Y) / 2), min.Z + ((max.Z - min.Z) / 2));
                var forward = new XYZ(-Math.Sin(azimuth), -Math.Cos(azimuth), -Math.Tan(altitude));
                var up = forward.CrossProduct(new XYZ(Math.Cos(azimuth), -Math.Sin(azimuth), 0));  
                var v3d = (View3D)view;
                var t = new Transaction(doc);
                if (v3d.IsLocked ) {
                    TaskDialog.Show("ERROR", "View is locked, please unlock before rotating"); 
                    return;
                }
                t.Start("Rotate View");
                v3d.SetOrientation(new ViewOrientation3D(eye, up, forward));
                if (v3d.CanBeLocked()) {
                    try {
                        v3d.SaveOrientationAndLock();
                    } catch (InvalidOperationException e ) {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                } 
                udoc.RefreshActiveView();
                t.Commit();
            } else {
                TaskDialog.Show("ERROR", "Not a 3d view");
            }
        }
    }
}
