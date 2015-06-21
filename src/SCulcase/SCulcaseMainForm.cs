// (C) Copyright 2012-2014 by Andrew Nicholas
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

namespace SCaddins.SCulcase
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    
    public partial class SCulcaseMainForm : System.Windows.Forms.Form
    {
        private Document doc;
        private ConversionTypes conversionTypes;
        private ConversionMode conversionMode;

        public SCulcaseMainForm(Document doc)
        {
            this.doc = doc;    
            this.conversionTypes = ConversionTypes.None;
            this.conversionMode = ConversionMode.UpperCase;
            this.InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.TagCheckBoxes();
            this.ShowDialog();
        }
               
        private void TagCheckBoxes()
        {
            chkAnnotation.Tag = ConversionTypes.Text;
            chkRooms.Tag = ConversionTypes.RoomNames;
            chkSheets.Tag = ConversionTypes.SheetNames;
            chkViews.Tag = ConversionTypes.ViewNames;
            chkViewTitleOnSheets.Tag = ConversionTypes.TitlesOnSheets;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            UppercaseUtilities.ConvertAllDryRun(this.conversionMode, this.conversionTypes, this.doc);
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) {
                this.conversionMode = ConversionMode.UpperCase;
            }
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) {
                this.conversionMode = ConversionMode.LowerCase;
            }
        }
        
        private void RadioButton3CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked) {
                this.conversionMode = ConversionMode.TitleCase;
            }
        }

        private void ToggleConversionFlag(CheckBox box, ConversionTypes val)
        {
            if (box.Checked == true) {
                this.conversionTypes |= val;
            } else {
                this.conversionTypes = this.conversionTypes & ~val;  
            }
        }
        
        private void ToggleCheckBoxValue(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            ConversionTypes t = (ConversionTypes)c.Tag;
            this.ToggleConversionFlag(c, t);
        }
      
        private void BtnOKAYClick(object sender, EventArgs e)
        {
            UppercaseUtilities.ConvertAll(this.conversionMode, this.conversionTypes, this.doc);
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
