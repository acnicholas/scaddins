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
                            @"C:\Code\cs\scaddins\etc\en_AU.aff",
                            @"C:\Code\cs\scaddins\etc\en_AU.dic");

            //add some arch specific words
            hunspell.Add("approver");
            hunspell.Add(@"&");
            hunspell.Add(@"-");
            hunspell.Add(@"Autodesk");

            allTextParameters = GetAllTextParameters(document);
            currentIndex = 0;
        }

        ~SpellChecker()
        {
            hunspell.Dispose();
        }

        public void AddToIgnoreList(string ignore)
        {
            hunspell.Add(ignore);
        }

        /// <summary>
        /// Get the next spelling error.
        /// May be another word in the current candiate so only step candidates once the current one is complete.
        /// </summary>
        /// <returns></returns>
        public string GetNextSpellingError()
        {
            while (currentIndex < allTextParameters.Count) {

                if (allTextParameters[currentIndex].GetNextError(out string nextError, hunspell)) {
                    return nextError;
                } else {
                    currentIndex++;
                }
                
            }
            return string.Empty;
        }

        public List<string> GetSuggestions(string word)
        {
            return hunspell.Suggest(word);
        }

        // Fixme. check for multiple errors.
        //private bool HasIncorrectSpelling(CorrectionCandidate candidate)
        //{
        //    var testString = candidate.GetNextError();
        //    if (testString == string.Empty)
        //    {
        //        return false;
        //    }


        //    foreach (var s in strings) {
        //        //Dont check if number and some characters
        //        Regex rgx = new Regex(@"^.*[\d]+.*$");
        //        if (rgx.IsMatch(s)) continue;

        //        if (!hunspell.Spell(s.Trim())) {
        //            var suggestionsList = hunspell.Suggest(s.Trim());
        //            candidate.Suggestions = new List<string>(suggestionsList);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        /// <summary>
        /// Get all user modifiable parameters in the revit doc.
        /// Only get parameters of string storage types, as there's not much point spell cheking numbers.
        /// 
        /// </summary>
        /// <param name="doc">Revit doc to spell check</param>
        /// <returns>parmaeters</returns>
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
                            if (!string.IsNullOrEmpty(rc.OriginalText)) {
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
