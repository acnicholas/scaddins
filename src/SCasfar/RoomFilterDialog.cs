/*
 * Created by SharpDevelop.
 * User: andrewn
 * Date: 25/05/16
 * Time: 12:58 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
using SCaddins.SCasfar;

namespace SCaddins.SCasfar
{
    /// <summary>
    /// Description of RoomFilterDialog.
    /// </summary>
    public partial class RoomFilterDialog : System.Windows.Forms.Form
    {
        private RoomFilter filter;
        
        public RoomFilterDialog(Document doc, RoomFilter filter)
        {
            InitializeComponent();
            
            this.filter = filter;
            
            Room room = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).FirstElement() as Room;
            
            foreach (Parameter p in room.Parameters) {               
                comboBoxP1.Items.Add(p.Definition.Name);
                comboBoxP2.Items.Add(p.Definition.Name);
                comboBoxP3.Items.Add(p.Definition.Name);
                comboBoxP4.Items.Add(p.Definition.Name);
                comboBoxP5.Items.Add(p.Definition.Name);
                comboBoxP6.Items.Add(p.Definition.Name);
                comboBoxP7.Items.Add(p.Definition.Name);
            }
            
            comboBoxCO1.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBoxCO2.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBoxCO3.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBoxCO4.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBoxCO5.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBoxCO6.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBoxCO7.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            
            comboBoxLO2.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBoxLO3.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBoxLO4.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBoxLO5.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBoxLO6.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBoxLO7.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
        }
        
        void ButtonOKClick(object sender, EventArgs e)
        {
            if( !string.IsNullOrWhiteSpace(comboBoxP1.Text)){
                var item = new RoomFilterItem("AND", comboBoxCO1.Text, comboBoxP1.Text, textBox1.Text);
                filter.AddFilterItem(item);
                //TaskDialog.Show(comboBoxP1.Text, comboBoxP1.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP2.Text)){
                var item = new RoomFilterItem(comboBoxLO2.Text, comboBoxCO2.Text, comboBoxP2.Text, textBox2.Text);
                filter.AddFilterItem(item);
                //TaskDialog.Show(comboBoxP2.Text, comboBoxP2.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP3.Text)){
                var item = new RoomFilterItem(comboBoxLO3.Text, comboBoxCO3.Text, comboBoxP3.Text, textBox3.Text);
                filter.AddFilterItem(item);
                //TaskDialog.Show(comboBoxP3.Text, comboBoxP3.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP4.Text)){
                var item = new RoomFilterItem(comboBoxLO4.Text, comboBoxCO4.Text, comboBoxP4.Text, textBox4.Text);
                filter.AddFilterItem(item);
                //TaskDialog.Show(comboBoxP4.Text, comboBoxP4.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP5.Text)){
                var item = new RoomFilterItem(comboBoxLO5.Text, comboBoxCO5.Text, comboBoxP5.Text, textBox5.Text);
                filter.AddFilterItem(item);
                //TaskDialog.Show(comboBoxP5.Text, comboBoxP5.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP6.Text)){
                var item = new RoomFilterItem(comboBoxLO6.Text, comboBoxCO6.Text, comboBoxP6.Text, textBox6.Text);
                filter.AddFilterItem(item);
                //TaskDialog.Show(comboBoxP6.Text, comboBoxP6.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP7.Text)){
                var item = new RoomFilterItem(comboBoxLO7.Text, comboBoxCO7.Text, comboBoxP7.Text, textBox7.Text);
                filter.AddFilterItem(item);
                //TaskDialog.Show(comboBoxP7.Text, comboBoxP7.Text);
            }
        }
        
        public void Clear()
        {
            filter.Clear();
            foreach (System.Windows.Forms.Control c in this.Controls) {
                if(c is System.Windows.Forms.TextBox) {
                    ((System.Windows.Forms.TextBox)c).Text = string.Empty;
                }
                if(c is System.Windows.Forms.ComboBox) {
                   ((System.Windows.Forms.ComboBox)c).Text = string.Empty;
                }
            }    
        }
                
        void ButtonResetClick(object sender, EventArgs e)
        {
            Clear();
        }
        
        void ButtonApplyClick(object sender, EventArgs e)
        {
            ButtonOKClick(sender, e);
        }
    }
}
