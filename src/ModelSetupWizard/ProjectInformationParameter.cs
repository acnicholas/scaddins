namespace SCaddins.ModelSetupWizard
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using Autodesk.Revit.DB;

    public class ProjectInformationParameter : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "parameter")]
        private Parameter parameter;
        private string value;

        public ProjectInformationParameter(Autodesk.Revit.DB.Parameter parameter)
        {
            this.parameter = parameter;
            Name = parameter.Definition.Name;
            if (parameter.StorageType == StorageType.String)
            {
                Value = parameter.AsString();
            }
            else
            {
                Value = parameter.AsValueString();
            }
            OriginalValue = Value;
            Type = parameter.StorageType.ToString();
            IsEditable = !parameter.IsReadOnly;
            IsModified = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Format
        {
            get; set;
        }

        public bool IsEditable
        {
            get; private set;
        }

        public bool IsModified
        {
            get; private set;
        }

        public string Name {
            get; private set;
        }

        public string OriginalValue {
            get; private set;
        }

        public bool IsValid {
            get
            {
                return Regex.IsMatch(value, Format.Trim());
            }
        }

        public string Type
        {
            get; private set;
        }

        public string Value
        {
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
                if (value != OriginalValue) {
                    IsModified = true;
                } else {
                    IsModified = false;
                }
                this.value = value;
                NotifyPropertyChanged(nameof(IsModified));
                NotifyPropertyChanged(nameof(Value));
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

        public Parameter GetParameter()
        {
            return parameter;
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
