namespace SCaddins.SolarUtilities.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;

    class DirectSunViewModel : Screen
    {
        private UIDocument uidoc;
        private IList<Reference> faceSelection;
        private IList<Reference> massSelection;

        public DirectSunViewModel(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            massSelection = null;
            faceSelection = null;
            ////NotifyOfPropertyChange(() => SelectionInformation);
        }

        public string SelectionInformation
        {
            get
            {
                int m = massSelection != null ? massSelection.Count : 0;
                int f = faceSelection != null ? faceSelection.Count : 0;
                return string.Format("Masses: {0}, Faces {1}", m, f);
            }
        }

        public void RunAnalysis()
        {
            UV uv = new UV(0.5, 0.5);
            foreach (Reference r in faceSelection) { 
                Face f = (Face)uidoc.Document.GetElement(r).GetGeometryObjectFromReference(r);
                XYZ start = f.Evaluate(uv);
                XYZ end = f.ComputeNormal(uv).Multiply(10000).Add(start);
                Line line = Line.CreateBound(start, end);
                ////Transaction t = new Transaction(uidoc.Document);
                ////t.Start("test");
                ////Plane p = Plane.CreateByNormalAndOrigin(start, f.ComputeNormal(uv));
                ////SketchPlane sp = SketchPlane.Create(uidoc.Document, p);
                ////uidoc.Document.Create.NewModelCurve(line, sp);
                ////t.Commit();
            }
        }

        public void SelectAnalysisFaces()
        {
            faceSelection = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Face, "Select Faces");
            NotifyOfPropertyChange(() => SelectionInformation);
        }

        public void SelectMasses()
        {
            massSelection = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select Masses");
            NotifyOfPropertyChange(() => SelectionInformation);
        }

    }
}
