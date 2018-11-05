using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCaddins.ExportManager.Views
{
    public partial class ProgressDialog : Form
    {
        public ProgressDialog(int max)
        {
            InitializeComponent();
            progressBar1.Maximum = max;
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            //this.TopMost = true;
        }

        public void Run(ExportManager manager, List<ExportSheet> sheets, ExportLog log)
        {
            foreach (var sheet in sheets)
            {
                Step();
                BringToFront();
                Refresh();
                manager.ExportSheet(sheet, log);
                //TryHideAcrobatProgress();
            }
        }

        public void Step()
        {
            progressBar1.PerformStep();
            label1.Text = "Exporting " + progressBar1.Value + " of " + progressBar1.Maximum;
        }     
    }
}
