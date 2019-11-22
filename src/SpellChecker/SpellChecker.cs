using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Caliburn.Micro;

namespace SCaddins.SpellChecker
{    
    public class SpellChecker
    {
        private Document document;

        public SpellChecker(Document document)
        {
            this.document = document;
        }

        public List<String> IgnoreInSession { get; set; }

        public BindableCollection<CorrectionCandiate> SpellingErrors { get; set; }
    }
}
