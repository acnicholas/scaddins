// (C) Copyright 2013-2015 by Andrew Nicholas
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
    using System.Collections.Generic;
    using System.Dynamic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Analysis;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class DirectSunCommand : IExternalCommand
    {
        public static List<Solid> SolidsFromReferences(IList<Reference> massSelection, Document doc)
        {
            List<Solid> result = new List<Solid>();
            foreach (Reference solidRef in massSelection) {
                Element e = doc.GetElement(solidRef);
                GeometryElement geoElem = e.get_Geometry(new Options());
                foreach (GeometryObject obj in geoElem) {
                    if (obj is Solid) {
                        Solid solid = obj as Solid;
                        result.Add(solid);
                    }
                }
            }
            TaskDialog.Show("Debug", "Solids added: " + result.Count);
            return result;
        }

        public static List<Face> FacesFromReferences(IList<Reference> faceSelection, Document doc)
        {
            List<Face> result = new List<Face>();
            foreach (Reference r in faceSelection) {
                Face f = (Face)doc.GetElement(r).GetGeometryObjectFromReference(r);
                result.Add(f);
            }
            TaskDialog.Show("Debug", "Faces added: " + result.Count);
            return result;
        }

        ////public static List<Line> Lines


        public static void CreateAnalysisRays(IList<Reference> faceSelection, IList<Reference> massSelection, int divisions, UIDocument uidoc)
        {
            if (faceSelection == null) {
                return;
            }

            int lineCount = 0;

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var solids = SolidsFromReferences(massSelection, uidoc.Document);

            Transaction t = new Transaction(uidoc.Document);
            t.Start("testSolarVectorLines");

            foreach (Face face in FacesFromReferences(faceSelection, uidoc.Document)) {
                var bb = face.GetBoundingBox();
                for (double u = bb.Min.U; u <= bb.Max.U; u += (bb.Max.U - bb.Min.U) / divisions) {
                    for (double v = bb.Min.V; v <= bb.Max.V; v += (bb.Max.V - bb.Min.V) / divisions) {
                        UV uv = new UV(u, v);
                        
                        if (face.IsInside(uv)) {
                            
                            SunAndShadowSettings setting = uidoc.ActiveView.SunAndShadowSettings;
                            double hoursOfSun = setting.NumberOfFrames;
                            for (int activeFrame = 0; activeFrame < setting.NumberOfFrames; activeFrame++) {
                                setting.ActiveFrame = activeFrame;
                                ////TaskDialog.Show("Time", setting.ActiveFrameTime.ToLongTimeString());
                                XYZ start = face.Evaluate(uv);
                                start.Add(face.ComputeNormal(uv).Normalize().Multiply(100));
                                XYZ sunDirection = SolarViews.GetSunDirectionalVector(uidoc.ActiveView, SolarViews.GetProjectPosition(uidoc.Document), out double azimuth);
                                start = start.Subtract(sunDirection.Normalize());
                                XYZ end = start.Subtract(sunDirection.Multiply(1000));
                                BuildingCoder.Creator.CreateModelLine(uidoc.Document, start, end);
                                Line line = Line.CreateBound(start, end);
                                lineCount++;

                                foreach (Solid solid in solids) {

                                    var solidInt = solid.IntersectWithCurve(line, new SolidCurveIntersectionOptions());
                                    if (solidInt.SegmentCount > 0) {
                                        ////TaskDialog.Show("Debug", "Collision Found");
                                        hoursOfSun--;
                                        break;
                                    }

                                }

                            } //ray loop
                            TaskDialog.Show("RayHits", hoursOfSun.ToString());
                        }
                    }
                }
            }

            t.Commit();
            stopwatch.Stop();
            TaskDialog.Show("Time Elapsed", lineCount + " lines drawn in " + stopwatch.Elapsed.ToString() + @"(hh:mm:ss:uu)");
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            if (commandData == null) {
                return Autodesk.Revit.UI.Result.Failed;
            }

            UIDocument udoc = commandData.Application.ActiveUIDocument;
            Document doc = udoc.Document;

            dynamic settings = new ExpandoObject();
            settings.Height = 480;
            settings.Width = 300;
            settings.Title = "Direct Sun - By Andrew Nicholas";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;

            var vm = new ViewModels.DirectSunViewModel(commandData.Application.ActiveUIDocument);
            SCaddinsApp.WindowManager.ShowDialog(vm, null, settings);
            if (vm.SelectedCloseMode == ViewModels.DirectSunViewModel.CloseMode.Analize) {
                CreateAnalysisRays(vm.FaceSelection, vm.MassSelection, 1, udoc);
                ////CreateAnalysisSurfaces(udoc, vm.FaceSelection);
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        private static void CreateAnalysisSurfaces(UIDocument uiDoc, IList<Reference> faceSelection)
        {
            Document doc = uiDoc.Document;

            SpatialFieldManager sfm = SpatialFieldManager.GetSpatialFieldManager(doc.ActiveView);
            if (sfm == null) {
                sfm = SpatialFieldManager.CreateSpatialFieldManager(doc.ActiveView, 1);
            }
            ////Reference reference = uiDoc.Selection.PickObject(ObjectType.Face, "Select a face");
            ////int idx = sfm.AddSpatialFieldPrimitive(reference);
            sfm.Clear();

            int n = 0;

            foreach (Reference reference in faceSelection) {

                ////Reference reference = faceSelection[0];
                int idx = sfm.AddSpatialFieldPrimitive(reference);

                Face face = doc.GetElement(reference).GetGeometryObjectFromReference(reference) as Face;

                IList<UV> pointsUV = new List<UV>();
                BoundingBoxUV bb = face.GetBoundingBox();
                UV min = bb.Min;
                UV max = bb.Max;
                pointsUV.Add(new UV(min.U, min.V));
                pointsUV.Add(new UV(max.U, max.V));

                FieldDomainPointsByUV pnts = new FieldDomainPointsByUV(pointsUV);

                List<double> doubleList = new List<double>();
                IList<ValueAtPoint> valList = new List<ValueAtPoint>();
                doubleList.Add(0);
                valList.Add(new ValueAtPoint(doubleList));
                doubleList.Clear();
                doubleList.Add(10);
                valList.Add(new ValueAtPoint(doubleList));

                FieldValues vals = new FieldValues(valList);
                AnalysisResultSchema resultSchema = new AnalysisResultSchema("Total Hours of Direct Sun " + n, "Description");
                int schemaIndex = sfm.RegisterResult(resultSchema);
                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, schemaIndex);
                n++;
            }
        }
    }
}