// (C) Copyright 2015 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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
using System.Drawing;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace SCaddins
{
    /// <summary>
    /// Description of ScaddinsOptionsForm.
    /// </summary>
    public partial class ScaddinsOptionsForm : Form
    {
        public ScaddinsOptionsForm()
        {
            var collection = SCaddins.Scaddins.Default.DisplayOrder;
            
            if (collection.Count < 1) {
                collection = GetDefualtCollection();
            }
            
            InitializeComponent();
            PopulateListBox(collection);
        }
        
        private StringCollection GetDefualtCollection()
        {
            var collection = new StringCollection();
                collection.Add("SCexport");
                collection.Add("SCulcase");
                collection.Add("SCoord");
                collection.Add("SCaos");
                collection.Add("SCuv");
                collection.Add("SCopy");
                collection.Add("SCighlines");
                collection.Add("SCwash");
                collection.Add("SCincrement");
                return collection;
        }
        
        private void PopulateListBox(StringCollection collection)
        {
            for(int i = 0; i < collection.Count ; i++){
                this.checkedListBox1.Items.Add(collection[i]);
            }    
        }
        
        private void ApplyDefaultOrder(){
            checkedListBox1.Items.Clear();
            PopulateListBox(GetDefualtCollection());
        }
        
        private void ButtonUpClick(object sender, EventArgs e)
        {
            MoveUp();
        }
        
        private void MoveUp()
        {
            MoveItem(-1);
        }

        private void MoveDown()
        {
            MoveItem(1);
        }

        //from http://stackoverflow.com/questions/4796109/how-to-move-item-in-listbox-up-and-down
        private void MoveItem(int direction)
        {
            if (checkedListBox1.SelectedItem == null || checkedListBox1.SelectedIndex < 0)
                return; // No selected item - nothing to do

            int newIndex = checkedListBox1.SelectedIndex + direction;

            if (newIndex < 0 || newIndex >= checkedListBox1.Items.Count)
                return; // Index out of range - nothing to do

            object selected = checkedListBox1.SelectedItem;

            checkedListBox1.Items.Remove(selected);
            checkedListBox1.Items.Insert(newIndex, selected);
            checkedListBox1.SetSelected(newIndex, true);
        }
        
        private void ButtonDownClick(object sender, EventArgs e)
        {
             MoveDown();  
        }
        
        private void ButtonOKClick(object sender, EventArgs e)
        {
            var collection = new StringCollection();
            for(int i = 0; i < checkedListBox1.Items.Count; i++){
                collection.Add(checkedListBox1.Items[i].ToString());
            }
            SCaddins.Scaddins.Default.DisplayOrder = collection;
            SCaddins.Scaddins.Default.Save();
        }
    }
}
