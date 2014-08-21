namespace SCaddins.SCulcase
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    
    public partial class SCulcaseInfoDialog : Form
    {
        public SCulcaseInfoDialog()
        {
            this.InitializeComponent();
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
