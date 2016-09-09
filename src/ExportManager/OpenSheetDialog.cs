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
    using System.Windows.Forms;
    
    public partial class OpenSheetDialog : Form
    {
        public OpenSheetDialog()
        {
            this.InitializeComponent();
            this.textBox1.Focus();
            this.ActiveControl = this.textBox1;
        }
        
        public string Value
        {
            get { return this.textBox1.Text.ToUpper(CultureInfo.CurrentCulture); }
        }
    }
}
