// (C) Copyright 2012-2015 by Andrew Nicholas
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

namespace SCaddins.SCexport
{
    using System;

    public class FilterContextMenu
    {
        public FilterContextMenu(string label, int column, string filter)
        {
            this.Update(label, column, filter);
        }

        public string Label
        {
            get; set;
        }

        public string Filter
        {
            get; set;
        }
 
        public int Column
        {
            get; set;
        }

        public override string ToString()
        {
            return this.Label;
        }

        public void Update(string label, int column, string filter)
        {
            this.Label = label;
            this.Filter = filter;
            this.Column = column;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
