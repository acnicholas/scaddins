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

namespace SCaddins.RoomConvertor
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Forms;
    
    public partial class MainForm : System.Windows.Forms.Form
    {
        private RoomInfoDialog info;
        private RoomFilterDialog rfd;
        private RoomFilter rf;
        private RoomConversionManager roomConversionManager;

        public MainForm(RoomConversionManager scasfar)
        {
            InitializeComponent();          

            this.roomConversionManager = scasfar;
            this.AddDataGridColumns(); 
            this.PopulateTitleblockCombo();
            this.rf = new RoomFilter();
            this.rfd = new RoomFilterDialog(rf, scasfar.Doc);
            this.info = new RoomInfoDialog();
            info.TopMost = true;

            //make not editable columns gray
            this.dataGridView1.Columns[0].DefaultCellStyle.ForeColor = System.Drawing.Color.DarkSlateGray;  
            this.dataGridView1.Columns[1].DefaultCellStyle.ForeColor = System.Drawing.Color.DarkSlateGray;  

            //assign tooltips
            var filterTip = new ToolTip();
            filterTip.SetToolTip(this.buttonFilter,@"Filter the room list(above) by selected parameter values.");
            var renameTip = new ToolTip();
            renameTip.SetToolTip(this.buttonRename,@"Bulk rename selected items in the list above.");

            //load list into view
            LoadDataGridSource();

            //dataGridView1.Sort(dataGridView1.Columns[1], System.ComponentModel.ListSortDirection.Ascending);
        }

        private void PopulateTitleblockCombo()
        {
            foreach (var key in roomConversionManager.TitleBlocks.Keys){
                comboBoxTitles.Items.Add(key);
            }
            comboBoxTitles.SelectedIndex = 0;
            //scasfar.SetTitleBlockId();
        }

        private void LoadDataGridSource()
        {
            dataGridView1.DataSource = roomConversionManager.Candidates;     
        }

        private void AddDataGridColumns()
        {
            this.dataGridView1.AutoGenerateColumns = false;           
            SCaddins.SheetCopier.MainForm.AddColumn("Number", "Room Number", dataGridView1);
            SCaddins.SheetCopier.MainForm.AddColumn("Name", "Room Name", dataGridView1);            
            SCaddins.SheetCopier.MainForm.AddColumn("DestinationViewName", "New Plan Name", dataGridView1, false);
            SCaddins.SheetCopier.MainForm.AddColumn("DestinationSheetNumber", "New Sheet Number", dataGridView1, false);
            SCaddins.SheetCopier.MainForm.AddColumn("DestinationSheetName", "New Sheet Name", dataGridView1, false);
        }

        private void ButtonFilterClick(object sender, EventArgs e)
        {
            DialogResult dr = rfd.ShowDialog();
            if (dr == DialogResult.OK) {
                Collection<RoomConversionCandidate> toRemove = new Collection<RoomConversionCandidate>();
                foreach (RoomConversionCandidate c in roomConversionManager.Candidates) {
                    if (!c.PassesFilter(rf)) {
                        toRemove.Add(c);
                    }
                }
                foreach (RoomConversionCandidate c in toRemove) {
                    roomConversionManager.Candidates.Remove(c);
                }
            }
            dataGridView1.Refresh();
        }

        private void ButtonResetClick(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            roomConversionManager.Reset();
            rfd.Clear();
            LoadDataGridSource();
            dataGridView1.Refresh();
        }

        private System.ComponentModel.BindingList<RoomConversionCandidate> GetSelectedCandidates()
        {
            var c = new System.ComponentModel.BindingList<RoomConversionCandidate>(); 
            for (int i = 0; i < dataGridView1.SelectedRows.Count; i++){
                c.Add((RoomConversionCandidate)dataGridView1.SelectedRows[i].DataBoundItem);
            }
            return c;
        }

        private void ButtonGoClick(object sender, EventArgs e)
        {
            roomConversionManager.CreateViewsAndSheets(GetSelectedCandidates(), this.comboBoxTitles.Text);
        }

        void DataGridView1SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.RowIndex != -1) {
                var c = (RoomConversionCandidate)dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DataBoundItem;
                info.UpdateRoomInfo(c.Room);
                info.Refresh();
            }
        }

        void ButtonInfoClick(object sender, EventArgs e)
        {
            if (info.Visible ) {
                info.Hide();
            } else {
                info.Show();
            }
        }

        void Button3Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if(button.Text == "Create Masses") {
                roomConversionManager.CreateRoomMasses(GetSelectedCandidates());      
            } else {
                roomConversionManager.CreateViewsAndSheets(GetSelectedCandidates(), this.comboBoxTitles.Text);
            }
        }

        void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            info.Dispose();
            rfd.Dispose();
            Dispose();
        }
        
        void ToggleMainButtonText()
        {
            buttonMain.Text = radioButtonCreateMasses.Checked ? "Create Masses" : "Create Plans and Sheets" ;
            bool b = radioButtonCreateSheets.Checked;
            dataGridView1.Columns[2].Visible = b;
            dataGridView1.Columns[3].Visible = b;
            dataGridView1.Columns[4].Visible = b; 
        }

        void RadioButton1CheckedChanged(object sender, EventArgs e)
        {
            ToggleMainButtonText();
        }
               
        void Button4Click(object sender, EventArgs e)
        {
            roomConversionManager.SynchronizeMassesToRooms();
        }             
    } 
}
