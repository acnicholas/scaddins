using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SCaddins.RenameUtilities
{
    public class RenameCommand : INotifyPropertyChanged
    {
        private Func<string, string, string, string> renameFunction;
        public event PropertyChangedEventHandler PropertyChanged;
        private string replacementPattern;
        private string searchPattern;

        public string Name
        {
            get;
        }

        public string SearchPatternHint
        {
            get; set;
        }

        public string ReplacementPatternHint
        {
            get; set;
        }

        public string SearchPattern
        {
            get { return searchPattern; }
            set
            {
                if (value != searchPattern) {
                    searchPattern = value;
                }
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("SearchPattern"));
                }
            }
        }

        public string ReplacementPattern
        {
            get { return replacementPattern; }
            set
            {
                if (value != replacementPattern) {
                    replacementPattern = value;
                }
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("ReplacementPattern"));
                }
            }
        }

        public bool HasInputParameters
        {
            get; private set;
        }

        public string Rename(string inputString)
        {
                return renameFunction(inputString, SearchPattern, ReplacementPattern);
        }

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
            HasInputParameters = true; ;
            Name = name;
            ReplacementPatternHint = "Replacement Pattern";
            SearchPatternHint = "Search Pattern";
        }
    }
}
