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

namespace SCaddins.SCulcase
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    
    public partial class SCulcaseInfoDialog : Form
    {
        public SCulcaseInfoDialog()
        {
            this.InitializeComponent();
        }

        public void SetText(string s)
        {
            textBox1.Clear();
            textBox1.Text = s;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
