namespace SCaddins.ParameterUtils
{
    partial class SCincrementSettingsForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox offsetTextBox;
        private System.Windows.Forms.TextBox incrementTextBox;
        private System.Windows.Forms.TextBox replacementTextBox;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SCincrementSettingsForm));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.customParamTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.destReplacementTextBox = new System.Windows.Forms.TextBox();
            this.destSearchTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.offsetTextBox = new System.Windows.Forms.TextBox();
            this.incrementTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.replacementTextBox = new System.Windows.Forms.TextBox();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CustomParamCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 296);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(129, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Reset to Defaults";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(421, 296);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2Click);
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(340, 296);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CustomParamCheckBox);
            this.groupBox1.Controls.Add(this.customParamTextBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.destReplacementTextBox);
            this.groupBox1.Controls.Add(this.destSearchTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.offsetTextBox);
            this.groupBox1.Controls.Add(this.incrementTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.replacementTextBox);
            this.groupBox1.Controls.Add(this.searchTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(484, 278);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // customParamTextBox
            // 
            this.customParamTextBox.Location = new System.Drawing.Point(295, 245);
            this.customParamTextBox.Name = "customParamTextBox";
            this.customParamTextBox.Size = new System.Drawing.Size(183, 20);
            this.customParamTextBox.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 248);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(195, 23);
            this.label7.TabIndex = 16;
            this.label7.Text = "Custom Parameter Name";
            // 
            // destReplacementTextBox
            // 
            this.destReplacementTextBox.Location = new System.Drawing.Point(295, 98);
            this.destReplacementTextBox.Name = "destReplacementTextBox";
            this.destReplacementTextBox.Size = new System.Drawing.Size(183, 20);
            this.destReplacementTextBox.TabIndex = 15;
            // 
            // destSearchTextBox
            // 
            this.destSearchTextBox.Location = new System.Drawing.Point(295, 75);
            this.destSearchTextBox.Name = "destSearchTextBox";
            this.destSearchTextBox.Size = new System.Drawing.Size(183, 20);
            this.destSearchTextBox.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(182, 23);
            this.label5.TabIndex = 13;
            this.label5.Text = "Destination Replacement Pattern";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(139, 23);
            this.label6.TabIndex = 12;
            this.label6.Text = "Destination Search Pattern";
            // 
            // offsetTextBox
            // 
            this.offsetTextBox.Location = new System.Drawing.Point(295, 168);
            this.offsetTextBox.Name = "offsetTextBox";
            this.offsetTextBox.Size = new System.Drawing.Size(183, 20);
            this.offsetTextBox.TabIndex = 11;
            // 
            // incrementTextBox
            // 
            this.incrementTextBox.Location = new System.Drawing.Point(295, 145);
            this.incrementTextBox.Name = "incrementTextBox";
            this.incrementTextBox.Size = new System.Drawing.Size(183, 20);
            this.incrementTextBox.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 9;
            this.label4.Text = "Offset Value";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 8;
            this.label3.Text = "Increment Value";
            // 
            // replacementTextBox
            // 
            this.replacementTextBox.Location = new System.Drawing.Point(295, 49);
            this.replacementTextBox.Name = "replacementTextBox";
            this.replacementTextBox.Size = new System.Drawing.Size(183, 20);
            this.replacementTextBox.TabIndex = 5;
            // 
            // searchTextBox
            // 
            this.searchTextBox.Location = new System.Drawing.Point(295, 26);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(183, 20);
            this.searchTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(182, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Source Replacement Pattern";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source Search Pattern";
            // 
            // CustomParamCheckBox
            // 
            this.CustomParamCheckBox.Location = new System.Drawing.Point(6, 221);
            this.CustomParamCheckBox.Name = "CustomParamCheckBox";
            this.CustomParamCheckBox.Size = new System.Drawing.Size(170, 24);
            this.CustomParamCheckBox.TabIndex = 18;
            this.CustomParamCheckBox.Text = "Use Custom Parameter";
            this.CustomParamCheckBox.UseVisualStyleBackColor = true;
            // 
            // SCincrementSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 331);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SCincrementSettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SCincrement Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TextBox destSearchTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox destReplacementTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox customParamTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox CustomParamCheckBox;
    }
}
