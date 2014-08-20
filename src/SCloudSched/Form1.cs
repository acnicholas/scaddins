namespace SCaddins.SCloudSChed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    using SCaddins.Common;

    public partial class Form1 : System.Windows.Forms.Form
    {
        Document doc;

        public Form1(Document doc, SortableBindingList<RevisionItem> revisions)
        {
            this.doc = doc;
            InitializeComponent();
            SetIcon();
            dataGridView1.DataSource = revisions;
        }

        private void SetIcon()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream st = a.GetManifestResourceStream("SCloudSChed.Resources.scloudsched.ico");
            System.Drawing.Icon icnTask = new System.Drawing.Icon(st);
            this.Icon = icnTask;
        }

        private void SelectAll(bool all)
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RevisionItem rev = row.DataBoundItem as RevisionItem;
                if (all) {
                    if (!rev.Export) {
                        rev.Export = true;
                    }
                } else {
                    if (rev.Export) {
                        rev.Export = false;
                    }
                }
            }
            dataGridView1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectAll(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectAll(false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Dictionary<string, RevisionItem> dictionary = new Dictionary<string, RevisionItem>();
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RevisionItem rev = row.DataBoundItem as RevisionItem;
                string s = rev.Date + rev.Description;
                if (!dictionary.ContainsKey(s)) {
                    dictionary.Add(s, rev);
                }
            }
            SCloudSched.exportCloudInfo(doc, dictionary);
        }
    }
}
