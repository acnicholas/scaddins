// (C) Copyright 2015 by Andrew Nicholas
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

namespace SCaddins.SCincrement
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;
    
    public partial class SCincrementSettingsForm : Form
    {
        public SCincrementSettingsForm()
        {
            this.InitializeComponent();
            this.LoadSettings();     
        }
        
        private void LoadSettings()
        {
            this.offsetTextBox.Text = SCincrementSettings.Default.OffsetValue.ToString(CultureInfo.CurrentCulture);
            this.incrementTextBox.Text = SCincrementSettings.Default.IncrementValue.ToString(CultureInfo.CurrentCulture);
            this.replacementTextBox.Text = SCincrementSettings.Default.SourceReplacePattern;
            this.searchTextBox.Text = SCincrementSettings.Default.SourceSearchPattern;
            this.destReplacementTextBox.Text = SCincrementSettings.Default.DestinationReplacePattern;
            this.destSearchTextBox.Text = SCincrementSettings.Default.DestinationSearchPattern;
        }
        
        private void Button1Click(object sender, EventArgs e)
        {
            SCincrementSettings.Default.Reset();
            this.LoadSettings();
        }
        
        private void Button2Click(object sender, EventArgs e)
        {
            SCincrementSettings.Default.OffsetValue = int.Parse(this.offsetTextBox.Text, CultureInfo.InvariantCulture);
            SCincrementSettings.Default.IncrementValue = int.Parse(this.incrementTextBox.Text, CultureInfo.InvariantCulture);
            SCincrementSettings.Default.SourceSearchPattern = this.searchTextBox.Text;
            SCincrementSettings.Default.SourceReplacePattern = this.replacementTextBox.Text;
            SCincrementSettings.Default.DestinationReplacePattern = this.destReplacementTextBox.Text;
            SCincrementSettings.Default.DestinationSearchPattern = this.destSearchTextBox.Text;
            SCincrementSettings.Default.CustomParameterName = this.customParamTextBox.Text;
            SCincrementSettings.Default.UseCustomParameterName = this.CustomParamCheckBox.Checked;
            SCincrementSettings.Default.Save();
        }
        
        private void Button3Click(object sender, EventArgs e)
        {
          // do nothing...
        }
    }
}
