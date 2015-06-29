using System;
using System.Drawing;
using System.Windows.Forms;

namespace SCaddins.SCexport
{
    /// <summary>
    /// Description of ExportLogDialog.
    /// </summary>
    public partial class ExportLogDialog : Form
    {
        public ExportLogDialog(ExportLog log)
        {
            InitializeComponent();
            foreach ( string s in log.ErrorLog)
                errors.Items.Add(s);
        }
    }
}
