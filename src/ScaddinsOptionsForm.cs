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

namespace SCaddins
{
    using System;
    using System.Collections.Specialized;
    using System.Windows.Forms;
    
    /// <summary>
    /// Description of ScaddinsOptionsForm.
    /// </summary>
    public partial class SCaddinsOptionsForm : Form
    {
        public SCaddinsOptionsForm()
        {
            var collection = SCaddins.Scaddins.Default.DisplayOrder;
            
            if (collection.Count < 1) {
                collection = GetDefaultCollection();
            }
            
            this.InitializeComponent();
            this.checkBox1.Checked = SCaddins.Scaddins.Default.UpgradeCheckOnStartUp;
            this.PopulateListBox(collection);
        }
        
        public static StringCollection GetDefaultCollection()
        {
            var collection = new StringCollection();
                collection.Add("SCexport");
                collection.Add("SCulcase");
                collection.Add("SCoord");
                collection.Add("SCaos");
                collection.Add("SCuv");
                collection.Add("SCopy");
                collection.Add("SCightlines");
                collection.Add("SCwash");
                collection.Add("SCincrement");
                collection.Add("SCloudSched");
                return collection;
        }
        
        private void PopulateListBox(StringCollection collection)
        {
            for (int i = 0; i < collection.Count; i++) {
                this.listBox1.Items.Add(collection[i]);
            }    
        }
        
        private void ApplyDefaultOrder() {
            listBox1.Items.Clear();
            this.PopulateListBox(GetDefaultCollection());
        }
        
        private void ButtonUpClick(object sender, EventArgs e)
        {
            this.MoveUp();
        }
        
        private void MoveUp()
        {
            this.MoveItem(-1);
        }

        private void MoveDown()
        {
            this.MoveItem(1);
        }

        // from http://stackoverflow.com/questions/4796109/how-to-move-item-in-listbox-up-and-down
        private void MoveItem(int direction)
        {
            if (listBox1.SelectedItem == null || listBox1.SelectedIndex < 0) {
                return;
            }

            int newIndex = listBox1.SelectedIndex + direction;

            if (newIndex < 0 || newIndex >= listBox1.Items.Count) {
                return;
            }

            object selected = listBox1.SelectedItem;
            listBox1.Items.Remove(selected);
            listBox1.Items.Insert(newIndex, selected);
            listBox1.SetSelected(newIndex, true);
        }
        
        private void ButtonDownClick(object sender, EventArgs e)
        {
             this.MoveDown();  
        }
        
        private void ButtonOKClick(object sender, EventArgs e)
        {
            var collection = new StringCollection();
            for (int i = 0; i < listBox1.Items.Count; i++) {
                collection.Add(listBox1.Items[i].ToString());
            }
            SCaddins.Scaddins.Default.DisplayOrder = collection;
            SCaddins.Scaddins.Default.UpgradeCheckOnStartUp = checkBox1.Checked;
            SCaddins.Scaddins.Default.Save();
        }
        
        private void ButtonAllClick(object sender, EventArgs e)
        {
            this.ApplyDefaultOrder();
        }
        
        private void ButtonRemoveClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null || listBox1.SelectedIndex < 0) {
                return;
            }
            object selected = listBox1.SelectedItem;  
            listBox1.Items.Remove(selected);        
        }       
    }
}
