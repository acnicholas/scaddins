//
// (C) Copyright 2013-2014 by Andrew Nicholas
//
// This file is part of SCaos.
//
// SCaos is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaos is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaos.  If not, see <http://www.gnu.org/licenses/>.
//

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace SCaddins.SCaos
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        ProjectLocation projectLocation;
        ProjectPosition position;
        bool currentViewIsIso;
        
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
            ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            UIDocument udoc = commandData.Application.ActiveUIDocument;
            Document doc = udoc.Document;
            projectLocation = doc.ActiveProjectLocation;
            position = projectLocation.get_ProjectPosition(XYZ.Zero);
            currentViewIsIso = false;
            
            View view = doc.ActiveView;
            string[] s = getViewInfo(view, doc);
            var form = new SCaosForm(s, currentViewIsIso);
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
        
        private string[] getViewInfo(View view, Document doc)
        {
            if (view.ViewType != ViewType.ThreeD) {
                string[] info = new string[1];
                info[0] = "Not a 3d view...";
                currentViewIsIso = false;
                return info;
            } else {
                currentViewIsIso = true;
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                double frame = sunSettings.ActiveFrame;
                double azimuth = sunSettings.GetFrameAzimuth(frame);
                double altitude = sunSettings.GetFrameAltitude(frame);
                azimuth += position.Angle;
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
                azimuth += position.Angle;
                
                View3D v3d = (View3D)view;
                Transaction t = new Transaction(doc);
                t.Start("Draw Solar Ray");
                //REVIT 2014
                //Line ray = Line.CreateBound(XYZ.Zero, new XYZ(100 * Math.Cos(azimuth), 100 * Math.Sin(azimuth), 100 * Math.Tan(altitude)));
                //REVIT 2013
//                Line line = doc.Application.Create.NewLineBound(XYZ.Zero, new XYZ(Math.Sin(azimuth), Math.Cos(azimuth), 1 * Math.Tan(altitude)));
//                //check intesects
//                //SetComparisonResult scr = line.Intersect(
//                
//                var plane = new Plane(new XYZ(Math.Cos(azimuth), -Math.Sin(azimuth),0), XYZ.Zero);
//                SketchPlane sketchPlane = doc.Create.NewSketchPlane(plane);
//                if (sketchPlane != null){
//                    ModelCurve ray = doc.Create.NewModelCurve(line, sketchPlane);
//                }
                //udoc.RefreshActiveView();
                t.Commit();
            } else {
                TaskDialog.Show("ERROR", "Not a 3d view");
            }
        }
            
        private string getAllViewInfo(Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfClass(typeof(Autodesk.Revit.DB.View));
            string result = "";
            foreach (Autodesk.Revit.DB.View view in f) {
                string name = view.Name.ToUpper();
                if (view.ViewType == ViewType.ThreeD || view.ViewType == ViewType.FloorPlan) {
                    if (name.Contains("SOLAR") || name.Contains("SHADOW")) {
                        result += String.Join(System.Environment.NewLine, getViewInfo(view, doc));
                        result += System.Environment.NewLine + System.Environment.NewLine;
                    }
                }
            }
            LogText(result);
            return result;
        }
        
        private void LogText(string text)
        {
            System.IO.File.AppendAllText(@"c:\Temp\SCaos.txt", text);
        }
        
        private void CreateWinterViews(Document doc, UIDocument udoc)
        {
            ElementId id = null;
			
            //get the viewid
            var collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ViewFamilyType));
            foreach (Element e in collector) {
                var vft = (ViewFamilyType)e;
                if (vft.ViewFamily == ViewFamily.ThreeDimensional) {
                    id = vft.Id;
                    break;
                }
            }
			
            //FIXME add error message here
            if (id == null)
                return;
			
            for (int i = 9; i < 16; i++) {
                var t = new Transaction(doc);
                t.Start("Create Solar View");
                View view = View3D.CreateIsometric(doc, id);
                view.Name = "SOLAR ACCESS - " + i + " JUNE 21";
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                sunSettings.StartDateAndTime = new DateTime(2014, 06, 21, i, 0, 0, DateTimeKind.Local);
                sunSettings.SunAndShadowType = SunAndShadowType.StillImage;
                t.Commit();
                RotateView(view, doc, udoc);
            }
        }

        private void RotateView(View view, Document doc, UIDocument udoc)
        {
            if (view.ViewType == ViewType.ThreeD) {
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                double frame = sunSettings.ActiveFrame;
                double azimuth = sunSettings.GetFrameAzimuth(frame);
                double altitude = sunSettings.GetFrameAltitude(frame);
                azimuth += position.Angle;
                BoundingBoxXYZ viewBounds = view.get_BoundingBox(view);
                XYZ max = viewBounds.Max;
                XYZ min = viewBounds.Min;
                var eye = new XYZ(min.X + (max.X - min.X) / 2, min.Y + (max.Y - min.Y) / 2, min.Z + (max.Z - min.Z) / 2);
                var forward = new XYZ(-(Math.Sin(azimuth)), -(Math.Cos(azimuth)), -(Math.Tan(altitude)));
                var up = forward.CrossProduct(new XYZ((Math.Cos(azimuth)), -(Math.Sin(azimuth)), 0));
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
