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
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class DirectSunCommand : IExternalCommand
    {
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
                RunAnalysis(vm.FaceSelection, vm.MassSelection, 10, udoc);
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }


        public static void RunAnalysis(IList<Reference> faceSelection, IList<Reference> massSelection, int divisions, UIDocument uidoc)
        {
            UV uv = new UV(0.5, 0.5);
            if (faceSelection == null) {
                return;
            }
            foreach (Reference r in faceSelection) {
                Face f = (Face)uidoc.Document.GetElement(r).GetGeometryObjectFromReference(r);
                XYZ start = f.Evaluate(uv);
                XYZ end = new XYZ(start.X+10000, start.Y+10000, start.Z);
                Line line = Line.CreateBound(start, end);
                Transaction t = new Transaction(uidoc.Document);
                t.Start("test");
                Plane p = Plane.CreateByNormalAndOrigin(f.ComputeNormal(uv), start);
                SketchPlane sp = SketchPlane.Create(uidoc.Document, p);
                uidoc.Document.Create.NewModelCurve(line, sp);
                t.Commit();
            }
        }
    }
}