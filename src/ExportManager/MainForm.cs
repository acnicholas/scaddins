// (C) Copyright 2012-2016 by Andrew Nicholas
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Autodesk.Revit.UI;
    using SCaddins.Common;

    public partial class MainForm : Form
    {
        private ExportManager scx;
        private Autodesk.Revit.DB.Document doc;
        private UIDocument udoc;
        private FilterContextMenu filter;
        private MenuButton printButton;

        public MainForm(UIDocument udoc)
        {
            this.udoc = udoc;
            this.doc = udoc.Document;
            this.scx = new ExportManager(this.doc);
            this.filter = new FilterContextMenu("Filter", -1, null);
            this.InitializeComponent();
            this.printButton = new MenuButton(printButtonContextMenu);
            this.InitPrintButton();
            this.Controls.Add(this.printButton);
            var findTip = new ToolTip();
            var findTipText = "Use regular expressions to filter the sheet list" +
                Environment.NewLine +
                "Searches both sheet name and number.";
            findTip.SetToolTip(this.btnFind, findTipText);
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.PopulateViewSheetSetCombo();
            this.PopulateColumns();
            this.PopulateList(new ViewSheetSetCombo("<All views>")); 
            this.dataGridView1.Sort(this.dataGridView1.Columns[1], ListSortDirection.Ascending);
            this.dataGridView1.MultiSelect = true;
            this.KeyPreview = true;
            this.UpdateExportButton(0);
            this.dataGridView1.Focus();
            this.dataGridView1.Select();
        }

        public void OpenSelectedViewToolStripMenuItemClick(
                object sender, EventArgs e)
        {
            DialogHandler.AddRevitDialogHandler(new UIApplication(this.udoc.Application.Application));
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows) {
                 var sc = row.DataBoundItem as ExportSheet;
                if (sc == null) {
                    return;
                }
                 Autodesk.Revit.DB.FamilyInstance result =
                     ExportManager.TitleBlockInstanceFromSheetNumber(sc.SheetNumber, this.doc);
                if (result != null) {
                    this.udoc.ShowElements(result);
                }
            }

            this.Close();
        }

        private static void ShowHelp()
        {
            string s =
                "A\t    Select all" + System.Environment.NewLine +
                "C\t    Clear current filter" + System.Environment.NewLine +
                "J\t    Move selected row down" + System.Environment.NewLine +
                "K\t    Move selected row up" + System.Environment.NewLine +
                "L\t    Select *Latest* revision only" + System.Environment.NewLine +
                "N\t    Select none" + System.Environment.NewLine +
                "O\t    Open Selected Sheets" + System.Environment.NewLine +
                "P\t    Preliminary issue(date revision)" + System.Environment.NewLine +
                "Q\t    Quit" + System.Environment.NewLine +
                "S\t    Select current sheet only" + System.Environment.NewLine +
                "T\t    Tip of the day" + System.Environment.NewLine +
                "V\t    Verify selected sheets" + System.Environment.NewLine +
                "X\t    Start Export" + System.Environment.NewLine + System.Environment.NewLine +
                "0-9\t  Filter main view by sheet number" + System.Environment.NewLine + System.Environment.NewLine +
                "?\t    Help, show keyboard shortcuts (this dialog)" + System.Environment.NewLine +
                "/\t    Advanced search" + System.Environment.NewLine;
            var td = new TaskDialog("Keyboard Shortcuts");
            td.MainInstruction = "Keyboard Shortcuts";
            td.MainContent = s;
            td.Show();
        }
        
        private void InitPrintButton()
        {
            this.printButton.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.printButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.printButton.Location = new System.Drawing.Point(635, 504);
            this.printButton.Name = "btnPrint";
            this.printButton.Size = new System.Drawing.Size(55, 22);
            this.printButton.TabIndex = 13;
            this.printButton.Text = "Print";
            this.printButton.UseVisualStyleBackColor = true;
        }

        private void PopulateViewSheetSetCombo()
        {
            var allViews = new ViewSheetSetCombo("<All views>");
            this.cmbPrintSet.Items.Add(allViews);
            foreach (ViewSheetSetCombo s in this.scx.AllViewSheetSets) {
                this.cmbPrintSet.Items.Add(s);
            }
        }

        private void PopulateList(FilterContextMenu f)
        {
            switch (f.Column) {
                case 1:
                    var bs1 = new SortableBindingListCollection<ExportSheet>(
                                  this.scx.AllSheets.Where(
                                      obj => obj.SheetNumber.StartsWith(f.Filter, StringComparison.CurrentCulture) == true).ToList());
                    this.dataGridView1.DataSource = bs1;
                    break;
                case 3:
                    var bs3 = new SortableBindingListCollection<ExportSheet>(
                                  this.scx.AllSheets.Where(
                                      obj => obj.SheetRevision.StartsWith(f.Filter, StringComparison.CurrentCulture) == true).ToList());
                    this.dataGridView1.DataSource = bs3;
                    break;
                case 5:
                    var bs5 = new SortableBindingListCollection<ExportSheet>(
                                  this.scx.AllSheets.Where(
                                      obj => obj.SheetRevisionDate.StartsWith(f.Filter, StringComparison.CurrentCulture) == true).ToList());
                    this.dataGridView1.DataSource = bs5;
                    break;
            }
        }

        private void PopulateList(string s)
        {
            var bs = new SortableBindingListCollection<ExportSheet>(
                    this.scx.AllSheets.Where(obj => obj.SheetNumber
                        .StartsWith(s, StringComparison.CurrentCulture) || 
                        obj.SheetNumber.StartsWith("DA" + s, StringComparison.CurrentCulture) == true).ToList());
            this.dataGridView1.DataSource = bs;
        }

        private void PopulateList()
        {
            this.dataGridView1.DataSource = this.scx.AllSheets;
        }

        private void PopulateList(ViewSheetSetCombo vss)
        {
            if (vss.CustomName == "<All views>") {
                this.PopulateList();
            } else {
                var bs = new SortableBindingListCollection<ExportSheet>();
                for (int i = 0; i < this.scx.AllSheets.Count; i++) {
                    foreach (Autodesk.Revit.DB.View vs in vss.ViewSheetSet.Views) {
                        if (this.scx.AllSheets[i].Id.Equals(vs.Id)) {
                            bs.Add(this.scx.AllSheets[i]);
                            break;
                        }
                    }
                }
                this.dataGridView1.DataSource = bs;
            }
        }

        private void AddColumn(string name, string text)
        {
            var result = new DataGridViewTextBoxColumn();
            result.DataPropertyName = name;
            result.HeaderText = text;
            this.dataGridView1.Columns.Add(result);
        }

        private void AddDateColumn(string name, string text)
        {
            var result = new DataGridViewTextBoxColumn();
            result.DataPropertyName = name;
            var style = new DataGridViewCellStyle();
            style.Format = "dd/MM/yyyy";
            result.DefaultCellStyle = style;
            result.HeaderText = text;
            this.dataGridView1.Columns.Add(result);
        }

        private void PopulateColumns()
        {
            this.dataGridView1.AutoGenerateColumns = false;
            this.AddColumn("FullExportName", "Export Name");
            this.AddColumn("SheetNumber", "Number");
            this.AddColumn("SheetDescription", "Name");
            this.AddColumn("SheetRevision", "Revision");
            this.AddColumn("SheetRevisionDescription", "Revision Description");
            this.AddDateColumn("SheetRevisionDateTime", "Revision Date");
            this.AddColumn("ExportDir", "Export Dir");
            this.AddColumn("Scale", "Scale");
            this.AddColumn("NorthPointVisible", "North Point");
            this.AddColumn("PageSize", "Page Size");
            this.AddColumn("PrintSettingName", "Print Setting");
        }

        private List<ExportSheet> SelectedSheets()
        {
            var result = new List<ExportSheet>();
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows) {
                var sc = row.DataBoundItem as ExportSheet;
                result.Add(sc);
            }
            return result;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            this.btnExport.Hide();
            this.printButton.Hide();
            this.btnFind.Hide();
            this.Refresh();
            this.SetUpPBar(this.NumberOfSelectedViews());
            this.scx.ExportSheets(
                this.SelectedSheets(),
                this.progressBar,
                this.progressInfo,
                this.statusStrip1);
            this.Close();  // end SCexport
        }

        private void SetUpPBar(int count)
        {
            this.statusStrip1.Show();
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = count;
            this.progressBar.Step = 1;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
                var s = (ViewSheetSetCombo)this.cmbPrintSet.SelectedItem;
                if (s != null) {
                    this.PopulateList(s);
                }
        }

        private void SelectAllOrNone(bool all)
        {
            if (all) {
                this.dataGridView1.SelectAll();
            }
            if (!all) {
                this.dataGridView1.ClearSelection();
            }
            this.UpdateExportButton(this.NumberOfSelectedViews());
            this.dataGridView1.Refresh();
        }

        private void MnuSelectAll_Click(object sender, EventArgs e)
        {
            this.SelectAllOrNone(true);
        }

        private void MnuItemExportDir_Click(object sender, EventArgs e)
        {
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                this.folderBrowserDialog1.SelectedPath.ToString();
                this.scx.ExportDir = this.folderBrowserDialog1.SelectedPath;
            }
            this.dataGridView1.Refresh();
        }

        private void MnuVerify_Click(object sender, EventArgs e)
        {
            this.scx.Update();
            this.dataGridView1.Refresh();
        }

        private void MnuSelectNone_Click(object sender, EventArgs e)
        {
            this.SelectAllOrNone(false);
        }

        private void MoveUpOrDown(int move)
        {
            int column = this.dataGridView1.CurrentCell.ColumnIndex;
            int row = this.dataGridView1.CurrentCell.RowIndex;
            if ((row + move != this.dataGridView1.Rows.Count) && (row + move >= 0)) {
                this.dataGridView1.CurrentCell = this.dataGridView1[column, row + move];
            }
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!this.searchBox.Visible) {
                switch (e.KeyChar.ToString().ToUpper(CultureInfo.CurrentCulture)) {
                case "A":
                    this.SelectAllOrNone(true);
                    break;
                case "C":
                    this.PopulateList();
                    break;
                case "J":
                    this.MoveUpOrDown(1);
                    break;
                 case "K":
                    this.MoveUpOrDown(-1);
                    break;
                case ":":
                    this.ShowOptions();
                    break;
                case "?":
                    ShowHelp();
                    break;
                case "/":
                    this.searchBox.Clear();
                    this.searchBox.Show();
                    this.searchBox.Focus();
                    break;
                case "L":
                    string s = ExportManager.LatestRevisionDate();
                    if (s != null) {
                        this.PopulateList(new FilterContextMenu("temp", 5, s));
                    }
                    break;
                case "N":
                    this.SelectAllOrNone(false);
                    break;
                case "V":
                    this.MnuVerify_Click(sender, e);
                    break;
                case "O":
                    this.OpenSelectedViewToolStripMenuItemClick(sender, e);
                    break;
                case "P":
                    this.scx.ForceRevisionToDateString = !this.scx.ForceRevisionToDateString;
                        this.dataGridView1.Refresh();
                    break;
                case "Q":
                    this.Close();
                    break;
                case "S":
                    this.SelectItem(ExportManager.CurrentViewName());
                    this.dataGridView1.Refresh();
                    break;
                case "T":
                    TaskDialog.Show("Tip Of The Day", TipOfDay.Tip());
                    break;
                case "X":
                    this.BtnExport_Click(sender, e);
                    break;
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    this.PopulateList(e.KeyChar.ToString());
                    break;
                }
            }
        }

        /// <summary>
        /// Select multiple items from the main dataGrid.
        /// </summary>
        /// <param name="s">A string of the Sheet Number.</param>
        private void FilterItems(string s)
        {
            var bs = new SortableBindingListCollection<ExportSheet>();
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                var sc = row.DataBoundItem as ExportSheet;
                if (sc == null) {
                    return;
                }
                bool r1 = Regex.IsMatch(
                    sc.SheetNumber, s, RegexOptions.IgnoreCase);
                bool r2 = Regex.IsMatch(
                    sc.SheetDescription, s, RegexOptions.IgnoreCase);
                if (r1 || r2) {
                    bs.Add(sc);
                }
            }
            if (bs.Count > 0) {
                this.dataGridView1.DataSource = bs;
            }
        }

        /// <summary>
        /// Select one item from the main dataGrid.
        /// </summary>
        /// <param name="s">A string of the Sheet Number.</param>
        private void SelectItem(string s)
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                var sc = row.DataBoundItem as ExportSheet;
                if (sc.SheetNumber.Equals(s)) {
                    this.dataGridView1.ClearSelection();
                    row.Selected = true;
                    this.dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                    this.dataGridView1.CurrentCell = this.dataGridView1[1, row.Index];
                    continue;
                }
            }
            this.UpdateExportButton(this.NumberOfSelectedViews());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.dataGridView1.Focus();
        }

        private void ToggleConversionFlag(
                ToolStripMenuItem box, ExportOptions val)
        {
            if (box.Checked == true) {
                this.scx.AddExportOption(val);
            } else {
                this.scx.RemoveExportOption(val);
            }
        }

        private void ToggleCheckBoxValue(object sender, EventArgs e)
        {
            var c = (ToolStripMenuItem)sender;
            var t = (ExportOptions)c.Tag;
            this.ToggleConversionFlag(c, t);
            this.UpdateExportButton(this.NumberOfSelectedViews());
        }

        private void UpdateExportButton(int count)
        {
            string s = "Export[" + count + "]:";
            if (this.scx.HasExportOption(ExportOptions.PDF)) {
                s += @" " + ExportOptions.PDF.ToString();
            }
            if (this.scx.HasExportOption(ExportOptions.DWG)) {
                s += @" " + ExportOptions.DWG.ToString();
            }
            if (this.scx.HasExportOption(ExportOptions.DWF)) {
                s += @" " + ExportOptions.DWF.ToString();
            }
            if (this.scx.HasExportOption(ExportOptions.DGN)) {
                s += @" " + ExportOptions.DGN.ToString();
            }
            if (this.scx.HasExportOption(ExportOptions.GhostscriptPDF)) {
                s += @" " + ExportOptions.GhostscriptPDF.ToString();
            }
            this.btnExport.Text = s;
            this.dataGridView1.Refresh();
        }

        private void MnuSchemeSelected_Click(object sender, EventArgs e)
        {
            this.scx.SetFileNameScheme(sender.ToString());
            this.dataGridView1.Refresh();
        }

        private void ChangeLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(SCaddins.Constants.SourceLink);
        }

        private void CreateConfigFileToolStripMenuItem_Click(
                object sender, EventArgs e)
        {
            FileUtilities.CreateConfigFile(this.doc);
        }

        private void EditConfigFileToolStripMenuItem_Click(
                object sender, EventArgs e)
        {
            FileUtilities.EditConfigFile(this.doc);
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ab = new AboutBox1();
            ab.ShowDialog();
        }

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

      private void DataGridView1_CellMouseDown(
            object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1) {
                string s = "Filter";
                string f = "NA";
                this.filterToolStripMenuItem.Enabled = true;
                var row = (DataGridViewRow)this.dataGridView1.Rows[e.RowIndex];
                var sc = row.DataBoundItem as ExportSheet;
                switch (e.ColumnIndex) {
                case 1:
                    var c =
                        new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                        f = "Filter: Rev *"; 
                    int index = sc.SheetNumber.IndexOfAny(c);
                    if (index > -1) {
                        f = "Filter: Starts with *"; 
                        this.filter.Update(
                                f + sc.SheetNumber.Substring(0, index + 1),
                                1,
                                sc.SheetNumber.Substring(0, index + 1));
                        s = this.filter.Label;
                    }
                    break;
                case 3:
                        f = "Filter: Rev *"; 
                        this.filter.Update(
                            f + sc.SheetRevision, 3, sc.SheetRevision);
                        s = this.filter.Label;
                        break;
                case 5:
                        f = "Filter: Rev Date*"; 
                        this.filter.Update(
                            f + sc.SheetRevisionDate, 5, sc.SheetRevisionDate);
                        s = this.filter.Label;
                        break;
                default:
                        this.filterToolStripMenuItem.Enabled = false; 
                        break;
                }
                filterToolStripMenuItem.Text = s;
            }
        }

        private void FilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.PopulateList(this.filter);
        }

        private void NoFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.PopulateList();
        }

        private void Label2_Click(object sender, EventArgs e)
        {
            this.ForumToolStripMenuItem_Click(sender, e);
        }

        private void ForumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(SCaddins.Constants.HelpLink);
        }

        private void DataGridView1_CellContentClick(
                object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6) {
                this.MnuItemExportDir_Click(sender, e);
            }
            if (e.ColumnIndex == 7) {
                this.MnuVerify_Click(sender, e);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        private int NumberOfSelectedViews()
        {
            return this.dataGridView1.SelectedRows.Count;
        }

        private void DataGridView1CellMouseUp(
                object sender, DataGridViewCellMouseEventArgs e)
        {
                this.UpdateExportButton(this.NumberOfSelectedViews());
        }

        private void DataGridView1CellEndEdit(
                object sender, DataGridViewCellEventArgs e)
        {
            this.BindingContext[this.dataGridView1.DataSource].EndCurrentEdit();
        }

        private void DataGridView1MouseUp(object sender, MouseEventArgs e)
        {
            this.UpdateExportButton(this.NumberOfSelectedViews()); 
        }

        private void SearchBoxKeyDown(object sender, KeyEventArgs e)
        {
           if (e.KeyCode == Keys.Enter) {
                this.FilterItems(this.searchBox.Text);
                this.searchBox.Visible = false;
           }
        }
       
        private void BtnExportResize(object sender, EventArgs e)
        {
            this.printButton.Location = new Point(
                this.btnExport.Location.X - (this.printButton.Width + 2),
                this.btnExport.Location.Y);
        }

        private void MainFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && !this.searchBox.Visible) {
                this.Close();
            } else if (e.KeyCode == Keys.Escape && this.searchBox.Visible) {
                this.searchBox.Visible = false;
            }
        }
        
        private void Button2Click(object sender, EventArgs e)
        {
            this.ShowOptions();
        }
        
        private void MnuExportOptionsClick(object sender, EventArgs e)
        {
            this.ShowOptions();
        }
        
        private void ShowOptions()
        {
            var options = new OptionsDialog(this.doc, this.scx);
            options.ShowDialog();
            this.UpdateExportButton(this.NumberOfSelectedViews());
            this.dataGridView1.Refresh(); 
        }
        
        private void BtnFindClick(object sender, EventArgs e)
        {
            this.searchBox.Clear();
            this.searchBox.Show();
            this.searchBox.Focus();
        }
        
        private void AddRevisionToolStripMenuItemClick(object sender, EventArgs e)
        {
            ExportManager.AddRevisions(this.SelectedSheets());
            this.Update();
            this.dataGridView1.Refresh();           
        }
        
        private void RenameSelectedSheetsToolStripMenuItemClick(object sender, EventArgs e)
        {
            ExportManager.RenameSheets(this.SelectedSheets());  
            this.Update();
            this.dataGridView1.Refresh();
        }

        private void CreateUserViewsToolStripMenuItemClick(object sender, System.EventArgs e)
        {
            ViewUtilities.UserView.Create(this.SelectedSheets(), this.doc);
        }

        private void RemoveUnderlaysFromViewsToolStripMenuItemClick(object sender, EventArgs e)
        {
            ViewUtilities.ViewUnderlays.RemoveUnderlays(this.SelectedSheets(), this.doc);
        }

        private void FixScalesBarsToolStripMenuItemClick(object sender, EventArgs e)
        {
            ExportManager.FixScaleBars(this.SelectedSheets());  
            this.Update();
            this.dataGridView1.Refresh();  
        }

        private void CopySheetsToolStripMenuItemClick(object sender, EventArgs e)
        {          
            var scopy = new SCaddins.SheetCopier.SheetCopierManager(udoc);            
            var form  = new SCaddins.SheetCopier.MainForm(doc, SelectedSheets(), scopy);
            form.Enabled = true;
            DialogResult result = form.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                scopy.CreateSheets();
            }            
        }
        
        private void Print(string printerName, int scale)
        {
            btnExport.Hide();
            printButton.Hide();
            btnFind.Hide();
            Refresh();   
            SetUpPBar(NumberOfSelectedViews());
            scx.Print(SelectedSheets(),
                           printerName,
                           scale,
                           progressBar,
                           progressInfo,
                           statusStrip1);   
            Close();  // end SCexport     
        }

        private void PrintA3ToolStripMenuItemClick(object sender, EventArgs e)
        {
            Print(scx.PrinterNameA3, 3);
        }

        private void PrintA2ToolStripMenuItemClick(object sender, EventArgs e)
        {
            Print(scx.PrinterNameLargeFormat, 2);
        }

        private void PrintFullSizeToolStripMenuItemClick(object sender, EventArgs e)
        {
            Print(scx.PrinterNameLargeFormat, -1);
        }
        
        private void ToggleNorthPointToolStripMenuItemClick(object sender, EventArgs e)
        {
            ExportManager.ToggleNorthPoints(SelectedSheets());  
            Update();
            dataGridView1.Refresh();    
        } 
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
