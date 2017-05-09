// (C) Copyright 2013-2015 by Andrew Nicholas
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
    using Autodesk.Revit.DB;

    public partial class RevisionSelectionDialog : System.Windows.Forms.Form
    {
        private ElementId id;
        private Document doc;

        public RevisionSelectionDialog(Document doc)
        {
            this.doc = doc;
            this.id = null;
            this.InitializeComponent();
            this.PopulateList();
        }

        public ElementId Id {
            get { return this.id; }
        }

        private void PopulateList()
        {
            using (var collector = new FilteredElementCollector(this.doc)) {
                collector.OfCategory(BuiltInCategory.OST_Revisions);
                foreach (Element e in collector) {
                    var item = new ListViewItem();
                    item.Text = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsValueString();
                    item.SubItems.Add(e.get_Parameter(BuiltInParameter.PROJECT_REVISION_ENUMERATION).AsString());
                    item.SubItems.Add(e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DESCRIPTION).AsString());
                    item.SubItems.Add(e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString());
                    item.SubItems.Add(e.Id.ToString());
                    item.Tag = e.Id;
                    this.listView1.Items.Add(item);
                }
            }
        }

        private void ListView1SelectedIndexChanged(object sender, EventArgs e)
        {
            var i  = this.listView1.SelectedIndices;
            if (i.Count > 0) {
                ListViewItem item = this.listView1.Items[i[0]];
                this.id = item.Tag as ElementId;
            } else {
                this.id = null;
            }
        }
    }
}
