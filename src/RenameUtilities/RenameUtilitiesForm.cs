
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SCaddins.RenameUtilities
{
    /// <summary>
    /// Description of Form1.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1(List<RenameCandidate> candidates)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            PopulateCategoryComboBox();
            PopulatePresetsComboBox();
            dataGridView1.DataSource = candidates;
            //dataGridView1.Refresh;
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        
        private void PopulateCategoryComboBox()
        {
            comboBox1.Items.Add("Rooms");
            comboBox1.Items.Add("Generic Annotations");
            comboBox1.Items.Add("Views");
            comboBox1.Items.Add("Sheets");
            comboBox1.Items.Add("Revisions");
            comboBox1.Items.Add("Walls");
            comboBox1.Items.Add("Doors");
            comboBox1.Items.Add("Floors");
            comboBox1.Items.Add("Roofs");
        }
        
        private void PopulatePresetsComboBox()
        {
            comboBoxPresets.Items.Add("Custom");
            comboBoxPresets.Items.Add("Uppercase");
            comboBoxPresets.Items.Add("Lowercase");
            comboBoxPresets.Items.Add("TitleCase");
            comboBoxPresets.Items.Add("Lazy Increment");
            comboBoxPresets.Items.Add("Smart Increment");
            comboBoxPresets.Items.Add("Mirror");
        }
               
        void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Rooms") {
                
            }
            if (comboBox1.Text == "Views") {
                
            }
        }
        
        void Panel2Paint(object sender, PaintEventArgs e)
        {
          
        }
        
        void Panel1Paint(object sender, PaintEventArgs e)
        {
          
        }
        
        void Button3Click(object sender, EventArgs e)
        {
          
        }
        
        private static string GetReplacementResult(string s, string pattern, string replacement)
        {
            if (string.IsNullOrWhiteSpace(replacement) || string.IsNullOrEmpty(replacement)){
                   return s;
            }
            return Regex.Replace(s, pattern, replacement);
        }
        
        void DataGridToUpper()
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RenameCandidate candidate = (RenameCandidate)row.DataBoundItem;
                candidate.NewValue = candidate.OldValue.ToUpper();
            }
            dataGridView1.Refresh();
        }
        
        void DataGridToLower()
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RenameCandidate candidate = (RenameCandidate)row.DataBoundItem;
                candidate.NewValue = candidate.OldValue.ToLower();
            }
            dataGridView1.Refresh();
        }
        
        void UpdateDataGrid()
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows) {
                RenameCandidate candidate = (RenameCandidate)row.DataBoundItem;
                candidate.NewValue = GetReplacementResult(candidate.OldValue, textBoxFind.Text, textBoxReplace.Text);
            }
            dataGridView1.Refresh();
        }
        
        void ClearFindAndReplace()
        {
            textBoxFind.Text = string.Empty;
            textBoxReplace.Text = string.Empty;    
        }
        
        void EnableFindAndReplace(bool enable)
        {
            textBoxFind.Enabled = enable;
            textBoxReplace.Enabled = enable;  
            ClearFindAndReplace();
        }
        
        void ComboBoxPresetsSelectedValueChanged(object sender, EventArgs e)
        {
            if(comboBoxPresets.Text == "Custom") {
                EnableFindAndReplace(true);
            }
            if(comboBoxPresets.Text == "Uppercase") {
                EnableFindAndReplace(false);
                DataGridToUpper();
            }
            if(comboBoxPresets.Text == "Lowercase") {
                DataGridToLower();
            }
        }
                       
        void DataGridView1RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            RenameCandidate candidate = (RenameCandidate)(dataGridView1.Rows[e.RowIndex].DataBoundItem);
            if (candidate.ValueChanged()) {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
            } else {
                 dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;    
            }  
        }
        
        void TextBoxReplaceTextChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();    
        }
    }
}
