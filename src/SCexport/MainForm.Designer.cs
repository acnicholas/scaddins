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

namespace SCaddins.SCexport
{
    /// <summary>
    /// The main user visible form.
    /// </summary>
    public partial class MainForm
    {
        private System.Windows.Forms.Button btnPrint;
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
            this.removeUnderlaysFromViewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameSelectedSheetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixScalesBarsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.AutoSize = true;
            this.btnExport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExport.Location = new System.Drawing.Point(716, 504);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(51, 22);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            this.btnExport.Resize += new System.EventHandler(this.BtnExportResize);
            // 
            // cmbPrintSet
            // 
            this.cmbPrintSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPrintSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintSet.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbPrintSet.FormattingEnabled = true;
            this.cmbPrintSet.Location = new System.Drawing.Point(65, 12);
            this.cmbPrintSet.Name = "cmbPrintSet";
            this.cmbPrintSet.Size = new System.Drawing.Size(670, 21);
            this.cmbPrintSet.TabIndex = 3;
            this.cmbPrintSet.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Print Set:";
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
            this.removeUnderlaysFromViewsToolStripMenuItem,
            this.renameSelectedSheetsToolStripMenuItem,
            this.fixScalesBarsToolStripMenuItem,
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
            this.contextMenuStrip1.Size = new System.Drawing.Size(257, 348);
            // 
            // clearFilterToolStripMenuItem
            // 
            this.clearFilterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("clearFilterToolStripMenuItem.Image")));
            this.clearFilterToolStripMenuItem.Name = "clearFilterToolStripMenuItem";
            this.clearFilterToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.clearFilterToolStripMenuItem.Text = "No View Filter";
            this.clearFilterToolStripMenuItem.Click += new System.EventHandler(this.NoFilterToolStripMenuItem_Click);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.Enabled = false;
            this.filterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("filterToolStripMenuItem.Image")));
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.filterToolStripMenuItem.Text = "Filter";
            this.filterToolStripMenuItem.Click += new System.EventHandler(this.FilterToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(253, 6);
            // 
            // mnuExportOptions
            // 
            this.mnuExportOptions.Image = ((System.Drawing.Image)(resources.GetObject("mnuExportOptions.Image")));
            this.mnuExportOptions.Name = "mnuExportOptions";
            this.mnuExportOptions.Size = new System.Drawing.Size(256, 22);
            this.mnuExportOptions.Text = "Options";
            this.mnuExportOptions.Click += new System.EventHandler(this.MnuExportOptionsClick);
            // 
            // mnuSeparator0
            // 
            this.mnuSeparator0.Name = "mnuSeparator0";
            this.mnuSeparator0.Size = new System.Drawing.Size(253, 6);
            // 
            // openSelectedViewToolStripMenuItem
            // 
            this.openSelectedViewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openSelectedViewToolStripMenuItem.Image")));
            this.openSelectedViewToolStripMenuItem.Name = "openSelectedViewToolStripMenuItem";
            this.openSelectedViewToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.openSelectedViewToolStripMenuItem.Text = "Open Selected View";
            this.openSelectedViewToolStripMenuItem.Click += new System.EventHandler(this.OpenSelectedViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(253, 6);
            // 
            // addRevisionToolStripMenuItem
            // 
            this.addRevisionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addRevisionToolStripMenuItem.Image")));
            this.addRevisionToolStripMenuItem.Name = "addRevisionToolStripMenuItem";
            this.addRevisionToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.addRevisionToolStripMenuItem.Text = "Add Revision";
            this.addRevisionToolStripMenuItem.Click += new System.EventHandler(this.AddRevisionToolStripMenuItemClick);
            // 
            // removeUnderlaysFromViewsToolStripMenuItem
            // 
            this.removeUnderlaysFromViewsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeUnderlaysFromViewsToolStripMenuItem.Image")));
            this.removeUnderlaysFromViewsToolStripMenuItem.Name = "removeUnderlaysFromViewsToolStripMenuItem";
            this.removeUnderlaysFromViewsToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.removeUnderlaysFromViewsToolStripMenuItem.Text = "Remove Underlays from Views";
            this.removeUnderlaysFromViewsToolStripMenuItem.Click += new System.EventHandler(this.RemoveUnderlaysFromViewsToolStripMenuItemClick);
            // 
            // renameSelectedSheetsToolStripMenuItem
            // 
            this.renameSelectedSheetsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("renameSelectedSheetsToolStripMenuItem.Image")));
            this.renameSelectedSheetsToolStripMenuItem.Name = "renameSelectedSheetsToolStripMenuItem";
            this.renameSelectedSheetsToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.renameSelectedSheetsToolStripMenuItem.Text = "Rename Selected Sheets";
            this.renameSelectedSheetsToolStripMenuItem.Click += new System.EventHandler(this.RenameSelectedSheetsToolStripMenuItemClick);
            // 
            // fixScalesBarsToolStripMenuItem
            // 
            this.fixScalesBarsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fixScalesBarsToolStripMenuItem.Image")));
            this.fixScalesBarsToolStripMenuItem.Name = "fixScalesBarsToolStripMenuItem";
            this.fixScalesBarsToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.fixScalesBarsToolStripMenuItem.Text = "Fix Scales Bars";
            this.fixScalesBarsToolStripMenuItem.Click += new System.EventHandler(this.FixScalesBarsToolStripMenuItemClick);
            // 
            // createUserViewsToolStripMenuItem
            // 
            this.createUserViewsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createUserViewsToolStripMenuItem.Image")));
            this.createUserViewsToolStripMenuItem.Name = "createUserViewsToolStripMenuItem";
            this.createUserViewsToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.createUserViewsToolStripMenuItem.Text = "Create User Views";
            this.createUserViewsToolStripMenuItem.Click += new System.EventHandler(this.CreateUserViewsToolStripMenuItemClick);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(253, 6);
            // 
            // mnuSelectAll
            // 
            this.mnuSelectAll.Image = ((System.Drawing.Image)(resources.GetObject("mnuSelectAll.Image")));
            this.mnuSelectAll.Name = "mnuSelectAll";
            this.mnuSelectAll.Size = new System.Drawing.Size(256, 22);
            this.mnuSelectAll.Text = "SelectAll";
            this.mnuSelectAll.Click += new System.EventHandler(this.MnuSelectAll_Click);
            // 
            // mnuSelectNone
            // 
            this.mnuSelectNone.Image = ((System.Drawing.Image)(resources.GetObject("mnuSelectNone.Image")));
            this.mnuSelectNone.Name = "mnuSelectNone";
            this.mnuSelectNone.Size = new System.Drawing.Size(256, 22);
            this.mnuSelectNone.Text = "Select None";
            this.mnuSelectNone.Click += new System.EventHandler(this.MnuSelectNone_Click);
            // 
            // mnuSeparator2
            // 
            this.mnuSeparator2.Name = "mnuSeparator2";
            this.mnuSeparator2.Size = new System.Drawing.Size(253, 6);
            // 
            // mnuVerify
            // 
            this.mnuVerify.Image = ((System.Drawing.Image)(resources.GetObject("mnuVerify.Image")));
            this.mnuVerify.Name = "mnuVerify";
            this.mnuVerify.Size = new System.Drawing.Size(256, 22);
            this.mnuVerify.Text = "Verify Sheets";
            this.mnuVerify.Click += new System.EventHandler(this.MnuVerify_Click);
            // 
            // mnuSeparator3
            // 
            this.mnuSeparator3.Name = "mnuSeparator3";
            this.mnuSeparator3.Size = new System.Drawing.Size(253, 6);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeLogToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.forumToolStripMenuItem});
            this.mnuHelp.Image = ((System.Drawing.Image)(resources.GetObject("mnuHelp.Image")));
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(256, 22);
            this.mnuHelp.Text = "Help";
            // 
            // changeLogToolStripMenuItem
            // 
            this.changeLogToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("changeLogToolStripMenuItem.Image")));
            this.changeLogToolStripMenuItem.Name = "changeLogToolStripMenuItem";
            this.changeLogToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.changeLogToolStripMenuItem.Text = "Source Code";
            this.changeLogToolStripMenuItem.Click += new System.EventHandler(this.ChangeLogToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripMenuItem.Image")));
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.helpToolStripMenuItem.Text = "Keyboard Shortcuts";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItem_Click);
            // 
            // forumToolStripMenuItem
            // 
            this.forumToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("forumToolStripMenuItem.Image")));
            this.forumToolStripMenuItem.Name = "forumToolStripMenuItem";
            this.forumToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.forumToolStripMenuItem.Text = "Wiki";
            this.forumToolStripMenuItem.Click += new System.EventHandler(this.ForumToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllHeaders;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dataGridView1.Location = new System.Drawing.Point(12, 39);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.Size = new System.Drawing.Size(755, 454);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellContentClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1CellEndEdit);
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView1_CellMouseDown);
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView1CellMouseUp);
            this.dataGridView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DataGridView1MouseUp);
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox.Location = new System.Drawing.Point(44, 504);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(137, 20);
            this.searchBox.TabIndex = 11;
            this.searchBox.Visible = false;
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchBoxKeyDown);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressInfo,
            this.progressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 481);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.statusStrip1.Size = new System.Drawing.Size(735, 30);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.Visible = false;
            // 
            // progressInfo
            // 
            this.progressInfo.Name = "progressInfo";
            this.progressInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.progressInfo.Size = new System.Drawing.Size(268, 25);
            this.progressInfo.Spring = true;
            this.progressInfo.Text = "toolStripStatusLabel1";
            this.progressInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.progressBar.Name = "progressBar";
            this.progressBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.progressBar.Size = new System.Drawing.Size(450, 24);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnPrint.Location = new System.Drawing.Point(635, 504);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(55, 22);
            this.btnPrint.TabIndex = 13;
            this.btnPrint.Text = "Print (A3)";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.Button1Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnOptions.FlatAppearance.BorderSize = 0;
            this.btnOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.Image")));
            this.btnOptions.Location = new System.Drawing.Point(741, 10);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(26, 23);
            this.btnOptions.TabIndex = 14;
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.Button2Click);
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnFind.FlatAppearance.BorderSize = 0;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.Location = new System.Drawing.Point(12, 503);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(26, 22);
            this.btnFind.TabIndex = 15;
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.BtnFindClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 534);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cmbPrintSet);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExport);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(751, 549);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SCexport - Scott Carver Export Utility - By Andrew Nicholas";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainFormKeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.ToolStripMenuItem createUserViewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameSelectedSheetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeUnderlaysFromViewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixScalesBarsToolStripMenuItem;
        #endregion
        }
    }

/* vim: set ts=4 sw=4 nu expandtab: */
