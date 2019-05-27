using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace SCaddins.ModelSetupWizard
{
    class ElementCollectors
    {
        public static IEnumerable<ProjectInformationParameter> GetProjectInformationParameters(Document doc)
        {
            FilteredElementCollector fec = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ProjectInformation);
            foreach (Element e in fec) {
                foreach (var param in e.GetOrderedParameters()) {
                    yield return new ProjectInformationParameter(param);
                }
            }
        }
    }
}
