// (C) Copyright 2015 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System;
    using System.Windows.Forms;
    using SCaddins.Properties;

    public partial class ExportLogDialog : Form
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
        public ExportLogDialog(ExportLog log)
        {
            if (log == null) {
                log = new ExportLog();
                log.AddError(null, Resources.ExportLogInitializationError);
            }

            this.InitializeComponent();
            foreach (ExportLogItem errorItem in log.ErrorLog) {
                errors.Items.Add(
                    new ListViewItem(new string[] { errorItem.Filename, errorItem.Description }));
            }
            foreach (ExportLogItem warningItem in log.WarningLog) {
                warnings.Items.Add(
                    new ListViewItem(new string[] { warningItem.Filename, warningItem.Description }));
            }

            textBox1.Text = log.FullOutputLog;
            textBox1.ScrollToCaret();
            Tabs.TabPages[1].Text = log.Warnings + @" " + Resources.Warnings;
            Tabs.TabPages[2].Text = log.Errors + @" " + Resources.Errors;
            label1.Text = log.TotalExports + @" " + Resources.LogDialogExportsAttempted + @" " + log.Errors + @" " + Resources.Errors;
        }
    }
}
