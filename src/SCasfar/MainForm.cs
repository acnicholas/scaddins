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



namespace SCaddins.SCasfar
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.ObjectModel;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    
    public partial class MainForm : System.Windows.Forms.Form
    {
        private System.ComponentModel.BindingList<RoomToPlanCandidate> candidates;
        private System.ComponentModel.BindingList<RoomToPlanCandidate> originalCandidates;
        private Document doc;
        private RoomFilterDialog rfd;
        private RoomFilter rf;
        
        public MainForm(System.ComponentModel.BindingList<RoomToPlanCandidate> candidates, Document doc)
        {
            InitializeComponent();          
            this.AddDataGridColumns();  
            this.candidates = candidates; 
            this.originalCandidates = new System.ComponentModel.BindingList<RoomToPlanCandidate>();
            Copy(candidates, originalCandidates);
            this.doc = doc;
            this.rf = new RoomFilter();
            this.rfd = new RoomFilterDialog(doc, rf);
        }
        
        private void AddDataGridColumns()
        {
            this.dataGridView1.AutoGenerateColumns = false;
                      
            SCaddins.SCopy.MainForm.AddColumn("Number", "Room Number", this.dataGridView1);
            SCaddins.SCopy.MainForm.AddColumn("Name", "Room Name", this.dataGridView1);     
            SCaddins.SCopy.MainForm.AddColumn("DestViewName", "New Plan Name", this.dataGridView1);
            SCaddins.SCopy.MainForm.AddColumn("DestSheetNumber", "New Sheet Number", this.dataGridView1);
            SCaddins.SCopy.MainForm.AddColumn("DestSheetName", "New Sheet Name", this.dataGridView1);
            //this.AddComboBoxColumns(); 
        }
        
        private  void ButtonRefreshClick(object sender, EventArgs e)
        {
            dataGridView1.DataSource = candidates;   
        }
        
        private void Copy(Collection<RoomToPlanCandidate> src, Collection<RoomToPlanCandidate> dest)
        {
             dest.Clear();
             foreach (RoomToPlanCandidate c in src) {
                dest.Add(c);
            }    
        }
        
        private void ButtonFilterClick(object sender, EventArgs e)
        {
            DialogResult dr = rfd.ShowDialog();
            if (dr == DialogResult.OK) {
                Collection<RoomToPlanCandidate> toRemove = new Collection<RoomToPlanCandidate>();
                foreach (RoomToPlanCandidate c in candidates) {
                    if (!c.PassesFilter(rf)) {
                        toRemove.Add(c);
                    }
                }
                foreach (RoomToPlanCandidate c in toRemove) {
                    candidates.Remove(c);
                }
            }
            dataGridView1.Refresh();
        }
        
        private void ButtonResetClick(object sender, EventArgs e)
        {
            Copy(originalCandidates, candidates);  
            dataGridView1.Refresh();
        }
        
        private void ButtonGoClick(object sender, EventArgs e)
        {
          
        }
        
    }
  
}
