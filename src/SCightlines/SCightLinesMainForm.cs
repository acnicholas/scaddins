// (C) Copyright 2012-2013 by Andrew Nicholas
//
// This file is part of SCightlines.
//
// SCightlines is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCightlines is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCightlines.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCightLines
{
    using System;
    using System.Windows.Forms;
    
    /// <summary>
    /// The main form for the Line of Sight Utility
    /// </summary>
    /// <author>
    /// Andrew Nicholas
    /// </author>
    public partial class SCightLinesMainForm : Form
    {
        /// <summary>
        /// Form to display Riser/Tread info
        /// </summary>
        private SCightOutputForm info;
        
        /// <summary>
        /// Sight Line Class
        /// </summary>
        private SCightLines sightLines;
        
        private Autodesk.Revit.DB.Document doc;

        public SCightLinesMainForm(Autodesk.Revit.DB.Document doc)
        {
            this.InitializeComponent();
            this.doc = doc;
            txtEyeHeight.Text = "1220";
            txtGoing.Text = "900";
            txtInc.Text = "15";
            txtMinC.Text = "60";
            txtRiser.Text = "180";
            txtRows.Text = "20";
            txtX.Text = "12000";
            txtY.Text = "1000";
            this.info = new SCightOutputForm("Update first");
            this.info.Hide();
            this.sightLines = new SCightLines(
                doc,
                this.GetDub(txtEyeHeight, 1220),
                this.GetDub(txtGoing, 900),
                this.GetDub(txtInc, 15),
                this.GetDub(txtMinC, 60),
                this.GetDub(txtRiser, 180), 
                this.GetDub(txtRows, 20),
                this.GetDub(txtX, 12000),
                this.GetDub(txtY, 1000));
            this.Show();
            this.Focus();
        }

        /// <summary>
        /// Get a double value from a textbox
        /// </summary>
        /// <param name="t">
        /// Name of the the TextBox
        /// </param>
        /// <param name="fallback">
        /// Fallback value.
        /// i.e. value to apply if text cannot be parsed
        /// </param>
        private double GetDub(TextBox t, double fallback)
        {
            double d;
            try {
                d = double.Parse(t.Text);
            } catch (Exception) {
                Autodesk.Revit.UI.TaskDialog.Show(
                    "Error", t.Text + ": is not a valid number, SCightlines will use a fallback value");
                d = fallback;
            }
            return d;
        }

        private void ButtonTest_Click(object sender, EventArgs e)
        {
            this.sightLines.Update(
                this.GetDub(this.txtEyeHeight, 1220),
                this.GetDub(this.txtGoing, 900),
                this.GetDub(this.txtInc, 15),
                this.GetDub(this.txtMinC, 60),
                this.GetDub(this.txtRiser, 280),
                this.GetDub(this.txtRows, 10),
                this.GetDub(this.txtX, 10000),
                this.GetDub(this.txtY, 1000));
            try {
                this.info.Update(this.sightLines.InfoString);
                this.info.Show();
            } catch {
                this.info = new SCightOutputForm("Update first");
                this.info.Update(this.sightLines.InfoString);
                this.info.Show();
            }
        }

        private void ButtonCreate_Click(object sender, EventArgs e)
        {
            this.sightLines.Update(
                this.GetDub(this.txtEyeHeight, 1220),
                this.GetDub(this.txtGoing, 900),
                this.GetDub(this.txtInc, 15),
                this.GetDub(this.txtMinC, 60),
                this.GetDub(this.txtRiser, 180),
                this.GetDub(this.txtRows, 20),
                this.GetDub(this.txtX, 12000),
                this.GetDub(this.txtY, 1000));
            this.sightLines.Draw();
            this.Focus();
        }

        private void ButtonQuit_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.info.Dispose();
        }
    }
}
