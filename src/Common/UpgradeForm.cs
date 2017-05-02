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
    using SCaddins.Properties;

    public partial class UpgradeForm : Form
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
        public UpgradeForm(Version installed, Version remote, string body)
        {
            if (installed == null) {
                installed = new Version(0, 0, 0);
                body = Resources.UpgrateVersionCheckErrorMessage;
            }
            if (remote == null) {
                remote = new Version(0, 0, 0);
                body = Resources.UpgrateVersionCheckErrorMessage;
            }
            this.InitializeComponent();
            labelInstalledVersion.Text = SCaddins.Properties.Resources.InstalledVersion + @": " + installed.ToString();
            labelLatestVersion.Text = SCaddins.Properties.Resources.LatestVersion + @": "  + remote.ToString();
            textBox1.Text = body;
            if (installed < remote) {
                labelUpgradeNote.Text = Resources.UpgrateNewVersionAvailableMessage;
                buttonDownload.Enabled = true;
                buttonLog.Enabled = true;
            } else {
                labelUpgradeNote.Text = Resources.UpgradeUpToDateMessage;
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
