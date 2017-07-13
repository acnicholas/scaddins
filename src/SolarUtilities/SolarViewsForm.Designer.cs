namespace SCaddins.SolarUtilities
{
    using SCaddins.Properties;

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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.Name = "listBox1";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.Text = global::SCaddins.Properties.Resources.OK;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.Text = global::SCaddins.Properties.Resources.Cancel;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonShadowPlans);
            this.groupBox2.Controls.Add(this.radioButtonWinterViews);
            this.groupBox2.Controls.Add(this.radioButtonRotateCurrent);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonShadowPlans
            // 
            resources.ApplyResources(this.radioButtonShadowPlans, "radioButtonShadowPlans");
            this.radioButtonShadowPlans.Name = "radioButtonShadowPlans";
            this.radioButtonShadowPlans.TabStop = true;
            this.radioButtonShadowPlans.Text = global::SCaddins.Properties.Resources.SolarViewsCreateMultipleShadowPlans;
            this.radioButtonShadowPlans.UseVisualStyleBackColor = true;
            this.radioButtonShadowPlans.CheckedChanged += new System.EventHandler(this.RadioButtonShadowPlansCheckedChanged);
            // 
            // radioButtonWinterViews
            // 
            resources.ApplyResources(this.radioButtonWinterViews, "radioButtonWinterViews");
            this.radioButtonWinterViews.Name = "radioButtonWinterViews";
            this.radioButtonWinterViews.TabStop = true;
            this.radioButtonWinterViews.Text = global::SCaddins.Properties.Resources.SolarViewsCreateMultipleSunViews;
            this.radioButtonWinterViews.UseVisualStyleBackColor = true;
            this.radioButtonWinterViews.CheckedChanged += new System.EventHandler(this.RadioButtonWinterViewsCheckedChanged);
            // 
            // radioButtonRotateCurrent
            // 
            resources.ApplyResources(this.radioButtonRotateCurrent, "radioButtonRotateCurrent");
            this.radioButtonRotateCurrent.Name = "radioButtonRotateCurrent";
            this.radioButtonRotateCurrent.TabStop = true;
            this.radioButtonRotateCurrent.Text = global::SCaddins.Properties.Resources.SolarViewsRotateCurrentView;
            this.radioButtonRotateCurrent.UseVisualStyleBackColor = true;
            this.radioButtonRotateCurrent.CheckedChanged += new System.EventHandler(this.RadioButtonRotateCurrentCheckedChanged);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.Text = global::SCaddins.Properties.Resources.Help;
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
            resources.ApplyResources(this.groupBoxWinterViewOptions, "groupBoxWinterViewOptions");
            this.groupBoxWinterViewOptions.Name = "groupBoxWinterViewOptions";
            this.groupBoxWinterViewOptions.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            resources.ApplyResources(this.dateTimePicker1, "dateTimePicker1");
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Value = new System.DateTime(2017, 6, 21, 0, 0, 0, 0);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.DateTimePicker1ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
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
            // interval
            // 
            this.interval.FormattingEnabled = true;
            resources.ApplyResources(this.interval, "interval");
            this.interval.Name = "interval";
            // 
            // endTime
            // 
            this.endTime.FormattingEnabled = true;
            resources.ApplyResources(this.endTime, "endTime");
            this.endTime.Name = "endTime";
            // 
            // startTime
            // 
            this.startTime.FormattingEnabled = true;
            resources.ApplyResources(this.startTime, "startTime");
            this.startTime.Name = "startTime";
            // 
            // SCaosForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxWinterViewOptions);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SCaosForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBoxWinterViewOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }
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
