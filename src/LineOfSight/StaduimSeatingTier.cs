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

using System.Diagnostics.CodeAnalysis;

namespace SCaddins.LineOfSight
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.DB;
    using Common;

    public class StadiumSeatingTier : INotifyPropertyChanged
    {
        private double distanceToFirstRowX;

        private double distanceToFirstRowY;

        [SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "doc")]
        private Document doc;

        private double eyeHeight;

        private string infoString;

        private double minimumCValue;

        private double minimumRiserHeight;

        private int numberOfRows;

        private double riserIncrement;

        private StadiumSeatingTread[] rows;

        private double treadSize;

        [SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "view")]
        private View view;

        private double tolerance = 0.001;

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
        /// <param name="distanceToFirstRowX"></param>
        /// <param name="distanceToFirstRowY"></param>
        public StadiumSeatingTier(
            Document doc,
            double eyeHeight = 1200,
            double treadSize = 900,
            double riserIncrement = 15,
            double minimumCValue = 60,
            double minimumRiserHeight = 180,
            int numberOfRows = 20,
            double distanceToFirstRowX = 12000,
            double distanceToFirstRowY = 1000)
        {
            this.view = null;
            this.doc = doc;
            this.eyeHeight = eyeHeight;
            this.treadSize = treadSize;
            this.riserIncrement = riserIncrement;
            this.minimumCValue = minimumCValue;
            this.minimumRiserHeight = minimumRiserHeight;
            this.numberOfRows = numberOfRows;
            this.distanceToFirstRowX = distanceToFirstRowX;
            this.distanceToFirstRowY = distanceToFirstRowY;
            this.rows = new StadiumSeatingTread[100];
            for (int i = 0; i < 100; i++)
            {
                this.rows[i] = new StadiumSeatingTread();
            }
            this.UpdateRows();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //// Properties
        #region

        public double DistanceToFirstRowX
        {
            get => distanceToFirstRowX;

            set
            {
                if (Math.Abs(distanceToFirstRowX - value) > tolerance) {
                    try {
                        distanceToFirstRowX = value;
                        if (distanceToFirstRowX > 1) {
                            UpdateRows();
                        }
                    } catch {
                        ////FIXME
                    }
                }
            }
        }

        public double DistanceToFirstRowY
        {
            get => distanceToFirstRowY;

            set
            {
                if (Math.Abs(distanceToFirstRowY - value) > tolerance) {
                    distanceToFirstRowY = value;
                    UpdateRows();
                }
            }
        }

        public double EyeHeight
        {
            get => eyeHeight;

            set
            {
                if (!(Math.Abs(eyeHeight - value) > tolerance)) {
                    return;
                }
                eyeHeight = value;
                UpdateRows();
            }
        }

        public string InfoString
        {
            get => this.infoString;

            private set
            {
                this.infoString = value;
                NotifyPropertyChanged(nameof(InfoString));
            }
        }

        public double MinimumCValue
        {
            get => minimumCValue;

            set
            {
                minimumCValue = value;
                UpdateRows();
            }
        }

        public double MinimumRiserHeight
        {
            get => minimumRiserHeight;

            set
            {
                minimumRiserHeight = value;
                UpdateRows();
            }
        }

        public int NumberOfRows
        {
            get => numberOfRows;

            set
            {
                numberOfRows = value;
                UpdateRows();
            }
        }

        public double RiserIncrement
        {
            get => riserIncrement;

            set
            {
                riserIncrement = value;
                UpdateRows();
            }
        }

        public double TreadSize
        {
            get => treadSize;

            set
            {
                if (Math.Abs(treadSize - value) > tolerance)
                {
                    treadSize = value;
                    UpdateRows();
                }
            }
        }

        public View View => view;

        #endregion

        public ViewDrafting CreateLineOfSightDraftingView(string newViewName)
        {
            ViewFamilyType viewFamilyType =
                new FilteredElementCollector(this.doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .FirstOrDefault(x => ViewFamily.Drafting == x.ViewFamily);
            if (viewFamilyType == null) {
                return null;
            }
            var viewDrafting = ViewDrafting.Create(this.doc, viewFamilyType.Id);
            #if REVIT2019 || REVIT2020
            viewDrafting.Name = newViewName;
            #else
            view.ViewName = newViewName;
            #endif
            return viewDrafting;
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString")]
        [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
        public void Draw()
        {
            using (var t = new Transaction(doc, "Create sight line view"))
            {
                t.Start();

                string times = DateTime.Now.Ticks.ToString(CultureInfo.CurrentCulture);

                this.view = this.CreateLineOfSightDraftingView(
                    "LOS-X" + this.distanceToFirstRowX + "-Y" + this.distanceToFirstRowY + "-T" +
                    this.TreadSize + "-MinN" + this.MinimumRiserHeight + "-Inc" + this.RiserIncrement +
                    "-Eye" + this.EyeHeight + "-MinC" + this.MinimumCValue + "_" + times);

                this.view.Scale = 50;
                int i;

                for (i = 0; i < this.NumberOfRows; i++)
                {
                    if (i == 0)
                    {
                        this.DrawLine(0, 0, this.distanceToFirstRowX, 0, "Thin Lines");
                        this.DrawText(
                            this.distanceToFirstRowX / 2,
                            0,
                            this.distanceToFirstRowX.ToString(CultureInfo.InvariantCulture),
                            TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_TOP);
                        this.DrawText(
                            this.distanceToFirstRowX,
                            this.distanceToFirstRowY / 2,
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
                        "c:" + Math.Round(this.rows[i].CValue, 2).ToString(CultureInfo.InvariantCulture),
                        TextAlignFlags.TEF_ALIGN_LEFT);

                    //// Draw the going text (treadSize)
                    this.DrawText(
                        this.rows[i].EyeToFocusX - (this.rows[i].Going / 2),
                        this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                        "R" + (i + 1).ToString(CultureInfo.InvariantCulture) + " : " + this.TreadSize.ToString(CultureInfo.InvariantCulture),
                        TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_TOP);

                    //// Draw the riser text)
                    if (i > 0)
                    {
                        this.DrawText(
                            this.rows[i].EyeToFocusX - this.TreadSize,
                            this.rows[i].HeightToFocus - this.rows[i].EyeHeight - (this.rows[i].RiserHeight / 2),
                            this.rows[i].RiserHeight.ToString(CultureInfo.InvariantCulture),
                            TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_BOTTOM);
                    }
                }
                t.Commit();

                Autodesk.Revit.UI.UIDocument uidoc = new Autodesk.Revit.UI.UIDocument(doc);
                uidoc.ActiveView = view;
            }
        }

        public double GetCValue(int i, double nextn)
        {
            return ((this.rows[i].EyeToFocusX * (this.rows[i].HeightToFocus + nextn)) / (this.rows[i].EyeToFocusX + this.TreadSize)) - this.rows[i].HeightToFocus;
        }

        private void DrawCircle(double x1, double y1, string s)
        {
            var app = this.doc.Application;
            var point1 = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x1), MiscUtilities.MillimetersToFeet(y1), MiscUtilities.MillimetersToFeet(0));
            using (Arc arc = Arc.Create(
                          point1,
                          MiscUtilities.MillimetersToFeet(125),
                          0,
                          360,
                          app.Create.NewXYZ(1, 0, 0),
                          app.Create.NewXYZ(0, 1, 0)))
            {
                var detailCurve = this.doc.Create.NewDetailCurve(this.view, arc) as DetailArc;
                this.SetLineTypeByName(detailCurve, s);
            }
        }

        private void DrawGoing(int i)
        {
            string igo = "Wide Lines";
            if (i == 0)
            {
                igo = "Thin Lines";
            }
            this.DrawLine(
                this.rows[i].EyeToFocusX,
                this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                this.rows[i].EyeToFocusX - this.rows[i].Going,
                this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                igo);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, string s)
        {
            var app = this.doc.Application;
            var point1 = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x1), MiscUtilities.MillimetersToFeet(y1), MiscUtilities.MillimetersToFeet(0.0));
            var point2 = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x2), MiscUtilities.MillimetersToFeet(y2), MiscUtilities.MillimetersToFeet(0.0));
            try
            {
                using (Line line = Line.CreateBound(point1, point2))
                {
                    var detailCurve = this.doc.Create.NewDetailCurve(this.view, line) as DetailLine;
                    this.SetLineTypeByName(detailCurve, s);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentsInconsistentException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void DrawRiser(int i)
        {
            if (i == 0)
            {
                this.DrawLine(
                    this.rows[i].EyeToFocusX,
                    this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                    this.rows[i].EyeToFocusX,
                    this.rows[i].HeightToFocus - (this.rows[i].EyeHeight + this.rows[i].RiserHeight),
                    "Thin Lines");
            }
            else
            {
                this.DrawLine(
                    this.rows[i].EyeToFocusX - this.rows[i].Going,
                    this.rows[i].HeightToFocus - this.rows[i].EyeHeight,
                    this.rows[i].EyeToFocusX - this.rows[i].Going,
                    this.rows[i].HeightToFocus - (this.rows[i].EyeHeight + this.rows[i].RiserHeight),
                    "Wide Lines");
            }
        }

        private void DrawText(double x, double y, string s, TextAlignFlags f)
        {
            Application app = this.doc.Application;
            XYZ origin = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x), MiscUtilities.MillimetersToFeet(y), 0);
            using (TextNoteOptions tno = new TextNoteOptions())
            {
                tno.TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
                if (f.HasFlag(TextAlignFlags.TEF_ALIGN_CENTER))
                {
                    tno.HorizontalAlignment = HorizontalTextAlignment.Center;
                }
                if (f.HasFlag(TextAlignFlags.TEF_ALIGN_LEFT))
                {
                    tno.HorizontalAlignment = HorizontalTextAlignment.Left;
                }
                TextNote.Create(this.doc, this.view.Id, origin, s, tno).Dispose();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SetLineTypeByName(DetailCurve detailLine, string styleName)
        {
            foreach (ElementId styleId in detailLine.GetLineStyleIds())
            {
                Element style = this.doc.GetElement(styleId);
                if (style.Name.Equals(styleName, StringComparison.CurrentCulture))
                {
                    detailLine.LineStyle = style;
                }
            }
        }

        private void UpdateInfoString()
        {
            string s;
            int i;

            s = string.Empty;
            s += "row:\triser:\tdist:\telev:\tc-value:\r\n";

            for (i = 0; i < this.NumberOfRows; i++)
            {
                string c = i > 0 ? Math.Round(this.rows[i - 1].CValue, 2).ToString(CultureInfo.InvariantCulture) : "NA";
                string r = i > 0 ? Math.Round(this.rows[i].RiserHeight, 2).ToString(CultureInfo.InvariantCulture) : "NA";
                s += i + 1 + "\t" + r + "\t"
                + Math.Round(this.rows[i].EyeToFocusX, 2) + "\t"
                + Math.Round(this.rows[i].HeightToFocus - this.EyeHeight, 2) + "\t"
                + c + "\r\n";
            }

            InfoString = s;
        }

        private void UpdateRows()
        {
            for (int i = 0; i < this.NumberOfRows; i++)
            {
                this.rows[i].Initialize(
                    this.distanceToFirstRowX + (i * this.TreadSize),
                    this.distanceToFirstRowY,
                    this.distanceToFirstRowY + this.EyeHeight,
                    this.treadSize,
                    this.eyeHeight);
                if (i > 0)
                {
                    this.rows[i].RiserHeight = this.MinimumRiserHeight;
                    this.rows[i].HeightToFocus = this.rows[i - 1].HeightToFocus + this.MinimumRiserHeight;
                    while (this.GetCValue(i - 1, this.rows[i].RiserHeight) < this.minimumCValue)
                    {
                        this.rows[i].RiserHeight += this.riserIncrement;
                        this.rows[i].HeightToFocus += this.riserIncrement;
                    }
                    this.rows[i - 1].CValue = this.GetCValue(i - 1, this.rows[i].RiserHeight);
                }
            }
            this.UpdateInfoString();
        }
    }
}