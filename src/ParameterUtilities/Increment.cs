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

namespace SCaddins.ParameterUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Events;
    using Autodesk.Revit.UI.Selection;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public static void DismissDuplicateQuestion(object value, DialogBoxShowingEventArgs e)
        {
            var t = e as MessageBoxShowingEventArgs;
            if (t != null && t.Message == @"Elements have duplicate 'Number' values.") {
                e.OverrideResult((int)TaskDialogResult.Ok);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Because this hack only works this way...")]  
        public static void RenumberByPicks(UIDocument uidoc, Document doc, UIApplication app)
        {
            if (uidoc == null || app == null) {
                return;
            }

            IList<Reference> refList = new List<Reference>();
            try {
                while (true) {
                    refList.Add(uidoc.Selection.PickObject(ObjectType.Element, "Select elements in order to be renumbered. ESC when finished."));
                }
            } catch {
            }

            if (refList.Count == 0) {
                return;
            }

            using (var t = new Transaction(doc, "Renumber")) {
                t.Start();
                int ctr = 1;
                int startValue = 0;
                foreach (Reference r in refList) {
                    Parameter p = GetParameterForReference(doc, r);
                    if (p == null) {
                        return;
                    }

                    if (ctr == 1) {
                        if (p.StorageType == StorageType.Integer) {
                            startValue = p.AsInteger();
                        } else if (p.StorageType == StorageType.String) {
                            string s = p.AsString();
                            startValue = Convert.ToInt32(GetSourceNumberAsString(s), CultureInfo.InvariantCulture);
                        }
                    }

                    if (p.StorageType == StorageType.Integer) {
                        SetParameterToValue(p, ctr + 12345); // hope this # is unused (could use Failure API to make this more robust
                    } else if (p.StorageType == StorageType.String) {
                        var ns = p.AsString() + @"zz" + (ctr + 12345).ToString(CultureInfo.InvariantCulture);
                        p.Set(ns); 
                    }
                    ctr++;
                }

                ctr = startValue;
                foreach (Reference r in refList) {
                    Parameter p = GetParameterForReference(doc, r);
                    app.DialogBoxShowing += DismissDuplicateQuestion;
                    SetParameterToValue(p, ctr);
                    ctr++;
                }
                t.Commit();
            }
        }

        public static void RenumberBySpline(ElementId id, Document doc)
        {
            if (doc != null) {
                using (FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id)) {
                    collector.OfCategory(BuiltInCategory.OST_Rooms);
                    Element spline = doc.GetElement(id);
                    if (spline is CurveElement) {
                        CurveElement ce = spline as CurveElement;
                        foreach (Element e in collector) {
                            Room room = e as Room;
                            if (ce.CurveElementType == CurveElementType.ModelCurve) {
                                // IntersectionResultArray results;
                                // SetComparisonResult result = ce.GeometryCurve.Intersect(room.Geometry, out results );
                            }
                        }
                    }
                }
            }
        }
        
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (commandData == null) {
                return Result.Failed;
            }

            UIDocument udoc = commandData.Application.ActiveUIDocument;
            Document doc = udoc.Document;
            UIApplication app = commandData.Application;
            commandData.Application.DialogBoxShowing += DismissDuplicateQuestion;
            
            IList<ElementId> elems = udoc.Selection.GetElementIds().ToList<ElementId>();
            
            if (elems.Count == 0) {
                RenumberByPicks(udoc, doc, app);
            } else {
                RenumberBySpline(elems[0], doc);
            }
            return Result.Succeeded;
        }

        private static string GetSourceNumberAsString(string s)
        {
            return Regex.Replace(s, SCincrementSettings.Default.SourceSearchPattern, SCincrementSettings.Default.SourceReplacePattern);  
        }

        private static string GetDestinationNumberAsString(string s, int i)
        { 
            s = Regex.Replace(s, @"(^.*)(zz.*$)", @"$1");
            s = Regex.Replace(s, SCincrementSettings.Default.DestinationSearchPattern, SCincrementSettings.Default.DestinationReplacePattern);
            return s.Replace("#VAL#", i.ToString(CultureInfo.InvariantCulture));
        }

        private static Parameter GetParameterByName(
            Element e,
            string parameterName)
        {
            return e.LookupParameter(parameterName);
        }

        private static Parameter GetParameterForReference(Document doc, Reference r)
        {
            Element e = doc.GetElement(r);
            if (e is Grid) {
                return GetParameterByName(e, "Name");
            } else if (e is Room) {
                return GetParameterByName(e, "Number");
            } else if (e is FamilyInstance) {
                if (SCincrementSettings.Default.UseCustomParameterName) {
                    return GetParameterByName(e, SCincrementSettings.Default.CustomParameterName);    
                }
                return GetParameterByName(e, "Mark");
            } else if (e is TextNote) {
                return GetParameterByName(e, "Text");
            } else {
                SCaddinsApp.WindowManager.ShowMessageBox("Error", "Unsupported element");
                return null;
            }
        }

        private static void SetParameterToValue(Parameter p, int i)
        {
            if (p.StorageType == StorageType.Integer) {
                p.Set(i);
            } else if (p.StorageType == StorageType.String) {
                string s = p.AsString();
                if (string.IsNullOrEmpty(s)) {
                    s = "0";
                }
                p.Set(GetDestinationNumberAsString(p.AsString(), i));
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
