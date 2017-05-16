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
            this.OutputTab = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.WarningTab = new System.Windows.Forms.TabPage();
            this.warnings = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.ErrorTab = new System.Windows.Forms.TabPage();
            this.errors = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.Tabs.SuspendLayout();
            this.OutputTab.SuspendLayout();
            this.WarningTab.SuspendLayout();
            this.ErrorTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tabs
            // 
            resources.ApplyResources(this.Tabs, "Tabs");
            this.Tabs.Controls.Add(this.OutputTab);
            this.Tabs.Controls.Add(this.WarningTab);
            this.Tabs.Controls.Add(this.ErrorTab);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            // 
            // OutputTab
            // 
            this.OutputTab.Controls.Add(this.textBox1);
            resources.ApplyResources(this.OutputTab, "OutputTab");
            this.OutputTab.Name = "OutputTab";
            this.OutputTab.Text = global::SCaddins.Properties.Resources.Output;
            this.OutputTab.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // WarningTab
            // 
            this.WarningTab.Controls.Add(this.warnings);
            resources.ApplyResources(this.WarningTab, "WarningTab");
            this.WarningTab.Name = "WarningTab";
            this.WarningTab.Text = global::SCaddins.Properties.Resources.Warnings;
            this.WarningTab.UseVisualStyleBackColor = true;
            // 
            // warnings
            // 
            this.warnings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            resources.ApplyResources(this.warnings, "warnings");
            this.warnings.Name = "warnings";
            this.warnings.UseCompatibleStateImageBehavior = false;
            this.warnings.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = global::SCaddins.Properties.Resources.Filename;
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = global::SCaddins.Properties.Resources.Description;
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // ErrorTab
            // 
            this.ErrorTab.Controls.Add(this.errors);
            resources.ApplyResources(this.ErrorTab, "ErrorTab");
            this.ErrorTab.Name = "ErrorTab";
            this.ErrorTab.Text = global::SCaddins.Properties.Resources.Errors;
            this.ErrorTab.UseVisualStyleBackColor = true;
            // 
            // errors
            // 
            this.errors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            resources.ApplyResources(this.errors, "errors");
            this.errors.Name = "errors";
            this.errors.UseCompatibleStateImageBehavior = false;
            this.errors.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = global::SCaddins.Properties.Resources.Filename;
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = global::SCaddins.Properties.Resources.Description;
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ExportLogDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Tabs);
            this.Name = "ExportLogDialog";
            this.TopMost = true;
            this.Tabs.ResumeLayout(false);
            this.OutputTab.ResumeLayout(false);
            this.OutputTab.PerformLayout();
            this.WarningTab.ResumeLayout(false);
            this.ErrorTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
