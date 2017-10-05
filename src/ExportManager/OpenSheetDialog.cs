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
            this.dataGridView1.DataSource = SCaddins.ExportManager.ExportManager.ViewsInModel(doc);
        }
        
        public string Value
        {
            get { return this.textBox1.Text.ToUpper(CultureInfo.CurrentCulture); }
        }
        
        private void TextBox1TextChanged(object sender, EventArgs e)
        {
            List<OpenableView> list = SCaddins.ExportManager.ExportManager.ViewsInModel(doc).Where(x => x.Name.Contains(textBox1.Text)).ToList();
            if(list.Count > 0) {
                this.dataGridView1.DataSource = list;
            }
        }

        void TextBox1PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(dataGridView1.Rows.Count < 1) return;
            int selectedRow = dataGridView1.CurrentCell.RowIndex;
            switch (e.KeyCode) {   
                case Keys.Down:
                    if (selectedRow < dataGridView1.Rows.Count - 1){
                        dataGridView1.Rows[selectedRow].Selected = false;
                        dataGridView1.Rows[selectedRow + 1].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[selectedRow + 1].Cells[0];
                    } else {
                        dataGridView1.Rows[selectedRow].Selected = false;
                        dataGridView1.Rows[0].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];   
                    }
                    break;
                case Keys.Up:
                    if (selectedRow > 0) {
                        dataGridView1.Rows[selectedRow].Selected = false;
                        dataGridView1.Rows[selectedRow -1].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[selectedRow -1].Cells[0];
                    } else {
                        dataGridView1.Rows[selectedRow].Selected = false;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];   
                    }
                    break;
                case Keys.Enter:
                    DataGridViewRow row = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex];
                    var ov = row.DataBoundItem as OpenableView;
                    ov.Open();
                    this.Close();
                    this.Dispose();
                    break;
            }
        }
    }
}
