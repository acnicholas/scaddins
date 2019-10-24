// (C) Copyright 2017 by Andrew Nicholas
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

namespace SCaddins.RenameUtilities
{
    using System.ComponentModel;
    using Autodesk.Revit.DB;

    public class RenameCandidate : INotifyPropertyChanged
    {
        private string newValue;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "note")]
        private TextElement note;
        private string oldValue;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "parameter")]
        private Parameter parameter;

        public RenameCandidate(Parameter parameter)
        {
            this.parameter = parameter;
            note = null;
            oldValue = parameter.AsString();
            newValue = parameter.AsString();
        }

        public RenameCandidate(TextElement note)
        {
            parameter = null;
            this.note = note;
            oldValue = note.Text;
            newValue = note.Text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string NewValue
        {
            get
            {
                return newValue;
            }

            set
            {
                newValue = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(NewValue)));
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(ValueChanged)));
                }
            }
        }

        public string OldValue
        {
            get
            {
                if (string.IsNullOrEmpty(oldValue))
                {
                    return string.Empty;
                }
                return oldValue;
            }
        }

        public bool ValueChanged
        {
            get { return !string.Equals(oldValue, newValue, System.StringComparison.CurrentCulture); }
        }
        
        public bool Rename()
        {
            if (ValueChanged)
            {
                if (note == null)
                {
                    if (!parameter.IsReadOnly)
                    {
                        return parameter.Set(NewValue);
                    }
                }
                else
                {
                    try
                    {
                        note.Text = NewValue;
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */