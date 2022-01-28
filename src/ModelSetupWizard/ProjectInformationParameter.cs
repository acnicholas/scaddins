// (C) Copyright 2019-2020 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

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

        public ProjectInformationParameter(Parameter parameter)
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

        public string Name
        {
            get; private set;
        }

        public string OriginalValue
        {
            get; private set;
        }

        public bool IsValid
        {
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
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                if (this.value == value)
                {
                    return;
                }
                if (value != OriginalValue)
                {
                    IsModified = true;
                }
                else
                {
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

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Parameter:{0}, Original Value:{1}, New Value:{2}", Name, OriginalValue, Value);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
