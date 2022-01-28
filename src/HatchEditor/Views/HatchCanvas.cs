// (C) Copyright 2019-2020 by Andrew Nicholas
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

namespace SCaddins.HatchEditor.Views
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    public class HatchCanvas : FrameworkElement
    {
        public static readonly DependencyProperty ActiveHatchProperty = DependencyProperty.Register(
            "ActiveHatch",
            typeof(Hatch),
            typeof(HatchCanvas),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPatternChange, CoercedCallback));

        private double canvasScale;
        private VisualCollection children;
        private double height;
        private double width;

        public HatchCanvas()
        {
            ActiveHatch = new Hatch();
            children = new VisualCollection(this);
            canvasScale = 1;
        }

        public Hatch ActiveHatch
        {
            get { return (Hatch)GetValue(ActiveHatchProperty); }
            set { SetValue(ActiveHatchProperty, value); }
        }

        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }

        public static void OnPatternChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Hatch v = (Hatch)e.NewValue;
            HatchCanvas h = (HatchCanvas)d;
            h.Update(v, 1);
        }

        public static object CoercedCallback(DependencyObject d, object baseValue)
        {
            if (baseValue != null && (d.GetValue(ActiveHatchProperty) == baseValue))
            {
                d.SetCurrentValue(ActiveHatchProperty, null);
                d.SetCurrentValue(ActiveHatchProperty, baseValue);
            }
            return baseValue;
        }

        public void Update(Hatch hatch, double scale)
        {
            canvasScale = scale;
            Update(hatch);
            InvalidateVisual();
        }

        public void Update(Hatch hatch)
        {
            if (hatch == null)
            {
                return;
            }
            if (children == null || string.IsNullOrEmpty(hatch.Definition))
            {
                return;
            }
            children.Clear();
            children.Add(CreateDrawingVisualHatch(hatch));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            width = finalSize.Width;
            height = finalSize.Height;
            Update(ActiveHatch);
            return base.ArrangeOverride(finalSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return children[index];
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            canvasScale = canvasScale + ((double)e.Delta / 1800);
            Update(ActiveHatch);
            base.OnMouseWheel(e);
        }

        private static double GetDashedLineLength(Autodesk.Revit.DB.FillGrid line, int repetitions, double scale)
        {
            if (line.GetSegments().Count == 0)
            {
                return 0;
            }
            double result = 0;
            foreach (var dash in line.GetSegments())
            {
                result += Math.Abs(dash).ToMM(scale);
            }
            return result * repetitions;
        }

        private DrawingVisual CreateDrawingVisualHatch(Hatch hatch)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            if (hatch == null)
            {
                return drawingVisual;
            }

            DrawingContext drawingContext = drawingVisual.RenderOpen();

            double dx = 0;
            double dy = 0;

            var scale = hatch.IsDrafting ? 10 : 1;

            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, width, height)));
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));
            drawingContext.PushTransform(new TranslateTransform(width / 2, height / 2));
            drawingContext.DrawEllipse(null, new Pen(Brushes.LightBlue, 1), new Point(0, 0), 10, 10);
            drawingContext.DrawLine(new Pen(Brushes.LightBlue, 1), new Point(-20, 0), new Point(20, 0));
            drawingContext.DrawLine(new Pen(Brushes.LightBlue, 1), new Point(0, -20), new Point(0, 20));
            drawingContext.PushTransform(new ScaleTransform(canvasScale, canvasScale));

            double maxLength = width > height ? width / canvasScale * 2 : height / canvasScale * 2;

            foreach (var fillGrid in hatch.HatchPattern.GetFillGrids())
            {
                double scaledSequenceLength = GetDashedLineLength(fillGrid, 1, scale);
                double initialShiftOffset = scaledSequenceLength > 0 ? (int)Math.Floor(maxLength / scaledSequenceLength) * scaledSequenceLength : maxLength;
                if (scaledSequenceLength > maxLength / 4)
                {
                    initialShiftOffset = scaledSequenceLength * 16;
                }

                var segsInMM = new List<double>();
                foreach (var s in fillGrid.GetSegments())
                {
                    segsInMM.Add(s.ToMM(scale));
                }

                if (Math.Abs(fillGrid.Offset) < 0.001)
                {
                    continue;
                }

                int b = 0;

                var pen = new Pen(Brushes.Black, 1);
                pen.DashStyle = new DashStyle(segsInMM, initialShiftOffset);

                double a = fillGrid.Angle.ToDeg();
                drawingContext.PushTransform(new RotateTransform(-a, fillGrid.Origin.U.ToMM(scale), -fillGrid.Origin.V.ToMM(scale)));

                double cumulativeShift = 0;

                while (Math.Abs(dy) < maxLength * 2)
                {
                    b++;
                    if (b > 100)
                    {
                        break;
                    }
                    double x = fillGrid.Origin.U.ToMM(scale) - initialShiftOffset;
                    double y = -fillGrid.Origin.V.ToMM(scale);
                    drawingContext.PushTransform(new TranslateTransform(x + dx, y - dy));
                    drawingContext.DrawLine(pen, new Point(0, 0), new Point(initialShiftOffset * 2, 0));
                    drawingContext.Pop();
                    drawingContext.PushTransform(new TranslateTransform(x - dx, y + dy));
                    drawingContext.DrawLine(pen, new Point(0, 0), new Point(initialShiftOffset * 2, 0));
                    drawingContext.Pop();
                    dx += fillGrid.Shift.ToMM(scale);
                    cumulativeShift += fillGrid.Shift.ToMM(scale);
                    if (Math.Abs(cumulativeShift) > scaledSequenceLength)
                    {
                        dx -= scaledSequenceLength;
                        cumulativeShift = 0;
                    }
                    dy += fillGrid.Offset.ToMM(scale);
                }
                drawingContext.Pop();
                dx = 0;
                dy = 0;
            }
            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();

            drawingContext.DrawText(
                new FormattedText(
                    string.Format(System.Globalization.CultureInfo.InvariantCulture, "Scale: {0}", scale * canvasScale),
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("arial"),
                    10,
                    Brushes.LightBlue,
                    1),
                new Point(10, 10));

            drawingContext.Close();
            return drawingVisual;
        }
    }
}
