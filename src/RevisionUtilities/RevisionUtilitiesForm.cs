// (C) Copyright 2013-2016 by Andrew Nicholas
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

namespace SCaddins.RevisionUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    using SCaddins.Properties;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public partial class RevisionUtilitiesForm : System.Windows.Forms.Form
    {
        private Document doc;

        public RevisionUtilitiesForm(Document doc)
        {
            this.doc = doc;
            this.InitializeComponent();
            this.RefreshDataGridView();
        }

        private void SelectAll(bool all)
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RevisionItem rev = row.DataBoundItem as RevisionItem;
                if (rev != null) {
                    rev.Export = all;
                }
            }
            dataGridView1.Refresh();
        }

        private void ButtonSelectAll_Click(object sender, EventArgs e)
        {
            this.SelectAll(true);
        }

        private void ButtonSelectNone_Click(object sender, EventArgs e)
        {
             this.SelectAll(false);
        }
        
        private void ButtonScheduleRevisionsClick(object sender, EventArgs e)
        {         
            var dictionary = new Dictionary<string, RevisionItem>();
            foreach (RevisionItem rev in this.SelectedRevisionItems()) {
                if (rev != null) {
                    string s = rev.Date + rev.Description;
                    if (!dictionary.ContainsKey(s)) {
                        dictionary.Add(s, rev);
                    }
                }
            }
            var saveFileDialogResult = saveFileDialog1.ShowDialog();
            
            var saveFileName = string.Empty;
            if (saveFileDialogResult == DialogResult.OK) {
                saveFileName = saveFileDialog1.FileName;
            }
            RevisionUtilities.ExportCloudInfo(this.doc, dictionary, saveFileName);
        }
                     
        private void ButtonAssignRevisionsClick(object sender, EventArgs e)
        { 
            RevisionUtilities.AssignRevisionToClouds(doc, SelectedRevisionCloudItems());
            RefreshDataGridView();
        }
        
        private void ButtonDeleteRevisionsClick(object sender, EventArgs e)
        {
            RevisionUtilities.DeleteRevisionClouds(doc, SelectedRevisionCloudItems());
            RefreshDataGridView();  
        }
        
        private Collection<RevisionCloudItem> SelectedRevisionCloudItems()
        {
           Collection<RevisionCloudItem> cloudSelection = new Collection<RevisionCloudItem>();
           foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RevisionCloudItem rev = row.DataBoundItem as RevisionCloudItem;
                if (rev.Export) {
                    cloudSelection.Add(rev);
                } 
            } 
           return cloudSelection;
        }
        
        private Collection<RevisionItem> SelectedRevisionItems()
        {
           Collection<RevisionItem> selection = new Collection<RevisionItem>();
           foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RevisionItem rev = (RevisionItem)row.DataBoundItem;
                if (rev.Export) {
                    selection.Add(rev);
                } 
            } 
           return selection;
        }
        
        private void RefreshDataGridView()
        {
            if (radioButtonRevisions.Checked) {  
                dataGridView1.DataSource = RevisionUtilities.GetRevisions(doc);                  
            } else {
                dataGridView1.DataSource = RevisionUtilities.GetRevisionClouds(doc); 
                AdjustColumnOrder();
            }
            this.dataGridView1.Refresh();
        }
        
        private void AdjustColumnOrder()
        {
            // customersDataGridView.Columns["CustomerID"].Visible = false;
            dataGridView1.Columns["Export"].DisplayIndex = 0;
            dataGridView1.Columns["Description"].DisplayIndex = 1;
            dataGridView1.Columns["Date"].DisplayIndex = 2;
            dataGridView1.Columns["SheetNumber"].DisplayIndex = 3;
            dataGridView1.Columns["Revision"].DisplayIndex = 4;
            dataGridView1.Columns["SheetName"].DisplayIndex = 5;
            dataGridView1.Columns["HostViewName"].DisplayIndex = 6;
            dataGridView1.Columns["Mark"].DisplayIndex = 7;
            dataGridView1.Columns["Comments"].DisplayIndex = 8;
            dataGridView1.Columns["Id"].DisplayIndex = 9;
            dataGridView1.Columns["Sequence"].DisplayIndex = 10;
            dataGridView1.Columns["Issued"].DisplayIndex = 11;
        }
        
        private void RadioButtonRevisionsCheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRevisions.Checked) {
                this.labelDataGridTitle.Text = Resources.SelectRevisionToSchedule;
            }
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked) {
                this.buttonAssignRevisons.Enabled = false;
                this.buttonDeleteRevisions.Enabled = false;
                RefreshDataGridView();
            }
        }
        
        private void RadioButtonCloudsCheckedChanged(object sender, EventArgs e)
        {
             if (radioButtonClouds.Checked) {
                this.labelDataGridTitle.Text = Resources.SelectRevisionCloudToScheduleDeleteOrAssign;
            }
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked) {
                this.buttonAssignRevisons.Enabled = true;
                this.buttonDeleteRevisions.Enabled = true;
                RefreshDataGridView();
            }
        }
    }
}
