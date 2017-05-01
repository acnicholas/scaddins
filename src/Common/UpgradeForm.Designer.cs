/*
 * Created by SharpDevelop.
 * User: andrewn
 * Date: 28/04/15
 * Time: 4:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SCaddins.Common
{
    partial class UpgradeForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.Label labelLatestVersion;
        private System.Windows.Forms.Label labelInstalledVersion;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelUpgradeNote;
        private System.Windows.Forms.Button buttonLog;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpgradeForm));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonLog = new System.Windows.Forms.Button();
            this.labelUpgradeNote = new System.Windows.Forms.Label();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.labelLatestVersion = new System.Windows.Forms.Label();
            this.labelInstalledVersion = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.buttonLog);
            this.groupBox2.Controls.Add(this.labelUpgradeNote);
            this.groupBox2.Controls.Add(this.buttonDownload);
            this.groupBox2.Controls.Add(this.labelLatestVersion);
            this.groupBox2.Controls.Add(this.labelInstalledVersion);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(421, 302);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = SCaddins.Properties.Resources.VersionInformation;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 97);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(409, 170);
            this.textBox1.TabIndex = 5;
            // 
            // buttonLog
            // 
            this.buttonLog.Location = new System.Drawing.Point(235, 273);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new System.Drawing.Size(180, 23);
            this.buttonLog.TabIndex = 4;
            this.buttonLog.Text = "View Complete Change Log";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.Button1Click);
            // 
            // labelUpgradeNote
            // 
            this.labelUpgradeNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUpgradeNote.Location = new System.Drawing.Point(6, 71);
            this.labelUpgradeNote.Name = "labelUpgradeNote";
            this.labelUpgradeNote.Size = new System.Drawing.Size(409, 23);
            this.labelUpgradeNote.TabIndex = 3;
            this.labelUpgradeNote.Text = "...";
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(6, 273);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(131, 23);
            this.buttonDownload.TabIndex = 2;
            this.buttonDownload.Text = SCaddins.Properties.Resources.Download;
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.Button3Click);
            // 
            // labelLatestVersion
            // 
            this.labelLatestVersion.Location = new System.Drawing.Point(6, 48);
            this.labelLatestVersion.Name = "labelLatestVersion";
            this.labelLatestVersion.Size = new System.Drawing.Size(409, 23);
            this.labelLatestVersion.TabIndex = 1;
            this.labelLatestVersion.Text = Properties.Resources.LatestVersion;
            // 
            // labelInstalledVersion
            // 
            this.labelInstalledVersion.Location = new System.Drawing.Point(6, 25);
            this.labelInstalledVersion.Name = "labelInstalledVersion";
            this.labelInstalledVersion.Size = new System.Drawing.Size(409, 23);
            this.labelInstalledVersion.TabIndex = 0;
            this.labelInstalledVersion.Text = Properties.Resources.InstalledVersion;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(359, 320);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = Properties.Resources.Close;
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // UpgradeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 355);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpgradeForm";
            this.Text = "SCaddins Version Information";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
