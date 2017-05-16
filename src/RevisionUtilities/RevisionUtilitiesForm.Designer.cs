namespace SCaddins.RevisionUtilities
{
    partial class RevisionUtilitiesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RevisionUtilitiesForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonSelectNone = new System.Windows.Forms.Button();
            this.buttonScheduleRevisions = new System.Windows.Forms.Button();
            this.labelDataGridTitle = new System.Windows.Forms.Label();
            this.buttonAssignRevisons = new System.Windows.Forms.Button();
            this.buttonDeleteRevisions = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonClouds = new System.Windows.Forms.RadioButton();
            this.radioButtonRevisions = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle1;
            // 
            // buttonSelectAll
            // 
            resources.ApplyResources(this.buttonSelectAll, "buttonSelectAll");
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.ButtonSelectAll_Click);
            // 
            // buttonSelectNone
            // 
            resources.ApplyResources(this.buttonSelectNone, "buttonSelectNone");
            this.buttonSelectNone.Name = "buttonSelectNone";
            this.buttonSelectNone.UseVisualStyleBackColor = true;
            this.buttonSelectNone.Click += new System.EventHandler(this.ButtonSelectNone_Click);
            // 
            // buttonScheduleRevisions
            // 
            resources.ApplyResources(this.buttonScheduleRevisions, "buttonScheduleRevisions");
            this.buttonScheduleRevisions.Name = "buttonScheduleRevisions";
            this.buttonScheduleRevisions.UseVisualStyleBackColor = true;
            this.buttonScheduleRevisions.Click += new System.EventHandler(this.ButtonScheduleRevisionsClick);
            // 
            // labelDataGridTitle
            // 
            resources.ApplyResources(this.labelDataGridTitle, "labelDataGridTitle");
            this.labelDataGridTitle.Name = "labelDataGridTitle";
            // 
            // buttonAssignRevisons
            // 
            resources.ApplyResources(this.buttonAssignRevisons, "buttonAssignRevisons");
            this.buttonAssignRevisons.Name = "buttonAssignRevisons";
            this.buttonAssignRevisons.UseVisualStyleBackColor = true;
            this.buttonAssignRevisons.Click += new System.EventHandler(this.ButtonAssignRevisionsClick);
            // 
            // buttonDeleteRevisions
            // 
            resources.ApplyResources(this.buttonDeleteRevisions, "buttonDeleteRevisions");
            this.buttonDeleteRevisions.Name = "buttonDeleteRevisions";
            this.buttonDeleteRevisions.UseVisualStyleBackColor = true;
            this.buttonDeleteRevisions.Click += new System.EventHandler(this.ButtonDeleteRevisionsClick);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "*.xls";
            this.saveFileDialog1.FileName = "ExportedClouds";
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            this.saveFileDialog1.SupportMultiDottedExtensions = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.radioButtonClouds);
            this.groupBox1.Controls.Add(this.radioButtonRevisions);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonClouds
            // 
            resources.ApplyResources(this.radioButtonClouds, "radioButtonClouds");
            this.radioButtonClouds.Name = "radioButtonClouds";
            this.radioButtonClouds.UseVisualStyleBackColor = true;
            this.radioButtonClouds.CheckedChanged += new System.EventHandler(this.RadioButtonCloudsCheckedChanged);
            // 
            // radioButtonRevisions
            // 
            this.radioButtonRevisions.Checked = true;
            resources.ApplyResources(this.radioButtonRevisions, "radioButtonRevisions");
            this.radioButtonRevisions.Name = "radioButtonRevisions";
            this.radioButtonRevisions.TabStop = true;
            this.radioButtonRevisions.UseVisualStyleBackColor = true;
            this.radioButtonRevisions.CheckedChanged += new System.EventHandler(this.RadioButtonRevisionsCheckedChanged);
            // 
            // RevisionUtilitiesForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonDeleteRevisions);
            this.Controls.Add(this.buttonAssignRevisons);
            this.Controls.Add(this.labelDataGridTitle);
            this.Controls.Add(this.buttonScheduleRevisions);
            this.Controls.Add(this.buttonSelectNone);
            this.Controls.Add(this.buttonSelectAll);
            this.Controls.Add(this.dataGridView1);
            this.Name = "RevisionUtilitiesForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonSelectNone;
        private System.Windows.Forms.Button buttonScheduleRevisions;
        private System.Windows.Forms.Label labelDataGridTitle;
        private System.Windows.Forms.Button buttonAssignRevisons;
        private System.Windows.Forms.Button buttonDeleteRevisions;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonClouds;
        private System.Windows.Forms.RadioButton radioButtonRevisions;

    }
}