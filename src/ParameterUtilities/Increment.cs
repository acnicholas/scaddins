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
            if (t != null && t.Message == @"Elements have duplicate 'Number' values.")
            {
                e.OverrideResult((int)TaskDialogResult.Ok);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Because this hack only works this way...")]
        public static void RenumberByPicks(UIDocument uidoc, Document doc, UIApplication app)
        {
            if (uidoc == null || app == null)
            {
                return;
            }

            IList<Reference> refList = new List<Reference>();
            try
            {
                while (true)
                {
                    refList.Add(uidoc.Selection.PickObject(ObjectType.Element, "Select elements in order to be renumbered. ESC when finished."));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            if (refList.Count == 0)
            {
                return;
            }

            using (var t = new Transaction(doc, "Renumber"))
            {
                t.Start();
                var incAmount = IncrementSettings.Default.IncrementValue;
                string startValue;
                string startText;
                var firstRef = refList[0];
                var firstParam = GetParameterForReference(doc, firstRef);
                if (firstParam == null)
                {
                    return;
                }
                else
                {
                    startText = firstParam.AsString();
                    startValue = GetSourceNumberAsString(startText);
                }

                if (int.TryParse(startValue, out var StartValueAsInt))
                {
                    int aggregateInc = IncrementSettings.Default.OffsetValue;
                    foreach (Reference r in refList)
                    {
                        if (r == null)
                        {
                            continue;
                        }
                        Parameter param = GetParameterForReference(doc, r);
                        if (param == null)
                        {
                            continue;
                        }
                        app.DialogBoxShowing += DismissDuplicateQuestion;

                        if (IncrementSettings.Default.UseDestinationSearchPattern)
                        {
                            SetParameterToValue(
                               param,
                               GetDestinationNumberAsString(param.AsString(), IncrementString(startValue, aggregateInc, IncrementSettings.Default.KeepLeadingZeros)));
                        }
                        else
                        {
                            SetParameterToValue(
                                param,
                                GetDestinationNumberAsString(startText, IncrementString(startValue, aggregateInc, IncrementSettings.Default.KeepLeadingZeros)));
                        }

                        aggregateInc += incAmount;
                    }
                }
                t.Commit();
            }
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (commandData == null)
            {
                return Result.Failed;
            }

            UIDocument udoc = commandData.Application.ActiveUIDocument;
            Document doc = udoc.Document;
            UIApplication app = commandData.Application;
            commandData.Application.DialogBoxShowing += DismissDuplicateQuestion;

            IList<ElementId> elems = udoc.Selection.GetElementIds().ToList();

            if (elems.Count == 0)
            {
                RenumberByPicks(udoc, doc, app);
            }
            return Result.Succeeded;
        }

        private static string GetDestinationNumberAsString(string s, string i)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = string.Empty;
            }
            if (IncrementSettings.Default.UseDestinationSearchPattern)
            {
                s = Regex.Replace(s, IncrementSettings.Default.DestinationSearchPattern, IncrementSettings.Default.DestinationReplacePattern);
            }
            else
            {
                s = Regex.Replace(s, IncrementSettings.Default.SourceSearchPattern, IncrementSettings.Default.DestinationReplacePattern);
            }
            return s.Replace("#VAL#", i);
        }

        private static Parameter GetParameterByName(
            Element e,
            string parameterName)
        {
            if (IncrementSettings.Default.UseCustomParameterName)
            {
                return e.LookupParameter(IncrementSettings.Default.CustomParameterName);
            }
            else
            {
                return e.LookupParameter(parameterName);
            }
        }

        private static Parameter GetParameterForReference(Document doc, Reference r)
        {
            Element e = doc.GetElement(r);
            if (e is Grid)
            {
                return GetParameterByName(e, "Name");
            }
            else if (e is Room)
            {
                return GetParameterByName(e, "Number");
            }
            else if (e is Area)
            {
                return GetParameterByName(e, "Number");
            }
            else if (e is FamilyInstance)
            {
                return GetParameterByName(e, "Mark");
            }
            else if (e is Wall)
            {
                return GetParameterByName(e, "Mark");
            }
            else if (e is TextNote)
            {
                return GetParameterByName(e, "Text");
            }
            else if (e is Viewport)
            {
                return GetParameterByName(e, "Detail Number");
            }
            else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Error", "Unsupported element");
                return null;
            }
        }

        private static string GetSourceNumberAsString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = string.Empty;
            }
            return Regex.Replace(s, IncrementSettings.Default.SourceSearchPattern, IncrementSettings.Default.SourceReplacePattern);
        }

        private static string IncrementString(string startNumber, int incVal, bool keepLeadingZeros)
        {
            var matchLength = startNumber.Length;
            if (!string.IsNullOrEmpty(startNumber) && int.TryParse(startNumber, out int n))
            {
                var i = n + incVal;
                var pad = string.Empty;
                if (i > 0)
                {
                    for (var j = (int)Math.Floor(Math.Log10(i)); j < (matchLength - 1); j++)
                    {
                        pad += "0";
                    }
                }
                return keepLeadingZeros ? pad + i : i.ToString();
            }
            return startNumber;
        }

        private static void SetParameterToValue(Parameter p, string s)
        {
            switch (p.StorageType)
            {
                case StorageType.Integer:
                    p.Set(int.Parse(s));
                    break;
                case StorageType.String:
                    if (!string.IsNullOrEmpty(s))
                    {
                        p.Set(s);
                    }
                    break;
                case StorageType.None:
                    break;
                case StorageType.Double:
                    break;
                case StorageType.ElementId:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
