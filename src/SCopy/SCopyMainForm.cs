// (C) Copyright 2014 by Andrew Nicholas
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

namespace SCaddins.SCopy
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        private string origVal;
        private IList<SCopy> scopies;

        public MainForm(Document doc, Autodesk.Revit.DB.ViewSheet viewSheet)
        {
            this.InitializeComponent();
            this.SetTitle();
            var s = new SCopy(doc, viewSheet);
            this.scopies.Add(s);
            s.AddViewInfoToList(ref this.listView1);
            this.AddDataGridColumns();
        }
        
        public MainForm(Document doc,ICollection<SCaddins.SCexport.SCexportSheet> sheets)
        {
            this.InitializeComponent();
            this.SetTitle();  
            var s = new SCopy(doc, sheet.Sheet);
            foreach (SCaddins.SCexport.SCexportSheet sheet in sheets) {
                
                this.scopies.Add(s);
                s.AddViewInfoToList(ref this.listView1);
            }               
            this.AddDataGridColumns();            
        }
    
        #region init component
    
        private void AddDataGridColumns()
        {
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView2.AutoGenerateColumns = false;
            this.AddColumn("Number", "Number", this.dataGridView1);
            this.AddColumn("Title", "Title", this.dataGridView1);
            this.AddColumn("OriginalTitle", "Original Title", this.dataGridView2);
            this.AddColumn("Title", "Proposed Title", this.dataGridView2);
            this.AddComboBoxColumns();
            this.AddColumn("RevitViewType", "View Type", this.dataGridView2);
            this.AddCheckBoxColumn(
                "DuplicateWithDetailing", "Copy Detailing", this.dataGridView2); 
        }

        private void AddCheckBoxColumn(string name, string text, DataGridView grid)
        {
            var result = new DataGridViewCheckBoxColumn();
            this.AddColumnHeader(name, text, result);
            grid.Columns.Add(result);
        }
    
        private void AddColumnHeader(
            string name, string text, DataGridViewColumn column)
        {
            column.HeaderText = text;
            column.DataPropertyName = name;
        }
    
        private DataGridViewComboBoxColumn CreateComboBoxColumn()
        {
            var result = new DataGridViewComboBoxColumn();
            result.FlatStyle = FlatStyle.Flat;
            return result;        
        }

        private void AddComboBoxColumns()
        {
            DataGridViewComboBoxColumn result2 = this.CreateComboBoxColumn();
            this.AddColumnHeader("ViewTemplateName", "View Template", result2);
            result2.Items.Add(SCopyConstants.MenuItemCopy);
            var sc = dataGridView1.SelectedRows[0].DataBoundItem as SCopy;
            foreach (string s2 in sc.ViewTemplates.Keys) {
                result2.Items.Add(s2);
            }
            dataGridView2.Columns.Add(result2);
        
            DataGridViewComboBoxColumn result = this.CreateComboBoxColumn();
            this.AddColumnHeader("AssociatedLevelName", "Associated Level", result);
            result.Items.Add(SCopyConstants.MenuItemCopy);
            foreach (string s in sc.Levels.Keys) {
                result.Items.Add(s);
            }
            dataGridView2.Columns.Add(result);
        }

        private void AddColumn(string name, string text, DataGridView grid)
        {
            var result = new DataGridViewTextBoxColumn();
            this.AddColumnHeader(name, text, result);
            grid.Columns.Add(result);
        }

        private void SetTitle()
        {
            this.Text = "SCopy by Andrew Nicholas";
        }
    
        #endregion

        private void ButtonGO(object sender, EventArgs e)
        {
            foreach(SCopy sc in dataGridView1.Rows){
                sc.CreateSheets();
            }
            this.Dispose();
            this.Close();
        }

        private void ButtonAdd(object sender, EventArgs e)
        {
            buttonRemove.Enabled = true;
            this.scopies.AddCopy();
            dataGridView1.DataSource = this.scopies.Sheets;
        }

        private void DataGridView1CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var sheet = dataGridView1.Rows[e.RowIndex].DataBoundItem as SCopySheet;
            dataGridView2.DataSource = sheet.ViewsOnSheet;
            dataGridView2.Refresh();
        }

        private void DataGridView1CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) {
                return;
            }
            DataGridViewCell cell = dataGridView1[e.ColumnIndex, e.RowIndex];
            this.origVal = cell.Value.ToString();
        }
        
        private void DataGridView2CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) {
                return;
            }
            if (e.ColumnIndex == 2) {
                var cell = (DataGridViewComboBoxCell)dataGridView2[e.ColumnIndex, e.RowIndex];
                var viewOnSheet =
                    dataGridView2.Rows[e.RowIndex].DataBoundItem as SCopyViewOnSheet;
                if (viewOnSheet.OldView.ViewType != ViewType.FloorPlan) {
                    cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                    cell.ReadOnly = true;
                } else {
                    cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                    cell.ReadOnly = false;
                }
            }
        }

        private void ButtonRemoveClick(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows) {
                var sheet = row.DataBoundItem as SCopySheet;
                this.scopies.Sheets.Remove(sheet);
            }
            if (dataGridView1.Rows.Count == 0) {
                dataGridView2.Rows.Clear();
            }
            dataGridView1.Refresh();
            dataGridView2.Refresh();
        }

        private void DataGridView1SelectionChanged(object sender, EventArgs e)
        {
            buttonRemove.Enabled = dataGridView1.SelectedRows.Count > 0;
        }
           
        private void DataGridView2SelectionChanged(object sender, EventArgs e)
        {
            bool planEnough = true;
            foreach (DataGridViewRow row in dataGridView2.SelectedRows) {
                var view = row.DataBoundItem as SCopyViewOnSheet;
                if (!view.PlanEnough()) {
                    planEnough = false;       
                }
            }
            buttonReplace.Enabled = (dataGridView2.SelectedRows.Count == 1) && planEnough;
        }
        
        private void ButtonCopyClick(object sender, EventArgs e)
        {  
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
