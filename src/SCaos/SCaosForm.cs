//
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
//

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace SCaddins.SCaos
{
    /// <summary>
    /// Description of SCaosForm.
    /// </summary>
    public partial class SCaosForm : Form
    {
        public SCaosForm(string[] s, bool currentViewIsIso)
        {
            InitializeComponent();
			if(!currentViewIsIso){
				radioButtonRotateCurrent.Enabled = false;
			}
            for(int i = 0; i < s.Length; i++){
                listBox1.Items.Add(s[i]);
            }
            SetTitle();
        }
        
        private void SetTitle()
        {
            string version =
                Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string name =
                Assembly.GetExecutingAssembly().GetName().Name.ToString();
            string company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof(AssemblyCompanyAttribute), false)).Company;
            this.Text = name + " [" + version + "] by " + company;
        }
		void Button3Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start(Constants.HelpLink);
		}
		void RadioButton3CheckedChanged(object sender, System.EventArgs e)
		{
			throw new NotImplementedException();
		}    
    }
}
