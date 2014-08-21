using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SCaddins.SCulcase
{
    public partial class SCulcaseInfoDialog : Form
    {
        public SCulcaseInfoDialog()
        {
            InitializeComponent();
        }

        public void SetText(string s)
        {
            textBox1.Clear();
            textBox1.Text = s;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
