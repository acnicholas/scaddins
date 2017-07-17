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

namespace SCaddins.LineOfSight
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;
    
    public partial class SCightLinesMainForm : Form
    {
        private SCightOutputForm info;
        private StadiumSection sightLines;

        public SCightLinesMainForm(StadiumSection sightlines)
        {
            if (sightlines == null) {
                throw new ArgumentNullException("sightlines");
            }
            this.InitializeComponent();
            txtEyeHeight.Text = sightlines.EyeHeight.ToString(CultureInfo.InvariantCulture);
            txtGoing.Text = sightlines.TreadSize.ToString(CultureInfo.InvariantCulture);
            txtInc.Text = sightlines.RiserIncrement.ToString(CultureInfo.InvariantCulture);
            txtMinC.Text = sightlines.MinimumCValue.ToString(CultureInfo.InvariantCulture);
            txtRiser.Text = sightlines.MinimumRiserHeight.ToString(CultureInfo.InvariantCulture);
            txtRows.Text = sightlines.NumberOfRows.ToString(CultureInfo.InvariantCulture);
            txtX.Text = sightlines.DistanceToFirstRowX.ToString(CultureInfo.InvariantCulture);
            txtY.Text = sightlines.DistanceToFirstRowY.ToString(CultureInfo.InvariantCulture);
            this.info = new SCightOutputForm("Update first");
            this.info.Hide();
            this.sightLines = sightlines;
        }

        public static double GetDub(Control textBox, double fallback)
        {
            if (textBox == null) {
                return fallback;
            }
            double d;
            if (textBox.Text == null) {
                return fallback;
            }
            try {
                d = double.Parse(textBox.Text, CultureInfo.InvariantCulture);
            } catch (FormatException) {
                Autodesk.Revit.UI.TaskDialog.Show(
                    "Error", textBox.Text + ": is not a valid number, fallback value will be used");
                d = fallback;
            } catch (OverflowException) {
                Autodesk.Revit.UI.TaskDialog.Show(
                    "Error", textBox.Text + ": is too big/small, fallback value will be used");
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
