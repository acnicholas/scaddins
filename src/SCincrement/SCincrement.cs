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

namespace SCaddins.SCincrement
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Selection;
    using Autodesk.Revit.UI.Events;
    
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
            UIDocument udoc = commandData.Application.ActiveUIDocument;
            Document doc = udoc.Document;
            UIApplication app = commandData.Application;
            commandData.Application.DialogBoxShowing += DismissDuplicateQuestion;
            this.RenumberByPicks(udoc, doc, app);
            return Autodesk.Revit.UI.Result.Succeeded;
        }
        
        public void DismissDuplicateQuestion(object o, DialogBoxShowingEventArgs e)
        {
            //TaskDialog.Show("test","test");
            var t = e as MessageBoxShowingEventArgs;
            if (t != null && t.Message == @"Elements have duplicate 'Number' values.") {
                e.OverrideResult((int)TaskDialogResult.Ok);
            }
        }

        /*
        * Some code below here is from:
        * http://boostyourbim.wordpress.com/2013/01/17/quick-way-to-renumber-doors-grids-and-levels/
        * Available under a Creative Commons Attribution-Noncommercial-Share Alike license. Copyright © 2003  Harry Mattison.
        */
        public void RenumberByPicks(UIDocument uidoc, Document doc, UIApplication app)
        {
            // list that will contain references to the selected elements
            IList<Reference> refList = new List<Reference>();
            try {
                // create a loop to repeatedly prompt the user to select an element
                // When the user hits ESC Revit will throw an OperationCanceledException which will get them out of the while loop
                while (true) {
                    refList.Add(uidoc.Selection.PickObject(ObjectType.Element, "Select elements in order to be renumbered. ESC when finished."));
                }
                
                //pick 2 items
                //refList.Add(uidoc.Selection.PickObject(ObjectType.Element, "Pick first rooom."));
                //refList.Add(uidoc.Selection.PickObject(ObjectType.Element, "Pick second room."));
            }
            catch
            {
            }
            
            int incVal = -1;
                   
            using (Transaction t = new Transaction(doc, "Renumber")) {
                t.Start();      
                int ctr = 1;
                int startValue = 0;
                string leftPad = string.Empty;
                foreach (Reference r in refList) {
                    Parameter p = this.GetParameterForReference(doc, r);
                    if (p == null) {
                        return;
                    }

                    // get the value of the first element to use as the start value for the renumbering in the next loop
                    if (ctr == 1) {
                        string s = p.AsString();
                        startValue = Convert.ToInt16(s);
                            //TaskDialog.Show("DEBUG",startValue.ToString());
                        }
                    }

                    this.SetParameterToValue(p, ctr + 12345, string.Empty); // hope this # is unused (could use Failure API to make this more robust
                    ctr++;
                }

                ctr = startValue;
                foreach (Reference r in refList) {
                    Parameter p = this.GetParameterForReference(doc, r);
                                app.DialogBoxShowing += DismissDuplicateQuestion;
                    this.SetParameterToValue(p, ctr, leftPad);
                    ctr++;
                    ctr += incVal;
                }
                t.Commit();
            }
        }
    
        private4 stin
        
        private Parameter GetParameterForReference(Document doc, Reference r)
        {
            Element e = doc.GetElement(r);
            
            // TaskDialog.Show("Error", e.GetType().ToString());
            Parameter p = null;
            if (e is Grid) {
                #if REVIT2014
                p = e.get_Parameter("Name");
                #else
                p = e.LookupParameter("Name");
                #endif
            } else if (e is Room) {
                #if REVIT2014
                p = e.get_Parameter("Number");
                #else
                p = e.LookupParameter("Number");
                #endif
            } else if (e is FamilyInstance) {
                #if REVIT2014
                p = e.get_Parameter("Mark");
                #else
                p = e.LookupParameter("Mark");
                #endif
            } else if (e is TextNote) {
                #if REVIT2014
                p = e.get_Parameter("Text");
                TaskDialog.Show("test", p.ToString());
                #else
                p = e.LookupParameter("Text");
                #endif
                return null;
            } else {
                TaskDialog.Show("Error", "Unsupported element");
                return null;
            }
            return p;
        }
        
        private void SetParameterToValue(Parameter p, int i, string pad)
        {
            if (p.StorageType == StorageType.Integer) {
                p.Set(i);
            } else if (p.StorageType == StorageType.String) {
                p.Set(pad + i.ToString());
            }
        }
    }
}
