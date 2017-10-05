/*
 * Created by SharpDevelop.
 * User: andrewn
 * Date: 25/08/17
 * Time: 2:43 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SCaddins.RenameUtilities
{
    partial class Form1
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonRenameAll;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox grpParameter;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox grpCategory;
        private System.Windows.Forms.GroupBox grpRename;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBoxReplace;
        private System.Windows.Forms.TextBox textBoxFind;
        private System.Windows.Forms.ComboBox comboBoxPresets;
        private System.Windows.Forms.Button buttonRenameSelected;
        private System.Windows.Forms.Label labelReplacement;
        private System.Windows.Forms.Label labelPattern;
        private System.Windows.Forms.Label label1;
        
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpParameter = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.grpCategory = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.grpRename = new System.Windows.Forms.GroupBox();
            this.labelReplacement = new System.Windows.Forms.Label();
            this.labelPattern = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxReplace = new System.Windows.Forms.TextBox();
            this.textBoxFind = new System.Windows.Forms.TextBox();
            this.comboBoxPresets = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonRenameAll = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonRenameSelected = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpParameter.SuspendLayout();
            this.grpCategory.SuspendLayout();
            this.grpRename.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(859, 421);
            this.panel1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grpParameter);
            this.splitContainer1.Panel1.Controls.Add(this.grpCategory);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grpRename);
            this.splitContainer1.Size = new System.Drawing.Size(859, 421);
            this.splitContainer1.SplitterDistance = 339;
            this.splitContainer1.TabIndex = 0;
            // 
            // grpParameter
            // 
            this.grpParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpParameter.Controls.Add(this.listBox1);
            this.grpParameter.Location = new System.Drawing.Point(3, 59);
            this.grpParameter.Name = "grpParameter";
            this.grpParameter.Size = new System.Drawing.Size(333, 359);
            this.grpParameter.TabIndex = 1;
            this.grpParameter.TabStop = false;
            this.grpParameter.Text = "2. Parameter";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(321, 329);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.ListBox1SelectedIndexChanged);
            // 
            // grpCategory
            // 
            this.grpCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCategory.Controls.Add(this.comboBox1);
            this.grpCategory.Location = new System.Drawing.Point(0, 0);
            this.grpCategory.Name = "grpCategory";
            this.grpCategory.Size = new System.Drawing.Size(330, 53);
            this.grpCategory.TabIndex = 0;
            this.grpCategory.TabStop = false;
            this.grpCategory.Text = "1. Category";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(0, 19);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(330, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
            // 
            // grpRename
            // 
            this.grpRename.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRename.Controls.Add(this.labelReplacement);
            this.grpRename.Controls.Add(this.labelPattern);
            this.grpRename.Controls.Add(this.label1);
            this.grpRename.Controls.Add(this.textBoxReplace);
            this.grpRename.Controls.Add(this.textBoxFind);
            this.grpRename.Controls.Add(this.comboBoxPresets);
            this.grpRename.Controls.Add(this.dataGridView1);
            this.grpRename.Location = new System.Drawing.Point(3, 3);
            this.grpRename.MinimumSize = new System.Drawing.Size(500, 300);
            this.grpRename.Name = "grpRename";
            this.grpRename.Size = new System.Drawing.Size(513, 418);
            this.grpRename.TabIndex = 0;
            this.grpRename.TabStop = false;
            this.grpRename.Text = "3. Rename";
            // 
            // labelReplacement
            // 
            this.labelReplacement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReplacement.Location = new System.Drawing.Point(247, 46);
            this.labelReplacement.Name = "labelReplacement";
            this.labelReplacement.Size = new System.Drawing.Size(83, 18);
            this.labelReplacement.TabIndex = 6;
            this.labelReplacement.Text = "Replacement";
            this.labelReplacement.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPattern
            // 
            this.labelPattern.Location = new System.Drawing.Point(6, 46);
            this.labelPattern.Name = "labelPattern";
            this.labelPattern.Size = new System.Drawing.Size(59, 18);
            this.labelPattern.TabIndex = 5;
            this.labelPattern.Text = "Pattern";
            this.labelPattern.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Mode";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxReplace
            // 
            this.textBoxReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxReplace.Location = new System.Drawing.Point(336, 43);
            this.textBoxReplace.Name = "textBoxReplace";
            this.textBoxReplace.Size = new System.Drawing.Size(171, 20);
            this.textBoxReplace.TabIndex = 3;
            this.textBoxReplace.TextChanged += new System.EventHandler(this.TextBoxReplaceTextChanged);
            // 
            // textBoxFind
            // 
            this.textBoxFind.Location = new System.Drawing.Point(71, 43);
            this.textBoxFind.Name = "textBoxFind";
            this.textBoxFind.Size = new System.Drawing.Size(145, 20);
            this.textBoxFind.TabIndex = 2;
            this.textBoxFind.TextChanged += new System.EventHandler(this.TextBoxFindTextChanged);
            // 
            // comboBoxPresets
            // 
            this.comboBoxPresets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPresets.FormattingEnabled = true;
            this.comboBoxPresets.Location = new System.Drawing.Point(71, 16);
            this.comboBoxPresets.Name = "comboBoxPresets";
            this.comboBoxPresets.Size = new System.Drawing.Size(436, 21);
            this.comboBoxPresets.TabIndex = 1;
            this.comboBoxPresets.SelectedValueChanged += new System.EventHandler(this.ComboBoxPresetsSelectedValueChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 75);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(501, 329);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.DataGridView1RowPrePaint);
            // 
            // buttonRenameAll
            // 
            this.buttonRenameAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRenameAll.Location = new System.Drawing.Point(747, 444);
            this.buttonRenameAll.Name = "buttonRenameAll";
            this.buttonRenameAll.Size = new System.Drawing.Size(124, 23);
            this.buttonRenameAll.TabIndex = 1;
            this.buttonRenameAll.Text = "Rename All Matches";
            this.buttonRenameAll.UseVisualStyleBackColor = true;
            this.buttonRenameAll.Click += new System.EventHandler(this.Button1Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(12, 444);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // buttonRenameSelected
            // 
            this.buttonRenameSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRenameSelected.Location = new System.Drawing.Point(588, 444);
            this.buttonRenameSelected.Name = "buttonRenameSelected";
            this.buttonRenameSelected.Size = new System.Drawing.Size(153, 23);
            this.buttonRenameSelected.TabIndex = 2;
            this.buttonRenameSelected.Text = "Rename Selected Matches";
            this.buttonRenameSelected.UseVisualStyleBackColor = true;
            this.buttonRenameSelected.Click += new System.EventHandler(this.ButtonRenameSelectedClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 479);
            this.Controls.Add(this.buttonRenameSelected);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonRenameAll);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Rename";
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpParameter.ResumeLayout(false);
            this.grpCategory.ResumeLayout(false);
            this.grpRename.ResumeLayout(false);
            this.grpRename.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
