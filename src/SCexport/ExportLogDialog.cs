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

namespace SCaddins.SCexport
{
    using System;
    using System.Windows.Forms;

    public partial class ExportLogDialog : Form
    {
        public ExportLogDialog(ExportLog log)
        {
            this.InitializeComponent();
            foreach (ExportLogItem errorItem in log.ErrorLog) {
                errors.Items.Add(
                    new ListViewItem(new string[] { errorItem.Filename, errorItem.Description }));
            }
            foreach (ExportLogItem warningItem in log.WarningLog) {
                warnings.Items.Add(
                    new ListViewItem(new string[] { warningItem.Filename, warningItem.Description }));
            }
            foreach (ExportLogItem messageItem in log.MessageLog) {
                messages.Items.Add(
                    new ListViewItem(new string[] { messageItem.Filename, messageItem.Description }));
            }
            textBox1.Text = log.FullOutputLog;
            Tabs.TabPages[0].Text = log.Messages + " Messages";
            Tabs.TabPages[1].Text = log.Warnings + " Warnings";
            Tabs.TabPages[2].Text = log.Errors + " Errors";
            label1.Text = log.TotalExports + " Exports attempted with " + log.Errors + " errors.";
        }
    }
}
