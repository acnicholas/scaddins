namespace SCaddins.SpellChecker
{
    using Autodesk.Revit.DB;
    using System.Collections.Generic;

    public class CorrectionCandidate : SCaddins.RenameUtilities.RenameCandidate
    {
        public CorrectionCandidate(Parameter parameter) : base(parameter)
        {

        }

        public CorrectionCandidate(TextElement note) : base(note)
        {

        }

        public List<string> Suggestions {
            get; set;
        }


    }
}
