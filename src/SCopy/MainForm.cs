namespace SCaddins.SCopy
{
    using System;
    using System.Windows.Forms;
    using System.Reflection;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        private string origVal;
        private SCopy scopy;

        public MainForm(Document doc, Autodesk.Revit.DB.ViewSheet viewSheet)
        {
            this.InitializeComponent();
            this.SetTitle();
            this.scopy = new SCopy(doc, viewSheet);
            this.scopy.AddViewInfoToList(ref this.listView1);
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
            DataGridViewCheckBoxColumn result = new DataGridViewCheckBoxColumn();
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
            DataGridViewComboBoxColumn result = new DataGridViewComboBoxColumn();
            result.FlatStyle = FlatStyle.Flat;
            return result;        
        }

        private void AddComboBoxColumns()
        {
            DataGridViewComboBoxColumn result2 = this.CreateComboBoxColumn();
            this.AddColumnHeader("ViewTemplateName", "View Template", result2);
            result2.Items.Add(SCopyConstants.MenuItemCopy);
            foreach (string s2 in this.scopy.ViewTemplates.Keys) {
                result2.Items.Add(s2);
            }
            dataGridView2.Columns.Add(result2);
        
            DataGridViewComboBoxColumn result = this.CreateComboBoxColumn();
            this.AddColumnHeader("AssociatedLevelName", "Associated Level", result);
            result.Items.Add(SCopyConstants.MenuItemCopy);
            foreach (string s in this.scopy.Levels.Keys) {
                result.Items.Add(s);
            }
            dataGridView2.Columns.Add(result);
        }

        private void AddColumn(string name, string text, DataGridView grid)
        {
            DataGridViewTextBoxColumn result = new DataGridViewTextBoxColumn();
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
            this.scopy.CreateSheets();
            this.Dispose();
            this.Close();
        }

        private void ButtonAdd(object sender, EventArgs e)
        {
            buttonRemove.Enabled = true;
            this.scopy.Add();
            dataGridView1.DataSource = this.scopy.Sheets;
        }

        private void DataGridView1CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SCopySheet sheet =
                dataGridView1.Rows[e.RowIndex].DataBoundItem as SCopySheet;
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
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView2[e.ColumnIndex, e.RowIndex];
                SCopyViewOnSheet viewOnSheet =
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
                SCopySheet sheet =
                    row.DataBoundItem as SCopySheet;
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
        }
        
        private void ButtonReplaceClick(object sender, EventArgs e)
        {
            // display a list of plans in the model.
            SCopyViewSelectionDialog vd = new SCopyViewSelectionDialog();
            foreach (Autodesk.Revit.DB.View v in this.scopy.ExistingViews.Values) {
                Parameter p2 = v.get_Parameter("Sheet Number");
                if (p2 == null) {
                    if (SCopyViewOnSheet.PlanEnough(v.ViewType) && !v.IsTemplate) {
                        vd.Add(v);
                    }
                }
            }
            vd.ShowDialog();
            string test = vd.SelectedView();
        
            Autodesk.Revit.DB.View testView;
            if (this.scopy.ExistingViews.TryGetValue(test, out testView)) {
                TaskDialog.Show("DEBUG", @"View OK");            
            }
             
            // should only be one!
            foreach (DataGridViewRow row in dataGridView2.SelectedRows) {
                SCopyViewOnSheet viewOnSheet =
                    row.DataBoundItem as SCopyViewOnSheet;
                TaskDialog.Show("DEBUG", @"TODO: add (" + test + @") to sheet ");
                
                // TODO crop view to match src view
                // get the current sheet
                // remove the old view
                // scopy.ExistingViews.Remove(viewOnSheet);
                // replace it with the new view
                // scopy.ExistingViews.Add(new SCopySheet(        
            }
        }
        
        private void DataGridView2SelectionChanged(object sender, EventArgs e)
        {
            bool planEnough = true;
            foreach (DataGridViewRow row in dataGridView2.SelectedRows) {
                SCopyViewOnSheet view =
                    row.DataBoundItem as SCopyViewOnSheet;
                if (!view.PlanEnough()) {
                    planEnough = false;       
                }
            }
            buttonReplace.Enabled = (dataGridView2.SelectedRows.Count == 1) && planEnough;
        }       
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
