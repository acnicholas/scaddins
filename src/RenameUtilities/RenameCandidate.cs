// (C) Copyright 2017-2023 by Andrew Nicholas
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

    public abstract class RenameCandidate : IRenameCandidate
    {
        private string newValue;
        private string oldValue;

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

            set
            {
                this.oldValue = value;
            }
        }

        public bool ValueChanged => !string.Equals(this.oldValue, this.newValue, System.StringComparison.CurrentCulture);

        public virtual bool Rename()
        {
            throw new System.NotImplementedException();
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
