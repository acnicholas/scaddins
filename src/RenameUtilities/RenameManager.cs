
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace SCaddins.RenameUtilities
{
    /// <summary>
    /// Description of Class1.
    /// </summary>
    public class RenameManager
    {
        private Document doc;
        
        public RenameManager(Document doc)
        {
            this.doc = doc;
        }
        
        public void Rename(List<RenameCandidate> renameCandidates)
        {
            int fails = 0;
            int successes = 0;
            using (var t = new Transaction(doc)) {
                if (t.Start("Bulk Rename") == TransactionStatus.Started) { 
                    foreach (RenameCandidate candidate in renameCandidates) {
                        if(candidate.ValueChanged() && candidate.Rename()){
                            successes++;
                        } else {
                            fails++;
                        }
                    }
                    t.Commit();
                    Autodesk.Revit.UI.TaskDialog.Show(@"Bulk Rename", successes + @" parameters succesfully renames, " + fails + @" errors.");
                } else {
                    Autodesk.Revit.UI.TaskDialog.Show("Error", "Failed to start Bulk Rename Revit Transaction...");
                }
            }
        }
        
         public List<RenameCandidate> GetTextNoteValues(BuiltInCategory category){
            List<RenameCandidate> candidates = new List<RenameCandidate>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(category);
            foreach (Element element in collector) {
                var textNote = (TextElement)element;
                if(textNote != null) {
                    candidates.Add(new RenameCandidate(textNote));
                }
            }
            Autodesk.Revit.UI.TaskDialog.Show("test", candidates.Count.ToString());
            return candidates;
        }
             
        public List<RenameCandidate> GetParameterValues(Parameter parameter, BuiltInCategory category){
            if (category == BuiltInCategory.OST_TextNotes) {
                return GetTextNoteValues(category);
            }
            List<RenameCandidate> candidates = new List<RenameCandidate>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(category);
            foreach (Element element in collector) {
                var p = element.GetParameters(parameter.Definition.Name);
                if (p.Count > 0) {
                    candidates.Add(new RenameCandidate(p[0]));
                }
            }
            return candidates;
        }
        
        public List<RenameParameter> GetParametersByCategory(BuiltInCategory category)
        {
            List<RenameParameter> parametersList = new List<RenameParameter>();
            if(category == BuiltInCategory.OST_TextNotes) {
                parametersList.Add(new RenameParameter(true));
            }
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(category);
            var elem = collector.FirstElement();
            foreach (Parameter param in elem.Parameters) {
                if (param.StorageType == StorageType.String && !param.IsReadOnly) {
                    parametersList.Add(new RenameParameter(param, category));
                }
            }
            return parametersList;
        }
    }
}
