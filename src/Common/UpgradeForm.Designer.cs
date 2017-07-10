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
    using SCaddins.Properties;

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
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.buttonLog);
            this.groupBox2.Controls.Add(this.labelUpgradeNote);
            this.groupBox2.Controls.Add(this.buttonDownload);
            this.groupBox2.Controls.Add(this.labelLatestVersion);
            this.groupBox2.Controls.Add(this.labelInstalledVersion);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // buttonLog
            // 
            resources.ApplyResources(this.buttonLog, "buttonLog");
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Text = global::SCaddins.Properties.Resources.UpdateViewChangeLog;
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.Button1Click);
            // 
            // labelUpgradeNote
            // 
            resources.ApplyResources(this.labelUpgradeNote, "labelUpgradeNote");
            this.labelUpgradeNote.Name = "labelUpgradeNote";
            // 
            // buttonDownload
            // 
            resources.ApplyResources(this.buttonDownload, "buttonDownload");
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Text = global::SCaddins.Properties.Resources.Download;
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.Button3Click);
            // 
            // labelLatestVersion
            // 
            resources.ApplyResources(this.labelLatestVersion, "labelLatestVersion");
            this.labelLatestVersion.Name = "labelLatestVersion";
            // 
            // labelInstalledVersion
            // 
            resources.ApplyResources(this.labelInstalledVersion, "labelInstalledVersion");
            this.labelInstalledVersion.Name = "labelInstalledVersion";
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Text = global::SCaddins.Properties.Resources.Close;
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // UpgradeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBox2);
            this.Name = "UpgradeForm";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
