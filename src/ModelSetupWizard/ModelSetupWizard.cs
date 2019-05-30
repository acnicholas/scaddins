using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.ModelSetupWizard
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;

    class ModelSetupWizard
    {
        public static void ApplyWorksetModifications
            (Document doc, List<WorksetParameter> worksets)
        {

        }

        public static void ApplyProjectInfoModifications
            (Document doc, List<ProjectInformationParameter> worksets)
        {

        }

        public static void SetParameterValue(Parameter param, string value, Document doc)
        {
            using (Transaction t = new Transaction(doc))
            {
                if (t.Start("Set Parameter Value") == TransactionStatus.Started)
                {
                    param.Set(value);
                }
            }
        }
    }
}
