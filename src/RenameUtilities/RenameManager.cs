
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
        
        public List<RenameCandidate> GetRoomParameterValues(Parameter parameter){
            List<RenameCandidate> candidates = new List<RenameCandidate>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Rooms);
            foreach (SpatialElement view in collector) {
                var p = view.GetParameters(parameter.Definition.Name);
                if (p.Count > 0) {
                    candidates.Add(new RenameCandidate(p[0]));
                }
            }
            return candidates;
        }
        
        public List<RenameParameter> GetRoomParameters2() {
            List<RenameParameter> parametersList = new List<RenameParameter>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Rooms);
            SpatialElement view = collector.FirstElement() as SpatialElement;
                foreach (Parameter param in view.Parameters) {
                    if (param.StorageType == StorageType.String) {
                        parametersList.Add(new RenameParameter(param));
                    }
                }
            return parametersList;
        }
        
    }
}
