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
            ReplacementPatternHint = string.Empty;
            SearchPatternHint = string.Empty;
        }

        public RenameCommand(Func<string, string, string, string> renameFunction, string name, string search, string replacement)
        {
            this.renameFunction = renameFunction;
            ReplacementPattern = replacement;
            SearchPattern = search;
            HasInputParameters = true;
            ReplacementPatternHint = "Replacement Pattern";
            SearchPatternHint = "Search Pattern";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HasInputParameters
        {
            get; private set;
        }

        public string ReplacementPattern
        {
            get => replacementPattern;

            set
            {
                if (replacementPattern != null && value != replacementPattern)
                {
                    replacementPattern = value;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReplacementPattern)));
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
                if (value != searchPattern) {
                    searchPattern = value;
                }
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SearchPattern)));
                }
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