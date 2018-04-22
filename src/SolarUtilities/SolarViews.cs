// (C) Copyright 2013-2017 by Andrew Nicholas
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

namespace SCaddins.SolarUtilities
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SolarViews
    {
        private ProjectLocation projectLocation;
        private ProjectPosition position;
        private View activeView;
        private Document doc;
        private UIDocument udoc;
        
        public bool RotateCurrentView
        {
            get; set;
        }
        
        public bool Create3dViews
        {
            get; set;
        }
        
        public bool CreateShadowPlans
        {
            get; set;
        }

        public bool CanRotateActiveView
        {
            get
            {
                return ViewIsIso(activeView);
            }
        }
        
        public string ActiveIewInformation
        {
            get {return GetViewInfo(activeView);}
        }
        
        public DateTime StartTime
        {
            get; set;
        }
        
        public DateTime EndTime
        {
            get; set;
        }
                
       public TimeSpan ExportTimeInterval{
            get; set;
        }
        
        public SolarViews(UIDocument udoc)
        {
            this.doc = udoc.Document;
            this.udoc = udoc;
            projectLocation = doc.ActiveProjectLocation;
            #if REVIT2018 || REVIT2019
                position = projectLocation.GetProjectPosition(XYZ.Zero);
            #else
                position = this.projectLocation.get_ProjectPosition(XYZ.Zero);
            #endif
            
            StartTime = new DateTime(2018,6,21,9,0,0);
            EndTime = new DateTime(2018,6,21,15,0,0);
            ExportTimeInterval = new TimeSpan(1,0,0);
            activeView = doc.ActiveView;
        }
        
        // FIXME put this somewhere else.
        public static bool ViewNameIsAvailable(Document doc, string name)
        {
            using (var c = new FilteredElementCollector(doc)) {
                c.OfClass(typeof(Autodesk.Revit.DB.View));
                foreach (View view in c) {
                    var v = view as View;
                    if (v.ViewName == name) {
                        return false;
                    }
                }
            }
            return true;
        }

        // FIXME this can go in a utiliy class.
        public static string GetNiceViewName(Document doc, string request) {
            if (ViewNameIsAvailable(doc, request)) {
                return request;
            } else {
                return request + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) + @")";
            }
        }

        private static ElementId GetViewFamilyId(Document doc, ViewFamily viewFamilyType)
        {
            using (var collector = new FilteredElementCollector(doc)) {
                collector.OfClass(typeof(ViewFamilyType));
                foreach (Element e in collector) {
                    var vft = (ViewFamilyType)e;
                    if (vft.ViewFamily == viewFamilyType) {
                        return vft.Id;
                    }
                }
            }
            return null;
        }

        private static ElementId GetHighestLevel(Document doc)
        {
            double highestLevel = -1;
            ElementId highestId = null;
            using (var collector = new FilteredElementCollector(doc)) {
                collector.OfClass(typeof(Level));
                foreach (Element e in collector) {
                    var level = (Level)e;
                    if (highestLevel < 0 || level.Elevation > highestLevel) {
                        highestLevel = level.Elevation;
                        highestId = level.Id;
                    }
                }
            }
            return highestId;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CreateShadowPlanViews()
        {
             ElementId id = GetViewFamilyId(doc, ViewFamily.FloorPlan);
             ElementId levelId = GetHighestLevel(doc);
             if (id == null || levelId == null) {
                 return;
             }

              while (StartTime <= EndTime) {
                using (var t = new Transaction(doc)) {
                    if (t.Start("Create Shadow Plans") == TransactionStatus.Started) {
                        View view = ViewPlan.Create(doc, id, levelId);
                        view.ViewTemplateId = ElementId.InvalidElementId;
                        var niceMinutes = "00";
                        if (StartTime.Minute > 0) {
                            niceMinutes = StartTime.Minute.ToString(CultureInfo.CurrentCulture);
                        }
                        var vname = "SHADOW PLAN - " + StartTime.ToShortDateString() + "-" + StartTime.Hour + "." + niceMinutes;
                        view.Name = GetNiceViewName(doc, vname);
                        SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                        sunSettings.StartDateAndTime = StartTime.ToLocalTime();
                        sunSettings.SunAndShadowType = SunAndShadowType.StillImage;
                        view.SunlightIntensity = 50;
                        t.Commit();
                        StartTime = StartTime.Add(ExportTimeInterval);
                    }
                }
            }
        }
        
        public static bool ViewIsIso(View view)
        {
            return view.ViewType == ViewType.ThreeD;
        }

        private string GetViewInfo(View view) {
            System.Text.StringBuilder info = new System.Text.StringBuilder();
            if (!ViewIsIso(view)) {
                info.AppendLine("Not a 3d view...");
                info.AppendLine(string.Empty);
                info.AppendLine("Please select a 3d view to rotate");
                info.AppendLine("or use the create winter views feature");
                return info.ToString();
            } else {
                SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                double frame = sunSettings.ActiveFrame;
                double azimuth = sunSettings.GetFrameAzimuth(frame);
                double altitude = sunSettings.GetFrameAltitude(frame);
                azimuth += position.Angle;
                double azdeg = azimuth * 180 / System.Math.PI;
                double altdeg = altitude * 180 / System.Math.PI;
                info.AppendLine("Date - " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongDateString());
                info.AppendLine("Time - " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongTimeString());
                info.AppendLine("Sunrise - " + sunSettings.GetSunrise(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString());
                info.AppendLine("Sunset - " + sunSettings.GetSunset(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString());
                info.AppendLine("Sun Altitude - " + altdeg.ToString(CultureInfo.InvariantCulture));
                info.AppendLine("Sun Azimuth - " + azdeg.ToString(CultureInfo.InvariantCulture));
                return info.ToString();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CreateWinterViews()
        {
            ElementId id = GetViewFamilyId(doc, ViewFamily.ThreeDimensional);

            // FIXME add error message here
            if (id == null) {
                return;
            }

            while (StartTime <= EndTime) {
                using (var t = new Transaction(doc)) {
                    t.Start("Create Solar View");
                    View view = View3D.CreateIsometric(doc, id);
                    view.ViewTemplateId = ElementId.InvalidElementId;
                    var niceMinutes = "00";
                    if (StartTime.Minute > 0) {
                        niceMinutes = StartTime.Minute.ToString(CultureInfo.CurrentCulture);
                    }
                    var vname = "SOLAR ACCESS - " + StartTime.ToShortDateString() + "-" + StartTime.Hour + "." + niceMinutes;
                    view.Name = GetNiceViewName(doc, vname);
                    SunAndShadowSettings sunSettings = view.SunAndShadowSettings;
                    sunSettings.StartDateAndTime = StartTime.ToLocalTime();
                    sunSettings.SunAndShadowType = SunAndShadowType.StillImage;
                    t.Commit();

                    // FIXME too many transactions here and above...
                    this.RotateView(view);
                    StartTime = StartTime.Add(ExportTimeInterval);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void RotateView(View view)
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
                var eye = new XYZ(min.X + ((max.X - min.X) / 2), min.Y + ((max.Y - min.Y) / 2), min.Z + ((max.Z - min.Z) / 2));
                var forward = new XYZ(
                    -Math.Sin(azimuth),
                    -Math.Cos(azimuth),
                    -Math.Tan(altitude - GetAtmosphericRefraction(altitude))
                );
                var up = forward.CrossProduct(new XYZ(Math.Cos(azimuth), -Math.Sin(azimuth), 0));  
                var v3d = (View3D)view;
                if (v3d.IsLocked) {
                    TaskDialog.Show("ERROR", "View is locked, please unlock before rotating");
                    return;
                }
                using (var t = new Transaction(doc)) {
                    t.Start("Rotate View");
                    v3d.SetOrientation(new ViewOrientation3D(eye, up, forward));
                    if (v3d.CanBeLocked() && !v3d.Name.StartsWith("{", StringComparison.OrdinalIgnoreCase)) {
                        try {
                            v3d.SaveOrientationAndLock();
                        } catch (InvalidOperationException e) {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                        }
                    }
                    udoc.RefreshActiveView();
                    t.Commit();
                }
            } else {
                TaskDialog.Show("ERROR", "Not a 3d view");
            }
        }
        
        public void Go()
        {
                if (this.RotateCurrentView) {
                    this.RotateView(activeView);
                }
                if (this.Create3dViews) {
                    this.CreateWinterViews();
                }
                if (this.CreateShadowPlans) {
                    CreateShadowPlanViews();
                }
        }

        /// <summary>
        /// Gets the Atmospheric Refraction using Bennett's formula
        /// </summary>
        /// <param name="altitudeRadians">Altitude in radians</param>
        /// <returns>The Atmospheric Refraction in radians</returns>
        public static double GetAtmosphericRefraction(double altitudeRadians) {
            double altitudeDegrees = altitudeRadians * 180 / Math.PI;
            double formula = altitudeDegrees + (7.31 / (altitudeDegrees + 4.4));
            double radians = Math.PI * altitudeDegrees / 180.0;
            return 1 / (Math.Tan(radians)) * 0.00029088820866572;
        }

        /// <summary>
        /// Gets the Atmospheric Refraction using Sæmundsson's formula
        /// </summary>
        /// <param name="altitudeRadians">Altitude in radians</param>
        /// <returns>The Atmospheric Refraction in radians</returns>
        public static double GetSæmundssonAtmosphericRefraction(double altitudeRadians) {
            double altitudeDegrees = altitudeRadians * 180 / Math.PI;
            double formula = 1.02 * altitudeDegrees + (10.3 / (altitudeDegrees + 5.11));
            double radians = Math.PI * altitudeDegrees / 180.0;
            return 1 / (Math.Tan(radians)) * 0.00029088820866572;
        }
    }
}
