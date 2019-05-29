using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace SCaddins.ModelSetupWizard
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    class ProjectInformationParameter : INotifyPropertyChanged
    {
        private string value;
        private Parameter parameter;
        private string originalValue;
        public event PropertyChangedEventHandler PropertyChanged;

        public ProjectInformationParameter(Autodesk.Revit.DB.Parameter parameter)
        {
            this.parameter = parameter;
            Name = parameter.Definition.Name;
            if (parameter.StorageType == StorageType.String)
            {
                Value = parameter.AsString();
            } else
            {
                Value = parameter.AsValueString();
            }
            originalValue = Value;
            Type = parameter.StorageType.ToString();
            IsEditable = !parameter.IsReadOnly;
            isModified = false;
        }

        public bool IsEditable
        {
            get; private set;
        }

        public bool isModified
        {
            get; private set;
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
                if (this.value == value) {
                    return;
                } 
                if (value != originalValue) {
                    isModified = true;
                } else {
                    isModified = false;
                }
                this.value = value;
                NotifyPropertyChanged(nameof(isModified));
            }
        }

        public string Type {
            get; private set;
        }

        public string Format {
            get; set;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
