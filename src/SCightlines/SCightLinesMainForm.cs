//
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
//

using System;
using System.Windows.Forms;

namespace SCaddins.SCightLines
{
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
        SCightOutputForm info;
        /// <summary>
        /// Sight Line Class
        /// </summary>
        SCightLines sightLines;
        private Autodesk.Revit.DB.Document doc;

        public SCightLinesMainForm(Autodesk.Revit.DB.Document doc)
        {
            InitializeComponent();
            this.doc = doc;
            txtEyeHeight.Text = "1220";
            txtGoing.Text = "900";
            txtInc.Text = "15";
            txtMinC.Text = "60";
            txtRiser.Text = "180";
            txtRows.Text = "20";
            txtX.Text = "12000";
            txtY.Text = "1000";
            info = new SCightOutputForm("Update first");
            info.Hide();
            sightLines = new SCightLines(doc, getDub(txtEyeHeight,1220),
                getDub(txtGoing,900), getDub(txtInc,15),
                getDub(txtMinC,60), getDub(txtRiser,180), 
                getDub(txtRows,20), getDub(txtX,12000), getDub(txtY,1000));
            setTitle();
            this.Show();
            this.Focus();
        }

        private void setTitle()
        {
            this.Text = "Stadium Line Of Sight [" +
                System.Reflection.Assembly.GetExecutingAssembly().
                GetName().Version.ToString() + "]";
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
        private double getDub(TextBox t, double fallback)
        {
            double d;
            try
            {
                d = Double.Parse(t.Text);
            }
            catch (Exception)
            {
                Autodesk.Revit.UI.TaskDialog.Show(
                    "Error",t.Text + ": is not a valid number, SCightlines will use a fallback value");
                d = fallback;
            }
            return d;
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            sightLines.Update(getDub(txtEyeHeight, 1220), getDub(txtGoing, 900),
                getDub(txtInc, 15), getDub(txtMinC, 60),
                getDub(txtRiser, 280), getDub(txtRows, 10),
                getDub(txtX, 10000), getDub(txtY, 1000));
            try
            {
                info.Update(sightLines.InfoString);
                info.Show();
            }
            catch
            {
                info = new SCightOutputForm("Update first");
                info.Update(sightLines.InfoString);
                info.Show();
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            sightLines.Update(getDub(txtEyeHeight, 1220),
                getDub(txtGoing, 900), getDub(txtInc, 15),
                getDub(txtMinC, 60), getDub(txtRiser, 180),
                getDub(txtRows, 20), getDub(txtX, 12000), getDub(txtY, 1000));
            sightLines.Draw();
            this.Focus();
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            this.Dispose();
            info.Dispose();
        }

    }

}
