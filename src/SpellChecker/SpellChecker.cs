// (C) Copyright 2019-2020 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SpellChecker
{
    using System.Collections;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using NHunspell;
    using SCaddins;

    public class SpellChecker : IEnumerator
    {
        private List<CorrectionCandidate> allTextParameters;
        private Dictionary<string, string> autoReplacementList = new Dictionary<string, string>();
        private int currentIndex;
        private Document document;
        private Hunspell hunspell;
        private List<string> ignoreList;

        public SpellChecker(Document document)
        {
            if (hunspell != null)
            {
                hunspell.Dispose();
            }

            this.document = document;

            ignoreList = new List<string>();
            UpdateIgnoreList();

            string dll = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);

            try
            {
                if (Hunspell.NativeDllPath != dll)
                {
                    Hunspell.NativeDllPath = dll;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

#if DEBUG
            //// SCaddinsApp.WindowManager.ShowMessageBox(System.IO.Path.Combine(dll, "Assets"));
            hunspell = new Hunspell(
                            System.IO.Path.Combine(dll, @"Assets/en_AU.aff"),
                            System.IO.Path.Combine(dll, @"Assets/en_AU.dic"));
#else
            hunspell = new Hunspell(
                            System.IO.Path.Combine(Constants.InstallDirectory, "etc", "en_AU.aff"),
                            System.IO.Path.Combine(Constants.InstallDirectory, "etc", "en_AU.dic"));
#endif

            // add some arch specific words
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
        public object Current => allTextParameters[SafeCurrentIndex];

        /// <summary>
        /// Returns the curent CorrectionCandiate
        /// </summary>
        public CorrectionCandidate CurrentCandidate => (CorrectionCandidate)Current;

        public string CurrentElementType => CurrentCandidate.TypeString;

        /// <summary>
        /// Returns the current unknown word.
        /// </summary>
        public string CurrentUnknownWord => CurrentCandidate.Current as string;

        public Autodesk.Revit.DB.Document Document => document;

        private int SafeCurrentIndex => currentIndex < allTextParameters.Count ? currentIndex : allTextParameters.Count - 1;

        public void AddToAutoReplacementList(string word, string replacement)
        {
            if (autoReplacementList.ContainsKey(word))
            {
                return;
            }
            autoReplacementList.Add(word, replacement);
        }

        /// <summary>
        /// Commit text changes to the the current mode.
        /// </summary>
        public void CommitSpellingChangesToModel()
        {
            int fails = 0;
            int successes = 0;

            using (var t = new Transaction(document))
            {
                if (t.Start("Spelling") == TransactionStatus.Started)
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
        /// get spelling suggestions for the current CorrectionCandidate
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentSuggestions()
        {
            if (currentIndex < 0)
            {
                return new List<string>();
            }
            if (hunspell != null && allTextParameters.Count > 0 && currentIndex < allTextParameters.Count)
            {
                return hunspell.Suggest(allTextParameters[currentIndex].CurrentAsString);
            }
            return new List<string>();
        }

        /// <summary>
        /// Ingnore all future instances of the CurrentUnknownWord
        /// </summary>
        public void IgnoreAll()
        {
            hunspell.Add(CurrentUnknownWord);
        }

        /// <summary>
        /// Attempt to move to the next word.
        /// 
        /// </summary>
        /// <returns>
        /// return false if a spelling error is found. true to move to the next string.
        /// </returns>
        public bool MoveNext()
        {
            // No point running if there are no elements to check.
            if (allTextParameters == null || allTextParameters.Count <= 0)
            {
                return false;
            }

            // Run till a spelling error is found.
            while (currentIndex < allTextParameters.Count)
            {
                if (currentIndex == -1)
                {
                    currentIndex = 0;
                }

                // Skip if type is in the ignore list.
                if (ignoreList.Contains(CurrentCandidate.TypeString))
                {
                    currentIndex++;
                    continue;
                }

                if (!allTextParameters[currentIndex].MoveNext())
                {
                    return false;
                }
                currentIndex++;
            }

            // A move to the next candiate is possible.
            return true;
        }

        public void ProcessAllAutoReplacements()
        {
            Reset();
            while (currentIndex < allTextParameters.Count)
            {
                MoveNext();
            }
        }

        public void Reset()
        {
            currentIndex = -1;
        }

        public void UpdateIgnoreList()
        {
            ignoreList.Clear();
            foreach (var ignore in SpellCheckerSettings.Default.ElementIgnoreList)
            {
                if (!ignore.Trim().StartsWith(@"#"))
                {
                    ignoreList.Add(ignore);
                }
            }
        }

        private List<CorrectionCandidate> GetTextNoteElements(Document doc)
        {
            var candidates = new List<CorrectionCandidate>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_TextNotes);
            foreach (Element element in collector)
            {
                var note = (TextElement)element;
                if (note != null)
                {
                    var cc = new CorrectionCandidate(note, hunspell, ref autoReplacementList);
                    candidates.Add(cc);
                }
            }
            return candidates;
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

            // Get TextNote Elements
            candidates.AddRange(GetTextNoteElements(doc));

            foreach (Element element in collector)
            {
                var parameterSet = element.Parameters;
                if (parameterSet == null || parameterSet.IsEmpty)
                {
                    continue;
                }
                foreach (var parameter in parameterSet)
                {
                    if (parameter is Autodesk.Revit.DB.Parameter)
                    {
                        Autodesk.Revit.DB.Parameter p = (Autodesk.Revit.DB.Parameter)parameter;
                        if (p == null || !p.HasValue)
                        {
                            continue;
                        }
                        if (p.IsReadOnly)
                        {
                            continue;
                        }
                        try
                        {
                            if (p.StorageType == StorageType.String)
                            {
                                var rc = new CorrectionCandidate(p, hunspell, ref autoReplacementList);

                                if (!string.IsNullOrEmpty(rc.OriginalText))
                                {
                                    candidates.Add(rc);
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                        }
                    }
                }
            }
            return candidates;
        }
    }
}
