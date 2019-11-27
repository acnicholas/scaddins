namespace SCaddins.SpellChecker
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using NHunspell;
    using System.Collections;

    public class SpellChecker : IEnumerator
    {
        private Document document;
        private Hunspell hunspell;
        private List<CorrectionCandidate> allTextParameters;
        private int currentIndex;
        private CorrectionCandidate current;

        public SpellChecker(Document document)
        {
            this.document = document;

            hunspell = new Hunspell(
                            @"C:\Code\cs\scaddins\etc\en_AU.aff",
                            @"C:\Code\\cs\scaddins\etc\en_AU.dic");

            //add some arch specific words
            hunspell.Add("approver");
            hunspell.Add(@"&");
            hunspell.Add(@"-");
            hunspell.Add(@"Autodesk");

            allTextParameters = GetAllTextParameters(document);
            currentIndex = -1;
        }

        ~SpellChecker()
        {
            hunspell.Dispose();
        }

        /// <summary>
        /// Return the current CorrectionCandidate object
        /// </summary>
        public object Current => allTextParameters[currentIndex];

        /// <summary>
        /// Returns the curent CorrectionCandiate
        /// </summary>
        public CorrectionCandidate CurrentCandidate => (CorrectionCandidate)Current;

        /// <summary>
        /// Returns the current unknown word.
        /// </summary>
        public string CurrentUnknownWord => CurrentCandidate.Current as string;

        public string CurrentElementType => CurrentCandidate.TypeString;

        /// <summary>
        /// Ingnore all future instances of the CurrentUnknownWord
        /// </summary>
        public void IgnoreAll()
        {
            hunspell.Add(CurrentUnknownWord);
        }

        /// <summary>
        /// get spelling suggestions for the current CorrectionCandidate
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentSuggestions()
        {
            if (currentIndex < 0) return new List<string>();
            if (hunspell != null && allTextParameters.Count > 0 && currentIndex < allTextParameters.Count)
            {
                return hunspell.Suggest(allTextParameters[currentIndex].Current as string);
            }
            return new List<string>();
        }

        public bool MoveNext()
        {
            if (allTextParameters == null || allTextParameters.Count <= 0) return false;
            while (currentIndex < allTextParameters.Count) {
                if (currentIndex == -1)
                {
                    currentIndex = 0;
                }
                if (allTextParameters[currentIndex].MoveNext()) {
                    return true;
                }
                currentIndex++;
            }
            return false;
        }

        public void Reset()
        {
            currentIndex = -1;
        }

        public void CommitSpellingChangesToModel()
        {
            int fails = 0;
            int successes = 0;

            using (var t = new Transaction(document))
            {
                if (t.Start("Bulk Rename") == TransactionStatus.Started)
                {
                    foreach (CorrectionCandidate candidate in allTextParameters)
                    {
                        if (candidate.IsModified)
                        {
                            if (candidate.Rename())
                            {
                                successes++;
                            }
                            else
                            {
                                fails++;
                            }
                        }
                    }
                    t.Commit();
                    SCaddinsApp.WindowManager.ShowMessageBox(
                        @"Spelling", successes + @" parameters succesfully renamed, " + fails + @" errors.");
                }
                else
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("Error", "Failed to start Spelling Transaction...");
                }
            }
        }

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
                if (parameterSet == null || parameterSet.IsEmpty) continue;
                foreach (var parameter in parameterSet) {
                    if (parameter is Autodesk.Revit.DB.Parameter) {
                          Autodesk.Revit.DB.Parameter p = (Autodesk.Revit.DB.Parameter)parameter ;
                          if (p == null || !p.HasValue) continue;
                          if(p.IsReadOnly) continue;
                          if (p.StorageType == StorageType.String) {
                              var rc = new CorrectionCandidate(p, hunspell);
                            try
                            {
                                if (!string.IsNullOrEmpty(rc.OriginalText))
                                {
                                    candidates.Add(rc);
                                }
                            } catch { }
                        }
                    }
                }
            }
            return candidates;
        }
    }
}
