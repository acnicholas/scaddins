// (C) Copyright 2012-2015 by Andrew Nicholas
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
    using System.Linq;
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.DB;
    using SCaddins.Common;

    public class LineOfSight
    {
        private Document doc;
        private double treadSize;
        private double eyeHeight;
        private double distanceToFirstRowX;
        private double distanceToFirstRowY;
        private int numberOfRows;
        private double minimumRiserHeight;
        private double minimumCValue;
        private double riserIncrement;
        private View view;
        private SCightLinesRow[] rows;
        private string infoString;

        /// <summary>
        /// A class to create line of sight drafting views in Revit
        /// </summary>
        /// <param name="doc">The Revit Document.</param>
        /// <param name="eyeHeight">The eye height of the seated person.</param>
        /// <param name="treadSize">The tread size of each seating plat.</param>
        /// <param name="riserIncrement">The max increment in each r</param>
        /// <param name="minimumCValue">The minimum C value.</param>
        /// <param name="minimumRiserHeight">The minimum riser height of each seating plat.</param>
        /// <param name="numberOfRows">The number of row in the stand</param>
        /// <param name="xDistanceToFirstRow"></param>
        /// <param name="yDistanceToFirstRow"></param>
        public LineOfSight(
            Document doc,
            double eyeHeight,
            double treadSize,
            double riserIncrement,
            double minimumCValue,
            double minimumRiserHeight,
            double numberOfRows,
            double distanceToFirstRowX,
            double distanceToFirstRowY)
        {
            this.doc = doc;
            numberOfRows = 10;
            this.rows = new SCightLinesRow[100];
            for (int i = 0; i < 100; i++) {
                this.rows[i] = new SCightLinesRow();
            }
            this.Update(
                eyeHeight,
                treadSize,
                riserIncrement,
                minimumCValue,
                minimumRiserHeight,
                numberOfRows,
                distanceToFirstRowX,
                distanceToFirstRowY);
        }

        public string InfoString {
            get { return this.infoString; }
        }

        public void Update(
            double eyeHeight,
            double treadSize,
            double riserIncrement,
            double minimumCValue,
            double minimumRiserHeight,
            double numberOfRows,
            double distanceToFirstRowX,
            double distanceToFirstRowY)
        {
            this.eyeHeight = eyeHeight;
            this.treadSize = treadSize;
            this.riserIncrement = riserIncrement;
            this.minimumCValue = minimumCValue;
            this.minimumRiserHeight = minimumRiserHeight;
            this.numberOfRows = Convert.ToInt32(numberOfRows);
            this.distanceToFirstRowX = distanceToFirstRowX;
            this.distanceToFirstRowY = distanceToFirstRowY;
            this.UpdateRows();
            this.infoString = this.UpdateInfoString();
        }

        public void Draw()
        {
            
            Transaction t = new Transaction(doc, "Create sight line view");
            t.Start(); 
            
            string times = System.DateTime.Now.ToString();

            this.view = this.CreateLineOfSightDraftingView(
                "LOS-X" + this.distanceToFirstRowX + "-Y" + this.distanceToFirstRowY + "-T" +
                this.treadSize + "-MinN" + this.minimumRiserHeight + "-Inc" + this.riserIncrement +
                "-Eye" + this.eyeHeight + "-MinC" + this.minimumCValue + "_" + times);

            this.view.Scale = 50;
            int i;

            for (i = 0; i < this.numberOfRows; i++) {
                if (i == 0) {
                    this.DrawLine(0, 0, this.distanceToFirstRowX, 0, "Thin Lines");
                    this.DrawText(
                        this.distanceToFirstRowX / 2,
                        0,
                        1,
                        0,
                        this.distanceToFirstRowX.ToString(CultureInfo.InvariantCulture),
                        TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_TOP);
                    this.DrawText(
                        this.distanceToFirstRowX,
                        this.distanceToFirstRowY / 2,
                        0,
                        1,
                        this.distanceToFirstRowY.ToString(CultureInfo.InvariantCulture),
                        TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_BOTTOM);
                }

                // Draw the sight line
                this.DrawLine(0, 0, this.rows[i].EyeToFocusX, this.rows[i].HeightToFocus, "Dash - 0.1 pen");

                // Draw the head
                this.DrawCircle(this.rows[i].EyeToFocusX, this.rows[i].HeightToFocus, "Medium Lines");

                // Draw the body
                this.DrawLine(
                    this.rows[i].EyeToFocusX,
                    this.rows[i].HeightToFocus,
                    this.rows[i].EyeToFocusX,
                    this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                    "Thin Lines");

                // Draw the riser
                this.DrawRiser(i);

                // Draw the going
                this.DrawGoing(i);

                // Draw the c-value text
                this.DrawText(
                    this.rows[i].EyeToFocusX + 125,
                    this.rows[i].HeightToFocus,
                    1,
                    0,
                    "c:" + Math.Round(this.rows[i].CValue, 2).ToString(CultureInfo.InvariantCulture),
                    TextAlignFlags.TEF_ALIGN_LEFT);

                // Draw the going text (treadSize)
                this.DrawText(
                    this.rows[i].EyeToFocusX - (this.rows[i].Going / 2),
                    this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                    1,
                    0,
                    "R" + (i + 1).ToString(CultureInfo.InvariantCulture) + " : " + this.treadSize.ToString(CultureInfo.InvariantCulture),
                    TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_TOP);

                // Draw the riser text)
                if (i > 0) {
                    this.DrawText(
                        this.rows[i].EyeToFocusX - this.treadSize,
                        this.rows[i].HeightToFocus - this.rows[i].EyeHeight - (this.rows[i].RiserHeight / 2),
                        0,
                        1,
                        this.rows[i].RiserHeight.ToString(CultureInfo.InvariantCulture),
                        TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_BOTTOM);
                }
            }    
            t.Commit();
        }

        private ViewDrafting CreateLineOfSightDraftingView(string newViewName)
        {
            ViewDrafting view = null;
            ViewFamilyType viewFamilyType =
                new FilteredElementCollector(this.doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .FirstOrDefault<ViewFamilyType>(x => ViewFamily.Drafting == x.ViewFamily);
            view = ViewDrafting.Create(this.doc, viewFamilyType.Id);
            view.ViewName = newViewName;
            return view;
        }

        private void UpdateRows()
        {
            for (int i = 0; i < this.numberOfRows; i++) {
                this.rows[i].Initialize(
                    this.distanceToFirstRowX + (i * this.treadSize),
                    this.distanceToFirstRowY, 
                    this.distanceToFirstRowY + this.eyeHeight,
                    this.treadSize,
                    this.eyeHeight);
                if (i > 0) {
                    this.rows[i].RiserHeight = this.minimumRiserHeight;
                    this.rows[i].HeightToFocus = this.rows[i - 1].HeightToFocus + this.minimumRiserHeight;
                    while (this.GetCValue(i - 1, this.rows[i].RiserHeight) < this.minimumCValue) {
                        this.rows[i].RiserHeight += this.riserIncrement;
                        this.rows[i].HeightToFocus += this.riserIncrement;
                    }
                    this.rows[i - 1].CValue = this.GetCValue(i - 1, this.rows[i].RiserHeight);
                }
            }
        }

        private void DrawRiser(int i)
        {
            if (i == 0) {
                this.DrawLine(
                    this.rows[i].EyeToFocusX,
                    this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                    this.rows[i].EyeToFocusX,
                    this.rows[i].HeightToFocus - (this.rows[i].EyeHeight + this.rows[i].RiserHeight),
                    "Thin Lines");
            } else {
                this.DrawLine(
                    this.rows[i].EyeToFocusX - this.rows[i].Going,
                    this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                    this.rows[i].EyeToFocusX - this.rows[i].Going,
                    this.rows[i].HeightToFocus - (this.rows[i].EyeHeight + this.rows[i].RiserHeight),
                    "Wide Lines");
            }
        }

        private void DrawGoing(int i)
        {
            string igo = "Wide Lines";
            if (i == 0) {
                igo = "Thin Lines";
            }
            this.DrawLine(
                this.rows[i].EyeToFocusX,
                this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                this.rows[i].EyeToFocusX - this.rows[i].Going,
                this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                igo);
        }

        private void DrawText(double x, double y, double vx, double vy, string s, TextAlignFlags f)
        {
            Application app = this.doc.Application;
            XYZ origin = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x), MiscUtilities.MillimetersToFeet(y), 0);
            XYZ normal_base = app.Create.NewXYZ(vx, vy, 0);
            XYZ normal_up = app.Create.NewXYZ(0, 1, 0);
            #if REVIT2016 || REVIT2017
            TextNoteOptions tno = new TextNoteOptions();
            tno.TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
            if (f.HasFlag(TextAlignFlags.TEF_ALIGN_CENTER)) {
                tno.HorizontalAlignment = HorizontalTextAlignment.Center;
            }
            if (f.HasFlag(TextAlignFlags.TEF_ALIGN_LEFT)) {
                tno.HorizontalAlignment = HorizontalTextAlignment.Left;
            }  
            TextNote.Create(this.doc, this.view.Id, origin, s, tno);
            #else
            this.doc.Create.NewTextNote(this.view, origin, normal_base, normal_up, MiscUtilities.MillimetersToFeet(10), f, s);
            #endif
        }

        private string UpdateInfoString()
        {
            string s;
            int i;

            s = string.Empty;
            s += "Number of Rows in Section            =\t" + this.numberOfRows + "\r\n";
            s += "Distance to first spectator          =\t" + this.distanceToFirstRowX + "\r\n";
            s += "Minimum sight line clearance         =\t" + this.minimumCValue + "\r\n";
            s += "Eye level above tread                =\t" + this.eyeHeight + "\r\n";
            s += "Elevation of first tread above datum =\t" + this.distanceToFirstRowY + "\r\n";
            s += "Tread size                           =\t" + this.treadSize + "\r\n";
            s += "Minimum riser height                 =\t" + this.minimumRiserHeight + "\r\n";
            s += "Minimum riser increment              =\t" + this.riserIncrement + "\r\n\r\n";
            s += "row:\triser:\tdist:\telev:\tc-value:\r\n";

            for (i = 0; i < this.numberOfRows; i++) {
                string c = i > 0 ? Math.Round(this.rows[i - 1].CValue, 2).ToString(CultureInfo.InvariantCulture) : "NA";
                string r = i > 0 ? Math.Round(this.rows[i].RiserHeight, 2).ToString(CultureInfo.InvariantCulture) : "NA";
                s += i + 1 + "\t" + r + "\t"
                + Math.Round(this.rows[i].EyeToFocusX, 2) + "\t"
                + Math.Round(this.rows[i].HeightToFocus - this.eyeHeight, 2) + "\t"
                + c + "\r\n";
            }

            return s;
        }

        private void DrawLine(double x1, double y1, double x2, double y2, string s)
        {
            Autodesk.Revit.ApplicationServices.Application app = this.doc.Application;
            const double Z = 0.0;
            XYZ point1 = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x1), MiscUtilities.MillimetersToFeet(y1), MiscUtilities.MillimetersToFeet(Z));
            XYZ point2 = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x2), MiscUtilities.MillimetersToFeet(y2), MiscUtilities.MillimetersToFeet(Z));
            try {
                Line line = Line.CreateBound(point1, point2);
                var detailCurve = this.doc.Create.NewDetailCurve(this.view, line) as DetailLine;
                this.SetLineTypeByName(detailCurve, s);
            } catch (ArgumentNullException e) {
                Console.WriteLine(e.Message);
            } catch (Autodesk.Revit.Exceptions.ArgumentsInconsistentException e) {
                Console.WriteLine(e.Message);
            } catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }
        }

        private void DrawCircle(double x1, double y1, string s)
        {
            Autodesk.Revit.ApplicationServices.Application app = this.doc.Application;
            const double Z = 0.0;
            XYZ point1 = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x1), MiscUtilities.MillimetersToFeet(y1), MiscUtilities.MillimetersToFeet(Z));
            Arc arc = Arc.Create(
                          point1,
                          MiscUtilities.MillimetersToFeet(125),
                          0,
                          360, 
                          app.Create.NewXYZ(1, 0, 0),
                          app.Create.NewXYZ(0, 1, 0));
            var detailCurve = this.doc.Create.NewDetailCurve(this.view, arc) as DetailArc;
            this.SetLineTypeByName(detailCurve, s);
        }

        private void SetLineTypeByName(DetailCurve detailLine, string styleName)
        {
            foreach (ElementId styleId in detailLine.GetLineStyleIds()) {
                Element style = this.doc.GetElement(styleId);
                if (style.Name.Equals(styleName)) {
                    detailLine.LineStyle = style;
                }
            }
        }

        private double GetCValue(int i, double nextn)
        {
            return ((this.rows[i].EyeToFocusX * (this.rows[i].HeightToFocus + nextn)) / (this.rows[i].EyeToFocusX + this.treadSize)) - this.rows[i].HeightToFocus;
        }
    }
}
