namespace SCaddins.SCulcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIApplication application = commandData.Application;
            UIDocument document = application.ActiveUIDocument;
            #if REVIT2014
            ElementSet eset = document.Selection.Elements;
            IList<ElementId> elems = new List<ElementId>();
            foreach (Element e in eset) {
                elems.Add(e.Id);
            }         
            #else
            IList<ElementId> elems = document.Selection.GetElementIds().ToList<ElementId>();
            #endif
            using (TransactionGroup t = new TransactionGroup(doc, "SCulcase")) {
                      t.Start();
                      if (elems.Count == 0) {
                          SCulcaseMainForm form = new SCulcaseMainForm(doc);
                      } else {
                          SCulcase.ConvertSelection(SCulcase.ConversionMode.UPPER_CASE, ref doc, elems);
                      }
                      t.Commit();
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}
