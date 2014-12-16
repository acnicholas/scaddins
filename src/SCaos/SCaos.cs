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
            string[] s = this.GetViewInfo(view, doc);
            var form = new SCaosForm(s, this.currentViewIsIso);
            System.Windows.Forms.DialogResult result = form.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                if (form.radioButtonRotateCurrent.Checked) {
                    this.RotateView(doc.ActiveView, doc, udoc);
                }
                if (form.radioButtonWinterViews.Checked) {
                    this.CreateWinterViews(doc, udoc);
                }
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
        
        private string[] GetViewInfo(View view, Document doc)
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
                string[] info = new string[7];
                info[0] = view.Name;
                info[1] = "Date - " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongDateString();
                info[2] = "Time - " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongTimeString();
                info[3] = "Sunrise - " + sunSettings.GetSunrise(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString();
                info[4] = "Sunset - " + sunSettings.GetSunset(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString();
                info[5] = "Sun Altitude - " + altdeg.ToString();
                info[6] = "Sun Azimuth - " + azdeg.ToString();
                return info;
            }
        }
        
        /// <summary>
        /// Attempt to draw a line along the path of the sun.
        /// </summary>
        /// <param name="udoc"></param>
        /// <param name="doc"></param>
        private void DrawSolarRay(UIDocument udoc, Document doc)
        {
            View view = doc.ActiveView;
            if (view.ViewType == ViewType.ThreeD) {
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                double frame = sunSettings.ActiveFrame;
                double azimuth = sunSettings.GetFrameAzimuth(frame);
                double altitude = sunSettings.GetFrameAltitude(frame);
                azimuth += this.position.Angle;
                
                var v3d = (View3D)view;
                var t = new Transaction(doc);
                t.Start("Draw Solar Ray");
                
                // REVIT 2014
                // Line ray = Line.CreateBound(XYZ.Zero, new XYZ(100 * Math.Cos(azimuth), 100 * Math.Sin(azimuth), 100 * Math.Tan(altitude)));
                // REVIT 2013
//                Line line = doc.Application.Create.NewLineBound(XYZ.Zero, new XYZ(Math.Sin(azimuth), Math.Cos(azimuth), 1 * Math.Tan(altitude)));
//                //check intesects
//                //SetComparisonResult scr = line.Intersect(
//                
//                var plane = new Plane(new XYZ(Math.Cos(azimuth), -Math.Sin(azimuth),0), XYZ.Zero);
//                SketchPlane sketchPlane = doc.Create.NewSketchPlane(plane);
//                if (sketchPlane != null){
//                    ModelCurve ray = doc.Create.NewModelCurve(line, sketchPlane);
//                }
                // udoc.RefreshActiveView();
                t.Commit();
            } else {
                TaskDialog.Show("ERROR", "Not a 3d view");
            }
        }
            
        private string GetAllViewInfo(Document doc)
        {
            var f = new FilteredElementCollector(doc);
            f.OfClass(typeof(Autodesk.Revit.DB.View));
            string result = string.Empty;
            foreach (Autodesk.Revit.DB.View view in f) {
                string name = view.Name.ToUpper();
                if (view.ViewType == ViewType.ThreeD || view.ViewType == ViewType.FloorPlan) {
                    if (name.Contains("SOLAR") || name.Contains("SHADOW")) {
                        result += string.Join(System.Environment.NewLine, this.GetViewInfo(view, doc));
                        result += System.Environment.NewLine + System.Environment.NewLine;
                    }
                }
            }
            this.LogText(result);
            return result;
        }
        
        private void LogText(string text)
        {
            System.IO.File.AppendAllText(@"c:\Temp\SCaos.txt", text);
        }
        
        private bool ViewNameIsAvailable(Document doc, string name)
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
        
        private void CreateWinterViews(Document doc, UIDocument udoc)
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

            for (int i = 9; i < 16; i++) {
                var t = new Transaction(doc);
                t.Start("Create Solar View");
                View view = View3D.CreateIsometric(doc, id);
                var vname = "SOLAR ACCESS - " + i + " JUNE 21";
                if (this.ViewNameIsAvailable(doc, vname)) {
                    view.Name = vname;
                } else {
                    view.Name = vname + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString() + @")";    
                }
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                sunSettings.StartDateAndTime = new DateTime(2014, 06, 21, i, 0, 0, DateTimeKind.Local);
                sunSettings.SunAndShadowType = SunAndShadowType.StillImage;
                t.Commit();
                this.RotateView(view, doc, udoc);
            }
        }

        private string XYZToString(XYZ xyz)
        {
            return xyz.X.ToString() + @"," + xyz.Y.ToString() + @"," + xyz.Z.ToString();
        }
        
        private void RotateView(View view, Document doc, UIDocument udoc)
        {
            if (view.ViewType == ViewType.ThreeD) {
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                double frame = sunSettings.ActiveFrame;
                double azimuth = sunSettings.GetFrameAzimuth(frame);
                double altitude = sunSettings.GetFrameAltitude(frame);
                //double azimuth = sunSettings.GetFrameAzimuth(frame) + 0.001;
                //double altitude = sunSettings.GetFrameAltitude(frame) + 0.001;
                azimuth += this.position.Angle;
                BoundingBoxXYZ viewBounds = view.get_BoundingBox(view);
                XYZ max = viewBounds.Max;
                XYZ min = viewBounds.Min;
                var eye = new XYZ(min.X + ((max.X - min.X) / 2), min.Y + ((max.Y - min.Y) / 2), min.Z + ((max.Z - min.Z) / 2));
                var forward = new XYZ(-Math.Sin(azimuth), -Math.Cos(azimuth), -Math.Tan(altitude));
                var up = forward.CrossProduct(new XYZ(Math.Cos(azimuth), -Math.Sin(azimuth), 0));
                TaskDialog.Show("forward/eye/up",XYZToString(forward) + System.Environment.NewLine +
                                XYZToString(eye) + System.Environment.NewLine +
                                XYZToString(up));
                var v3d = (View3D)view;
                var t = new Transaction(doc);
                t.Start("Rotate View");
                v3d.SetOrientation(new ViewOrientation3D(eye, up, forward));
                udoc.RefreshActiveView();
                t.Commit();
            } else {
                TaskDialog.Show("ERROR", "Not a 3d view");
            }
        }
    }
}
