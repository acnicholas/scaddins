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
            this.originalCandidates = candidates;
            this.doc = doc;
            this.rf = new RoomFilter();
            this.rfd = new RoomFilterDialog(doc, ref rf);
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
        
        private  void Button2Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = candidates;   
        }
        
        private void Button3Click(object sender, EventArgs e)
        {
            rfd.ShowDialog();

            Collection<RoomToPlanCandidate> toRemove = new Collection<RoomToPlanCandidate>();

            foreach(RoomToPlanCandidate c in candidates){
                TaskDialog.Show("test", c.DestViewName);
                //if(!c.PassesFilter()){
                //    toRemove.Add(c);
                //}
            }
            foreach(RoomToPlanCandidate c in toRemove){
                candidates.Remove(c);
            }
            
            dataGridView1.Refresh();
        }
        
    }
  
}
