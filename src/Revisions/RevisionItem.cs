// (C) Copyright 2013-2014 by Andrew Nicholas
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

namespace SCaddins.SCloudSChed
{
    public class RevisionItem
    {
        private bool export;
        private string description;
        private string date;

        public RevisionItem(string date, string description, bool issued, int sequence)
        {
            this.description = description;
            this.date = date;
            this.Issued = issued;
            this.Sequence = sequence;
        }

        public bool Export {
            get { return this.export; }
            set { this.export = value; }
        }

        public string Description {
            get { return this.description; }
        }

        public string Date {
            get { return this.date; }
        }

        public bool Issued {
            get;
            set;
        }

        public int Sequence {
            get;
            set;
        }
    }
}
