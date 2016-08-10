// (C) Copyright 2015 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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

namespace SCaddins
{
    using System;
    using System.Collections.Specialized;
    using System.Windows.Forms;
    
    public partial class SCaddinsOptionsForm : Form
    {
        public SCaddinsOptionsForm()
        {           
            this.InitializeComponent();
            this.checkBox1.Checked = SCaddins.Scaddins.Default.UpgradeCheckOnStartUp;
            this.checkBox2.Checked = SCaddins.Scaddins.Default.UnjoinNewWalls;
        }
                                  
        private void ButtonOKClick(object sender, EventArgs e)
        {
            SCaddins.Scaddins.Default.UpgradeCheckOnStartUp = checkBox1.Checked;
            SCaddins.Scaddins.Default.UnjoinNewWalls = checkBox2.Checked;
            SCaddins.Scaddins.Default.Save();
        }
    }
}
