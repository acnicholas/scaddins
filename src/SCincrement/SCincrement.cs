/*
 * This work is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Australia License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/3.0/au/
 * or send a letter to Creative Commons, 444 Castro Street, Suite 900, Mountain View, California, 94041, USA.
 */

/*
 * This code is an adaptation of the code
 * "Quick way to renumber doors, grids, and family instances"
 * available here:
 * http://boostyourbim.wordpress.com/2013/01/17/quick-way-to-renumber-doors-grids-and-levels/
 * Available under a Creative Commons Attribution-Noncommercial-Share Alike license. Copyright © 2013  Harry Mattison.
 */

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;

namespace SCaddins.SCincrement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {

        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
          ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            UIDocument udoc = commandData.Application.ActiveUIDocument;
            Document doc = udoc.Document;

            renumberByPicks(udoc, doc);

            return Autodesk.Revit.UI.Result.Succeeded;
        }

        /*
        * Code below here is from:
        * http://boostyourbim.wordpress.com/2013/01/17/quick-way-to-renumber-doors-grids-and-levels/
        * Available under a Creative Commons Attribution-Noncommercial-Share Alike license. Copyright © 2003  Harry Mattison.
        */
        public void renumberByPicks(UIDocument uidoc, Document doc)
        {
            // list that will contain references to the selected elements
            IList<Reference> refList = new List<Reference>();
            try {
                // create a loop to repeatedly prompt the user to select an element
                while (true)
                    refList.Add(uidoc.Selection.PickObject(ObjectType.Element, "Select elements in order to be renumbered. ESC when finished."));
            }
                // When the user hits ESC Revit will throw an OperationCanceledException which will get them out of the while loop
            catch { }

            using (Transaction t = new Transaction(doc, "Renumber")) {
                t.Start();
                // need to avoid encountering the error "The name entered is already in use. Enter a unique name."
                // for example, if there is already a grid 2 we can't renumber some other grid to 2
                // therefore, first renumber everny element to a temporary name then to the real one
                int ctr = 1;
                int startValue = 0;
                foreach (Reference r in refList) {
                    Parameter p = getParameterForReference(doc, r);

                    // get the value of the first element to use as the start value for the renumbering in the next loop
                    if (ctr == 1)
                        startValue = Convert.ToInt16(p.AsString());

                    setParameterToValue(p, ctr + 12345); // hope this # is unused (could use Failure API to make this more robust
                    ctr++;
                }

                ctr = startValue;
                foreach (Reference r in refList) {
                    Parameter p = getParameterForReference(doc, r);
                    this.setParameterToValue(p, ctr);
                    ctr++;
                }
                t.Commit();
            }

        }
        private Parameter getParameterForReference(Document doc, Reference r)
        {
            Element e = doc.GetElement(r);
            Parameter p = null;
            if (e is Grid)
                p = e.get_Parameter("Name");
            else if (e is Room)
                p = e.get_Parameter("Number");
            else if (e is FamilyInstance)
                p = e.get_Parameter("Mark");
            else if (e is AnnotationSymbol){
                TaskDialog.Show("Error", "Unsupported element");
                return null;
            }
            else {
                TaskDialog.Show("Error", "Unsupported element");
                return null;
            }
            return p;
        }
        private void setParameterToValue(Parameter p, int i)
        {
            if (p.StorageType == StorageType.Integer)
                p.Set(i);
            else if (p.StorageType == StorageType.String)
                p.Set(i.ToString());
        }

    }

}
