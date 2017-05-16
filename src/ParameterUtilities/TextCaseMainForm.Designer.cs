namespace SCaddins.ParameterUtils
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
            resources.ApplyResources(this.chkAnnotation, "chkAnnotation");
            this.chkAnnotation.Name = "chkAnnotation";
            this.chkAnnotation.UseVisualStyleBackColor = true;
            this.chkAnnotation.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnOKAY
            // 
            this.btnOKAY.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOKAY, "btnOKAY");
            this.btnOKAY.Name = "btnOKAY";
            this.btnOKAY.UseVisualStyleBackColor = true;
            this.btnOKAY.Click += new System.EventHandler(this.BtnOKAYClick);
            // 
            // chkSheets
            // 
            resources.ApplyResources(this.chkSheets, "chkSheets");
            this.chkSheets.Name = "chkSheets";
            this.chkSheets.UseVisualStyleBackColor = true;
            this.chkSheets.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // chkViews
            // 
            resources.ApplyResources(this.chkViews, "chkViews");
            this.chkViews.Name = "chkViews";
            this.chkViews.UseVisualStyleBackColor = true;
            this.chkViews.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // chkViewTitleOnSheets
            // 
            resources.ApplyResources(this.chkViewTitleOnSheets, "chkViewTitleOnSheets");
            this.chkViewTitleOnSheets.Name = "chkViewTitleOnSheets";
            this.chkViewTitleOnSheets.UseVisualStyleBackColor = true;
            this.chkViewTitleOnSheets.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // chkRooms
            // 
            resources.ApplyResources(this.chkRooms, "chkRooms");
            this.chkRooms.Name = "chkRooms";
            this.chkRooms.UseVisualStyleBackColor = true;
            this.chkRooms.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // chkRevisions
            // 
            resources.ApplyResources(this.chkRevisions, "chkRevisions");
            this.chkRevisions.Name = "chkRevisions";
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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton3);
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButton3
            // 
            resources.ApplyResources(this.radioButton3, "radioButton3");
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.RadioButton3CheckedChanged);
            // 
            // radioButton2
            // 
            resources.ApplyResources(this.radioButton2, "radioButton2");
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.RadioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            resources.ApplyResources(this.radioButton1, "radioButton1");
            this.radioButton1.Checked = true;
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.TabStop = true;
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.RadioButton1_CheckedChanged);
            // 
            // SCulcaseMainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnOKAY);
            this.Controls.Add(this.btnCancel);
            this.Name = "SCulcaseMainForm";
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