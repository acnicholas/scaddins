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
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Analysis;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class DirectSunCommand : IExternalCommand
    {
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

        private static List<DirectSunTestFace> CreateEmptyTestFaces(IList<Reference> faceSelection, Document doc)
        {
            int n = 0;
            List<DirectSunTestFace> result = new List<DirectSunTestFace>();
            foreach (Reference r in faceSelection) {
                n++;
                Element elem = doc.GetElement(r);
                Face f = (Face)elem.GetGeometryObjectFromReference(r);
                result.Add(new DirectSunTestFace(r, @"DirectSun(" + n.ToString() +  @")", doc));
            }
            ////TaskDialog.Show("Debug", "Faces added: " + result.Count);
            return result;
        }

        public static void CreateTestFaces(IList<Reference> faceSelection, IList<Reference> massSelection, double analysysGridSize, UIDocument uidoc, View view)
        {
            if (faceSelection == null) {
                return;
            }

            List<DirectSunTestFace> testFaces = CreateEmptyTestFaces(faceSelection, uidoc.Document);

            ////int lineCount = 0;

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var solids = SolidsFromReferences(massSelection, uidoc.Document);

            Transaction t = new Transaction(uidoc.Document);
            t.Start("testSolarVectorLines");

            foreach (DirectSunTestFace testFace in testFaces) {
                var boundingBox = testFace.Face.GetBoundingBox();


                ////TaskDialog.Show("U", (boundingBox.Max.U - boundingBox.Min.U).ToString());
                ////TaskDialog.Show("V", (boundingBox.Max.V - boundingBox.Min.V).ToString());

                //double uSize = boundingBox.Max.U - boundingBox.Min.U;
                //double vSize = boundingBox.Max.V - boundingBox.Min.V;
                //int uGridDivisions = uSize > 2 * analysysGridSize ? (int)(uSize / analysysGridSize) : 2;
                //int vGridDivisions = vSize > 2 * analysysGridSize ? (int)(vSize / analysysGridSize) : 2;
                double uGridSize = analysysGridSize;
                double vGridSize = analysysGridSize;

                ////TaskDialog.Show("U", uGridDivisions.ToString() + "-" + uGridSize.ToString());
                ////TaskDialog.Show("V", vGridDivisions.ToString() + "-" + vGridSize.ToString());

                for (double u = boundingBox.Min.U + uGridSize / 2; u <= boundingBox.Max.U - uGridSize / 2; u += uGridSize) {
                    for (double v = boundingBox.Min.V + vGridSize / 2; v <= boundingBox.Max.V - vGridSize / 2; v += vGridSize) {
                        UV uv = new UV(u, v);
                        
                        if (testFace.Face.IsInside(uv)) {
                            
                            SunAndShadowSettings setting = view.SunAndShadowSettings;
                            double hoursOfSun = setting.NumberOfFrames;
                            for (int activeFrame = 0; activeFrame < setting.NumberOfFrames; activeFrame++) {
                                setting.ActiveFrame = activeFrame;
                                ////TaskDialog.Show("Time", setting.ActiveFrameTime.ToLongTimeString());
                                XYZ start = testFace.Face.Evaluate(uv);
                                ////start.Add(testFace.Face.ComputeNormal(uv).Normalize().Multiply(100));
                                XYZ sunDirection = SolarViews.GetSunDirectionalVector(uidoc.ActiveView, SolarViews.GetProjectPosition(uidoc.Document), out double azimuth);
                                start = start.Subtract(sunDirection.Normalize()/16);
                                XYZ end = start.Subtract(sunDirection.Multiply(1000));
                                ////BuildingCoder.Creator.CreateModelLine(uidoc.Document, start, end);
                                Line line = Line.CreateBound(start, end);
                                ////lineCount++;

                                foreach (Solid solid in solids) { 
                                    try {
                                        var solidInt = solid.IntersectWithCurve(line, new SolidCurveIntersectionOptions());
                                        if (solidInt.SegmentCount > 0) {
                                            ////TaskDialog.Show("Debug", "Collision Found");
                                            hoursOfSun--;
                                            break;
                                        }
                                   } catch {
                                        continue;
                                   }
                                }

                            } //ray loop
                            testFace.AddValueAtPoint(uv, hoursOfSun);
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

            t.Commit();
            stopwatch.Stop();
            TaskDialog.Show("Time Elapsed", "Time elepsed " + stopwatch.Elapsed.ToString() + @"(hh:mm:ss:uu)");
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
            ////Document doc = udoc.Document;

            var vm = new ViewModels.DirectSunViewModel(commandData.Application.ActiveUIDocument);
            SCaddinsApp.WindowManager.ShowDialog(vm, null, ViewModels.DirectSunViewModel.DefaultViewSettings);
            if (vm.SelectedCloseMode == ViewModels.DirectSunViewModel.CloseMode.Analize) {
                CreateTestFaces(vm.FaceSelection, vm.MassSelection, 3, udoc, udoc.ActiveView);
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}