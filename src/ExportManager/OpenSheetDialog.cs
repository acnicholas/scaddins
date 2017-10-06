// (C) Copyright 2013-2014 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Linq;
    
    public partial class OpenSheetDialog : Form
    {
        private Autodesk.Revit.DB.Document doc;
        
        public OpenSheetDialog(Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
            this.InitializeComponent();
            this.textBox1.Focus();
            this.ActiveControl = this.textBox1;
            this.dataGridView1.DataSource = SCaddins.ExportManager.OpenSheet.ViewsInModel(doc);
        }
        
        public string Value {
            get { return this.textBox1.Text.ToUpper(CultureInfo.CurrentCulture); }
        }
        
        private void TextBox1TextChanged(object sender, EventArgs e)
        {
            List<OpenableView> list = SCaddins.ExportManager.OpenSheet.ViewsInModel(doc)
                .Where(x => (x.Name.ToUpper().Contains(textBox1.Text.ToUpper()) || x.SheetNumber.ToUpper().Contains(textBox1.Text.ToUpper())))
        		.ToList();
            if (list.Count > 0) {
                this.dataGridView1.DataSource = list;
            }
        }
        
        private void ScrollDown()
        {
            int selectedRow = dataGridView1.CurrentCell.RowIndex;
            if (selectedRow < dataGridView1.Rows.Count - 1) {
                dataGridView1.Rows[selectedRow].Selected = false;
                dataGridView1.Rows[selectedRow + 1].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[selectedRow + 1].Cells[0];
            } else {
                dataGridView1.Rows[selectedRow].Selected = false;
                dataGridView1.Rows[0].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];   
            }	
        }
        
        private void ScrollUp()
        {
            int selectedRow = dataGridView1.CurrentCell.RowIndex;
            if (selectedRow > 0) {
                dataGridView1.Rows[selectedRow].Selected = false;
                dataGridView1.Rows[selectedRow - 1].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[selectedRow - 1].Cells[0];
            } else {
                dataGridView1.Rows[selectedRow].Selected = false;
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];   
            }	
        }

        void TextBox1PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (dataGridView1.Rows.Count < 1)
                return;
            if (e.Modifiers == Keys.Control) {
                switch (e.KeyCode) { 
                    case Keys.J:
                        ScrollDown();
                        break;
                    case Keys.K:
                        ScrollUp();
                        break;
                }
                return;
            }
            switch (e.KeyCode) {   
                case Keys.Down:
                    ScrollDown();
                    break;
                case Keys.Up:
                    ScrollUp();
                    break;
                case Keys.Enter:
                    OpenSelection();
                    break;
                case Keys.Escape:
                    this.Close();
                    this.Dispose();	 
                    break;
            }
        }
        
        private void OpenSelection()
        {
            DataGridViewRow row = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex];
            var ov = row.DataBoundItem as OpenableView;
            ov.Open();
            this.Close();
            this.Dispose();	
        }
        
        void DataGridView1DoubleClick(object sender, EventArgs e)
        {
            OpenSelection();
        }
		
        void DataGridView1PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            textBox1.Focus();
        }
        void OpenSheetDialogPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            textBox1.Focus();
        }
    }
}
