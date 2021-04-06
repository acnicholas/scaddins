// (C) Copyright 2019-2021 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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

namespace SCaddins.SpellChecker.ViewModels
{
    using System;
    using System.Collections.Specialized;
    using Caliburn.Micro;

    public class SpellCheckerOptionsViewModel : PropertyChangedBase, Common.ViewModels.ISettingPanel
    {
        private StringCollection stringCollection;

        public SpellCheckerOptionsViewModel()
        {      
            Reset();  
        }

        public string ElementsToIgnore { get; set; }

        public string WordsToIgnore { get; set; }

        public void Apply()
        {
            var collecton = new StringCollection();
            string[] lines = ElementsToIgnore.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines) {
                if (!string.IsNullOrEmpty(line)) {
                    collecton.Add(line);
                }
            }

            var collecton2 = new StringCollection();
            string[] lines2 = WordsToIgnore.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines2)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    collecton2.Add(line);
                }
            }

            SpellCheckerSettings.Default.ElementIgnoreList = collecton;
            SpellCheckerSettings.Default.WordsIgnoreList = collecton2;
            SpellCheckerSettings.Default.Save();
        }

        public void Reset()
        {
            ElementsToIgnore = string.Empty;
            foreach (var item in SpellCheckerSettings.Default.ElementIgnoreList)
            {
                ElementsToIgnore += item;
                ElementsToIgnore += Environment.NewLine;
            }
            NotifyOfPropertyChange(() => ElementsToIgnore);

            WordsToIgnore = string.Empty;
            foreach (var item in SpellCheckerSettings.Default.WordsIgnoreList)
            {
                WordsToIgnore += item;
                WordsToIgnore += Environment.NewLine;
            }
            NotifyOfPropertyChange(() => WordsToIgnore);

        }

        public void ResetToDefault()
        {
            throw new NotImplementedException();
        }
    }
}
