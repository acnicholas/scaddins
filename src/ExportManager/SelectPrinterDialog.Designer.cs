/*
 * Created by SharpDevelop.
 * User: andrewn
 * Date: 12/05/14
 * Time: 4:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SCaddins.ExportManager
{
    using SCaddins.Properties;

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
            // 
            // grpSelectPrinter
            // 
            this.grpSelectPrinter.Controls.Add(this.comboBoxPrinter);
            resources.ApplyResources(this.grpSelectPrinter, "grpSelectPrinter");
            this.grpSelectPrinter.Name = "grpSelectPrinter";
            this.grpSelectPrinter.TabStop = false;
            // 
            // comboBoxPrinter
            // 
            this.comboBoxPrinter.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxPrinter, "comboBoxPrinter");
            this.comboBoxPrinter.Name = "comboBoxPrinter";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.Text = global::SCaddins.Properties.Resources.OK;
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Text = global::SCaddins.Properties.Resources.Cancel;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // SelectPrinterDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpSelectPrinter);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SelectPrinterDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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
