namespace SCaddins.RenameUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;

    public static class RenameParameterCollector
    {
        public static List<Element> GetRenameParameterCollector(Document doc, RenameParameter renameParameter)
        {
            var collector = new FilteredElementCollector(doc);
            switch (renameParameter.RenameType)
            {
                case RenameTypes.ByCategory:
                        return collector.OfCategory(renameParameter.Category).ToList<Element>();
                default:
                    return null;
            }
        }
    }
}
