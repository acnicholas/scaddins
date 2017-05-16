// (C) Copyright 2014 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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

    public partial class RenameSheetForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenameSheetForm));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.TestRunButton = new System.Windows.Forms.Button();
            this.RenameButton = new System.Windows.Forms.Button();
            this.CancelRenameButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxNameReplace = new System.Windows.Forms.TextBox();
            this.textBoxNamePattern = new System.Windows.Forms.TextBox();
            this.Match = new System.Windows.Forms.Label();
            this.textBoxNumberReplace = new System.Windows.Forms.TextBox();
            this.textBoxNumberPattern = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = global::SCaddins.Properties.Resources.ExistingNumber;
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = global::SCaddins.Properties.Resources.ExistingName;
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = global::SCaddins.Properties.Resources.NewNumber;
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = global::SCaddins.Properties.Resources.NewName;
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // TestRunButton
            // 
            resources.ApplyResources(this.TestRunButton, "TestRunButton");
            this.TestRunButton.Name = "TestRunButton";
            this.TestRunButton.Text = global::SCaddins.Properties.Resources.TestRun;
            this.TestRunButton.UseVisualStyleBackColor = true;
            this.TestRunButton.Click += new System.EventHandler(this.TestRunButtonClick);
            // 
            // RenameButton
            // 
            resources.ApplyResources(this.RenameButton, "RenameButton");
            this.RenameButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.RenameButton.Name = "RenameButton";
            this.RenameButton.Text = global::SCaddins.Properties.Resources.Rename;
            this.RenameButton.UseVisualStyleBackColor = true;
            this.RenameButton.Click += new System.EventHandler(this.RenameButtonClick);
            // 
            // CancelRenameButton
            // 
            resources.ApplyResources(this.CancelRenameButton, "CancelRenameButton");
            this.CancelRenameButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelRenameButton.Name = "CancelRenameButton";
            this.CancelRenameButton.Text = global::SCaddins.Properties.Resources.Cancel;
            this.CancelRenameButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxNameReplace);
            this.groupBox1.Controls.Add(this.textBoxNamePattern);
            this.groupBox1.Controls.Add(this.Match);
            this.groupBox1.Controls.Add(this.textBoxNumberReplace);
            this.groupBox1.Controls.Add(this.textBoxNumberPattern);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBoxNameReplace
            // 
            resources.ApplyResources(this.textBoxNameReplace, "textBoxNameReplace");
            this.textBoxNameReplace.Name = "textBoxNameReplace";
            // 
            // textBoxNamePattern
            // 
            resources.ApplyResources(this.textBoxNamePattern, "textBoxNamePattern");
            this.textBoxNamePattern.Name = "textBoxNamePattern";
            // 
            // Match
            // 
            resources.ApplyResources(this.Match, "Match");
            this.Match.Name = "Match";
            // 
            // textBoxNumberReplace
            // 
            resources.ApplyResources(this.textBoxNumberReplace, "textBoxNumberReplace");
            this.textBoxNumberReplace.Name = "textBoxNumberReplace";
            // 
            // textBoxNumberPattern
            // 
            resources.ApplyResources(this.textBoxNumberPattern, "textBoxNumberPattern");
            this.textBoxNumberPattern.Name = "textBoxNumberPattern";
            // 
            // RenameSheetForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CancelRenameButton);
            this.Controls.Add(this.RenameButton);
            this.Controls.Add(this.TestRunButton);
            this.Controls.Add(this.listView1);
            this.Name = "RenameSheetForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxNameReplace;
        private System.Windows.Forms.TextBox textBoxNamePattern;
        
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Match;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button CancelRenameButton;
        private System.Windows.Forms.Button RenameButton;
        private System.Windows.Forms.Button TestRunButton;
        private System.Windows.Forms.TextBox textBoxNumberReplace;
        private System.Windows.Forms.TextBox textBoxNumberPattern;
        private System.Windows.Forms.ListView listView1;
    }
}
