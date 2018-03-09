using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace SCaddins.SheetCopier
{
    class SheetInformation
    {
        public string ParameterName
        {
            get; private set;
        }

        public string ParameterValue
        {
            get; private set;
        }

        public SheetInformation(Parameter param)
        {
            ParameterName = param.Definition.Name;
            switch (param.StorageType) {
                case StorageType.Double:
                    ParameterValue = param.AsString();
                    break;
                case StorageType.ElementId:
                    ParameterValue = param.AsString();
                break;
                case StorageType.Integer:
                    ParameterValue = param.AsInteger().ToString();
                    break;
                case StorageType.None:
                    ParameterValue = string.Empty;
                    break;
                case StorageType.String:
                ParameterValue = param.AsString();
                    break;
                default:
                    ParameterValue = param.AsString();
                break;
            }           
        }
    }
}
