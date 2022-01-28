// (C) Copyright 2017-2020 by Andrew Nicholas
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

namespace SCaddins.RenameUtilities
{
    using System;
    using System.ComponentModel;

    public class RenameCommand : INotifyPropertyChanged
    {
        private Func<string, string, string, string> renameFunction;

        private string replacementPattern;

        private string searchPattern;

        public RenameCommand(Func<string, string, string, string> renameFunction, string name)
        {
            this.renameFunction = renameFunction;
            replacementPattern = string.Empty;
            searchPattern = string.Empty;
            HasInputParameters = false;
            Name = name;
            ReplacementPatternHint = string.Empty;
            SearchPatternHint = string.Empty;
        }

        public RenameCommand(Func<string, string, string, string> renameFunction, string name, string search, string replacement)
        {
            this.renameFunction = renameFunction;
            ReplacementPattern = replacement;
            SearchPattern = search;
            HasInputParameters = true;
            Name = name;
            ReplacementPatternHint = "Replacement Pattern";
            SearchPatternHint = "Search Pattern";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasInputParameters
        {
            get; private set;
        }

        public string Name
        {
            get;
        }

        public string ReplacementPattern
        {
            get
            {
                return replacementPattern;
            }

            set
            {
                if (value != replacementPattern)
                {
                    replacementPattern = value;
                }
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(ReplacementPattern)));
                }
            }
        }

        public string ReplacementPatternHint
        {
            get; set;
        }

        public string SearchPattern
        {
            get
            {
                return searchPattern;
            }

            set
            {
                if (value == searchPattern)
                {
                    return;
                }
                searchPattern = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchPattern)));
            }
        }

        public string SearchPatternHint
        {
            get; set;
        }

        public string Rename(string inputString)
        {
            return renameFunction(inputString, SearchPattern, ReplacementPattern);
        }
    }
}