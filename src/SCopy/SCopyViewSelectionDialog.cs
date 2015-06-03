// (C) Copyright 2014 by Andrew Nicholas
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

namespace SCaddins.SCopy
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Description of Form1.
    /// </summary>
    public partial class SCopyViewSelectionDialog : Form
    {
        public SCopyViewSelectionDialog()
        {
            this.InitializeComponent();
        }
        
        public void Add(Autodesk.Revit.DB.Element view)
        {
            listBox1.Items.Add(view.Name);
            
            // TODO just do this once.
            listBox1.Sorted = true;
        }
        
        public string SelectedView() {
            return listBox1.Text;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
