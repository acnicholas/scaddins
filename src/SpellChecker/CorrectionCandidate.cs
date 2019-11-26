namespace SCaddins.SpellChecker
{
    using Autodesk.Revit.DB;
    using NHunspell;
    using System.Collections;
    using System.Text.RegularExpressions;

    public class CorrectionCandidate : IEnumerator
    {
        private Parameter parameter;
        private Hunspell hunspell;
        private int currentIndex;
        private string[] originalWords;
        private Regex rgx;

        public CorrectionCandidate(Parameter parameter, Hunspell hunspell)
        {
            this.parameter = parameter;
            this.hunspell = hunspell;
            OriginalText = parameter.AsString();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\\', '/', '(', ')' };
            if (!string.IsNullOrEmpty(OriginalText))
            {
                originalWords = OriginalText.Split(delimiterChars);
                NewText = OriginalText;
            }
            else
            {
                originalWords = null;
                NewText = OriginalText;
            }
            currentIndex = -1;
            rgx = new Regex(@"^.*[\d]+.*$");
        }

        public string OriginalText
        {
            get; private set;
        }

        public string NewText
        {
            get; private set;
        }

        public string TypeString
        {
            get
            {
                //SCaddinsApp.WindowManager.ShowMessageBox(parameter.Element.GetType().ToString());
                return parameter.Element.GetType().ToString();
                
            }
        }

        public void ReplaceCurrent(string word)
        {
            ReplaceFirst(NewText, (string)Current, word);
        }

        private string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0) {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public bool IsModified => NewText != OriginalText;

        public object Current => originalWords[currentIndex].Trim();

        public bool MoveNext()
        {
            while (originalWords != null && currentIndex < (originalWords.Length - 1)) {
                currentIndex++;

                //continue if a number is found...
                if (rgx.IsMatch(originalWords[currentIndex])) continue;

                if (!hunspell.Spell(originalWords[currentIndex].Trim())) {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            currentIndex = -1;
        }
    }
}
