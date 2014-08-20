using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace SCaddins.SCulcase
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
          ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIApplication application = commandData.Application;
            UIDocument document = application.ActiveUIDocument;
            ElementSet elems = document.Selection.Elements;

            using (TransactionGroup t = new TransactionGroup(doc, "SCulcase")) {
                      t.Start();
                      if (elems.Size == 0) {
                          SCulcaseMainForm form = new SCulcaseMainForm(doc);
                      } else {
                          SCulcase.ConvertSelection(SCulcase.ConversionMode.UPPER_CASE, ref doc, ref elems);
                      }
                      t.Commit();
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}
