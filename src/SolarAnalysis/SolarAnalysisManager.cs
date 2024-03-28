// (C) Copyright 2013-2021 by Andrew Nicholas
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
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
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
            StartTime = new DateTime(2018, 6, 21, 9, 0, 0, DateTimeKind.Local);
            EndTime = new DateTime(2018, 6, 21, 15, 0, 0, DateTimeKind.Local);
            ExportTimeInterval = new TimeSpan(1, 0, 0);
            activeView = doc.ActiveView;
        }

        public string ActiveIewInformation => GetViewInfo(activeView);

        public bool CanCreateAnalysisView => ViewIsSingleDay(activeView);

        public bool CanRotateActiveView => ViewIsIso(activeView);

        public bool Create3dViews { get; set; }

        public bool CreateAnalysisView { get; set; }

        public bool CreateShadowPlans { get; set; }

        public bool DrawSolarRay { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan ExportTimeInterval { get; set; }

        public bool RotateCurrentView { get; set; }

        public DateTime StartTime { get; set; }

        public UIDocument UIDoc => udoc;

        public static void CreateTestFaces(IList<Reference> faceSelection, IList<Reference> massSelection, double analysysGridSize, UIDocument uidoc, View view)
        {
            double totalUVPoints = 0;
            double totalUVPointsWith2PlusHours = 0;

            if (faceSelection == null)
            {
                return;
            }

            var testFaces = CreateEmptyTestFaces(faceSelection, uidoc.Document);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // not required when using ReferenceIntersector 
            var solids = SolidsFromReferences(massSelection, uidoc.Document);

            // create a ReferenceIntersection
            var ids = massSelection.Select(s => s.ElementId).ToList();
            var referenceIntersector = new ReferenceIntersector(ids, FindReferenceTarget.All, view as View3D);

            Transaction t = new Transaction(uidoc.Document);
            t.Start("testSolarVectorLines");

            ////create colour scheme
            SolarAnalysisColourSchemes.CreateAnalysisScheme(SolarAnalysisColourSchemes.DefaultColours, uidoc.Document, "Direct Sunlight Hours", true);
            var schemeId = SolarAnalysisColourSchemes.CreateAnalysisScheme(SolarAnalysisColourSchemes.DefaultColours, uidoc.Document, "Direct Sunlight Hours - No Legend", false);

            foreach (DirectSunTestFace testFace in testFaces)
            {
                var boundingBox = testFace.Face.GetBoundingBox();
                var boundingBoxUTotal = boundingBox.Max.U - boundingBox.Min.U;
                var boundingBoxVTotal = boundingBox.Max.V - boundingBox.Min.V;
                var gridDivisionsU = boundingBoxUTotal > 2 * analysysGridSize ? (boundingBoxUTotal / analysysGridSize) : 2;
                var gridDivisionsV = boundingBoxVTotal > 2 * analysysGridSize ? (boundingBoxVTotal / analysysGridSize) : 2;
                var gridSizeU = boundingBoxUTotal / gridDivisionsU;
                var gridSizeV = boundingBoxVTotal / gridDivisionsV;

                for (var u = boundingBox.Min.U + (gridSizeU / 2); u <= boundingBox.Max.U; u += gridSizeU)
                {
                    for (var v = boundingBox.Min.V + (gridSizeV / 2); v <= boundingBox.Max.V; v += gridSizeV)
                    {
                        UV uv = new UV(u, v);

                        if (testFace.Face.IsInside(uv))
                        {
                            var setting = view.SunAndShadowSettings;
                            double interval = 1;
                            switch (setting.TimeInterval)
                            {
                                case SunStudyTimeInterval.Hour:
                                    interval = 1;
                                    break;
                                case SunStudyTimeInterval.Minutes30:
                                    interval = 0.5;
                                    break;
                                case SunStudyTimeInterval.Minutes15:
                                    interval = 0.25;
                                    break;
                                case SunStudyTimeInterval.Minutes45:
                                    interval = 0.75;
                                    break;
                            }

                            var hoursOfSun = (setting.NumberOfFrames - 1) * interval;
                            //// Autodesk makes active frame starts from 1..
                            for (int activeFrame = 1; activeFrame <= setting.NumberOfFrames; activeFrame++)
                            {
                                setting.ActiveFrame = activeFrame;
                                var start = testFace.Face.Evaluate(uv);
                                start = start.Add(testFace.Face.ComputeNormal(uv).Normalize() / 16);
                                var sunDirection = GetSunDirectionalVector(uidoc.ActiveView, GetProjectPosition(uidoc.Document), out var _);
                                var end = start.Subtract(sunDirection.Multiply(1000));

                                //// use this only for testing.
                                //// BuildingCoder.Creator.CreateModelLine(uidoc.Document, start, end);

                                var line = Line.CreateBound(start, end);

                                // Brute Force
                                // remove if ReferenceIntersector is faster...
                                foreach (var solid in solids)
                                {
                                    try
                                    {
                                        var solidInt = solid.IntersectWithCurve(line, new SolidCurveIntersectionOptions());
                                        if (solidInt.SegmentCount > 0)
                                        {
                                            hoursOfSun = hoursOfSun - interval;
                                            break;
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception.Message);
                                    }
                                }
                            }
                            totalUVPoints++;

                            if (hoursOfSun >= 2)
                            {
                                totalUVPointsWith2PlusHours++;
                            }

                            testFace.AddValueAtPoint(uv, hoursOfSun);
                        }
                    }
                }
            }

            var sfm = DirectSunTestFace.GetSpatialFieldManager(uidoc.Document);
            sfm.Clear();

            foreach (var testFace in testFaces)
            {
                testFace.CreateAnalysisSurface(uidoc, sfm);
            }

            view.AnalysisDisplayStyleId = schemeId;

            t.Commit();
            stopwatch.Stop();
            var percent = totalUVPointsWith2PlusHours / totalUVPoints * 100;
            var percentString = percent.ToString();
            SCaddinsApp.WindowManager.ShowMessageBox("Summary", "Time elepsed " + stopwatch.Elapsed.ToString() + @"(hh:mm:ss:uu), Percent above 2hrs: " + percentString);
        }

        /// <summary>
        ///     Gets the Atmospheric Refraction using Bennett's formula
        /// </summary>
        /// <param name="altitudeRadians">Altitude in radians</param>
        /// <returns>The Atmospheric Refraction in radians</returns>
        public static double GetAtmosphericRefraction(double altitudeRadians)
        {
            var altitudeDegrees = altitudeRadians * 180 / Math.PI;
            //// var formula = altitudeDegrees + (7.31 / (altitudeDegrees + 4.4));
            var radians = Math.PI * altitudeDegrees / 180.0;
            return 1 / Math.Tan(radians) * 0.00029088820866572;
        }

        public static ElementId GetHighestLevel(Document doc)
        {
            double highestLevel = -1;
            ElementId highestId = null;
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfClass(typeof(Level));
                foreach (var element in collector)
                {
                    var level = (Level)element;
                    if (!(highestLevel < 0) && !(level.Elevation > highestLevel))
                    {
                        continue;
                    }
                    highestLevel = level.Elevation;
                    highestId = level.Id;
                }
            }

            return highestId;
        }

        // FIXME this can go in a utility class.
        public static string GetNiceViewName(Document doc, string request)
        {
            if (ViewNameIsAvailable(doc, request))
            {
                return request;
            }
            return request + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) +
                   @")";
        }

        public static ProjectPosition GetProjectPosition(Document doc)
        {
            var projectLocation = doc.ActiveProjectLocation;
            return projectLocation.GetProjectPosition(XYZ.Zero);
        }

        /// <summary>
        ///     Gets the Atmospheric Refraction using Sæmundsson's formula
        /// </summary>
        /// <param name="altitudeRadians">Altitude in radians</param>
        /// <returns>The Atmospheric Refraction in radians</returns>
        public static double GetSæmundssonAtmosphericRefraction(double altitudeRadians)
        {
            var altitudeDegrees = altitudeRadians * 180 / Math.PI;
            //// var formula = ((1.02 * altitudeDegrees) + 10.3) / (altitudeDegrees + 5.11);
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
            if (sunSettings == null)
            {
                return false;
            }
            return sunSettings.SunAndShadowType == SunAndShadowType.OneDayStudy;
        }

        /// <summary>
        /// Draw a model line in the direction of the sun,
        /// This will use the Sun Settings of the current(active) Revit view.
        /// This will also prompt the user for the origin point.
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="rayLengthMM"></param>
        /// <returns></returns>
        public static bool DrawSolarRayAsModelLine(UIDocument uidoc, double rayLengthMM)
        {
                double rayLength = rayLengthMM / 304.8; // 100 meters in plan view
                var view = uidoc.Document.ActiveView;
                var sunSettings = view.SunAndShadowSettings;
                var frame = sunSettings.ActiveFrame;
                var azimuth = sunSettings.GetFrameAzimuth(frame);
                var altitude = sunSettings.GetFrameAltitude(frame);
                var startpoint = uidoc.Selection.PickPoint("Pick point do draw solar ray from"); 
                using (var t = new Transaction(uidoc.Document))
                {
                        if (t.Start("Draw Solar Ray") == TransactionStatus.Started) {
                                var endpoint = new XYZ(
                                                startpoint.X + (rayLength * Math.Sin(azimuth)),
                                                startpoint.Y + (rayLength * Math.Cos(azimuth)),
                                                startpoint.Z + (rayLength * Math.Tan(altitude)));
                                var ray = Autodesk.Revit.DB.Line.CreateBound(startpoint, endpoint);
                                var plane = Autodesk.Revit.DB.Plane.CreateByThreePoints(startpoint, endpoint, new XYZ(endpoint.X, endpoint.Y, endpoint.Z + rayLength));
                                var doc = uidoc.Document;
                                var sketchPlane = Autodesk.Revit.DB.SketchPlane.Create(doc, plane);
                                doc.Create.NewModelCurve(ray, sketchPlane);
                                t.Commit();
                    }
                }
                return true;
        }

        /// <summary>
        /// Check if a view name can be used in the active Revit doc.
        /// </summary>
        /// <param name="doc">Active Revit document</param>
        /// <param name="name">Name of view to test</param>
        /// <returns></returns>
        public static bool ViewNameIsAvailable(Document doc, string name)
        {
            if (doc == null || name == null)
            {
                return false;
            }
            using (var c = new FilteredElementCollector(doc))
            {
                c.OfClass(typeof(View));
                foreach (var element in c)
                {
                    var view = (View)element;
                    var v = view;
                    if (v.Name == name)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Attemp to excecture the selected option.
        /// Options that will run with this method are:
        ///     * RotateVew
        ///     * Create3dViews
        ///     * Create Shadow Plan
        /// </summary>
        /// <param name="log"> This log will hold warnings/errors if any occur,</param>
        /// <returns></returns>
        public bool Go(ModelSetupWizard.TransactionLog log)
        {
            if (RotateCurrentView)
            {
                var result = RotateView(activeView);
                if (result) {
                        log.AddSuccess("View rotation successfull");
                        return true;
                } else {
                        log.AddFailure("View rotation unsuccessfull");
                        return false;
                } 
            }
            if (Create3dViews)
            {
                return CreateWinterViews(log);
            }
            if (CreateShadowPlans)
            {
                return CreateShadowPlanViews(log);
            }
            return false;
        }

        private static List<DirectSunTestFace> CreateEmptyTestFaces(IList<Reference> faceSelection, Document doc)
        {
            int n = 0;
            List<DirectSunTestFace> result = new List<DirectSunTestFace>();
            foreach (Reference r in faceSelection)
            {
                n++;
                Element elem = doc.GetElement(r);
                Face f = (Face)elem.GetGeometryObjectFromReference(r);
                f.ComputeNormal(new UV(0, 0));
                result.Add(new DirectSunTestFace(r, @"DirectSun(" + n.ToString(CultureInfo.CurrentCulture) + @")", doc));
            }
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
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfClass(typeof(ViewFamilyType));
                foreach (var e in collector)
                {
                    var vft = (ViewFamilyType)e;
                    if (vft.ViewFamily == viewFamilyType)
                    {
                        return vft.Id;
                    }
                }
            }

            return null;
        }

        private static List<Solid> SolidsFromReferences(IList<Reference> massSelection, Document doc)
        {
            List<Solid> result = new List<Solid>();
            foreach (var solidRef in massSelection)
            {
                var e = doc.GetElement(solidRef);

                Options opt = new Options()
                {
                    ComputeReferences = false,
                    IncludeNonVisibleObjects = false,
                    View = doc.ActiveView
                };

                var geoElem = e.get_Geometry(opt);
                foreach (var obj in geoElem)
                {
                    if (!(obj is Solid))
                    {
                        continue;
                    }
                    var solid = obj as Solid;
                    if (solid.IsElementGeometry && solid.Faces.Size > 0)
                    {
                        result.Add(solid);
                    }
                }
            }
            return result;
        }
        ////[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private bool CreateShadowPlanViews(ModelSetupWizard.TransactionLog log)
        {
            var id = GetViewFamilyId(doc, ViewFamily.FloorPlan);
            var levelId = GetHighestLevel(doc);
            if (id == null || levelId == null)
            {
                log.AddFailure("Could not gererate shadow plans. FamilyId or LevelId not found");
                return false;
            }

            while (StartTime <= EndTime)
            {
                using (var t = new Transaction(doc))
                {
                    if (t.Start("Create Shadow Plans") == TransactionStatus.Started)
                    {
                        View view = ViewPlan.Create(doc, id, levelId);
                        view.ViewTemplateId = ElementId.InvalidElementId;
                        var niceMinutes = "00";
                        if (StartTime.Minute > 0)
                        {
                            niceMinutes = StartTime.Minute.ToString(CultureInfo.CurrentCulture);
                        }
                        var vname = "SHADOW PLAN - " + StartTime.ToShortDateString() + "-" + StartTime.Hour + "." +
                                    niceMinutes;
                        view.Name = GetNiceViewName(doc, vname);
                        var sunSettings = view.SunAndShadowSettings;
                        sunSettings.StartDateAndTime = StartTime;
                        sunSettings.SunAndShadowType = SunAndShadowType.StillImage;
                        view.SunlightIntensity = 50;
                        log.AddSuccess("Shadow Plan created: " + vname);
                        t.Commit();
                        StartTime = StartTime.Add(ExportTimeInterval);
                    }
                }
            }
            return true;
        }

        ////[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private bool CreateWinterViews(ModelSetupWizard.TransactionLog log)
        {
            var id = GetViewFamilyId(doc, ViewFamily.ThreeDimensional);

            // FIXME add error message here
            if (id == null)
            {
                log.AddFailure("Could not gererate 3d view. FamilyId not found");
                return false;
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

                    if (StartTime <= sunSettings.GetSunrise(StartTime).ToLocalTime() || StartTime >= sunSettings.GetSunset(EndTime).ToLocalTime())
                    {
                        doc.Delete(view.Id);
                        log.AddFailure("Cannot rotate a view that is not in daylight hours: " + vname);
                        StartTime = StartTime.Add(ExportTimeInterval);
                        continue;
                    }

                    sunSettings.SunAndShadowType = SunAndShadowType.StillImage;
                    t.Commit();

                    if (!RotateView(view))
                    { 
                        doc.Delete(view.Id);
                        log.AddFailure("Could not rotate view: " + vname);
                        continue;
                    }
                    log.AddSuccess("View created: " + vname);
                    StartTime = StartTime.Add(ExportTimeInterval);
                }
            }
            return true;
        }

        private string GetDMSfromDegrees(double degrees)
        {
                var d = Math.Floor(degrees);
                var m = (degrees - d) * 60;
                var s = (m - Math.Floor(m)) * 60;
                return string.Format("{0}°{1}'{2}\"", (int)d, (int)m, (int)s);
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
            var azdeg = azimuth * 180 / Math.PI;
            azdeg = azdeg < 0 ? 360 + azdeg : azdeg;
            var altdeg = altitude * 180 / Math.PI;
            info.AppendLine("Date: " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongDateString());
            info.AppendLine("Time: " + sunSettings.ActiveFrameTime.ToLocalTime().ToLongTimeString());
            info.AppendLine("Sunrise: " +
                            sunSettings.GetSunrise(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString());
            info.AppendLine("Sunset: " +
                            sunSettings.GetSunset(sunSettings.ActiveFrameTime).ToLocalTime().ToLongTimeString());
            info.AppendLine("Sun Altitude: " + altdeg.ToString("0.####") + " (" + GetDMSfromDegrees(altdeg) + ")");
            info.AppendLine("Sun Azimuth: " + azdeg.ToString("0.####") + " (" + GetDMSfromDegrees(azdeg) + ")");
            return info.ToString();
        }
        ////[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private bool RotateView(View view)
        {
            if (view.ViewType == ViewType.ThreeD)
            {
                var forward = GetSunDirectionalVector(view, position, out var azimuth);
                var up = forward.CrossProduct(new XYZ(Math.Cos(azimuth), -Math.Sin(azimuth), 0));

                var v3d = (View3D)view;
                if (v3d.IsLocked)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("ERROR", "View is locked, please unlock before rotating");
                    return false;
                }

                using (var t = new Transaction(doc))
                {
                    t.Start("Rotate View");
                    v3d.SetOrientation(new ViewOrientation3D(GetEyeLocation(view), up, forward));
                    if (v3d.CanBeLocked() && !v3d.Name.StartsWith("{", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            v3d.SaveOrientationAndLock();
                        }
                        catch (InvalidOperationException e)
                        {
                            Debug.WriteLine(e.Message);
                            return false;
                        }
                    }

                    udoc.RefreshActiveView();
                    t.Commit();
                }
            }
            else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("ERROR", "Not a 3d view");
                return false;
            }
            return true;
        }
    }
}
