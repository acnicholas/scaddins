/*
 * Created by SharpDevelop.
 * User: derob
 * Date: 24/05/2016
 * Time: 9:44 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SCaddins.RoomConvertor
{
    partial class MainForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonFilter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonRename;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonMain;

        private System.Windows.Forms.RadioButton radioButtonCreateMasses;

        private System.Windows.Forms.RadioButton radioButtonCreateSheets;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        
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
        private void InitializeComponent()        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonFilter = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonRename = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonMain = new System.Windows.Forms.Button();
            this.radioButtonCreateSheets = new System.Windows.Forms.RadioButton();
            this.radioButtonCreateMasses = new System.Windows.Forms.RadioButton();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1SelectionChanged);
            // 
            // buttonFilter
            // 
            resources.ApplyResources(this.buttonFilter, "buttonFilter");
            this.buttonFilter.FlatAppearance.BorderSize = 0;
            this.buttonFilter.Name = "buttonFilter";
            this.buttonFilter.UseVisualStyleBackColor = false;
            this.buttonFilter.Click += new System.EventHandler(this.ButtonFilterClick);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // buttonRename
            // 
            resources.ApplyResources(this.buttonRename, "buttonRename");
            this.buttonRename.FlatAppearance.BorderSize = 0;
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.ButtonInfoClick);
            // 
            // buttonMain
            // 
            resources.ApplyResources(this.buttonMain, "buttonMain");
            this.buttonMain.Name = "buttonMain";
            this.buttonMain.UseVisualStyleBackColor = true;
            this.buttonMain.Click += new System.EventHandler(this.ButtonMainClick);
            // 
            // radioButtonCreateSheets
            // 
            resources.ApplyResources(this.radioButtonCreateSheets, "radioButtonCreateSheets");
            this.radioButtonCreateSheets.Name = "radioButtonCreateSheets";
            this.radioButtonCreateSheets.UseVisualStyleBackColor = true;
            this.radioButtonCreateSheets.CheckedChanged += new System.EventHandler(this.RadioButton1CheckedChanged);
            // 
            // radioButtonCreateMasses
            // 
            this.radioButtonCreateMasses.Checked = true;
            resources.ApplyResources(this.radioButtonCreateMasses, "radioButtonCreateMasses");
            this.radioButtonCreateMasses.Name = "radioButtonCreateMasses";
            this.radioButtonCreateMasses.TabStop = true;
            this.radioButtonCreateMasses.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            resources.ApplyResources(this.button4, "button4");
            this.button4.Name = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Button4Click);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.ButtonResetFiltersClick);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.radioButtonCreateMasses);
            this.Controls.Add(this.radioButtonCreateSheets);
            this.Controls.Add(this.buttonMain);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonRename);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonFilter);
            this.Controls.Add(this.dataGridView1);
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label1;
    }
}
