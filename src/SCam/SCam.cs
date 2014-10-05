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

            IEnumerable<ViewFamilyType> viewFamilyTypes
                = from elem in new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                let type = elem as ViewFamilyType
                where type.ViewFamily == ViewFamily.ThreeDimensional
                select type;

            var v3 = currentView as View3D;
            ViewOrientation3D vo = v3.GetOrientation();

            var t = new Transaction(doc);
            t.Start("Create perspective view");

            View3D np = View3D.CreatePerspective(doc, viewFamilyTypes.First().Id);
            np.SetOrientation(new ViewOrientation3D(vo.EyePosition, vo.UpDirection, vo.ForwardDirection));

            t.Commit();

            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
