// (C) Copyright 2012-2014 by Andrew Nicholas
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

    /// <summary>
    /// A class to hold context sensitive information about the users
    /// last right mouse click.
    /// </summary>
    public class FilterContextMenu
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterContextMenu"/> class.
        /// </summary>
        /// <param name="label"> The label to display in the contect menu.</param>
        /// <param name="column">The column to filer in the main dataGridView.</param>
        /// <param name="filter">The filter to apply.</param>
        public FilterContextMenu(string label, int column, string filter)
        {
            this.Update(label, column, filter);
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label to display in the contect menu.</value>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter
        {
            get;
            set;
        }
                
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column.</value>
        public int Column
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="FilterContextMenu"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="FilterContextMenu"/>.</returns>
        public override string ToString()
        {
            return this.Label;
        }

        /// <summary>
        /// Update the specified label, column and filter.
        /// </summary>
        /// <param name="label"> The label to display in the contect menu.</param>
        /// <param name="column">The column to filer in the main dataGridView.</param>
        /// <param name="filter">The filter to apply.</param>
        public void Update(string label, int column, string filter)
        {
            this.Label = label;
            this.Filter = filter;
            this.Column = column;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
