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

namespace SCaddins.SCaos
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Description of SCaosForm.
    /// </summary>
    public partial class SCaosForm : Form
    {
        public SCaosForm(string[] informationText, bool currentViewIsIso)
        {
            this.InitializeComponent();
            if (!currentViewIsIso) {
                radioButtonRotateCurrent.Enabled = false;
            }
            
            for (int i = 0; i < informationText.Length; i++) {
                listBox1.Items.Add(informationText[i]);
            }
            
            PopulateTimeSpansDropDowns(new DateTime(2015, 6, 21, 12, 0, 0, DateTimeKind.Local));
                        
            interval.Items.Add(new TimeSpan(0, 15, 0));
            interval.Items.Add(new TimeSpan(0, 30, 0));
            interval.Items.Add(new TimeSpan(1, 0, 0));
            interval.SelectedIndex = 2;         
            this.Text = "SCaos by Andrew Nicholas";
        }
        
        private void PopulateTimeSpansDropDowns(DateTime day)
        {
            startTime.Items.Clear();
            endTime.Items.Clear();
            for (int i = 8; i < 16; i++) {
                startTime.Items.Add(new DateTime(day.Year, day.Month, day.Day, i, 0, 0, DateTimeKind.Local));
                endTime.Items.Add(new DateTime(day.Year, day.Month, day.Day, i + 1, 0, 0, DateTimeKind.Local));
            }
            startTime.SelectedIndex = 1;   
            endTime.SelectedIndex = 6;    
        }
              
        private void Button3Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(Constants.HelpLink);
        }
        
        private void updateOkButton()
        {
            if (radioButtonRotateCurrent.Checked || radioButtonWinterViews.Checked){
                button1.Enabled = true;
            } else {
                button1.Enabled = false;    
            }
        }
        
        private void RadioButtonWinterViewsCheckedChanged(object sender, EventArgs e)
        {
            this.updateOkButton();
        }
        
        private void RadioButtonRotateCurrentCheckedChanged(object sender, EventArgs e)
        {
            this.updateOkButton();  
        }
        
        void DateTimePicker1ValueChanged(object sender, EventArgs e)
        {
            PopulateTimeSpansDropDowns(dateTimePicker1.Value);
        }               
    }
}
