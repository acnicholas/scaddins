// (C) Copyright 2014 by Andrew Nicholas
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

namespace SCaddins.SCam
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            View currentView = doc.ActiveView;
            
            switch (currentView.ViewType) {
                case ViewType.ThreeD:
                    CreatePerspectiveFrom3D(commandData.Application.ActiveUIDocument, currentView as View3D);
                    break;
               case ViewType.FloorPlan:
                    CreatePerspectiveFromPlan(commandData.Application.ActiveUIDocument, currentView);
                    break;
                default:
                    TaskDialog td = new TaskDialog("SCam - SC Camera Tool");
                    td.MainInstruction = "Oops!";
                    td.MainContent = "Currently cameras can only be created in 3d and Plan views" +
                        System.Environment.NewLine +
                        "Please create sections/elevations from an isometric view";
                    td.Show();
                    break;
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
        
        private static IEnumerable<ViewFamilyType> Get3DViewFamilyTypes(Document doc)
        {
            IEnumerable<ViewFamilyType> viewFamilyTypes
                = from elem in new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                let type = elem as ViewFamilyType
                where type.ViewFamily == ViewFamily.ThreeDimensional
                select type;   
            return viewFamilyTypes;
        }
        
        public static UIView ActiveUIView(UIDocument udoc, View planView)
        {
                foreach (UIView view in udoc.GetOpenUIViews()) {
                View v = (View)udoc.Document.GetElement(view.ViewId);
                if(v.Name == planView.Name) {
                    return view;
                }
            }
            return null;   
        }
        
        public static XYZ GetMiddleOfActiveViewWindow(UIView view)
        { 
            if(view == null) {
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
        
        public static BoundingBoxXYZ ViewExtentsBoundingBox(UIView view)
        {
            if (view == null) return new BoundingBoxXYZ();
            BoundingBoxXYZ result = new BoundingBoxXYZ();
            XYZ min = new XYZ(view.GetZoomCorners()[0].X, view.GetZoomCorners()[0].Y, view.GetZoomCorners()[0].Z - 4);
            XYZ max = new XYZ(view.GetZoomCorners()[1].X, view.GetZoomCorners()[1].Y, view.GetZoomCorners()[1].Z + 4);
            result.set_Bounds(0, min);
            result.set_Bounds(1, max);
            return result;
        }

        public static void ApplySectionBoxToView(BoundingBoxXYZ bounds, View3D view)
        {
            view.SetSectionBox(bounds);
        }
                
        private static void CreatePerspectiveFromPlan(UIDocument udoc, View planView)
        {
            UIView view = ActiveUIView(udoc, planView);
            XYZ eye = GetMiddleOfActiveViewWindow(view);            
            XYZ up = new XYZ(0,1,0);
            XYZ forward = new XYZ(0,0,-1);
            ViewOrientation3D v = new ViewOrientation3D(eye, up, forward);
            var t = new Transaction(udoc.Document);
            t.Start("Create perspective view");
            View3D np = View3D.CreatePerspective(udoc.Document, Get3DViewFamilyTypes(udoc.Document).First().Id);
            np.SetOrientation(new ViewOrientation3D(v.EyePosition, v.UpDirection, v.ForwardDirection));
            ApplySectionBoxToView(ViewExtentsBoundingBox(view), np);
            t.Commit();
        }
    
        private static void CreatePerspectiveFrom3D(UIDocument udoc, View3D view)
        {
            ViewOrientation3D v = view.GetOrientation();
            var t = new Transaction(udoc.Document);
            t.Start("Create perspective view");
            XYZ centreOfScreen = GetMiddleOfActiveViewWindow(ActiveUIView(udoc, (View)view));
            View3D np = View3D.CreatePerspective(udoc.Document, Get3DViewFamilyTypes(udoc.Document).First().Id);
            np.SetOrientation(new ViewOrientation3D(new XYZ(centreOfScreen.X, centreOfScreen.Y, v.EyePosition.Z), v.UpDirection, v.ForwardDirection));
            t.Commit();
        }   
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
