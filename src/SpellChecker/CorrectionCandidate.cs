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
    using System.Text.RegularExpressions;
    using Autodesk.Revit.DB;
    using NHunspell;

    public class CorrectionCandidate : IEnumerator
    {
        private Dictionary<string, string> autoReplacementList;
        private int currentIndex;
        private Hunspell hunspell;
        private string[] originalWords;
        private Parameter parameter;
        private TextElement textElement;
        private Regex rgx;

        public CorrectionCandidate(Parameter parameter, Hunspell hunspell, ref Dictionary<string, string> autoReplacementList)
        {
            this.parameter = parameter;
            this.textElement = null;
            this.hunspell = hunspell;
            this.autoReplacementList = autoReplacementList;
            OriginalText = parameter.AsString().Replace(@"\r", string.Empty);
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\\', '/', '(', ')', '<', '>' };
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

        public CorrectionCandidate(TextElement textElement, Hunspell hunspell, ref Dictionary<string, string> autoReplacementList)
        {
            this.parameter = null;
            this.textElement = textElement;
            this.hunspell = hunspell;
            this.autoReplacementList = autoReplacementList;
            OriginalText = textElement.Text;
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\\', '/', '(', ')', '<', '>', '\r' };
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

        public object Current
        {
            get
            {
                return currentIndex < originalWords.Length && currentIndex != -1 ? originalWords[currentIndex].Trim() : null;
            }
        }

        public string CurrentAsString
        {
            get
            {
                return Current != null ? (string)Current : string.Empty;
            }
        }

        public ElementId ParentElementId
        {
            get
            {
                return parameter != null ? parameter.Element.Id : textElement.Id;
            }
        }

        public bool IsModified => !string.Equals(this.OriginalText, this.NewText, System.StringComparison.CurrentCulture);

        public string NewText
        {
            get; private set;
        }

        public string OriginalText
        {
            get; private set;
        }

        /// <summary>
        /// The full revit class name in string format.
        /// </summary>
        public string TypeString
        {
            get
            {
                if (parameter != null)
                {
                    return parameter.Element.GetType().ToString();
                }
                else
                {
                    return textElement.GetType().ToString();
                }
            }
        }

        /// <summary>
        /// Try to move to the next word in the current string
        /// </summary>
        /// <returns>true if a move is possible. false if a spelling error is found</returns>
        public bool MoveNext()
        {
            while (originalWords != null && currentIndex < (originalWords.Length - 1))
            {
                currentIndex++;

                // Continue if a number is found...
                if (rgx.IsMatch(CurrentAsString))
                {
                    continue;
                }

                if (autoReplacementList.ContainsKey(CurrentAsString))
                {
                    string replacement;
                    if (autoReplacementList.TryGetValue(CurrentAsString, out replacement))
                    {
                        // FIXME this reaplces the first, not the current.
                        ReplaceCurrent(replacement);
                        continue;
                    }
                }

                if (!hunspell.Spell(originalWords[currentIndex].Trim()))
                {
                    return false;
                }
            }

            // Reset here so any futures checks in this actually check the whole string.
            Reset();
            return true;
        }

        /// <summary>
        /// Commit rename to current revit model
        /// Needs tobe run in a transaction.
        /// </summary>
        /// <returns></returns>
        public bool Rename()
        {
            if (IsModified)
            {
                if (parameter != null && !parameter.IsReadOnly)
                {
                    return parameter.Set(NewText);
                }
                else
                {
                    try
                    {
                        textElement.Text = NewText;
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replace the occurance of word at the current location in the string.
        /// </summary>
        /// <param name="word"></param>
        public void ReplaceCurrent(string word)
        {
            // FIXME - this will not work if you want to second+ instance and keep the first.
            NewText = ReplaceFirst(NewText, (string)Current, word);
        }

        /// <summary>
        /// Reset the index to the first word.
        /// </summary>
        public void Reset()
        {
            currentIndex = -1;
        }

        private string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
