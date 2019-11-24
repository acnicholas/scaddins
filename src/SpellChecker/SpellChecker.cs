using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using NHunspell;
using Caliburn.Micro;
using System.Text.RegularExpressions;
using System.Collections;

namespace SCaddins.SpellChecker
{
    public class SpellChecker
    {
        private Document document;
        private Hunspell hunspell;
        private List<CorrectionCandidate> allTextParameters;
        private int currentIndex;

        public SpellChecker(Document document)
        {
            this.document = document;

            hunspell = new Hunspell(
                            @"C:\code\scaddins\etc\en_AU.aff",
                            @"C:\code\scaddins\etc\en_AU.dic");

            //add some arch specific words
            hunspell.Add("approver");
            hunspell.Add(@"&");
            hunspell.Add(@"-");

            allTextParameters = GetAllTextParameters(document);
            currentIndex = -1;
        }

        ~SpellChecker()
        {
            hunspell.Dispose();
        }

        public void AddToIgnoreList(string ignore)
        {
            hunspell.Add(ignore);
        }

        public CorrectionCandidate GetNextSpellingError()
        {
            while (currentIndex < allTextParameters.Count) {
                currentIndex++;
                if (HasIncorrectSpelling(allTextParameters[currentIndex])) {  
                    return allTextParameters[currentIndex];
                }
                
            }
            return null;
        }

        // Fixme. check for multiple errors.
        private bool HasIncorrectSpelling(CorrectionCandidate candidate)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\\', '/' };
            var strings = candidate.OldValue.Split(delimiterChars);
            foreach (var s in strings) {
                //Dont check if number and some characters
                Regex rgx = new Regex(@"^.*[\d]+.*$");
                if (rgx.IsMatch(s)) continue;

                if (!hunspell.Spell(s.Trim())) {
                    var suggestionsList = hunspell.Suggest(s.Trim());
                    candidate.Suggestions = new List<string>(suggestionsList);
                    return true;
                }
            }
            return false;
        }

        private List<CorrectionCandidate> GetAllTextParameters(Document doc)
        {
            var candidates = new List<CorrectionCandidate>();
            var collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();

            foreach (Element element in collector) {
                var parameterSet = element.Parameters;
                foreach (var parameter in parameterSet) {

                    if (parameter is Autodesk.Revit.DB.Parameter) {
                        var p = (Autodesk.Revit.DB.Parameter)parameter;
                        if (p.StorageType == StorageType.String) {
                            var rc = new CorrectionCandidate(p);
                            if (!string.IsNullOrEmpty(rc.OldValue)) {
                                rc.NewValue = rc.OldValue;
                                candidates.Add(rc);
                            }
                        }
                    }
                }
            }
            return candidates;
        }
    }
}
