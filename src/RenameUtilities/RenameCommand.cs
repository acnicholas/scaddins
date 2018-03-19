using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.RenameUtilities
{
    public class RenameCommand
    {
        private Func<string, string, string, string> renameFunction;

        public string Name
        {
            get;
        }

        public string SearchPattern
        {
            get; set;
        }

        public string ReplacementPattern
        {
            get; set;
        }

        public bool EnableSearchPattern
        {
            get
            {
                return !string.IsNullOrEmpty(SearchPattern);
            }
        }

        public bool EnableReplacementPattern
        {
            get
            {
                return !string.IsNullOrEmpty(ReplacementPattern);
            }
        }

        public string Rename(string inputString)
        {
                return renameFunction(inputString, SearchPattern, ReplacementPattern);
        }

        public RenameCommand(Func<string, string, string, string> renameFunction, string name)
        {
            this.renameFunction = renameFunction;
            ReplacementPattern = string.Empty;
            SearchPattern = string.Empty;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
