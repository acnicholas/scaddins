namespace SCaddins.SpellChecker
{
    using Autodesk.Revit.DB;
    using NHunspell;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class CorrectionCandidate
    {
        private Parameter parameter;

        public CorrectionCandidate(Parameter parameter)
        {
            this.parameter = parameter;
            OriginalText = parameter.AsString();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\\', '/', '(', ')' };
            //OriginalWords = new List<string>(OriginalText.Split(delimiterChars));
            NewText = OriginalText;
        }

        public string OriginalText
        {
            get; private set;
        }

        public string NewText
        {
            get; set;
        }

        public List<string> OriginalWords
        {
            get; set;
        }

        public bool AllWordsChecked
        {
            get; set;
        }


        public int CurrentWordIndex
        {
            get; set;
        }

        public bool ValueChanged
        {
            get
            {
                return NewText != OriginalText;
            }
        }


        private int NumberOfWords
        {
            get
            {
                return OriginalWords != null ? OriginalWords.Count : -1;
            }
        }


        //public List<string> CurrentSuggestions {
        //   get; set;
        //}

        /// <summary>
        /// Get the next error in this string.
        /// </summary>
        /// <returns>The Next error, if any are found, or string.Empty if no errors are found</returns>
        public bool GetNextError(out string nextError, Hunspell hunspell)
        {
            nextError = string.Empty;
            //return false;

            //for (int i CurrentWordIndex =  in strings) { }
            //    Regex rgx = new Regex(@"^.*[\d]+.*$");
            //        if (rgx.IsMatch(s)) continue;

            //        if (!hunspell.Spell(s.Trim())) {
            //            var suggestionsList = hunspell.Suggest(s.Trim());
            //            candidate.Suggestions = new List<string>(suggestionsList);
            //            return true;
            //        }
             //}

             return false;
        }


    }
}
