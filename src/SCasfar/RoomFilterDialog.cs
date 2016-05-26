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

namespace SCaddins.SCasfar
{
    /// <summary>
    /// Description of RoomFilterDialog.
    /// </summary>
    public partial class RoomFilterDialog : System.Windows.Forms.Form
    {
        public RoomFilterDialog(Document doc, ref RoomFilter filter)
        {
            InitializeComponent();
            
              
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
            
            comboBox1.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBox4.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBox8.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBox11.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBox14.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBox17.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            comboBox20.DataSource = Enum.GetValues(typeof(RoomFilterItem.ComparisonOperators));
            
            comboBox5.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBox6.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBox9.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBox12.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBox15.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
            comboBox18.DataSource = Enum.GetValues(typeof(RoomFilterItem.LogicalOperators));
        }
        void Button1Click(object sender, EventArgs e)
        {
            if( !string.IsNullOrWhiteSpace(comboBoxP1.Text)){
                TaskDialog.Show(comboBoxP1.Text, comboBoxP1.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP2.Text)){
                TaskDialog.Show(comboBoxP2.Text, comboBoxP2.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP3.Text)){
                TaskDialog.Show(comboBoxP3.Text, comboBoxP3.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP4.Text)){
                TaskDialog.Show(comboBoxP4.Text, comboBoxP4.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP5.Text)){
                TaskDialog.Show(comboBoxP5.Text, comboBoxP5.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP6.Text)){
                TaskDialog.Show(comboBoxP6.Text, comboBoxP6.Text);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP7.Text)){
                TaskDialog.Show(comboBoxP7.Text, comboBoxP7.Text);
            }
        }
    }
}
