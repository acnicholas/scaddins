/*
 * Created by SharpDevelop.
 * User: andrewn
 * Date: 12/05/14
 * Time: 4:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SCaddins.SCexport
{
    partial class SelectPrinterDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectPrinterDialog));
            this.grpSelectPrinter = new System.Windows.Forms.GroupBox();
            this.comboBoxPrinter = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.grpSelectPrinter.SuspendLayout();
            this.SuspendLayout(); 
            this.grpSelectPrinter.Controls.Add(this.comboBoxPrinter);
            this.grpSelectPrinter.Location = new System.Drawing.Point(12, 12);
            this.grpSelectPrinter.Name = "grpSelectPrinter";
            this.grpSelectPrinter.Size = new System.Drawing.Size(293, 77);
            this.grpSelectPrinter.TabIndex = 0;
            this.grpSelectPrinter.TabStop = false;
            this.grpSelectPrinter.Text = "Select printer from combo box";
            this.comboBoxPrinter.FormattingEnabled = true;
            this.comboBoxPrinter.Location = new System.Drawing.Point(16, 32);
            this.comboBoxPrinter.Name = "cbxPrinter";
            this.comboBoxPrinter.Size = new System.Drawing.Size(256, 21);
            this.comboBoxPrinter.TabIndex = 0;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(212, 95);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(93, 26);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(12, 95);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 26);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(317, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 129);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpSelectPrinter);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SelectPrinterDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Printer...";
            this.grpSelectPrinter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        private System.Windows.Forms.GroupBox grpSelectPrinter;
        internal System.Windows.Forms.ComboBox comboBoxPrinter;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}
