// (C) Copyright 2014-2015 by Andrew Nicholas
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

namespace SCaddins.SheetCopier
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        private SheetCopierManager scopy;
        private Document doc;
        private DataGridViewComboBoxColumn cheetCategoryCombo;

        public MainForm(Document doc, Autodesk.Revit.DB.ViewSheet viewSheet, SheetCopierManager scopy)
        {
            this.doc = doc;
            this.InitializeComponent();
            this.SetTitle();
            this.scopy = scopy;
            this.PopulateViewInfoList(viewSheet);
            this.AddDataGridColumns();
            dataGridView1.DataSource = this.scopy.Sheets; 
            this.dataGridView1.CellValueChanged += this.DataGridView1_CellValueChanged;
            this.dataGridView1.CurrentCellDirtyStateChanged += this.DataGridView1_CurrentCellDirtyStateChanged;
        }
        
        public MainForm(
            Document doc,
            ICollection<SCaddins.ExportManager.ExportSheet> sheets, SheetCopierManager scopy)
        {
            if (sheets == null) {
                throw new ArgumentNullException("sheets");
            }
            
            if (scopy == null) {
                throw new ArgumentNullException("scopy");
            }

            this.scopy = scopy;
            this.doc = doc;
            this.InitializeComponent();
            this.SetTitle();
            this.scopy = scopy;
            foreach (SCaddins.ExportManager.ExportSheet sheet in sheets) {
                this.scopy.AddSheet(sheet.Sheet);
            } 
            this.AddDataGridColumns();  
            dataGridView1.DataSource = this.scopy.Sheets; 
            this.dataGridView1.CellValueChanged += this.DataGridView1_CellValueChanged;
            this.dataGridView1.CurrentCellDirtyStateChanged += this.DataGridView1_CurrentCellDirtyStateChanged;
        }
        
        /// <summary>
        /// Add some nice data about a Revit view to a list.
        /// </summary>
        public void PopulateViewInfoList(ViewSheet viewSheet)
        {
            if (viewSheet == null) {
                return;
            }
            this.listView1.Items.Clear();
            var colour = System.Drawing.Color.Gray;
            this.AddItemToViewInfoList("Title", viewSheet.Name, colour, 0);
            this.AddItemToViewInfoList("Sheet Number", viewSheet.SheetNumber, colour, 0);
            this.AddViewsToViewInfoList(viewSheet.GetAllPlacedViews());
            this.listView1.Refresh();
        }
        
        private static void AddCheckBoxColumn(string name, string text, DataGridView grid)
        {
            var result = new DataGridViewCheckBoxColumn();
            AddColumnHeader(name, text, result);
            grid.Columns.Add(result);
        }
    
        private static void AddColumnHeader(
            string name, string text, DataGridViewColumn column)
        {
            column.HeaderText = text;
            column.DataPropertyName = name;
        }
    
        private static DataGridViewComboBoxColumn CreateComboBoxColumn()
        {
            var result = new DataGridViewComboBoxColumn();
            result.FlatStyle = FlatStyle.Flat;
            return result;        
        }
        
        public static void AddColumn(string name, string text, DataGridView grid, bool visible)
        {
            if (grid != null) {
                var result = new DataGridViewTextBoxColumn();
                result.Visible = visible;
                AddColumnHeader(name, text, result);
                grid.Columns.Add(result);
            }
        }
        
        public static void AddColumn(string name, string text, DataGridView grid)
        {
            AddColumn(name, text, grid, true);
        }
               
        private void AddViewsToViewInfoList(ViewSet views)
        {
            this.AddItemToViewInfoList(
                "Number of viewports",
                views.Size.ToString(CultureInfo.InvariantCulture),
                System.Drawing.Color.Gray,
                1);
            int i = 1;
            foreach (Autodesk.Revit.DB.View view in views) {
               this.AddItemToViewInfoList(
                    "View: " + i,
                    view.Name,
                    System.Drawing.Color.Black,
                    1);
                i++;
            }
        }
        
         private void AddItemToViewInfoList(
            string title,
            string value,
            System.Drawing.Color colour,
            int group)
        {
            System.Windows.Forms.ListViewItem item;
            item = new System.Windows.Forms.ListViewItem(new[] { title, value }, this.listView1.Groups[group]);
            item.ForeColor = colour;
            listView1.Items.Add(item);
        }
        
        private void AddViewsToViewInfoList(ISet<ElementId> views)
        {
            this.AddItemToViewInfoList(
                "Number of viewports",
                views.Count.ToString(CultureInfo.InvariantCulture),
                System.Drawing.Color.Gray,
                1);
            int i = 1;
            foreach (ElementId id in views) {
                var view = this.doc.GetElement(id) as Autodesk.Revit.DB.View;
                this.AddItemToViewInfoList(
                    "View: " + i,
                    view.Name,
                    System.Drawing.Color.Black,
                    1);
                i++;
            }
        }
  
        #region init component

        private void AddDataGridColumns()
        {
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView2.AutoGenerateColumns = false;
                      
            AddColumn("Number", "Number", this.dataGridView1);
            AddColumn("Title", "Title", this.dataGridView1);     
            AddColumn("OriginalTitle", "Original Title", this.dataGridView2);
            AddColumn("Title", "Proposed Title", this.dataGridView2);
            this.AddComboBoxColumns();
            AddColumn("RevitViewType", "View Type", this.dataGridView2);
            AddCheckBoxColumn(
                "DuplicateWithDetailing", "Copy Detailing", this.dataGridView2); 
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var cell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[2];
            if (cell.Value != null && (string)cell.Value == SheetCopierConstants.SheetCategoryCreateCustom) {
                var form = new SCopyTextInputForm();
                    System.Windows.Forms.DialogResult dr = form.ShowDialog();
                    if (dr == System.Windows.Forms.DialogResult.OK) {
                        this.cheetCategoryCombo.Items.Add(form.textBox1.Text);
                        dataGridView1.Rows[e.RowIndex].Cells[2].Value = form.textBox1.Text;
                    }
                dataGridView1.Invalidate();
                dataGridView1.EndEdit();
            }
        }
        
        private void DataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex != 2) {
                return;
            }
            if (this.dataGridView1.IsCurrentCellDirty) {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void AddComboBoxColumns()
        {
            this.cheetCategoryCombo = CreateComboBoxColumn();
            AddColumnHeader("SheetCategory", "Sheet Category", this.cheetCategoryCombo);
            this.cheetCategoryCombo.Items.Add(SheetCopierConstants.SheetCategoryCreateCustom);
            foreach (string s in this.scopy.SheetCategories) {
                this.cheetCategoryCombo.Items.Add(s);
            }
            dataGridView1.Columns.Add(this.cheetCategoryCombo);
            
            DataGridViewComboBoxColumn result2 = CreateComboBoxColumn();
            AddColumnHeader("ViewTemplateName", "View Template", result2);
            result2.Items.Add(SheetCopierConstants.MenuItemCopy);
            var sc = this.scopy;
            foreach (string s2 in sc.ViewTemplates.Keys) {
                result2.Items.Add(s2);
            }
            dataGridView2.Columns.Add(result2);
        
            DataGridViewComboBoxColumn result = CreateComboBoxColumn();
            AddColumnHeader("AssociatedLevelName", "Associated Level", result);
            result.Items.Add(SheetCopierConstants.MenuItemCopy);
            foreach (string s in sc.Levels.Keys) {
                result.Items.Add(s);
            }
            dataGridView2.Columns.Add(result);
        }

        private void SetTitle()
        {
            this.Text = "Sheet Copier by Andrew Nicholas";
        }
    
        #endregion

        private void ButtonGO(object sender, EventArgs e)
        {
            // this.Dispose();
            this.Close();
        }

        private void ButtonAdd(object sender, EventArgs e)
        {
            var view = this.doc.ActiveView;
            if (view == null) {
                return;
            }
            var viewSheet = SheetCopierManager.ViewToViewSheet(view);
            if (viewSheet != null)
            {
                this.scopy.AddSheet(viewSheet);
                dataGridView1.DataSource = this.scopy.Sheets;
            }
        }

        private void DataGridView1CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1) { 
                 return;
            }                 
            
            var sheet = (SheetCopierSheet)dataGridView1.Rows[e.RowIndex].DataBoundItem;
            this.PopulateViewInfoList(sheet.SourceSheet);
            dataGridView2.DataSource = sheet.ViewsOnSheet;
            dataGridView2.Refresh();
        }

        private void DataGridView1CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) {
                return;
            }
        }
        
        private void DataGridView2CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) {
                return;
            }
            if (e.ColumnIndex == 2) {
                var cell = (DataGridViewComboBoxCell)dataGridView2[e.ColumnIndex, e.RowIndex];
                var viewOnSheet =
                    dataGridView2.Rows[e.RowIndex].DataBoundItem as SheetCopierViewOnSheet;
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
                var sheet = row.DataBoundItem as SheetCopierSheet;
                this.scopy.Sheets.Remove(sheet);
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
            button1.Enabled = dataGridView1.SelectedRows.Count > 0;
        }
        
        private void DataGridView2SelectionChanged(object sender, EventArgs e)
        {
            buttonRemoveView.Enabled = dataGridView2.SelectedRows.Count > 0;
        }
          
        private void Button1Click(object sender, EventArgs e)
        {
            var sheet = (SheetCopierSheet)dataGridView1.SelectedRows[0].DataBoundItem;
            this.scopy.AddSheet(sheet.SourceSheet);
        }
        
        private void DataGridView1DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // FIXME this is a hack because I have no idea what I'm doing.  
        }

        private void ReplaceTextregexToolStripMenuItemClick(object sender, EventArgs e)
        {
            SCaddins.ExportManager.RenameSheetForm renameForm = new SCaddins.ExportManager.RenameSheetForm(this.scopy.Sheets, this.doc);
            renameForm.ShowDialog();
            dataGridView1.Refresh();
        }
        
        private void Button2Click(object sender, EventArgs e)
        {
            SCaddins.ExportManager.RenameSheetForm renameForm = new SCaddins.ExportManager.RenameSheetForm(this.scopy.Sheets, this.doc);
            renameForm.ShowDialog();
            dataGridView1.Refresh();  
        }
        
        private void ButtonRemoveViewClick(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView2.SelectedRows) {
                var view = row.DataBoundItem as SheetCopierViewOnSheet; 
                foreach (DataGridViewRow row2 in dataGridView1.SelectedRows) {
                    var sheet = row2.DataBoundItem as SheetCopierSheet;
                    sheet.ViewsOnSheet.Remove(view);
                }
            }  
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
