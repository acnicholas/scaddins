// (C) Copyright 2014-2020 by Andrew Nicholas
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

namespace SCaddins.ViewUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CameraFromViewCommand : IExternalCommand
    {
        public static UIView ActiveUIView(UIDocument udoc, Element planView)
        {
            if (udoc != null && planView != null)
            {
                foreach (UIView view in udoc.GetOpenUIViews())
                {
                    View v = (View)udoc.Document.GetElement(view.ViewId);
                    if (v.Name == planView.Name)
                    {
                        return view;
                    }
                }
            }
            return null;
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static void ApplySectionBoxToView(BoundingBoxXYZ bounds, View3D view)
        {
            if (bounds != null || view != null)
            {
                view.SetSectionBox(bounds);
            }
        }

        public static XYZ GetMiddleOfActiveViewWindow(UIView view)
        {
            if (view == null)
            {
                return new XYZ();
            }
            XYZ topLeft = view.GetZoomCorners()[0];
            XYZ bottomRight = view.GetZoomCorners()[1];
            double width = bottomRight.X - topLeft.X;
            double height = bottomRight.Y - topLeft.Y;
            double middleX = bottomRight.X - (width / 2);
            double middleY = bottomRight.Y - (height / 2);
            double eyeHeight = height > width ? (height * 1.5) : width;
            return new XYZ(middleX, middleY, eyeHeight);
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static BoundingBoxXYZ SectionViewExtentsBoundingBox(UIView view)
        {
            if (view == null)
            {
                return new BoundingBoxXYZ();
            }
            BoundingBoxXYZ result = new BoundingBoxXYZ();
            try
            {
                XYZ min = new XYZ(view.GetZoomCorners()[0].X, view.GetZoomCorners()[0].Y, view.GetZoomCorners()[0].Z - 4);
                XYZ max = new XYZ(view.GetZoomCorners()[1].X, view.GetZoomCorners()[1].Y, view.GetZoomCorners()[1].Z + 4);
                result.set_Bounds(0, min);
                result.set_Bounds(1, max);
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                result.Dispose();
                return null;
            }
            return result;
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static BoundingBoxXYZ ViewExtentsBoundingBox(UIView view)
        {
            if (view == null)
            {
                return new BoundingBoxXYZ();
            }
            BoundingBoxXYZ result = new BoundingBoxXYZ();
            XYZ min = new XYZ(view.GetZoomCorners()[0].X, view.GetZoomCorners()[0].Y, view.GetZoomCorners()[0].Z - 4);
            XYZ max = new XYZ(view.GetZoomCorners()[1].X, view.GetZoomCorners()[1].Y, view.GetZoomCorners()[1].Z + 4);
            result.set_Bounds(0, min);
            result.set_Bounds(1, max);
            return result;
        }

        public Result Execute(
                                                    ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (commandData == null)
            {
                return Result.Failed;
            }

            Document doc = commandData.Application.ActiveUIDocument.Document;
            View currentView = doc.ActiveView;

            switch (currentView.ViewType)
            {
                case ViewType.ThreeD:
                    CreatePerspectiveFrom3D(commandData.Application.ActiveUIDocument, currentView as View3D);
                    break;

                case ViewType.FloorPlan:
                    CreatePerspectiveFromPlan(commandData.Application.ActiveUIDocument, currentView);
                    break;

                case ViewType.CeilingPlan:
                    break;

                case ViewType.Section:
                    CreatePerspectiveFromSection(commandData.Application.ActiveUIDocument, currentView);
                    break;

                default:
                    var msg = "Currently cameras can only be created in 3d and Plan views" +
                       Environment.NewLine +
                       "Please create sections/elevations from an isometric view";

                    SCaddinsApp.WindowManager.ShowMessageBox("Oops", msg);
                    break;
            }

            return Result.Succeeded;
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static void CreatePerspectiveFrom3D(UIDocument udoc, View3D view)
        {
            var v = view.GetOrientation();
            using (var t = new Transaction(udoc.Document))
            {
                if (t.Start("Create perspective view") == TransactionStatus.Started)
                {
                    var centreOfScreen = GetMiddleOfActiveViewWindow(ActiveUIView(udoc, view));
                    var np = View3D.CreatePerspective(udoc.Document, Get3DViewFamilyTypes(udoc.Document).First().Id);
                    np.SetOrientation(new ViewOrientation3D(new XYZ(centreOfScreen.X, centreOfScreen.Y, v.EyePosition.Z), v.UpDirection, v.ForwardDirection));
                    t.Commit();
                    np.Dispose();
                }
            }
            v.Dispose();
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static void CreatePerspectiveFromPlan(UIDocument udoc, View planView)
        {
            UIView view = ActiveUIView(udoc, planView);
            XYZ eye = GetMiddleOfActiveViewWindow(view);
            XYZ up = new XYZ(0, 1, 0);
            XYZ forward = new XYZ(0, 0, -1);
            ViewOrientation3D v = new ViewOrientation3D(eye, up, forward);
            using (var t = new Transaction(udoc.Document))
            {
                t.Start("Create perspective view");
                View3D np = View3D.CreatePerspective(udoc.Document, Get3DViewFamilyTypes(udoc.Document).First().Id);
                np.SetOrientation(new ViewOrientation3D(v.EyePosition, v.UpDirection, v.ForwardDirection));
                ApplySectionBoxToView(ViewExtentsBoundingBox(view), np);
                t.Commit();
            }
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static void CreatePerspectiveFromSection(UIDocument udoc, View sectionView)
        {
            UIView view = ActiveUIView(udoc, sectionView);
            XYZ eye = GetMiddleOfActiveViewWindow(view);
            XYZ up = new XYZ(0, 0, 1);
            XYZ forward = new XYZ(0, 0, -1);
            ViewOrientation3D v = new ViewOrientation3D(eye, up, forward);
            using (var t = new Transaction(udoc.Document))
            {
                t.Start("Create perspective view");
                View3D np = View3D.CreatePerspective(udoc.Document, Get3DViewFamilyTypes(udoc.Document).First().Id);
                np.SetOrientation(new ViewOrientation3D(v.EyePosition, v.UpDirection, v.ForwardDirection));
                ApplySectionBoxToView(SectionViewExtentsBoundingBox(view), np);
                t.Commit();
            }
        }

        private static List<ViewFamilyType> Get3DViewFamilyTypes(Document doc)
        {
            List<ViewFamilyType> viewFamilyTypes = new List<ViewFamilyType>();
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfClass(typeof(ViewFamilyType));
                foreach (var element in collector)
                {
                    var viewFamilyType = (ViewFamilyType)element;
                    if (viewFamilyType.ViewFamily == ViewFamily.ThreeDimensional)
                    {
                        viewFamilyTypes.Add(viewFamilyType);
                    }
                }
            }
            return viewFamilyTypes;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
