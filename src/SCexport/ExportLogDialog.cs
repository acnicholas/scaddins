namespace SCaddins.SCexport
{  
    using System;
    using System.Windows.Forms;
    
    /// <summary>
    /// Description of ExportLogDialog.
    /// </summary>
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
            tabControl1.TabPages[0].Text = log.Messages + " Messages";
            tabControl1.TabPages[1].Text = log.Warnings + " Warnings";
            tabControl1.TabPages[2].Text = log.Errors + " Errors";
            label1.Text = log.TotalExports + " Exports attempted with " + log.Errors + " errors.";
        }
    }
}
