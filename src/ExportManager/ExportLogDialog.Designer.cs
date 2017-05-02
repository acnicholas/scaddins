// (C) Copyright 2015 by Andrew Nicholas
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
    using SCaddins.Properties;

    partial class ExportLogDialog
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.TabPage WarningTab;
        private System.Windows.Forms.ListView warnings;
        private System.Windows.Forms.TabPage ErrorTab;
        private System.Windows.Forms.ListView errors;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage OutputTab;
        private System.Windows.Forms.TextBox textBox1;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportLogDialog));
            this.Tabs = new System.Windows.Forms.TabControl();
            this.WarningTab = new System.Windows.Forms.TabPage();
            this.warnings = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.ErrorTab = new System.Windows.Forms.TabPage();
            this.errors = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.OutputTab = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Tabs.SuspendLayout();
            this.WarningTab.SuspendLayout();
            this.ErrorTab.SuspendLayout();
            this.OutputTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tabs
            // 
            this.Tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Tabs.Controls.Add(this.OutputTab);
            this.Tabs.Controls.Add(this.WarningTab);
            this.Tabs.Controls.Add(this.ErrorTab);
            this.Tabs.Location = new System.Drawing.Point(12, 35);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(519, 383);
            this.Tabs.TabIndex = 0;
            // 
            // WarningTab
            // 
            this.WarningTab.Controls.Add(this.warnings);
            this.WarningTab.Location = new System.Drawing.Point(4, 22);
            this.WarningTab.Name = "WarningTab";
            this.WarningTab.Padding = new System.Windows.Forms.Padding(3);
            this.WarningTab.Size = new System.Drawing.Size(511, 357);
            this.WarningTab.TabIndex = 1;
            this.WarningTab.Text = Resources.Warnings;
            this.WarningTab.UseVisualStyleBackColor = true;
            // 
            // warnings
            // 
            this.warnings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.warnings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.warnings.Location = new System.Drawing.Point(3, 3);
            this.warnings.Name = "warnings";
            this.warnings.Size = new System.Drawing.Size(505, 351);
            this.warnings.TabIndex = 0;
            this.warnings.UseCompatibleStateImageBehavior = false;
            this.warnings.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = Resources.Filename;
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = Resources.Description;
            this.columnHeader2.Width = 600;
            // 
            // ErrorTab
            // 
            this.ErrorTab.Controls.Add(this.errors);
            this.ErrorTab.Location = new System.Drawing.Point(4, 22);
            this.ErrorTab.Name = "ErrorTab";
            this.ErrorTab.Padding = new System.Windows.Forms.Padding(3);
            this.ErrorTab.Size = new System.Drawing.Size(511, 357);
            this.ErrorTab.TabIndex = 2;
            this.ErrorTab.Text = Resources.Errors;
            this.ErrorTab.UseVisualStyleBackColor = true;
            // 
            // errors
            // 
            this.errors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.errors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errors.Location = new System.Drawing.Point(3, 3);
            this.errors.Name = "errors";
            this.errors.Size = new System.Drawing.Size(505, 351);
            this.errors.TabIndex = 0;
            this.errors.UseCompatibleStateImageBehavior = false;
            this.errors.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = Resources.Filename;
            this.columnHeader3.Width = 120;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = Resources.Description;
            this.columnHeader4.Width = 600;
            // 
            // OutputTab
            // 
            this.OutputTab.Controls.Add(this.textBox1);
            this.OutputTab.Location = new System.Drawing.Point(4, 22);
            this.OutputTab.Name = "OutputTab";
            this.OutputTab.Padding = new System.Windows.Forms.Padding(3);
            this.OutputTab.Size = new System.Drawing.Size(511, 357);
            this.OutputTab.TabIndex = 3;
            this.OutputTab.Text = Resources.Output;
            this.OutputTab.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(505, 351);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = Resources.Summary;
            // 
            // ExportLogDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 430);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Tabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExportLogDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = Resources.ExportSummary;
            this.TopMost = true;
            this.Tabs.ResumeLayout(false);
            this.WarningTab.ResumeLayout(false);
            this.ErrorTab.ResumeLayout(false);
            this.OutputTab.ResumeLayout(false);
            this.OutputTab.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
