
namespace SCaddins.SpellChecker
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using NHunspell;
    using SCaddins;
    using SCaddins.RenameUtilities;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;

            string[] toStest = { "Views", "Rooms", "Sheets", "Annotation" };

            foreach (var t in toStest)
            {

            var p = RenameUtilities.RenameManager.GetParametersByCategoryName(t, document);

                using (Hunspell hunspell = new Hunspell(
                    @"C:\Code\cs\scaddins\etc\en_AU.aff",
                    @"C:\Code\cs\scaddins\etc\en_AU.dic"))
                {
                    //add some arch specific words
                    hunspell.Add("approver");
                    hunspell.Add(@"&");
                    hunspell.Add(@"-");

                    foreach (var pp in p)
                    {

                        var candidates = GetTxtValuesByParameter(pp.Parameter, pp.Category, pp.Type, document);
                        foreach (var candidate in candidates)
                        {
                            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\\', '/' };
                            var strings = candidate.OldValue.Split(delimiterChars);
                            bool correctSpelling = true;
                            foreach (var s in strings)
                            {
                                //Dont check if number and some characters
                                Regex rgx = new Regex(@"^.*[\d]+.*$");
                                if (rgx.IsMatch(s)) continue;

                                if (!hunspell.Spell(s))
                                {
                                    var suggestionsList = hunspell.Suggest(s);
                                    var suggestions = string.Empty;
                                    foreach (var suggestion in suggestionsList)
                                    {
                                        suggestions += suggestion + Environment.NewLine;
                                    }
                                    SCaddinsApp.WindowManager.ShowMessageBox("Word not found / Incorrect spelling of: " + s +
                                        Environment.NewLine +
                                        suggestions);

                                }
                            }
                        }

                        //var suggestions = dictionary.Suggest("teh");
                        //bool ok = dictionary.Check("the");
                    }
                }
            }

            return Result.Succeeded;
        }



        private List<RenameCandidate> GetTxtValuesByParameter(Parameter parameter, BuiltInCategory category, Type t, Document doc)
        {
            var candidates = new List<RenameCandidate>(); 

            //if (category == BuiltInCategory.OST_TextNotes || category == BuiltInCategory.OST_IOSModelGroups)
            //{
            //    GetTextNoteValues(category);
            //    return;
            //}
            //renameCandidates.Clear();
            //FilteredElementCollector collector;
            //if (elements == null)
            //{
            var collector = new FilteredElementCollector(doc);
            //}
            //else
            //{
            //    collector = new FilteredElementCollector(doc, elements);
            //}
            if (t != null)
            {
                collector.OfClass(t);
            }
            else
            {
                collector.OfCategory(category);
            }
            foreach (Element element in collector)
            {
                var p = element.GetParameters(parameter.Definition.Name);
                if (p.Count > 0)
                {
                    var rc = new RenameCandidate(p[0]);
                    if (!string.IsNullOrEmpty(rc.OldValue))
                    {
                        rc.NewValue = rc.OldValue;
                        candidates.Add(rc);
                    }
                }
            }
            return candidates;
        }
    }
}
