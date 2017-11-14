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

namespace SCaddins.ExportManager
{
    using SCaddins.Properties;

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
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxNorthPointVisibilty = new System.Windows.Forms.TextBox();
            this.textBoxScalebarScale = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnDefaultExportDir = new System.Windows.Forms.Button();
            this.textBoxExportDir = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseDateForEmptyRevisions = new System.Windows.Forms.CheckBox();
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
            this.groupBox7.SuspendLayout();
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
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.Text = global::SCaddins.Properties.Resources.ResetToDefault;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Name = "button1";
            this.button1.Text = global::SCaddins.Properties.Resources.Apply;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox5);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Text = global::SCaddins.Properties.Resources.General;
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.textBoxNorthPointVisibilty);
            this.groupBox7.Controls.Add(this.textBoxScalebarScale);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // textBoxNorthPointVisibilty
            // 
            resources.ApplyResources(this.textBoxNorthPointVisibilty, "textBoxNorthPointVisibilty");
            this.textBoxNorthPointVisibilty.Name = "textBoxNorthPointVisibilty";
            // 
            // textBoxScalebarScale
            // 
            resources.ApplyResources(this.textBoxScalebarScale, "textBoxScalebarScale");
            this.textBoxScalebarScale.Name = "textBoxScalebarScale";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnDefaultExportDir);
            this.groupBox6.Controls.Add(this.textBoxExportDir);
            this.groupBox6.Controls.Add(this.label9);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // btnDefaultExportDir
            // 
            resources.ApplyResources(this.btnDefaultExportDir, "btnDefaultExportDir");
            this.btnDefaultExportDir.Name = "btnDefaultExportDir";
            this.btnDefaultExportDir.UseVisualStyleBackColor = true;
            this.btnDefaultExportDir.Click += new System.EventHandler(this.BtnDefaultExportDirClick);
            // 
            // textBoxExportDir
            // 
            resources.ApplyResources(this.textBoxExportDir, "textBoxExportDir");
            this.textBoxExportDir.Name = "textBoxExportDir";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxUseDateForEmptyRevisions);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.comboBoxScheme);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.comboBoxAutocadVersion);
            this.groupBox4.Controls.Add(this.checkBoxForceDate);
            this.groupBox4.Controls.Add(this.checkBoxHideTitleblock);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // checkBoxUseDateForEmptyRevisions
            // 
            resources.ApplyResources(this.checkBoxUseDateForEmptyRevisions, "checkBoxUseDateForEmptyRevisions");
            this.checkBoxUseDateForEmptyRevisions.Name = "checkBoxUseDateForEmptyRevisions";
            this.checkBoxUseDateForEmptyRevisions.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // comboBoxScheme
            // 
            this.comboBoxScheme.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxScheme, "comboBoxScheme");
            this.comboBoxScheme.Name = "comboBoxScheme";
            this.comboBoxScheme.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // comboBoxAutocadVersion
            // 
            this.comboBoxAutocadVersion.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxAutocadVersion, "comboBoxAutocadVersion");
            this.comboBoxAutocadVersion.Name = "comboBoxAutocadVersion";
            this.comboBoxAutocadVersion.SelectedIndexChanged += new System.EventHandler(this.ComboBoxAutocadVersionSelectedIndexChanged);
            // 
            // checkBoxForceDate
            // 
            resources.ApplyResources(this.checkBoxForceDate, "checkBoxForceDate");
            this.checkBoxForceDate.Name = "checkBoxForceDate";
            this.checkBoxForceDate.UseVisualStyleBackColor = true;
            this.checkBoxForceDate.CheckedChanged += new System.EventHandler(this.ForceDateCheckedChanged);
            // 
            // checkBoxHideTitleblock
            // 
            resources.ApplyResources(this.checkBoxHideTitleblock, "checkBoxHideTitleblock");
            this.checkBoxHideTitleblock.Name = "checkBoxHideTitleblock";
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
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
            // 
            // radioGSPDF
            // 
            resources.ApplyResources(this.radioGSPDF, "radioGSPDF");
            this.radioGSPDF.Name = "radioGSPDF";
            this.radioGSPDF.UseVisualStyleBackColor = true;
            this.radioGSPDF.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
            // 
            // radioPDF
            // 
            resources.ApplyResources(this.radioPDF, "radioPDF");
            this.radioPDF.Name = "radioPDF";
            this.radioPDF.TabStop = true;
            this.radioPDF.UseVisualStyleBackColor = true;
            this.radioPDF.CheckedChanged += new System.EventHandler(this.RadioCheckedChanged);
            // 
            // checkBoxDWF
            // 
            resources.ApplyResources(this.checkBoxDWF, "checkBoxDWF");
            this.checkBoxDWF.Name = "checkBoxDWF";
            this.checkBoxDWF.UseVisualStyleBackColor = true;
            this.checkBoxDWF.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // checkBoxDGN
            // 
            resources.ApplyResources(this.checkBoxDGN, "checkBoxDGN");
            this.checkBoxDGN.Name = "checkBoxDGN";
            this.checkBoxDGN.UseVisualStyleBackColor = true;
            this.checkBoxDGN.CheckedChanged += new System.EventHandler(this.ToggleCheckBoxValue);
            // 
            // checkBoxDWG
            // 
            resources.ApplyResources(this.checkBoxDWG, "checkBoxDWG");
            this.checkBoxDWG.Name = "checkBoxDWG";
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
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowExportLog
            // 
            resources.ApplyResources(this.checkBoxShowExportLog, "checkBoxShowExportLog");
            this.checkBoxShowExportLog.Name = "checkBoxShowExportLog";
            this.checkBoxShowExportLog.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            resources.ApplyResources(this.button5, "button5");
            this.button5.Name = "button5";
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
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // btnSelectTextEditor
            // 
            resources.ApplyResources(this.btnSelectTextEditor, "btnSelectTextEditor");
            this.btnSelectTextEditor.Name = "btnSelectTextEditor";
            this.btnSelectTextEditor.UseVisualStyleBackColor = true;
            this.btnSelectTextEditor.Click += new System.EventHandler(this.BtnSelectTextEditorClick);
            // 
            // textBoxTextEditor
            // 
            resources.ApplyResources(this.textBoxTextEditor, "textBoxTextEditor");
            this.textBoxTextEditor.Name = "textBoxTextEditor";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // buttonEditConfig
            // 
            resources.ApplyResources(this.buttonEditConfig, "buttonEditConfig");
            this.buttonEditConfig.Name = "buttonEditConfig";
            this.buttonEditConfig.UseVisualStyleBackColor = true;
            this.buttonEditConfig.Click += new System.EventHandler(this.ButtonEditConfigClick);
            // 
            // buttonCreateConfig
            // 
            resources.ApplyResources(this.buttonCreateConfig, "buttonCreateConfig");
            this.buttonCreateConfig.Name = "buttonCreateConfig";
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
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // GSLibDir
            // 
            resources.ApplyResources(this.GSLibDir, "GSLibDir");
            this.GSLibDir.Name = "GSLibDir";
            this.GSLibDir.UseVisualStyleBackColor = true;
            this.GSLibDir.Click += new System.EventHandler(this.GSLibDirClick);
            // 
            // btnGSBinDir
            // 
            resources.ApplyResources(this.btnGSBinDir, "btnGSBinDir");
            this.btnGSBinDir.Name = "btnGSBinDir";
            this.btnGSBinDir.UseVisualStyleBackColor = true;
            this.btnGSBinDir.Click += new System.EventHandler(this.GSBinDirClick);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBoxGSLib
            // 
            resources.ApplyResources(this.textBoxGSLib, "textBoxGSLib");
            this.textBoxGSLib.Name = "textBoxGSLib";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // textBoxGSBin
            // 
            resources.ApplyResources(this.textBoxGSBin, "textBoxGSBin");
            this.textBoxGSBin.Name = "textBoxGSBin";
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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnSelectLargeFormatPrinter
            // 
            resources.ApplyResources(this.btnSelectLargeFormatPrinter, "btnSelectLargeFormatPrinter");
            this.btnSelectLargeFormatPrinter.Name = "btnSelectLargeFormatPrinter";
            this.btnSelectLargeFormatPrinter.UseVisualStyleBackColor = true;
            this.btnSelectLargeFormatPrinter.Click += new System.EventHandler(this.BtnSelectLargeFormatPrinterClick);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // textBoxLargeFormatPrinter
            // 
            resources.ApplyResources(this.textBoxLargeFormatPrinter, "textBoxLargeFormatPrinter");
            this.textBoxLargeFormatPrinter.Name = "textBoxLargeFormatPrinter";
            // 
            // btnSelectAdobePrinter
            // 
            resources.ApplyResources(this.btnSelectAdobePrinter, "btnSelectAdobePrinter");
            this.btnSelectAdobePrinter.Name = "btnSelectAdobePrinter";
            this.btnSelectAdobePrinter.UseVisualStyleBackColor = true;
            this.btnSelectAdobePrinter.Click += new System.EventHandler(this.ButtonAdobePrinterClick);
            // 
            // btnSelectA3Printer
            // 
            resources.ApplyResources(this.btnSelectA3Printer, "btnSelectA3Printer");
            this.btnSelectA3Printer.Name = "btnSelectA3Printer";
            this.btnSelectA3Printer.UseVisualStyleBackColor = true;
            this.btnSelectA3Printer.Click += new System.EventHandler(this.ButtonA3PrinterClick);
            // 
            // btnSelectPSPrinter
            // 
            resources.ApplyResources(this.btnSelectPSPrinter, "btnSelectPSPrinter");
            this.btnSelectPSPrinter.Name = "btnSelectPSPrinter";
            this.btnSelectPSPrinter.Text = global::SCaddins.Properties.Resources.DotDotDot;
            this.btnSelectPSPrinter.UseVisualStyleBackColor = true;
            this.btnSelectPSPrinter.Click += new System.EventHandler(this.ButtonPSPrinterClick);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // textBoxA3Printer
            // 
            resources.ApplyResources(this.textBoxA3Printer, "textBoxA3Printer");
            this.textBoxA3Printer.Name = "textBoxA3Printer";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxPSPrinter
            // 
            resources.ApplyResources(this.textBoxPSPrinter, "textBoxPSPrinter");
            this.textBoxPSPrinter.Name = "textBoxPSPrinter";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxAdobeDriver
            // 
            resources.ApplyResources(this.textBoxAdobeDriver, "textBoxAdobeDriver");
            this.textBoxAdobeDriver.Name = "textBoxAdobeDriver";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Name = "button3";
            this.button3.Text = global::SCaddins.Properties.Resources.Cancel;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // OptionsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "OptionsDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxNorthPointVisibilty;
        private System.Windows.Forms.TextBox textBoxScalebarScale;
        private System.Windows.Forms.CheckBox checkBoxUseDateForEmptyRevisions;
    }
}
