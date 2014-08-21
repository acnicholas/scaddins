// (C) Copyright 2014 by Andrew Nicholas
//
// This file is part of SCaos.
//
// SCaos is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaos is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaos.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCaos
{
    using System;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// Description of SCaosForm.
    /// </summary>
    public partial class SCaosForm : Form
    {
        public SCaosForm(string[] s, bool currentViewIsIso)
        {
            this.InitializeComponent();
            if (!currentViewIsIso) {
                radioButtonRotateCurrent.Enabled = false;
            }
            for (int i = 0; i < s.Length; i++) {
                listBox1.Items.Add(s[i]);
            }
            this.SetTitle();
        }
        
        private void SetTitle()
        {
            this.Text = "SCaos by Andrew Nicholas";
        }
        
        private void Button3Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(Constants.HelpLink);
        }
        
        private void RadioButton3CheckedChanged(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
