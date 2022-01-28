// (C) Copyright 2017-2020 by Andrew Nicholas
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
        private Family family;
        private Autodesk.Revit.DB.GroupType group;
        private string oldValue;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "parameter")]
        private Parameter parameter;

        public RenameCandidate(Parameter parameter)
        {
            this.parameter = parameter;
            this.note = null;
            this.family = null;
            this.group = null;
            this.oldValue = parameter.AsString();
            this.newValue = parameter.AsString();
        }

        public RenameCandidate(TextElement note)
        {
            this.parameter = null;
            this.note = note;
            this.family = null;
            this.group = null;
            this.oldValue = note.Text;
            this.newValue = note.Text;
        }

        public RenameCandidate(Family family)
        {
            this.parameter = null;
            this.note = null;
            this.family = family;
            this.oldValue = family.Name;
            this.newValue = family.Name;
            this.group = null;
        }

        public RenameCandidate(Autodesk.Revit.DB.GroupType group)
        {
            this.parameter = null;
            this.note = null;
            this.family = null;
            this.group = group;
            this.oldValue = group.Name;
            this.newValue = group.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string NewValue
        {
            get
            {
                return this.newValue;
            }

            set
            {
                this.newValue = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(NewValue)));
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(ValueChanged)));
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
                return this.oldValue;
            }
        }

        public bool ValueChanged => !string.Equals(this.oldValue, this.newValue, System.StringComparison.CurrentCulture);

        private Parameter RevitParameter => parameter;

        //// FIXME this is a mess :)
        public bool Rename()
        {
            if (ValueChanged)
            {
                if (note == null && family == null && group == null)
                {
                    if (!parameter.IsReadOnly)
                    {
                        return parameter.Set(NewValue);
                    }
                }
                else if (family == null && group == null)
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
                else if (note == null && group == null)
                {
                    try
                    {
                        family.Name = NewValue;
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }
                else if (note == null && family == null)
                {
                    try
                    {
                        group.Name = NewValue;
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
