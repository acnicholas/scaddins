namespace SCaddins.SCulcase
{
    partial class SCulcaseMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            //if (dialog != null) dialog.Dispose();
            if (disposing && (components != null)) {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SCulcaseMainForm));
            this.chkAnnotation = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOKAY = new System.Windows.Forms.Button();
            this.chkSheets = new System.Windows.Forms.CheckBox();
            this.chkViews = new System.Windows.Forms.CheckBox();
            this.chkViewTitleOnSheets = new System.Windows.Forms.CheckBox();
            this.chkRooms = new System.Windows.Forms.CheckBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.chkRevisions = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkAnnotation
            // 
            this.chkAnnotation.AutoSize = true;
            this.chkAnnotation.Location = new System.Drawing.Point(6, 19);
            this.chkAnnotation.Name = "chkAnnotation";
            this.chkAnnotation.Size = new System.Drawing.Size(122, 17);
            this.chkAnnotation.TabIndex = 0;
            this.chkAnnotation.Text = "General Annotations";
            this.chkAnnotation.UseVisualStyleBackColor = true;
            this.chkAnnotation.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(12, 287);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnOKAY
            // 
            this.btnOKAY.Location = new System.Drawing.Point(276, 287);
            this.btnOKAY.Name = "btnOKAY";
            this.btnOKAY.Size = new System.Drawing.Size(75, 23);
            this.btnOKAY.TabIndex = 2;
            this.btnOKAY.Text = "OK";
            this.btnOKAY.UseVisualStyleBackColor = true;
            this.btnOKAY.Click += new System.EventHandler(this.BtnOKAYClick);
            // 
            // chkSheets
            // 
            this.chkSheets.AutoSize = true;
            this.chkSheets.Location = new System.Drawing.Point(6, 42);
            this.chkSheets.Name = "chkSheets";
            this.chkSheets.Size = new System.Drawing.Size(183, 17);
            this.chkSheets.TabIndex = 3;
            this.chkSheets.Text = "Sheet Names (Title on Titleblock)";
            this.chkSheets.UseVisualStyleBackColor = true;
            this.chkSheets.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // chkViews
            // 
            this.chkViews.AutoSize = true;
            this.chkViews.Location = new System.Drawing.Point(6, 65);
            this.chkViews.Name = "chkViews";
            this.chkViews.Size = new System.Drawing.Size(85, 17);
            this.chkViews.TabIndex = 4;
            this.chkViews.Text = "View Names";
            this.chkViews.UseVisualStyleBackColor = true;
            this.chkViews.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // chkViewTitleOnSheets
            // 
            this.chkViewTitleOnSheets.AutoSize = true;
            this.chkViewTitleOnSheets.Location = new System.Drawing.Point(6, 88);
            this.chkViewTitleOnSheets.Name = "chkViewTitleOnSheets";
            this.chkViewTitleOnSheets.Size = new System.Drawing.Size(204, 17);
            this.chkViewTitleOnSheets.TabIndex = 5;
            this.chkViewTitleOnSheets.Text = "View name overrides (Title on Sheets)";
            this.chkViewTitleOnSheets.UseVisualStyleBackColor = true;
            this.chkViewTitleOnSheets.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // chkRooms
            // 
            this.chkRooms.AutoSize = true;
            this.chkRooms.Location = new System.Drawing.Point(6, 111);
            this.chkRooms.Name = "chkRooms";
            this.chkRooms.Size = new System.Drawing.Size(88, 17);
            this.chkRooms.TabIndex = 6;
            this.chkRooms.Text = "Room names";
            this.chkRooms.UseVisualStyleBackColor = true;
            this.chkRooms.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(195, 287);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 7;
            this.btnTest.Text = "Dry Run";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // chkRevisions
            // 
            this.chkRevisions.AutoSize = true;
            this.chkRevisions.Enabled = false;
            this.chkRevisions.Location = new System.Drawing.Point(6, 134);
            this.chkRevisions.Name = "chkRevisions";
            this.chkRevisions.Size = new System.Drawing.Size(125, 17);
            this.chkRevisions.TabIndex = 8;
            this.chkRevisions.Text = "Revision Desciptions";
            this.chkRevisions.UseVisualStyleBackColor = true;
            this.chkRevisions.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkAnnotation);
            this.groupBox1.Controls.Add(this.chkRevisions);
            this.groupBox1.Controls.Add(this.chkSheets);
            this.groupBox1.Controls.Add(this.chkViews);
            this.groupBox1.Controls.Add(this.chkRooms);
            this.groupBox1.Controls.Add(this.chkViewTitleOnSheets);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(339, 168);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Elements to Convert";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton3);
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Location = new System.Drawing.Point(12, 186);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(338, 95);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Conversion Type";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(9, 65);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(72, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.Text = "Title Case";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.RadioButton3CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(9, 42);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(73, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "lowercase";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.RadioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(9, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(90, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "UPPERCASE";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.RadioButton1_CheckedChanged);
            // 
            // SCulcaseMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 322);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnOKAY);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SCulcaseMainForm";
            this.Text = "SCulcase - SC Uppercase/Lowercase Tool";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.CheckBox chkRevisions;

        #endregion

        private System.Windows.Forms.Button btnOKAY;
        private System.Windows.Forms.CheckBox chkAnnotation;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkSheets;
        private System.Windows.Forms.CheckBox chkViews;
        private System.Windows.Forms.CheckBox chkViewTitleOnSheets;
        private System.Windows.Forms.CheckBox chkRooms;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        
    }
}