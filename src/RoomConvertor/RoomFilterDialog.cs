// (C) Copyright 2016 by Andrew Nicholas
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

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using SCaddins.RoomConvertor;

namespace SCaddins.RoomConvertor
{
    public partial class RoomFilterDialog : System.Windows.Forms.Form
    {
        private RoomFilter filter;
        
        public RoomFilterDialog(RoomFilter filter, Document doc)
        {
            InitializeComponent();
            
            this.filter = filter;
            
            Room room = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).FirstElement() as Room;
            
            var s = new List<String>();
            foreach (Parameter p in room.Parameters) {  
                //don't add ElementID values yet (too much effort)
                if (p.StorageType != StorageType.ElementId && p.StorageType != StorageType.None) {
                    s.Add(p.Definition.Name);
                }
            }
            
            s.Sort();
            string[] s2 = s.ToArray();
            
            comboBoxP1.Items.AddRange(s2);
            comboBoxP2.Items.AddRange(s2);
            comboBoxP3.Items.AddRange(s2);
            comboBoxP4.Items.AddRange(s2);
            comboBoxP5.Items.AddRange(s2);
            comboBoxP6.Items.AddRange(s2);
            comboBoxP7.Items.AddRange(s2);
            
            comboBoxCO1.DataSource = Enum.GetValues(typeof(ComparisonOperator));
            comboBoxCO2.DataSource = Enum.GetValues(typeof(ComparisonOperator));
            comboBoxCO3.DataSource = Enum.GetValues(typeof(ComparisonOperator));
            comboBoxCO4.DataSource = Enum.GetValues(typeof(ComparisonOperator));
            comboBoxCO5.DataSource = Enum.GetValues(typeof(ComparisonOperator));
            comboBoxCO6.DataSource = Enum.GetValues(typeof(ComparisonOperator));
            comboBoxCO7.DataSource = Enum.GetValues(typeof(ComparisonOperator));
            
            comboBoxLO2.DataSource = Enum.GetValues(typeof(LogicalOperator));
            comboBoxLO3.DataSource = Enum.GetValues(typeof(LogicalOperator));
            comboBoxLO4.DataSource = Enum.GetValues(typeof(LogicalOperator));
            comboBoxLO5.DataSource = Enum.GetValues(typeof(LogicalOperator));
            comboBoxLO6.DataSource = Enum.GetValues(typeof(LogicalOperator));
            comboBoxLO7.DataSource = Enum.GetValues(typeof(LogicalOperator));
        }
        
        void ButtonOKClick(object sender, EventArgs e)
        {
            if( !string.IsNullOrWhiteSpace(comboBoxP1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text)){
                var item = new RoomFilterItem("And", comboBoxCO1.Text, comboBoxP1.Text, textBox1.Text);
                filter.AddFilterItem(item);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP2.Text) && !string.IsNullOrWhiteSpace(textBox2.Text)){
                var item = new RoomFilterItem(comboBoxLO2.Text, comboBoxCO2.Text, comboBoxP2.Text, textBox2.Text);
                filter.AddFilterItem(item);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP3.Text) && !string.IsNullOrWhiteSpace(textBox3.Text)){
                var item = new RoomFilterItem(comboBoxLO3.Text, comboBoxCO3.Text, comboBoxP3.Text, textBox3.Text);
                filter.AddFilterItem(item);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP4.Text) && !string.IsNullOrWhiteSpace(textBox4.Text)){
                var item = new RoomFilterItem(comboBoxLO4.Text, comboBoxCO4.Text, comboBoxP4.Text, textBox4.Text);
                filter.AddFilterItem(item);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP5.Text) && !string.IsNullOrWhiteSpace(textBox5.Text)){
                var item = new RoomFilterItem(comboBoxLO5.Text, comboBoxCO5.Text, comboBoxP5.Text, textBox5.Text);
                filter.AddFilterItem(item);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP6.Text) && !string.IsNullOrWhiteSpace(textBox6.Text)){
                var item = new RoomFilterItem(comboBoxLO6.Text, comboBoxCO6.Text, comboBoxP6.Text, textBox6.Text);
                filter.AddFilterItem(item);
            }
            if( !string.IsNullOrWhiteSpace(comboBoxP7.Text) && !string.IsNullOrWhiteSpace(textBox7.Text)){
                var item = new RoomFilterItem(comboBoxLO7.Text, comboBoxCO7.Text, comboBoxP7.Text, textBox7.Text);
                filter.AddFilterItem(item);
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
