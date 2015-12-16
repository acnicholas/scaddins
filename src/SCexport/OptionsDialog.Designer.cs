// (C) Copyright 2013 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCexport.
//
// SCexport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCexport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCexport.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCexport
{
    public partial class OptionsDialog
    {           
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBoxA3Printer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnDefaultExportDir;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.RadioButton radioGSPDF;
        private System.Windows.Forms.ComboBox comboBoxScheme;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxDWG;
        private System.Windows.Forms.CheckBox checkBoxDGN;
        private System.Windows.Forms.CheckBox checkBoxDWF;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox comboBoxAutocadVersion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonCreateConfig;
        private System.Windows.Forms.Button buttonEditConfig;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxGSBin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxGSLib;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxAdobeDriver;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPSPrinter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox checkBoxHideTitleblock;
        private System.Windows.Forms.CheckBox checkBoxForceDate;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button button5;
        
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (this.components != null) {
                    this.components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnDefaultExportDir = new System.Windows.Forms.Button();
            this.textBoxExportDir = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxScheme = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxAutocadVersion = new System.Windows.Forms.ComboBox();
            this.checkBoxForceDate = new System.Windows.Forms.CheckBox();
            this.checkBoxHideTitleblock = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.radioGSPDF = new System.Windows.Forms.RadioButton();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.checkBoxDWF = new System.Windows.Forms.CheckBox();
            this.checkBoxDGN = new System.Windows.Forms.CheckBox();
            this.checkBoxDWG = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBoxShowExportLog = new System.Windows.Forms.CheckBox();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSelectTextEditor = new System.Windows.Forms.Button();
            this.textBoxTextEditor = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonEditConfig = new System.Windows.Forms.Button();
            this.buttonCreateConfig = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.GSLibDir = new System.Windows.Forms.Button();
            this.btnGSBinDir = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxGSLib = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxGSBin = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSelectLargeFormatPrinter = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxLargeFormatPrinter = new System.Windows.Forms.TextBox();
            this.btnSelectAdobePrinter = new System.Windows.Forms.Button();
            this.btnSelectA3Printer = new System.Windows.Forms.Button();
            this.btnSelectPSPrinter = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxA3Printer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPSPrinter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxAdobeDriver = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button3 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 462);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Reset to Default";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(343, 462);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(406, 444);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(398, 418);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "General";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnDefaultExportDir);
            this.groupBox6.Controls.Add(this.textBoxExportDir);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Location = new System.Drawing.Point(3, 134);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(389, 98);
            this.groupBox6.TabIndex = 20;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Export Destination";
            // 
            // btnDefaultExportDir
            // 
            this.btnDefaultExportDir.Location = new System.Drawing.Point(355, 26);
            this.btnDefaultExportDir.Name = "btnDefaultExportDir";
            this.btnDefaultExportDir.Size = new System.Drawing.Size(23, 23);
            this.btnDefaultExportDir.TabIndex = 25;
            this.btnDefaultExportDir.Text = "...";
            this.btnDefaultExportDir.UseVisualStyleBackColor = true;
            this.btnDefaultExportDir.Click += new System.EventHandler(this.BtnDefaultExportDirClick);
            // 
            // textBoxExportDir
            // 
            this.textBoxExportDir.Location = new System.Drawing.Point(137, 28);
            this.textBoxExportDir.Name = "textBoxExportDir";
            this.textBoxExportDir.Size = new System.Drawing.Size(212, 20);
            this.textBoxExportDir.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(7, 31);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(150, 23);
            this.label9.TabIndex = 23;
            this.label9.Text = "Default export directory";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.comboBoxScheme);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.comboBoxAutocadVersion);
            this.groupBox4.Controls.Add(this.checkBoxForceDate);
            this.groupBox4.Controls.Add(this.checkBoxHideTitleblock);
            this.groupBox4.Location = new System.Drawing.Point(3, 238);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(389, 174);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Export Options";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 23);
            this.label5.TabIndex = 14;
            this.label5.Text = "File naming scheme";
            // 
            // comboBoxScheme
            // 
            this.comboBoxScheme.FormattingEnabled = true;
            this.comboBoxScheme.Location = new System.Drawing.Point(192, 57);
            this.comboBoxScheme.Name = "comboBoxScheme";
            this.comboBoxScheme.Size = new System.Drawing.Size(180, 21);
            this.comboBoxScheme.TabIndex = 13;
            this.comboBoxScheme.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 12;
            this.label6.Text = "AutoCAD Version";
            // 
            // comboBoxAutocadVersion
            // 
            this.comboBoxAutocadVersion.FormattingEnabled = true;
            this.comboBoxAutocadVersion.Location = new System.Drawing.Point(192, 23);
            this.comboBoxAutocadVersion.Name = "comboBoxAutocadVersion";
            this.comboBoxAutocadVersion.Size = new System.Drawing.Size(180, 21);
            this.comboBoxAutocadVersion.TabIndex = 11;
            this.comboBoxAutocadVersion.SelectedIndexChanged += new System.EventHandler(this.ComboBoxAutocadVersionSelectedIndexChanged);
            // 
            // checkBoxForceDate
            // 
            this.checkBoxForceDate.Location = new System.Drawing.Point(6, 86);
            this.checkBoxForceDate.Name = "checkBoxForceDate";
            this.checkBoxForceDate.Size = new System.Drawing.Size(311, 24);
            this.checkBoxForceDate.TabIndex = 10;
            this.checkBoxForceDate.Text = "Force the revision to be today\'s date [YYYYMMDD]";
            this.checkBoxForceDate.UseVisualStyleBackColor = true;
            this.checkBoxForceDate.CheckedChanged += new System.EventHandler(this.ForceDateCheckedChanged);
            // 
            // checkBoxHideTitleblock
            // 
            this.checkBoxHideTitleblock.Enabled = false;
            this.checkBoxHideTitleblock.Location = new System.Drawing.Point(6, 116);
            this.checkBoxHideTitleblock.Name = "checkBoxHideTitleblock";
            this.checkBoxHideTitleblock.Size = new System.Drawing.Size(231, 24);
            this.checkBoxHideTitleblock.TabIndex = 8;
            this.checkBoxHideTitleblock.Text = "Hide titleblocks (DWG exports only)";
            this.checkBoxHideTitleblock.UseVisualStyleBackColor = true;
            this.checkBoxHideTitleblock.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBox1);
            this.groupBox5.Controls.Add(this.radioGSPDF);
            this.groupBox5.Controls.Add(this.radioPDF);
            this.groupBox5.Controls.Add(this.checkBoxDWF);
            this.groupBox5.Controls.Add(this.checkBoxDGN);
            this.groupBox5.Controls.Add(this.checkBoxDWG);
            this.groupBox5.Location = new System.Drawing.Point(6, 15);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(386, 113);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Export Types";
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(6, 19);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(220, 24);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "PDF";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
            // 
            // radioGSPDF
            // 
            this.radioGSPDF.Enabled = false;
            this.radioGSPDF.Location = new System.Drawing.Point(6, 78);
            this.radioGSPDF.Name = "radioGSPDF";
            this.radioGSPDF.Size = new System.Drawing.Size(231, 24);
            this.radioGSPDF.TabIndex = 5;
            this.radioGSPDF.Text = "PDF (Ghostscipt Export)";
            this.radioGSPDF.UseVisualStyleBackColor = true;
            this.radioGSPDF.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
            // 
            // radioPDF
            // 
            this.radioPDF.Enabled = false;
            this.radioPDF.Location = new System.Drawing.Point(6, 48);
            this.radioPDF.Name = "radioPDF";
            this.radioPDF.Size = new System.Drawing.Size(256, 24);
            this.radioPDF.TabIndex = 4;
            this.radioPDF.TabStop = true;
            this.radioPDF.Text = "PDF (Adobe Acrobat Export)";
            this.radioPDF.UseVisualStyleBackColor = true;
            this.radioPDF.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
            // 
            // checkBoxDWF
            // 
            this.checkBoxDWF.Location = new System.Drawing.Point(268, 79);
            this.checkBoxDWF.Name = "checkBoxDWF";
            this.checkBoxDWF.Size = new System.Drawing.Size(104, 24);
            this.checkBoxDWF.TabIndex = 3;
            this.checkBoxDWF.Text = "DWF";
            this.checkBoxDWF.UseVisualStyleBackColor = true;
            this.checkBoxDWF.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // checkBoxDGN
            // 
            this.checkBoxDGN.Enabled = false;
            this.checkBoxDGN.Location = new System.Drawing.Point(268, 49);
            this.checkBoxDGN.Name = "checkBoxDGN";
            this.checkBoxDGN.Size = new System.Drawing.Size(104, 24);
            this.checkBoxDGN.TabIndex = 2;
            this.checkBoxDGN.Text = "DGN";
            this.checkBoxDGN.UseVisualStyleBackColor = true;
            this.checkBoxDGN.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // checkBoxDWG
            // 
            this.checkBoxDWG.Location = new System.Drawing.Point(268, 19);
            this.checkBoxDWG.Name = "checkBoxDWG";
            this.checkBoxDWG.Size = new System.Drawing.Size(104, 24);
            this.checkBoxDWG.TabIndex = 1;
            this.checkBoxDWG.Text = "DWG";
            this.checkBoxDWG.UseVisualStyleBackColor = true;
            this.checkBoxDWG.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.checkBoxShowExportLog);
            this.tabPage3.Controls.Add(this.button5);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(398, 418);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Advanced";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowExportLog
            // 
            this.checkBoxShowExportLog.Location = new System.Drawing.Point(12, 359);
            this.checkBoxShowExportLog.Name = "checkBoxShowExportLog";
            this.checkBoxShowExportLog.Size = new System.Drawing.Size(380, 24);
            this.checkBoxShowExportLog.TabIndex = 10;
            this.checkBoxShowExportLog.Text = "Show export log after export completion (will always show on an error)";
            this.checkBoxShowExportLog.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(12, 389);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(370, 23);
            this.button5.TabIndex = 9;
            this.button5.Text = "Check for Updates";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.Button5Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSelectTextEditor);
            this.groupBox3.Controls.Add(this.textBoxTextEditor);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.buttonEditConfig);
            this.groupBox3.Controls.Add(this.buttonCreateConfig);
            this.groupBox3.Location = new System.Drawing.Point(6, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(386, 82);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Project Configuratation";
            // 
            // btnSelectTextEditor
            // 
            this.btnSelectTextEditor.Location = new System.Drawing.Point(352, 48);
            this.btnSelectTextEditor.Name = "btnSelectTextEditor";
            this.btnSelectTextEditor.Size = new System.Drawing.Size(24, 20);
            this.btnSelectTextEditor.TabIndex = 15;
            this.btnSelectTextEditor.Text = "...";
            this.btnSelectTextEditor.UseVisualStyleBackColor = true;
            this.btnSelectTextEditor.Click += new System.EventHandler(this.BtnSelectTextEditorClick);
            // 
            // textBoxTextEditor
            // 
            this.textBoxTextEditor.Location = new System.Drawing.Point(157, 49);
            this.textBoxTextEditor.Name = "textBoxTextEditor";
            this.textBoxTextEditor.Size = new System.Drawing.Size(189, 20);
            this.textBoxTextEditor.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(15, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(121, 17);
            this.label8.TabIndex = 6;
            this.label8.Text = "Text editor (for config)";
            // 
            // buttonEditConfig
            // 
            this.buttonEditConfig.Image = ((System.Drawing.Image)(resources.GetObject("buttonEditConfig.Image")));
            this.buttonEditConfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonEditConfig.Location = new System.Drawing.Point(208, 19);
            this.buttonEditConfig.Name = "buttonEditConfig";
            this.buttonEditConfig.Size = new System.Drawing.Size(168, 23);
            this.buttonEditConfig.TabIndex = 1;
            this.buttonEditConfig.Text = "Edit Config File";
            this.buttonEditConfig.UseVisualStyleBackColor = true;
            this.buttonEditConfig.Click += new System.EventHandler(this.ButtonEditConfigClick);
            // 
            // buttonCreateConfig
            // 
            this.buttonCreateConfig.Image = ((System.Drawing.Image)(resources.GetObject("buttonCreateConfig.Image")));
            this.buttonCreateConfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCreateConfig.Location = new System.Drawing.Point(17, 19);
            this.buttonCreateConfig.Name = "buttonCreateConfig";
            this.buttonCreateConfig.Size = new System.Drawing.Size(185, 23);
            this.buttonCreateConfig.TabIndex = 0;
            this.buttonCreateConfig.Text = "Create Project Config File";
            this.buttonCreateConfig.UseVisualStyleBackColor = true;
            this.buttonCreateConfig.Click += new System.EventHandler(this.ButtonCreateConfigClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.GSLibDir);
            this.groupBox2.Controls.Add(this.btnGSBinDir);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxGSLib);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxGSBin);
            this.groupBox2.Location = new System.Drawing.Point(6, 257);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(386, 93);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ghostscript Options";
            // 
            // GSLibDir
            // 
            this.GSLibDir.Location = new System.Drawing.Point(347, 55);
            this.GSLibDir.Name = "GSLibDir";
            this.GSLibDir.Size = new System.Drawing.Size(24, 20);
            this.GSLibDir.TabIndex = 13;
            this.GSLibDir.Text = "...";
            this.GSLibDir.UseVisualStyleBackColor = true;
            this.GSLibDir.Click += new System.EventHandler(this.GSLibDirClick);
            // 
            // btnGSBinDir
            // 
            this.btnGSBinDir.Location = new System.Drawing.Point(347, 29);
            this.btnGSBinDir.Name = "btnGSBinDir";
            this.btnGSBinDir.Size = new System.Drawing.Size(24, 20);
            this.btnGSBinDir.TabIndex = 12;
            this.btnGSBinDir.Text = "...";
            this.btnGSBinDir.UseVisualStyleBackColor = true;
            this.btnGSBinDir.Click += new System.EventHandler(this.GSBinDirClick);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(7, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Ghostscript lib location";
            // 
            // textBoxGSLib
            // 
            this.textBoxGSLib.Location = new System.Drawing.Point(149, 55);
            this.textBoxGSLib.Name = "textBoxGSLib";
            this.textBoxGSLib.Size = new System.Drawing.Size(192, 20);
            this.textBoxGSLib.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(7, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Ghostscript bin location";
            // 
            // textBoxGSBin
            // 
            this.textBoxGSBin.Location = new System.Drawing.Point(149, 29);
            this.textBoxGSBin.Name = "textBoxGSBin";
            this.textBoxGSBin.Size = new System.Drawing.Size(192, 20);
            this.textBoxGSBin.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSelectLargeFormatPrinter);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.textBoxLargeFormatPrinter);
            this.groupBox1.Controls.Add(this.btnSelectAdobePrinter);
            this.groupBox1.Controls.Add(this.btnSelectA3Printer);
            this.groupBox1.Controls.Add(this.btnSelectPSPrinter);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxA3Printer);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxPSPrinter);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxAdobeDriver);
            this.groupBox1.Location = new System.Drawing.Point(6, 108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 143);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Printer Options";
            // 
            // btnSelectLargeFormatPrinter
            // 
            this.btnSelectLargeFormatPrinter.Location = new System.Drawing.Point(347, 102);
            this.btnSelectLargeFormatPrinter.Name = "btnSelectLargeFormatPrinter";
            this.btnSelectLargeFormatPrinter.Size = new System.Drawing.Size(24, 20);
            this.btnSelectLargeFormatPrinter.TabIndex = 16;
            this.btnSelectLargeFormatPrinter.Text = "...";
            this.btnSelectLargeFormatPrinter.UseVisualStyleBackColor = true;
            this.btnSelectLargeFormatPrinter.Click += new System.EventHandler(this.BtnSelectLargeFormatPrinterClick);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(6, 106);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(140, 20);
            this.label10.TabIndex = 15;
            this.label10.Text = "Large format printer name";
            // 
            // textBoxLargeFormatPrinter
            // 
            this.textBoxLargeFormatPrinter.Location = new System.Drawing.Point(152, 103);
            this.textBoxLargeFormatPrinter.Name = "textBoxLargeFormatPrinter";
            this.textBoxLargeFormatPrinter.Size = new System.Drawing.Size(189, 20);
            this.textBoxLargeFormatPrinter.TabIndex = 14;
            // 
            // btnSelectAdobePrinter
            // 
            this.btnSelectAdobePrinter.Location = new System.Drawing.Point(347, 23);
            this.btnSelectAdobePrinter.Name = "btnSelectAdobePrinter";
            this.btnSelectAdobePrinter.Size = new System.Drawing.Size(24, 20);
            this.btnSelectAdobePrinter.TabIndex = 13;
            this.btnSelectAdobePrinter.Text = "...";
            this.btnSelectAdobePrinter.UseVisualStyleBackColor = true;
            this.btnSelectAdobePrinter.Click += new System.EventHandler(this.ButtonAdobePrinterClick);
            // 
            // btnSelectA3Printer
            // 
            this.btnSelectA3Printer.Location = new System.Drawing.Point(347, 75);
            this.btnSelectA3Printer.Name = "btnSelectA3Printer";
            this.btnSelectA3Printer.Size = new System.Drawing.Size(24, 20);
            this.btnSelectA3Printer.TabIndex = 12;
            this.btnSelectA3Printer.Text = "...";
            this.btnSelectA3Printer.UseVisualStyleBackColor = true;
            this.btnSelectA3Printer.Click += new System.EventHandler(this.ButtonA3PrinterClick);
            // 
            // btnSelectPSPrinter
            // 
            this.btnSelectPSPrinter.Location = new System.Drawing.Point(347, 49);
            this.btnSelectPSPrinter.Name = "btnSelectPSPrinter";
            this.btnSelectPSPrinter.Size = new System.Drawing.Size(24, 20);
            this.btnSelectPSPrinter.TabIndex = 11;
            this.btnSelectPSPrinter.Text = "...";
            this.btnSelectPSPrinter.UseVisualStyleBackColor = true;
            this.btnSelectPSPrinter.Click += new System.EventHandler(this.ButtonPSPrinterClick);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 79);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 20);
            this.label7.TabIndex = 9;
            this.label7.Text = "A3 printer name";
            // 
            // textBoxA3Printer
            // 
            this.textBoxA3Printer.Location = new System.Drawing.Point(152, 76);
            this.textBoxA3Printer.Name = "textBoxA3Printer";
            this.textBoxA3Printer.Size = new System.Drawing.Size(189, 20);
            this.textBoxA3Printer.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Postscript printer name";
            // 
            // textBoxPSPrinter
            // 
            this.textBoxPSPrinter.Location = new System.Drawing.Point(152, 50);
            this.textBoxPSPrinter.Name = "textBoxPSPrinter";
            this.textBoxPSPrinter.Size = new System.Drawing.Size(189, 20);
            this.textBoxPSPrinter.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Adobe pdf driver";
            // 
            // textBoxAdobeDriver
            // 
            this.textBoxAdobeDriver.Location = new System.Drawing.Point(152, 24);
            this.textBoxAdobeDriver.Name = "textBoxAdobeDriver";
            this.textBoxAdobeDriver.Size = new System.Drawing.Size(189, 20);
            this.textBoxAdobeDriver.TabIndex = 4;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(261, 462);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // OptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 497);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OptionsDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SCexport Options";
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        } 
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBoxExportDir;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSelectTextEditor;
        private System.Windows.Forms.TextBox textBoxTextEditor;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnSelectAdobePrinter;
        private System.Windows.Forms.Button GSLibDir;
        private System.Windows.Forms.Button btnGSBinDir;
        private System.Windows.Forms.Button btnSelectA3Printer;
        private System.Windows.Forms.Button btnSelectPSPrinter;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btnSelectLargeFormatPrinter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxLargeFormatPrinter;
        private System.Windows.Forms.CheckBox checkBoxShowExportLog;
    }
}
