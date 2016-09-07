namespace SCaddins.SolarUtils
{
    partial class SCaosForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SCaosForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButtonShadowPlans = new System.Windows.Forms.RadioButton();
            this.radioButtonWinterViews = new System.Windows.Forms.RadioButton();
            this.radioButtonRotateCurrent = new System.Windows.Forms.RadioButton();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBoxWinterViewOptions = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.interval = new System.Windows.Forms.ComboBox();
            this.endTime = new System.Windows.Forms.ComboBox();
            this.startTime = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxWinterViewOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 165);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current View Information";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(7, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(357, 134);
            this.listBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(307, 437);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(226, 437);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Controls.Add(this.radioButtonShadowPlans);
            this.groupBox2.Controls.Add(this.radioButtonWinterViews);
            this.groupBox2.Controls.Add(this.radioButtonRotateCurrent);
            this.groupBox2.Location = new System.Drawing.Point(12, 183);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(370, 132);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mode";
            // 
            // radioButton1
            // 
            this.radioButton1.Enabled = false;
            this.radioButton1.Location = new System.Drawing.Point(6, 95);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(358, 36);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Culculate Solar Hours - Mass Mode (EXPERIMENTAL)";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButtonShadowPlans
            // 
            this.radioButtonShadowPlans.Location = new System.Drawing.Point(6, 68);
            this.radioButtonShadowPlans.Name = "radioButtonShadowPlans";
            this.radioButtonShadowPlans.Size = new System.Drawing.Size(358, 36);
            this.radioButtonShadowPlans.TabIndex = 2;
            this.radioButtonShadowPlans.TabStop = true;
            this.radioButtonShadowPlans.Text = "Create Multiple Shadow Plans ";
            this.radioButtonShadowPlans.UseVisualStyleBackColor = true;
            this.radioButtonShadowPlans.CheckedChanged += new System.EventHandler(this.RadioButtonShadowPlansCheckedChanged);
            // 
            // radioButtonWinterViews
            // 
            this.radioButtonWinterViews.Location = new System.Drawing.Point(6, 46);
            this.radioButtonWinterViews.Name = "radioButtonWinterViews";
            this.radioButtonWinterViews.Size = new System.Drawing.Size(290, 29);
            this.radioButtonWinterViews.TabIndex = 1;
            this.radioButtonWinterViews.TabStop = true;
            this.radioButtonWinterViews.Text = "Create Multiple Angle of Sun Views (3d)";
            this.radioButtonWinterViews.UseVisualStyleBackColor = true;
            this.radioButtonWinterViews.CheckedChanged += new System.EventHandler(this.RadioButtonWinterViewsCheckedChanged);
            // 
            // radioButtonRotateCurrent
            // 
            this.radioButtonRotateCurrent.Location = new System.Drawing.Point(6, 19);
            this.radioButtonRotateCurrent.Name = "radioButtonRotateCurrent";
            this.radioButtonRotateCurrent.Size = new System.Drawing.Size(283, 32);
            this.radioButtonRotateCurrent.TabIndex = 0;
            this.radioButtonRotateCurrent.TabStop = true;
            this.radioButtonRotateCurrent.Text = "Rotate Current View";
            this.radioButtonRotateCurrent.UseVisualStyleBackColor = true;
            this.radioButtonRotateCurrent.CheckedChanged += new System.EventHandler(this.RadioButtonRotateCurrentCheckedChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(11, 437);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Help";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3Click);
            // 
            // groupBoxWinterViewOptions
            // 
            this.groupBoxWinterViewOptions.Controls.Add(this.label4);
            this.groupBoxWinterViewOptions.Controls.Add(this.dateTimePicker1);
            this.groupBoxWinterViewOptions.Controls.Add(this.label3);
            this.groupBoxWinterViewOptions.Controls.Add(this.label2);
            this.groupBoxWinterViewOptions.Controls.Add(this.label1);
            this.groupBoxWinterViewOptions.Controls.Add(this.interval);
            this.groupBoxWinterViewOptions.Controls.Add(this.endTime);
            this.groupBoxWinterViewOptions.Controls.Add(this.startTime);
            this.groupBoxWinterViewOptions.Location = new System.Drawing.Point(12, 320);
            this.groupBoxWinterViewOptions.Name = "groupBoxWinterViewOptions";
            this.groupBoxWinterViewOptions.Size = new System.Drawing.Size(370, 111);
            this.groupBoxWinterViewOptions.TabIndex = 5;
            this.groupBoxWinterViewOptions.TabStop = false;
            this.groupBoxWinterViewOptions.Text = "Multi View Creation Options";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Date";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(6, 38);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(133, 20);
            this.dateTimePicker1.TabIndex = 6;
            this.dateTimePicker1.Value = new System.DateTime(2015, 6, 21, 0, 0, 0, 0);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.DateTimePicker1ValueChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(295, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Interval";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(144, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "End Time";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Start Time";
            // 
            // interval
            // 
            this.interval.FormattingEnabled = true;
            this.interval.Location = new System.Drawing.Point(295, 79);
            this.interval.Name = "interval";
            this.interval.Size = new System.Drawing.Size(68, 21);
            this.interval.TabIndex = 2;
            // 
            // endTime
            // 
            this.endTime.FormattingEnabled = true;
            this.endTime.Location = new System.Drawing.Point(144, 79);
            this.endTime.Name = "endTime";
            this.endTime.Size = new System.Drawing.Size(144, 21);
            this.endTime.TabIndex = 1;
            // 
            // startTime
            // 
            this.startTime.FormattingEnabled = true;
            this.startTime.Location = new System.Drawing.Point(6, 79);
            this.startTime.Name = "startTime";
            this.startTime.Size = new System.Drawing.Size(132, 21);
            this.startTime.TabIndex = 0;
            // 
            // SCaosForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 469);
            this.Controls.Add(this.groupBoxWinterViewOptions);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SCaosForm";
            this.Text = "SCaos - Angle Of Sun";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBoxWinterViewOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button button3;
        internal System.Windows.Forms.RadioButton radioButtonRotateCurrent;
        internal System.Windows.Forms.RadioButton radioButtonWinterViews;
        internal System.Windows.Forms.RadioButton radioButtonShadowPlans;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBoxWinterViewOptions;
        internal System.Windows.Forms.ComboBox interval;
        internal System.Windows.Forms.ComboBox endTime;
        internal System.Windows.Forms.ComboBox startTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label4;
    }
}
