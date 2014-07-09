//
// (C) Copyright 2012-2013 by Andrew Nicholas
//
// This file is part of SCightlines.
//
// SCightlines is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCightlines is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCightlines.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Windows.Forms;

namespace SCaddins.SCightLines
{
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
        public SCightOutputForm(String s)
        {
            InitializeComponent();
            Update(s);
        }

        /// <summary>
        /// Update with a given string
        /// </summary>
        /// <param name="s">
        /// String to display
        /// </param>
        public void Update(String s)
        {
            textBox1.Text = s;
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; // this cancels the close event.
        }

    }
}
