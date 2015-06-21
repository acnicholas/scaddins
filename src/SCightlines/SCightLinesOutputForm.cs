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

namespace SCaddins.SCightLines
{
    using System;
    using System.Windows.Forms;
    
    /// <summary>
    /// Form to display text values of the current settings
    /// </summary>
    /// <author>
    /// Andrew Nicholas
    /// </author>
    public partial class SCightOutputForm : Form
    {
        /// <summary>
        /// Initialise with a given string
        /// </summary>
        /// <param name="s">
        /// String to display
        /// </param>
        public SCightOutputForm(string s)
        {
            this.InitializeComponent();
            this.Update(s);
        }

        /// <summary>
        /// Update with a given string
        /// </summary>
        /// <param name="displayValue">
        /// String to display
        /// </param>
        public void Update(string displayValue)
        {
            textBox1.Text = displayValue;
        }
    }
}
