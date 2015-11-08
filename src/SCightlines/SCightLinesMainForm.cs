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

namespace SCaddins.SCightLines
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;
    public partial class SCightLinesMainForm : Form
    {
        private SCightOutputForm info;
        private LineOfSight sightLines;

        public SCightLinesMainForm(Autodesk.Revit.DB.Document doc)
        {
            this.InitializeComponent();
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
            this.sightLines = new LineOfSight(
                doc,
                SCightLinesMainForm.GetDub(txtEyeHeight, 1220),
                SCightLinesMainForm.GetDub(txtGoing, 900),
                SCightLinesMainForm.GetDub(txtInc, 15),
                SCightLinesMainForm.GetDub(txtMinC, 60),
                SCightLinesMainForm.GetDub(txtRiser, 180), 
                SCightLinesMainForm.GetDub(txtRows, 20),
                SCightLinesMainForm.GetDub(txtX, 12000),
                SCightLinesMainForm.GetDub(txtY, 1000));
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
        private static double GetDub(TextBox t, double fallback)
        {
            double d;
            if (t.Text == null) {
                return fallback;
            }
            try {
                d = double.Parse(t.Text, CultureInfo.InvariantCulture);
            } catch (FormatException) {
                Autodesk.Revit.UI.TaskDialog.Show(
                    "Error", t.Text + ": is not a valid number, SCightlines will use a fallback value");
                d = fallback;
            } catch (OverflowException) {
                Autodesk.Revit.UI.TaskDialog.Show(
                    "Error", t.Text + ": is too big/small, SCightlines will use a fallback value");
                d = fallback;
            }
            return d;
        }

        private void ButtonTest_Click(object sender, EventArgs e)
        {
            this.sightLines.Update(
                SCightLinesMainForm.GetDub(this.txtEyeHeight, 1220),
                SCightLinesMainForm.GetDub(this.txtGoing, 900),
                SCightLinesMainForm.GetDub(this.txtInc, 15),
                SCightLinesMainForm.GetDub(this.txtMinC, 60),
                SCightLinesMainForm.GetDub(this.txtRiser, 280),
                SCightLinesMainForm.GetDub(this.txtRows, 10),
                SCightLinesMainForm.GetDub(this.txtX, 10000),
                SCightLinesMainForm.GetDub(this.txtY, 1000));
            this.info.Update(this.sightLines.InfoString);
            this.info.Show();
        }

        private void ButtonCreate_Click(object sender, EventArgs e)
        {
            this.sightLines.Update(
                SCightLinesMainForm.GetDub(this.txtEyeHeight, 1220),
                SCightLinesMainForm.GetDub(this.txtGoing, 900),
                SCightLinesMainForm.GetDub(this.txtInc, 15),
                SCightLinesMainForm.GetDub(this.txtMinC, 60),
                SCightLinesMainForm.GetDub(this.txtRiser, 180),
                SCightLinesMainForm.GetDub(this.txtRows, 20),
                SCightLinesMainForm.GetDub(this.txtX, 12000),
                SCightLinesMainForm.GetDub(this.txtY, 1000));
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
