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

    public class SCam : IExternalCommand
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

            View3D v3 = currentView as View3D;
            ViewOrientation3D vo = v3.GetOrientation();

            Transaction t = new Transaction(doc);
            t.Start("Create perspective view");
            
            View3D np = View3D.CreatePerspective(doc, viewFamilyTypes.First().Id);
            np.SetOrientation(new ViewOrientation3D(vo.EyePosition,vo.UpDirection,vo.ForwardDirection));
            
            t.Commit();

            return Autodesk.Revit.UI.Result.Succeeded;   
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
