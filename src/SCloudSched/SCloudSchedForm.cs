// (C) Copyright 2013-2014 by Andrew Nicholas
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
        private Document doc;

        public Form1(Document doc, SortableBindingList<RevisionItem> revisions)
        {
            this.doc = doc;
            this.InitializeComponent();
            dataGridView1.DataSource = revisions;
        }

        private void SelectAll(bool all)
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RevisionItem rev = row.DataBoundItem as RevisionItem;
                if (rev != null) {
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
            }
            dataGridView1.Refresh();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.SelectAll(true);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
             this.SelectAll(false);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Dictionary<string, RevisionItem> dictionary = new Dictionary<string, RevisionItem>();
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RevisionItem rev = row.DataBoundItem as RevisionItem;
                if (rev != null) {
                    string s = rev.Date + rev.Description;
                    if (!dictionary.ContainsKey(s)) {
                        dictionary.Add(s, rev);
                    }
                }
            }
            SCloudSched.ExportCloudInfo(this.doc, dictionary);
        }
    }
}
