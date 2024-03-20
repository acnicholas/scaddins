// (C) Copyright 2012-2023 by Andrew Nicholas
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
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
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
            view = null;
            this.doc = doc;
            this.eyeHeight = eyeHeight;
            this.treadSize = treadSize;
            this.riserIncrement = riserIncrement;
            this.minimumCValue = minimumCValue;
            this.minimumRiserHeight = minimumRiserHeight;
            this.numberOfRows = numberOfRows;
            this.distanceToFirstRowX = distanceToFirstRowX;
            this.distanceToFirstRowY = distanceToFirstRowY;
            rows = new StadiumSeatingTread[100];
            for (int i = 0; i < 100; i++)
            {
                rows[i] = new StadiumSeatingTread();
            }
            UpdateRows();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //// Properties
        #region

        public double DistanceToFirstRowX
        {
            get => distanceToFirstRowX;

            set
            {
                if (Math.Abs(distanceToFirstRowX - value) > tolerance)
                {
                    try
                    {
                        distanceToFirstRowX = value;
                        if (distanceToFirstRowX > 1)
                        {
                            UpdateRows();
                        }
                    }
                    catch
                    {
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
                if (Math.Abs(distanceToFirstRowY - value) > tolerance)
                {
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
                if (!(Math.Abs(eyeHeight - value) > tolerance))
                {
                    return;
                }
                eyeHeight = value;
                UpdateRows();
            }
        }

        public string InfoString
        {
            get => infoString;

            private set
            {
                infoString = value;
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
                new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .FirstOrDefault(x => ViewFamily.Drafting == x.ViewFamily);
            if (viewFamilyType == null)
            {
                return null;
            }
            var viewDrafting = ViewDrafting.Create(doc, viewFamilyType.Id);
            viewDrafting.Name = newViewName;
            return viewDrafting;
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString")]
        [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags", Justification = "Revit API...")]
        public void Draw()
        {
            using (var t = new Transaction(doc, "Create sight line view"))
            {
                t.Start();

                string times = DateTime.Now.Ticks.ToString(CultureInfo.CurrentCulture);

                view = CreateLineOfSightDraftingView(
                    "LOS-X" + distanceToFirstRowX + "-Y" + distanceToFirstRowY + "-T" +
                    TreadSize + "-MinN" + MinimumRiserHeight + "-Inc" + RiserIncrement +
                    "-Eye" + EyeHeight + "-MinC" + MinimumCValue + "_" + times);

                view.Scale = 50;
                int i;

                for (i = 0; i < NumberOfRows; i++)
                {
                    if (i == 0)
                    {
                        DrawLine(0, 0, distanceToFirstRowX, 0, "Thin Lines");
                        DrawText(
                            distanceToFirstRowX / 2,
                            0,
                            distanceToFirstRowX.ToString(CultureInfo.InvariantCulture),
                            TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_TOP);
                        DrawText(
                            distanceToFirstRowX,
                            distanceToFirstRowY / 2,
                            distanceToFirstRowY.ToString(CultureInfo.InvariantCulture),
                            TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_BOTTOM);
                    }

                    // Draw the sight line
                    DrawLine(0, 0, rows[i].EyeToFocusX, rows[i].HeightToFocus, "Dash - 0.1 pen");

                    // Draw the head
                    DrawCircle(rows[i].EyeToFocusX, rows[i].HeightToFocus, "Medium Lines");

                    // Draw the body
                    DrawLine(
                        rows[i].EyeToFocusX,
                        rows[i].HeightToFocus,
                        rows[i].EyeToFocusX,
                        rows[i].HeightToFocus - rows[i].EyeHeight,
                        "Thin Lines");

                    // Draw the riser
                    DrawRiser(i);

                    // Draw the going
                    DrawGoing(i);

                    // Draw the c-value text
                    DrawText(
                        rows[i].EyeToFocusX + 125,
                        rows[i].HeightToFocus,
                        "c:" + Math.Round(rows[i].CValue, 2).ToString(CultureInfo.InvariantCulture),
                        TextAlignFlags.TEF_ALIGN_LEFT);

                    //// Draw the going text (treadSize)
                    DrawText(
                        rows[i].EyeToFocusX - (rows[i].Going / 2),
                        rows[i].HeightToFocus - rows[i].EyeHeight,
                        "R" + (i + 1).ToString(CultureInfo.InvariantCulture) + " : " + TreadSize.ToString(CultureInfo.InvariantCulture),
                        TextAlignFlags.TEF_ALIGN_CENTER | TextAlignFlags.TEF_ALIGN_TOP);

                    //// Draw the riser text)
                    if (i > 0)
                    {
                        DrawText(
                            rows[i].EyeToFocusX - TreadSize,
                            rows[i].HeightToFocus - rows[i].EyeHeight - (rows[i].RiserHeight / 2),
                            rows[i].RiserHeight.ToString(CultureInfo.InvariantCulture),
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
            return ((rows[i].EyeToFocusX * (rows[i].HeightToFocus + nextn)) / (rows[i].EyeToFocusX + TreadSize)) - rows[i].HeightToFocus;
        }

        private void DrawCircle(double x1, double y1, string s)
        {
            var app = doc.Application;
            var point1 = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x1), MiscUtilities.MillimetersToFeet(y1), MiscUtilities.MillimetersToFeet(0));
            using (Arc arc = Arc.Create(
                          point1,
                          MiscUtilities.MillimetersToFeet(125),
                          0,
                          360,
                          app.Create.NewXYZ(1, 0, 0),
                          app.Create.NewXYZ(0, 1, 0)))
            {
                var detailCurve = doc.Create.NewDetailCurve(view, arc) as DetailArc;
                SetLineTypeByName(detailCurve, s);
            }
        }

        private void DrawGoing(int i)
        {
            string igo = "Wide Lines";
            if (i == 0)
            {
                igo = "Thin Lines";
            }
            DrawLine(
                rows[i].EyeToFocusX,
                rows[i].HeightToFocus - rows[i].EyeHeight,
                rows[i].EyeToFocusX - rows[i].Going,
                rows[i].HeightToFocus - rows[i].EyeHeight,
                igo);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, string s)
        {
            var app = doc.Application;
            var point1 = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x1), MiscUtilities.MillimetersToFeet(y1), MiscUtilities.MillimetersToFeet(0.0));
            var point2 = app.Create.NewXYZ(MiscUtilities.MillimetersToFeet(x2), MiscUtilities.MillimetersToFeet(y2), MiscUtilities.MillimetersToFeet(0.0));
            try
            {
                using (Line line = Line.CreateBound(point1, point2))
                {
                    var detailCurve = doc.Create.NewDetailCurve(view, line) as DetailLine;
                    SetLineTypeByName(detailCurve, s);
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
                DrawLine(
                    rows[i].EyeToFocusX,
                    rows[i].HeightToFocus - rows[i].EyeHeight,
                    rows[i].EyeToFocusX,
                    rows[i].HeightToFocus - (rows[i].EyeHeight + rows[i].RiserHeight),
                    "Thin Lines");
            }
            else
            {
                DrawLine(
                    rows[i].EyeToFocusX - rows[i].Going,
                    rows[i].HeightToFocus - rows[i].EyeHeight,
                    rows[i].EyeToFocusX - rows[i].Going,
                    rows[i].HeightToFocus - (rows[i].EyeHeight + rows[i].RiserHeight),
                    "Wide Lines");
            }
        }

        private void DrawText(double x, double y, string s, TextAlignFlags f)
        {
            Application app = doc.Application;
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
                TextNote.Create(doc, view.Id, origin, s, tno).Dispose();
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
                Element style = doc.GetElement(styleId);
                if (style.Name.Equals(styleName, StringComparison.CurrentCulture))
                {
                    detailLine.LineStyle = style;
                }
            }
        }

        private void UpdateInfoString()
        {
            int i;

            var s = string.Empty;
            s += "row:\triser:\tdist:\telev:\tc-value:\r\n";

            for (i = 0; i < NumberOfRows; i++)
            {
                string c = i > 0 ? Math.Round(rows[i - 1].CValue, 2).ToString(CultureInfo.InvariantCulture) : "NA";
                string r = i > 0 ? Math.Round(rows[i].RiserHeight, 2).ToString(CultureInfo.InvariantCulture) : "NA";
                s += i + 1 + "\t" + r + "\t"
                + Math.Round(rows[i].EyeToFocusX, 2) + "\t"
                + Math.Round(rows[i].HeightToFocus - EyeHeight, 2) + "\t"
                + c + "\r\n";
            }

            InfoString = s;
        }

        private void UpdateRows()
        {
            for (int i = 0; i < NumberOfRows; i++)
            {
                rows[i].Initialize(
                    distanceToFirstRowX + (i * TreadSize),
                    distanceToFirstRowY,
                    distanceToFirstRowY + EyeHeight,
                    treadSize,
                    eyeHeight);
                if (i > 0)
                {
                    rows[i].RiserHeight = MinimumRiserHeight;
                    rows[i].HeightToFocus = rows[i - 1].HeightToFocus + MinimumRiserHeight;
                    while (GetCValue(i - 1, rows[i].RiserHeight) < minimumCValue)
                    {
                        rows[i].RiserHeight += riserIncrement;
                        rows[i].HeightToFocus += riserIncrement;
                    }
                    rows[i - 1].CValue = GetCValue(i - 1, rows[i].RiserHeight);
                }
            }
            UpdateInfoString();
        }
    }
}
