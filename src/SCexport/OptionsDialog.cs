// (C) Copyright 2013-2014 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCexport
{
    using System;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    
    /// <summary>
    /// Description of SCexportOptionsDialog.
    /// </summary>
    public partial class OptionsDialog : System.Windows.Forms.Form
    {
        private Autodesk.Revit.DB.Document doc;
        private SCexport scx;
        private System.Windows.Forms.DataGridView parent;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsDialog"/> class.
        /// </summary>
        public OptionsDialog(Autodesk.Revit.DB.Document doc, SCexport scx, System.Windows.Forms.DataGridView parent)
        {
            this.doc = doc;
            this.scx = scx;
            this.parent = parent;
            this.InitializeComponent();
            this.InitializeComponentsMore();
            this.AssignDWGReleaseMenuTags();
            this.PopulateSchemeMenu();
            this.LoadValues();
        }
        
        private void LoadValues()
        {
            this.radioPDF.Checked = this.scx.HasFlag(SCexport.ExportFlags.PDF);
            this.radioGSPDF.Checked = this.scx.HasFlag(SCexport.ExportFlags.GS_PDF);
            this.checkBoxDGN.Checked = this.scx.HasFlag(SCexport.ExportFlags.DGN);
            this.checkBoxDWG.Checked = this.scx.HasFlag(SCexport.ExportFlags.DWG);
            this.checkBoxDWF.Checked = this.scx.HasFlag(SCexport.ExportFlags.DWF);
            this.checkBoxTagPDF.Checked =
                this.scx.HasFlag(SCexport.ExportFlags.TAG_PDF);
            this.checkBoxHideTitleblock.Checked =
                this.scx.HasFlag(SCexport.ExportFlags.NO_TITLE);
            this.checkBoxForceDate.Checked = this.scx.ForceDate;
            if (this.scx.FilenameScheme != null) {
                this.comboBoxScheme.Text = this.scx.FilenameScheme.Name;
            }
            this.checkBox1.Checked = true;
            #if REVIT2012
            this.checkBoxHideTitleblock.Enabled = false;
            #endif
            this.textBoxAdobeDriver.Text = this.scx.PdfPrinterName;
            this.textBoxPSPrinter.Text = this.scx.PostscriptPrinterName;
            this.textBoxGSBin.Text = this.scx.GhostsciptBinDir;
            this.textBoxGSLib.Text = this.scx.GhostsciptLibDir;
            this.textBoxA3Printer.Text = this.scx.PrinterNameA3;
            textBoxTextEditor.Text = SCaddins.SCexport.Settings1.Default.TextEditor;
            checkBoxTagPDF.Checked = SCaddins.SCexport.Settings1.Default.TagPDFExports;
            textBoxExportDir.Text = SCaddins.SCexport.Settings1.Default.ExportDir;
        }
        
        private void SaveValues()
        {
            this.scx.GhostsciptBinDir = textBoxGSBin.Text;
            this.scx.GhostsciptLibDir = textBoxGSLib.Text;
            this.scx.PrinterNameA3 = textBoxA3Printer.Text;
            this.scx.PostscriptPrinterName = textBoxPSPrinter.Text;
            this.scx.PdfPrinterName = textBoxAdobeDriver.Text;
            SCaddins.SCexport.Settings1.Default.GSBinDirectory = this.scx.GhostsciptBinDir;         
            SCaddins.SCexport.Settings1.Default.AdobePrinterDriver = this.scx.PdfPrinterName;  
            SCaddins.SCexport.Settings1.Default.A3PrinterDriver = this.scx.PrinterNameA3;   
            SCaddins.SCexport.Settings1.Default.PSPrinterDriver = this.scx.PostscriptPrinterName;  
            SCaddins.SCexport.Settings1.Default.GSLibDirectory = this.scx.GhostsciptLibDir;  
            SCaddins.SCexport.Settings1.Default.TextEditor = textBoxTextEditor.Text;
            SCaddins.SCexport.Settings1.Default.TagPDFExports = checkBoxTagPDF.Checked;
            SCaddins.SCexport.Settings1.Default.ExportDir = this.textBoxExportDir.Text;
            SCaddins.SCexport.Settings1.Default.AdobePDFMode = radioPDF.Checked;
            SCaddins.SCexport.Settings1.Default.ForceDateRevision = checkBoxForceDate.Checked;
            SCaddins.SCexport.Settings1.Default.TagPDFExports = checkBoxTagPDF.Checked;
            SCaddins.SCexport.Settings1.Default.HideTitleBlocks = checkBoxHideTitleblock.Checked;    
            SCaddins.SCexport.Settings1.Default.Save();
        }
        
        private void AssignDWGReleaseMenuTags()
        {
            foreach (var item in Enum.GetValues(typeof(ACADVersion))) {
                this.comboBoxAutocadVersion.Items.Add(item);
            }         
            this.comboBoxAutocadVersion.SelectedIndex = 1; 
        }
                
        private void PopulateSchemeMenu()
        {
            foreach (SheetName scxn in this.scx.FilenameTypes) {
                if (scxn.Name != null) {
                    this.comboBoxScheme.Items.Add(scxn.Name);
                }
            }
        }
        
        private void InitializeComponentsMore()
        {
            if (this.scx.GSSanityCheck()) {
                this.radioGSPDF.Enabled = true;
            }
            if (this.scx.PDFSanityCheck()) {
                this.radioPDF.Enabled = true;
            }
            if (!this.radioPDF.Enabled && !this.radioGSPDF.Enabled) {
                this.checkBox1.Enabled = false;
                this.checkBox1.Text = "PDF disabled, check settings!!!";
            }
            if (!FileUtils.ConfigFileExists(this.doc)) {
                this.buttonEditConfig.Enabled = false;
            }
            this.radioPDF.Tag = SCexport.ExportFlags.PDF;
            this.checkBoxDGN.Tag = SCexport.ExportFlags.DGN;
            this.checkBoxDWF.Tag = SCexport.ExportFlags.DWF;
            this.checkBoxDWG.Tag = SCexport.ExportFlags.DWG;
            this.radioGSPDF.Tag = SCexport.ExportFlags.GS_PDF;
            this.checkBoxTagPDF.Tag = SCexport.ExportFlags.TAG_PDF;
            this.checkBoxHideTitleblock.Tag = SCexport.ExportFlags.NO_TITLE;
        }
        
        private void ToggleCheckBoxValue(object sender, EventArgs e)
        {
            var c = (CheckBox)sender;
            var t = (SCexport.ExportFlags)c.Tag;
            this.ToggleConversionFlag(c.Checked, t);
            if (this.checkBoxDWG.Checked) {
                this.checkBoxHideTitleblock.Enabled = true;
            } else {
                this.checkBoxHideTitleblock.Enabled = false;
            }
        }
        
        private void ToggleConversionFlag(
            bool flagged, SCexport.ExportFlags val)
        {
            if (flagged == true) {
                this.scx.AddExportFlag(val);
            } else {
                this.scx.RemoveExportFlag(val);
            }
        }
        
        private void RadioCheckedChanged(object sender, EventArgs e)
        {
            var r = (RadioButton)sender;
            var t = (SCexport.ExportFlags)r.Tag;
            this.ToggleConversionFlag(r.Checked, t);
        }
        
        private void ForceDateCheckedChanged(object sender, EventArgs e)
        {
            this.scx.ForceDate = ((CheckBox)sender).Checked;
        }
        
        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = this.comboBoxScheme.Text;
            this.scx.SetFilenameScheme(s);
        }
        
        private void ComboBoxAutocadVersionSelectedIndexChanged(object sender, EventArgs e)
        {
            var version = (ACADVersion)this.comboBoxAutocadVersion.SelectedItem;
            this.scx.AcadVersion = version;
        }
        
        private void ButtonCreateConfigClick(object sender, EventArgs e)
        {
            FileUtils.CreateConfigFile(ref this.doc);
            this.buttonEditConfig.Enabled = true;
        }
        
        private void ButtonEditConfigClick(object sender, EventArgs e)
        {  
            FileUtils.EditConfigFile(ref this.doc);
        }
           
        private void Button3Click(object sender, EventArgs e)
        {
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                this.scx.ExportDir = this.folderBrowserDialog1.SelectedPath;
                this.parent.Update();
                this.parent.Refresh();
            }
        }
        
        private void ButtonWorkingFilesClick(object sender, EventArgs e)
        {
            this.scx.ExportDir = Constants.UnionSquareWorkingFiles;
            this.parent.Update();
            this.parent.Refresh();
        }
        
        private void Button5Click(object sender, EventArgs e)
        {
            SCaddins.SCaddinsApp.CheckForUpdates();
        }
        
        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            if (!this.checkBox1.Checked) {
                this.radioGSPDF.Checked = false;
                this.radioPDF.Checked = false;
                this.radioGSPDF.Enabled = false;
                this.radioPDF.Enabled = false;
            } else {
                this.radioGSPDF.Enabled |= this.scx.GSSanityCheck();
                this.radioPDF.Enabled |= this.scx.PDFSanityCheck();
            }
        }
        
        private void GSBinDirClick(object sender, System.EventArgs e)
        {
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                textBoxGSBin.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }
        
        private void GSLibDirClick(object sender, System.EventArgs e)
        {
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                textBoxGSLib.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }
        
        private void Button1Click(object sender, System.EventArgs e)
        {
            this.SaveValues();
        }
        
        private void SetPrinter(TextBox textBox)
        {
            var dialog = new SelectPrinterDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                textBox.Text = dialog.cbxPrinter.SelectedItem.ToString();
            }  
        }
        
        private void ButtonPSPrinterClick(object sender, System.EventArgs e)
        {
            this.SetPrinter(this.textBoxPSPrinter);
        }
        
        private void ButtonA3PrinterClick(object sender, System.EventArgs e)
        {
            this.SetPrinter(this.textBoxA3Printer);
        }
        
        private void ButtonAdobePrinterClick(object sender, System.EventArgs e)
        {
            this.SetPrinter(this.textBoxAdobeDriver);
        }
        
        private void BtnSelectTextEditorClick(object sender, System.EventArgs e)
        {
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                textBoxTextEditor.Text = this.openFileDialog1.FileName;
            }
        }
        
        private void Button2Click(object sender, System.EventArgs e)
        {
            SCaddins.SCexport.Settings1.Default.Reset();
            this.scx.LoadSettings();
            this.LoadValues();
        }
        
        private void OptionsDialogFormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            this.SaveValues();
        }
        
        private void BtnDefaultExportDirClick(object sender, System.EventArgs e)
        {
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                textBoxExportDir.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
