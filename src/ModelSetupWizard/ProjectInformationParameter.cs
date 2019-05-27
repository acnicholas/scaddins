using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace SCaddins.ModelSetupWizard
{
    class ProjectInformationParameter
    {
        private string value;

        public ProjectInformationParameter(Autodesk.Revit.DB.Parameter parameter)
        {
            Parameter = parameter;
            Name = Parameter.Definition.Name;
            Value = Parameter.AsString();
            Type = Parameter.StorageType.ToString();
        }

        public string Name {
            get; private set;
        }

  
        public string Value {
            get
            {
                return value;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) {
                    return;
                }
                if (Parameter.StorageType == StorageType.String) {
                    this.value = value;
                } else if (Parameter.StorageType == StorageType.Integer){
                    var oldVal = this.value;
                    int result = -1;
                    if (int.TryParse(value, out result)) {
                        this.value = value;
                    } else {
                        SCaddinsApp.WindowManager.ShowMessageBox(value + " is not a valid integer...");
                        this.value = oldVal;
                    }
                }
            }
        }

        public string Type {
            get; private set;
        }

        public string Format {
            get; set;
        }


        private Parameter Parameter {
            get; set;
        }
    }
}
