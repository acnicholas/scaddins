// (C) Copyright 2013-2018 by Andrew Nicholas
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

namespace SCaddins.SolarAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Analysis;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SolarAnalysisManager
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "activeView")]
        private readonly View activeView;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "doc")]
        private readonly Document doc;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "position")]
        private readonly ProjectPosition position;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "udoc")]
        private readonly UIDocument udoc;

        public SolarAnalysisManager(UIDocument udoc)
        {
            doc = udoc.Document;
            this.udoc = udoc;
            position = GetProjectPosition(doc);
            StartTime = new DateTime(2018, 6, 21, 9, 0, 0);
            EndTime = new DateTime(2018, 6, 21, 15, 0, 0);
            ExportTimeInterval = new TimeSpan(1, 0, 0);
            activeView = doc.ActiveView;
        }

        public string ActiveIewInformation => GetViewInfo(activeView);

        public bool CanCreateAnalysisView => ViewIsSingleDay(activeView);

        public bool CanRotateActiveView => ViewIsIso(activeView);

        public bool Create3dViews { get; set; }

        public bool CreateAnalysisView { get; set; }

        public bool CreateShadowPlans { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan ExportTimeInterval { get; set; }

        public bool RotateCurrentView { get; set; }

        public DateTime StartTime { get; set; }

        public UIDocument UIDoc
        {
            get
            {
                return udoc;
            }
        }

        public static void CreateTestFaces(IList<Reference> faceSelection, IList<Reference> massSelection, double analysysGridSize, UIDocument uidoc, View view)
        {
            if (faceSelection == null) {
                return;
            }

            List<DirectSunTestFace> testFaces = CreateEmptyTestFaces(faceSelection, uidoc.Document);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var solids = SolidsFromReferences(massSelection, uidoc.Document);

            Transaction t = new Transaction(uidoc.Document);
            t.Start("testSolarVectorLines");

            ////create colour scheme
            SolarAnalysisColourSchemes.CreateAnalysisScheme(SolarAnalysisColourSchemes.DefaultColours, uidoc.Document, "Direct Sunlight Hours", true);
            var schemeId = SolarAnalysisColourSchemes.CreateAnalysisScheme(SolarAnalysisColourSchemes.DefaultColours, uidoc.Document, "Direct Sunlight Hours - No Legend", false);

            foreach (DirectSunTestFace testFace in testFaces) {
                var boundingBox = testFace.Face.GetBoundingBox();
                double boundingBoxUTotal = boundingBox.Max.U - boundingBox.Min.U;
                double boundingBoxVTotal = boundingBox.Max.V - boundingBox.Min.V;
                double gridDivisionsU = boundingBoxUTotal > 2 * analysysGridSize ? (boundingBoxUTotal / analysysGridSize) : 2;
                double gridDivisionsV = boundingBoxVTotal > 2 * analysysGridSize ? (boundingBoxVTotal / analysysGridSize) : 2;
                double gridSizeU = boundingBoxUTotal / gridDivisionsU;
                double gridSizeV = boundingBoxVTotal / gridDivisionsV;

                for (double u = boundingBox.Min.U + (gridSizeU / 2); u <= boundingBox.Max.U; u += gridSizeU) {
                    for (double v = boundingBox.Min.V + (gridSizeV / 2); v <= boundingBox.Max.V; v += gridSizeV) {
                        UV uv = new UV(u, v);

                        if (testFace.Face.IsInside(uv)) {
                            SunAndShadowSettings setting = view.SunAndShadowSettings;
                            var hoursOfSun = setting.NumberOfFrames;
                            //// Autodesk makes active frame starts from 1..
                            for (int activeFrame = 1; activeFrame <= setting.NumberOfFrames; activeFrame++) {
                                setting.ActiveFrame = activeFrame;
                                XYZ start = testFace.Face.Evaluate(uv);
                                start = start.Add(testFace.Face.ComputeNormal(uv).Normalize() / 16);
                                XYZ sunDirection = SolarAnalysisManager.GetSunDirectionalVector(uidoc.ActiveView, SolarAnalysisManager.GetProjectPosition(uidoc.Document), out double azimuth);
                                XYZ end = start.Subtract(sunDirection.Multiply(1000));
                                ////BuildingCoder.Creator.CreateModelLine(uidoc.Document, start, end);
                                Line line = Line.CreateBound(start, end);

                                foreach (Solid solid in solids) {
                                    try {
                                        var solidInt = solid.IntersectWithCurve(line, new SolidCurveIntersectionOptions());
                                        if (solidInt.SegmentCount > 0) {
                                            ////TaskDialog.Show("Debug", "Collision Found");
                                            hoursOfSun = hoursOfSun - 1;
                                            break;
                                        }
                                    } catch {
                                        continue;
                                    }
                                }
                            } ////ray loop
                            testFace.AddValueAtPoint(uv, hoursOfSun - 1);
                            ////TaskDialog.Show("RayHits", hoursOfSun.ToString());
                        }
                    }
                }
            }

            SpatialFieldManager sfm = DirectSunTestFace.GetSpatialFieldManager(uidoc.Document);
            sfm.Clear();

            foreach (DirectSunTestFace testFace in testFaces) {
                testFace.CreateAnalysisSurface(uidoc, sfm);
            }

            view.AnalysisDisplayStyleId = schemeId;

            t.Commit();
            stopwatch.Stop();
            TaskDialog.Show("Time Elapsed", "Time elepsed " + stopwatch.Elapsed.ToString() + @"(hh:mm:ss:uu)");
        }

        /// <summary>
        ///     Gets the Atmospheric Refraction using Bennett's formula
        /// </summary>
        /// <param name="altitudeRadians">Altitude in radians</param>
        /// <returns>The Atmospheric Refraction in radians</returns>
        public static double GetAtmosphericRefraction(double altitudeRadians)
        {
            var altitudeDegrees = altitudeRadians * 180 / Math.PI;
            var formula = altitudeDegrees + (7.31 / (altitudeDegrees + 4.4));
            var radians = Math.PI * altitudeDegrees / 180.0;
            return 1 / Math.Tan(radians) * 0.00029088820866572;
        }

        public static ElementId GetHighestLevel(Document doc)
        {
            double highestLevel = -1;
            ElementId highestId = null;
            using (var collector = new FilteredElementCollector(doc)) {
                collector.OfClass(typeof(Level));
                foreach (Level level in collector) {
                    if (highestLevel < 0 || level.Elevation > highestLevel) {
                        highestLevel = level.Elevation;
                        highestId = level.Id;
                    }
                }
            }

            return highestId;
        }

        // FIXME this can go in a utiliy class.
        public static string GetNiceViewName(Document doc, string request)
        {
            if (ViewNameIsAvailable(doc, request)) {
                return request;
            }
            return request + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) +
                   @")";
        }

        public static ProjectPosition GetProjectPosition(Document doc)
        {
            var projectLocation = doc.ActiveProjectLocation;
#if REVIT2018 || REVIT2019
            return projectLocation.GetProjectPosition(XYZ.Zero);
#else
                return projectLocation.get_ProjectPosition(XYZ.Zero);
#endif
        }

        /// <summary>
        ///     Gets the Atmospheric Refraction using Sæmundsson's formula
        /// </summary>
        /// <param name="altitudeRadians">Altitude in radians</param>
        /// <returns>The Atmospheric Refraction in radians</returns>
        public static double GetSæmundssonAtmosphericRefraction(double altitudeRadians)
        {
            var altitudeDegrees = altitudeRadians * 180 / Math.PI;
            var formula = ((1.02 * altitudeDegrees) + 10.3) / (altitudeDegrees + 5.11);
            var radians = Math.PI * altitudeDegrees / 180.0;
            return 1 / Math.Tan(radians) * 0.00029088820866572;
        }

        public static XYZ GetSunDirectionalVector(View view, ProjectPosition projectPosition, out double azimuth)
        {
            var sunSettings = view.SunAndShadowSettings;
            var frame = sunSettings.ActiveFrame;
            azimuth = sunSettings.GetFrameAzimuth(frame);
            var altitude = sunSettings.GetFrameAltitude(frame);
            azimuth += projectPosition.Angle;
            var forward = new XYZ(
                -Math.Sin(azimuth),
                -Math.Cos(azimuth),
                -Math.Tan(altitude - GetAtmosphericRefraction(altitude)));
            return forward;
        }

        public static bool ViewIsIso(View view)
        {
            return view.ViewType == ViewType.ThreeD;
        }

        public static bool ViewIsSingleDay(View view)
        {
            var sunSettings = view.SunAndShadowSettings;
            if (sunSettings == null) {
                return false;
            }
            return sunSettings.TimeInterval == SunStudyTimeInterval.Hour && sunSettings.SunAndShadowType == SunAndShadowType.OneDayStudy;
        }

        // FIXME put this somewhere else.
        public static bool ViewNameIsAvailable(Document doc, string name)
        {
            using (var c = new FilteredElementCollector(doc)) {
                c.OfClass(typeof(View));
                foreach (View view in c) {
                    var v = view;
#if REVIT2019
                                        if (v.Name == name) {
                                            return false;
                                        }
#else
                    if (v.ViewName == name) {
                        return false;
                    }
#endif
                }
            }

            return true;
        }

        public void Go()
        {
            if (RotateCurrentView) {
                RotateView(activeView);
            }
            if (Create3dViews) {
                CreateWinterViews();
            }
            if (CreateShadowPlans) {
                CreateShadowPlanViews();
            }
        }

        private static List<DirectSunTestFace> CreateEmptyTestFaces(IList<Reference> faceSelection, Document doc)
        {
            int n = 0;
            List<DirectSunTestFace> result = new List<DirectSunTestFace>();
            foreach (Reference r in faceSelection) {
                n++;
                Element elem = doc.GetElement(r);
                Face f = (Face)elem.GetGeometryObjectFromReference(r);
                var normal = f.ComputeNormal(new UV(0, 0));
                ////if (normal.Z >= 0) {
                result.Add(new DirectSunTestFace(r, @"DirectSun(" + n.ToString(System.Globalization.CultureInfo.CurrentCulture) + @")", doc));
                ////}
            }
            ////TaskDialog.Show("Debug", "Faces added: " + result.Count);
            return result;
        }

        private static XYZ GetEyeLocation(View view)
        {
            var viewBounds = view.get_BoundingBox(view);
            var max = viewBounds.Max;
            var min = viewBounds.Min;
            return new XYZ(
                 min.X + ((max.X - min.X) / 2),
                 min.Y + ((max.Y - min.Y) / 2),
                 min.Z + ((max.Z - min.Z) / 2));
        }

        private static ElementId GetViewFamilyId(Document doc, ViewFamily viewFamilyType)
        {
            using (var collector = new FilteredElementCollector(doc)) {
                collector.OfClass(typeof(ViewFamilyType));
                foreach (var e in collector) {
                    var vft = (ViewFamilyType)e;
                    if (vft.ViewFamily == viewFamilyType) {
                        return vft.Id;
                    }
                }
            }

            return null;
        }

        private static List<Solid> SolidsFromReferences(IList<Reference> massSelection, Document doc)
        {
            List<Solid> result = new List<Solid>();
            foreach (Reference solidRef in massSelection) {
                Element e = doc.GetElement(solidRef);

                Options opt = new Options()
                {
                    ComputeReferences = false,
                    IncludeNonVisibleObjects = false,
                    View = doc.ActiveView
                };

                GeometryElement geoElem = e.get_Geometry(opt);
                foreach (GeometryObject obj in geoElem) {
                    if (obj is Solid) {
                        Solid solid = obj as Solid;
                        if (solid.IsElementGeometry && solid.Faces.Size > 0) {
                            result.Add(solid);
                        }
                    }
                }
            }
            ////TaskDialog.Show("Debug", "Solids added: " + result.Count);
            return result;
        }
        ////[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CreateShadowPlanViews()
        {
            var id = GetViewFamilyId(doc, ViewFamily.FloorPlan);
            var levelId = GetHighestLevel(doc);
            if (id == null || levelId == null) {
                return;
            }

            while (StartTime <= EndTime) {
                using (var t = new Transaction(doc))
                {
                    if (t.Start("Create Shadow Plans") == TransactionStatus.Started) {
                        View view = ViewPlan.Create(doc, id, levelId);
                        view.ViewTemplateId = ElementId.InvalidElementId;
                        var niceMinutes = "00";
                        if (StartTime.Minute > 0) {
                            niceMinutes = StartTime.Minute.ToString(CultureInfo.CurrentCulture);
                        }
                        var vname = "SHADOW PLAN - " + StartTime.ToShortDateString() + "-" + StartTime.Hour + "." +
                                    niceMinutes;
                        view.Name = GetNiceViewName(doc, vname);
                        var sunSettings = view.SunAndShadowSettings;
                        sunSettings.StartDateAndTime = StartTime;
                        sunSettings.SunAndShadowType = SunAndShadowType.StillImage;
                        view.SunlightIntensity = 50;
                        t.Commit();
                        StartTime = StartTime.Add(ExportTimeInterval);
                    }
                }
            }
        }

        ////[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CreateWinterViews()
        {
            var id = GetViewFamilyId(doc, ViewFamily.ThreeDimensional);

            // FIXME add error message here
            if (id == null) {
                return;
            }

            while (StartTime <= EndTime)
            {
                using (var t = new Transaction(doc))
                {
                    t.Start("Create Solar View");
                    View view = View3D.CreateIsometric(doc, id);
                    view.ViewTemplateId = ElementId.InvalidElementId;
                    var niceMinutes = "00";
                    if (StartTime.Minute > 0)
                    {
                        niceMinutes = StartTime.Minute.ToString(CultureInfo.CurrentCulture);
                    }
                    var vname = "SOLAR ACCESS - " + StartTime.ToShortDateString() + "-" + StartTime.Hour + "." +
                                niceMinutes;
                    view.Name = GetNiceViewName(doc, vname);
                    var sunSettings = view.SunAndShadowSettings;
                    sunSettings.StartDateAndTime = StartTime;
                    sunSettings.SunAndShadowType = SunAndShadowType.StillImage;
                    t.Commit();

                    // FIXME too many transactions here and above...
                    RotateView(view);
                    StartTime = StartTime.Add(ExportTimeInterval);
                }
            }
        }

        private string GetViewInfo(View view)
        {
            var info = new StringBuilder();
            if (!ViewIsIso(view))
            {
                info.AppendLine("Not a 3d view...");
                info.AppendLine(string.Empty);
                info.AppendLine("Please select a 3d view to rotate");
                info.AppendLine("or use the create winter views feature");
                return info.ToString();
            }

            var sunSettings = view.SunAndShadowSettings;
            var frame = sunSettings.ActiveFrame;
            var azimuth = sunSettings.GetFrameAzimuth(frame);
            var altitude = sunSettings.GetFrameAltitude(frame);
            azimuth += position.Angle;
            var azdeg = azimuth * 180 / Math.PI;
            var altdeg = altitude * 180 / Math.PI;
            info.AppendLine("Date - " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongDateString());
            info.AppendLine("Time - " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongTimeString());
            info.AppendLine("Sunrise - " +
                            sunSettings.GetSunrise(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString());
            info.AppendLine("Sunset - " +
                            sunSettings.GetSunset(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString());
            info.AppendLine("Sun Altitude - " + altdeg.ToString(CultureInfo.InvariantCulture));
            info.AppendLine("Sun Azimuth - " + azdeg.ToString(CultureInfo.InvariantCulture));
            return info.ToString();
        }
        ////[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void RotateView(View view)
        {
            if (view.ViewType == ViewType.ThreeD) {
                double azimuth;
                var forward = GetSunDirectionalVector(view, position, out azimuth);
                var up = forward.CrossProduct(new XYZ(Math.Cos(azimuth), -Math.Sin(azimuth), 0));

                var v3d = (View3D)view;
                if (v3d.IsLocked) {
                    TaskDialog.Show("ERROR", "View is locked, please unlock before rotating");
                    return;
                }

                using (var t = new Transaction(doc))
                {
                    t.Start("Rotate View");
                    v3d.SetOrientation(new ViewOrientation3D(GetEyeLocation(view), up, forward));
                    if (v3d.CanBeLocked() && !v3d.Name.StartsWith("{", StringComparison.OrdinalIgnoreCase)) {
                        try {
                            v3d.SaveOrientationAndLock();
                        } catch (InvalidOperationException e) {
                            Debug.WriteLine(e.Message);
                        }
                    }

                    udoc.RefreshActiveView();
                    t.Commit();
                }
            }
            else
            {
                TaskDialog.Show("ERROR", "Not a 3d view");
            }
        }
    }
}