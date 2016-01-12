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
                    CreatePerspectiveFrom3D(doc, currentView as View3D);
                    break;
                case ViewType.FloorPlan:
                    CreatePerspectiveFromPlan(commandData.Application.ActiveUIDocument, currentView);
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
        
        public static XYZ GetMiddleOfActiveViewWindow(UIDocument udoc, View planView)
        {
            foreach (UIView view in udoc.GetOpenUIViews()) {
                View v = (View)udoc.Document.GetElement(view.ViewId);
                //var viewName = v.Name + " - " + System.IO.Path.GetFileName(udoc.Document.PathName);
                if(v.Name == planView.Name) {
                    XYZ topLeft = view.GetZoomCorners()[0];
                    XYZ bottomRight = view.GetZoomCorners()[1];
                    double middleX = bottomRight.X - (bottomRight.X - topLeft.X)/2;
                    double middleY = bottomRight.Y - (bottomRight.Y - topLeft.Y)/2;                    
                    return new XYZ(middleX,middleY,view.GetZoomCorners()[0].Z + 30);
                }
            }
            return new XYZ();
        }    
                
        private static void CreatePerspectiveFromPlan(UIDocument udoc, View planView)
        {
            XYZ eye = GetMiddleOfActiveViewWindow(udoc, planView);
            TaskDialog.Show("test", eye.X + "-" + eye.Y + "-" + eye.Z);
            XYZ up = new XYZ(0,1,0);
            XYZ forward = new XYZ(0,0,-1);
            ViewOrientation3D v = new ViewOrientation3D(eye, up, forward);
            var t = new Transaction(udoc.Document);
            t.Start("Create perspective view");
            View3D np = View3D.CreatePerspective(udoc.Document, Get3DViewFamilyTypes(udoc.Document).First().Id);
            np.SetOrientation(new ViewOrientation3D(v.EyePosition, v.UpDirection, v.ForwardDirection));
            t.Commit();
        }
    
        private static void CreatePerspectiveFrom3D(Document doc, View3D view)
        {
            ViewOrientation3D v = view.GetOrientation();
            var t = new Transaction(doc);
            t.Start("Create perspective view");
            View3D np = View3D.CreatePerspective(doc, Get3DViewFamilyTypes(doc).First().Id);
            np.SetOrientation(new ViewOrientation3D(v.EyePosition, v.UpDirection, v.ForwardDirection));
            t.Commit();
        }   
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
