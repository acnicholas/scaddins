// (C) Copyright 2012 by Andrew Nicholas
//
// This file is part of SCexport.
//
// SCexport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCexport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCexport.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.ExportManager
{
    using SCaddins.Properties;

    /// <summary>
    /// The main user visible form.
    /// </summary>
    public partial class MainForm
    {
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel progressInfo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbPrintSet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuSelectAll;
        private System.Windows.Forms.ToolStripMenuItem mnuSelectNone;
        private System.Windows.Forms.ToolStripSeparator mnuSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuVerify;
        private System.Windows.Forms.ToolStripSeparator mnuSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuExportOptions;
        private System.Windows.Forms.ToolStripSeparator mnuSeparator0;
        private System.Windows.Forms.ToolStripMenuItem changeLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSelectedViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripMenuItem clearFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem forumToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem addRevisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnOptions;
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null)) {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnExport = new System.Windows.Forms.Button();
            this.cmbPrintSet = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExportOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparator0 = new System.Windows.Forms.ToolStripSeparator();
            this.openSelectedViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addRevisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySheetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeUnderlaysFromViewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameSelectedSheetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixScalesBarsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleNorthPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createUserViewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSelectNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuVerify = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.changeLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.printButtonContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.printA3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.printA2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.printFullSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.printButtonContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExport
            // 
            resources.ApplyResources(this.btnExport, "btnExport");
            this.btnExport.Name = "btnExport";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            this.btnExport.Resize += new System.EventHandler(this.BtnExportResize);
            // 
            // cmbPrintSet
            // 
            resources.ApplyResources(this.cmbPrintSet, "cmbPrintSet");
            this.cmbPrintSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintSet.FormattingEnabled = true;
            this.cmbPrintSet.Name = "cmbPrintSet";
            this.cmbPrintSet.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearFilterToolStripMenuItem,
            this.filterToolStripMenuItem,
            this.toolStripSeparator5,
            this.mnuExportOptions,
            this.mnuSeparator0,
            this.openSelectedViewToolStripMenuItem,
            this.toolStripSeparator1,
            this.addRevisionToolStripMenuItem,
            this.copySheetsToolStripMenuItem,
            this.removeUnderlaysFromViewsToolStripMenuItem,
            this.renameSelectedSheetsToolStripMenuItem,
            this.fixScalesBarsToolStripMenuItem,
            this.toggleNorthPointToolStripMenuItem,
            this.createUserViewsToolStripMenuItem,
            this.toolStripSeparator6,
            this.mnuSelectAll,
            this.mnuSelectNone,
            this.mnuSeparator2,
            this.mnuVerify,
            this.mnuSeparator3,
            this.mnuHelp});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowCheckMargin = true;
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // clearFilterToolStripMenuItem
            // 
            resources.ApplyResources(this.clearFilterToolStripMenuItem, "clearFilterToolStripMenuItem");
            this.clearFilterToolStripMenuItem.Name = "clearFilterToolStripMenuItem";
            this.clearFilterToolStripMenuItem.Click += new System.EventHandler(this.NoFilterToolStripMenuItem_Click);
            // 
            // filterToolStripMenuItem
            // 
            resources.ApplyResources(this.filterToolStripMenuItem, "filterToolStripMenuItem");
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Text = global::SCaddins.Properties.Resources.Filter;
            this.filterToolStripMenuItem.Click += new System.EventHandler(this.FilterToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // mnuExportOptions
            // 
            resources.ApplyResources(this.mnuExportOptions, "mnuExportOptions");
            this.mnuExportOptions.Name = "mnuExportOptions";
            this.mnuExportOptions.Text = global::SCaddins.Properties.Resources.Options;
            this.mnuExportOptions.Click += new System.EventHandler(this.MnuExportOptionsClick);
            // 
            // mnuSeparator0
            // 
            this.mnuSeparator0.Name = "mnuSeparator0";
            resources.ApplyResources(this.mnuSeparator0, "mnuSeparator0");
            // 
            // openSelectedViewToolStripMenuItem
            // 
            resources.ApplyResources(this.openSelectedViewToolStripMenuItem, "openSelectedViewToolStripMenuItem");
            this.openSelectedViewToolStripMenuItem.Name = "openSelectedViewToolStripMenuItem";
            this.openSelectedViewToolStripMenuItem.Click += new System.EventHandler(this.OpenSelectedViewToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // addRevisionToolStripMenuItem
            // 
            resources.ApplyResources(this.addRevisionToolStripMenuItem, "addRevisionToolStripMenuItem");
            this.addRevisionToolStripMenuItem.Name = "addRevisionToolStripMenuItem";
            this.addRevisionToolStripMenuItem.Text = global::SCaddins.Properties.Resources.AddRevision;
            this.addRevisionToolStripMenuItem.Click += new System.EventHandler(this.AddRevisionToolStripMenuItemClick);
            // 
            // copySheetsToolStripMenuItem
            // 
            resources.ApplyResources(this.copySheetsToolStripMenuItem, "copySheetsToolStripMenuItem");
            this.copySheetsToolStripMenuItem.Name = "copySheetsToolStripMenuItem";
            this.copySheetsToolStripMenuItem.Text = global::SCaddins.Properties.Resources.CopySheets;
            this.copySheetsToolStripMenuItem.Click += new System.EventHandler(this.CopySheetsToolStripMenuItemClick);
            // 
            // removeUnderlaysFromViewsToolStripMenuItem
            // 
            resources.ApplyResources(this.removeUnderlaysFromViewsToolStripMenuItem, "removeUnderlaysFromViewsToolStripMenuItem");
            this.removeUnderlaysFromViewsToolStripMenuItem.Name = "removeUnderlaysFromViewsToolStripMenuItem";
            this.removeUnderlaysFromViewsToolStripMenuItem.Text = global::SCaddins.Properties.Resources.RemoveUnderlaysfromViews;
            this.removeUnderlaysFromViewsToolStripMenuItem.Click += new System.EventHandler(this.RemoveUnderlaysFromViewsToolStripMenuItemClick);
            // 
            // renameSelectedSheetsToolStripMenuItem
            // 
            resources.ApplyResources(this.renameSelectedSheetsToolStripMenuItem, "renameSelectedSheetsToolStripMenuItem");
            this.renameSelectedSheetsToolStripMenuItem.Name = "renameSelectedSheetsToolStripMenuItem";
            this.renameSelectedSheetsToolStripMenuItem.Text = global::SCaddins.Properties.Resources.RenameSelectedSheets;
            this.renameSelectedSheetsToolStripMenuItem.Click += new System.EventHandler(this.RenameSelectedSheetsToolStripMenuItemClick);
            // 
            // fixScalesBarsToolStripMenuItem
            // 
            resources.ApplyResources(this.fixScalesBarsToolStripMenuItem, "fixScalesBarsToolStripMenuItem");
            this.fixScalesBarsToolStripMenuItem.Name = "fixScalesBarsToolStripMenuItem";
            this.fixScalesBarsToolStripMenuItem.Click += new System.EventHandler(this.FixScalesBarsToolStripMenuItemClick);
            // 
            // toggleNorthPointToolStripMenuItem
            // 
            resources.ApplyResources(this.toggleNorthPointToolStripMenuItem, "toggleNorthPointToolStripMenuItem");
            this.toggleNorthPointToolStripMenuItem.Name = "toggleNorthPointToolStripMenuItem";
            this.toggleNorthPointToolStripMenuItem.Click += new System.EventHandler(this.ToggleNorthPointToolStripMenuItemClick);
            // 
            // createUserViewsToolStripMenuItem
            // 
            resources.ApplyResources(this.createUserViewsToolStripMenuItem, "createUserViewsToolStripMenuItem");
            this.createUserViewsToolStripMenuItem.Name = "createUserViewsToolStripMenuItem";
            this.createUserViewsToolStripMenuItem.Click += new System.EventHandler(this.CreateUserViewsToolStripMenuItemClick);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // mnuSelectAll
            // 
            resources.ApplyResources(this.mnuSelectAll, "mnuSelectAll");
            this.mnuSelectAll.Name = "mnuSelectAll";
            this.mnuSelectAll.Text = global::SCaddins.Properties.Resources.SelectAll;
            this.mnuSelectAll.Click += new System.EventHandler(this.MnuSelectAll_Click);
            // 
            // mnuSelectNone
            // 
            resources.ApplyResources(this.mnuSelectNone, "mnuSelectNone");
            this.mnuSelectNone.Name = "mnuSelectNone";
            this.mnuSelectNone.Text = global::SCaddins.Properties.Resources.SelectNone;
            this.mnuSelectNone.Click += new System.EventHandler(this.MnuSelectNone_Click);
            // 
            // mnuSeparator2
            // 
            this.mnuSeparator2.Name = "mnuSeparator2";
            resources.ApplyResources(this.mnuSeparator2, "mnuSeparator2");
            // 
            // mnuVerify
            // 
            resources.ApplyResources(this.mnuVerify, "mnuVerify");
            this.mnuVerify.Name = "mnuVerify";
            this.mnuVerify.Text = global::SCaddins.Properties.Resources.VerifySheets;
            this.mnuVerify.Click += new System.EventHandler(this.MnuVerify_Click);
            // 
            // mnuSeparator3
            // 
            this.mnuSeparator3.Name = "mnuSeparator3";
            resources.ApplyResources(this.mnuSeparator3, "mnuSeparator3");
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeLogToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.forumToolStripMenuItem});
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Text = global::SCaddins.Properties.Resources.Help;
            // 
            // changeLogToolStripMenuItem
            // 
            resources.ApplyResources(this.changeLogToolStripMenuItem, "changeLogToolStripMenuItem");
            this.changeLogToolStripMenuItem.Name = "changeLogToolStripMenuItem";
            this.changeLogToolStripMenuItem.Text = global::SCaddins.Properties.Resources.SourceCode;
            this.changeLogToolStripMenuItem.Click += new System.EventHandler(this.ChangeLogToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Text = global::SCaddins.Properties.Resources.About;
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Text = global::SCaddins.Properties.Resources.KeyboardShortcuts;
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItem_Click);
            // 
            // forumToolStripMenuItem
            // 
            resources.ApplyResources(this.forumToolStripMenuItem, "forumToolStripMenuItem");
            this.forumToolStripMenuItem.Name = "forumToolStripMenuItem";
            this.forumToolStripMenuItem.Text = global::SCaddins.Properties.Resources.Wiki;
            this.forumToolStripMenuItem.Click += new System.EventHandler(this.ForumToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllHeaders;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellContentClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1CellEndEdit);
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView1_CellMouseDown);
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView1CellMouseUp);
            this.dataGridView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DataGridView1MouseUp);
            // 
            // searchBox
            // 
            resources.ApplyResources(this.searchBox, "searchBox");
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox.Name = "searchBox";
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchBoxKeyDown);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressInfo,
            this.progressBar});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // progressInfo
            // 
            this.progressInfo.Name = "progressInfo";
            resources.ApplyResources(this.progressInfo, "progressInfo");
            this.progressInfo.Spring = true;
            // 
            // progressBar
            // 
            this.progressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.progressBar.Name = "progressBar";
            resources.ApplyResources(this.progressBar, "progressBar");
            // 
            // btnOptions
            // 
            resources.ApplyResources(this.btnOptions, "btnOptions");
            this.btnOptions.FlatAppearance.BorderSize = 0;
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.Button2Click);
            // 
            // btnFind
            // 
            resources.ApplyResources(this.btnFind, "btnFind");
            this.btnFind.FlatAppearance.BorderSize = 0;
            this.btnFind.Name = "btnFind";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.BtnFindClick);
            // 
            // printButtonContextMenu
            // 
            this.printButtonContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printA3ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.printA2ToolStripMenuItem,
            this.toolStripMenuItem2,
            this.printFullSizeToolStripMenuItem});
            this.printButtonContextMenu.Name = "pintButtonContextMenu";
            resources.ApplyResources(this.printButtonContextMenu, "printButtonContextMenu");
            // 
            // printA3ToolStripMenuItem
            // 
            this.printA3ToolStripMenuItem.Name = "printA3ToolStripMenuItem";
            resources.ApplyResources(this.printA3ToolStripMenuItem, "printA3ToolStripMenuItem");
            this.printA3ToolStripMenuItem.Text = global::SCaddins.Properties.Resources.ExportManagerPrintA3Fit;
            this.printA3ToolStripMenuItem.Click += new System.EventHandler(this.PrintA3ToolStripMenuItemClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // printA2ToolStripMenuItem
            // 
            this.printA2ToolStripMenuItem.Name = "printA2ToolStripMenuItem";
            resources.ApplyResources(this.printA2ToolStripMenuItem, "printA2ToolStripMenuItem");
            this.printA2ToolStripMenuItem.Text = global::SCaddins.Properties.Resources.ExportManagerPrintA2Fit;
            this.printA2ToolStripMenuItem.Click += new System.EventHandler(this.PrintA2ToolStripMenuItemClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // printFullSizeToolStripMenuItem
            // 
            this.printFullSizeToolStripMenuItem.Name = "printFullSizeToolStripMenuItem";
            resources.ApplyResources(this.printFullSizeToolStripMenuItem, "printFullSizeToolStripMenuItem");
            this.printFullSizeToolStripMenuItem.Text = global::SCaddins.Properties.Resources.ExportManagerPrintFullSize;
            this.printFullSizeToolStripMenuItem.Click += new System.EventHandler(this.PrintFullSizeToolStripMenuItemClick);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cmbPrintSet);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExport);
            this.HelpButton = true;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainFormKeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.printButtonContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.ToolStripMenuItem createUserViewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameSelectedSheetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeUnderlaysFromViewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixScalesBarsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySheetsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip printButtonContextMenu;
        private System.Windows.Forms.ToolStripMenuItem printA3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem printA2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem printFullSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleNorthPointToolStripMenuItem;
        #endregion
        }
    }

/* vim: set ts=4 sw=4 nu expandtab: */
