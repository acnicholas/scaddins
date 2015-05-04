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

namespace SCaddins.Common
{   
    using System;
    using System.Windows.Forms;
    
    /// <summary>
    /// Description of UpgradeForm.
    /// </summary>
    public partial class UpgradeForm : Form     
    {
        public UpgradeForm(Version installed, Version remote)
        {
            this.InitializeComponent();
            labelInstalledVersion.Text = "Insalled Version: " + installed.ToString();
            labelLatestVersion.Text = "Latest Version: " + remote.ToString();
            if (installed < remote) {
                labelUpgradeNote.Text = "New Version Available!. Click Download (go on, you can do it)";
                buttonDownload.Enabled = true;
                buttonLog.Enabled = true;
            } else {
                labelUpgradeNote.Text = "SCaddins is up to date. Nice one.";
                buttonDownload.Enabled = false;   
                buttonLog.Enabled = true;
            }
        }
        
        private void Button3Click(object sender, EventArgs e)
        {
          System.Diagnostics.Process.Start(SCaddins.Constants.DownloadLink);
        }
        
        private void Button1Click(object sender, EventArgs e)
        {
          System.Diagnostics.Process.Start(SCaddins.Constants.ChangelogLink);  
        }
    }
}
