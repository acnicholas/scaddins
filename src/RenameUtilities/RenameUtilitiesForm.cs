/*
 * Created by SharpDevelop.
 * User: andrewn
 * Date: 25/08/17
 * Time: 2:43 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SCaddins.RenameUtilities
{
    /// <summary>
    /// Description of Form1.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            PopulateCategoryComboBox();
            PopulatePresetsComboBox();
            dataGridView1.Rows[0].Cells[0].Value = "test";
            
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
            return Regex.Replace(s, pattern, replacement);
        }
        
        void DataGridToUpper()
        {
            dataGridView1.Rows[0].Cells[1].Value = "sdgsdfg";
            //dataGridView1.Rows[0].Cells[1].Value = dataGridView1.Rows[0].Cells[0].Value.ToString().ToUpper();
        }
        
        void UpdateDataGrid()
        {
            dataGridView1.Rows[0].Cells[1].Value = 
                GetReplacementResult(dataGridView1.Rows[0].Cells[0].Value.ToString(), textBoxFind.Text, textBoxReplace.Text);
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
                UpdateDataGrid();
            }
            if(comboBoxPresets.Text == "Uppercase") {
                EnableFindAndReplace(false);
                DataGridToUpper();
            }
            if(comboBoxPresets.Text == "Lowercase") {
                textBoxFind.Text = @"^.*$";
                textBoxReplace.Text = @"\L($1)\E";
                UpdateDataGrid();
            }
            //UpdateDataGrid();
        }
    }
}
