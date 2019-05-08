namespace SCaddins.HatchEditor.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    public class HatchCanvas : FrameworkElement
    {
        private double width;
        private double height;
        private double canvasScale;

        public static readonly DependencyProperty ActiveHatchProperty = DependencyProperty.Register(
            "ActiveHatch",
            typeof(Hatch),
            typeof(HatchCanvas),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPatternChange)));

        private VisualCollection children;

        public HatchCanvas()
        {
            ActiveHatch = new Hatch();
            children = new VisualCollection(this);
            canvasScale = 1;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            width = finalSize.Width;
            height = finalSize.Height;
            this.Update(this.ActiveHatch);
            return base.ArrangeOverride(finalSize);
        }

        public Hatch ActiveHatch
        {
            get { return (Hatch)this.GetValue(ActiveHatchProperty); }
            set { this.SetValue(ActiveHatchProperty, value); }
        }

        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }

        public static void OnPatternChange(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            Hatch v = (Hatch)e.NewValue;         
            HatchCanvas h = (HatchCanvas)d;
            h.Update(v, 1);
        }

        public void Update(Hatch hatch, double scale)
        {
            canvasScale = 1;
            Update(hatch);
        }

        public void Update(Hatch hatch)
        {
            if (hatch == null) return;
            if (children == null || string.IsNullOrEmpty(hatch.Definition))
            {
                return;
            }
            children.Clear();
            children.Add(CreateDrawingVisualHatch(hatch));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            canvasScale = canvasScale +  ((double)e.Delta / 1800);
            this.Update(this.ActiveHatch);
            base.OnMouseWheel(e);
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            return children[index];
        }


        private double GetDashedLineLength(Autodesk.Revit.DB.FillGrid line, int repetitions, double scale)
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
            if (hatch == null) {
                return drawingVisual;
            }

            DrawingContext drawingContext = drawingVisual.RenderOpen();

            double dx = 0;
            double dy = 0;

            var scale = hatch.IsDrafting ? 10 : 1;

            drawingContext.PushClip(new RectangleGeometry(new Rect(0,0, width, height)));
            drawingContext.DrawRectangle(Brushes.White, null, (new Rect(0, 0, width, height)));
            drawingContext.PushTransform(new TranslateTransform(width / 2, height / 2));          
            drawingContext.DrawEllipse(Brushes.Azure, new Pen(), new Point(0,0), 100,100);
            drawingContext.PushTransform(new ScaleTransform(canvasScale, canvasScale));

            double maxLength = width > height ? width / canvasScale *2: height / canvasScale * 2 ;

            foreach (var fillGrid in hatch.HatchPattern.GetFillGrids())
            {
                double scaledSequenceLength = GetDashedLineLength(fillGrid, 1, scale);
                double initialShiftOffset = scaledSequenceLength > 0 ? (int)(Math.Floor(maxLength / scaledSequenceLength)) * scaledSequenceLength: maxLength;
                if (scaledSequenceLength > maxLength / 4) {
                    initialShiftOffset = scaledSequenceLength * 16;
                }

                var segsInMM = new List<double>();
                foreach (var s in fillGrid.GetSegments()) {
                    segsInMM.Add(s.ToMM(scale));
                }

                if (fillGrid.Offset == 0) continue;

                int b = 0;

                var pen = new Pen(Brushes.Black, 1);
                pen.DashStyle = new DashStyle(segsInMM, initialShiftOffset);

                double a = fillGrid.Angle.ToDeg();
                drawingContext.PushTransform(new RotateTransform(-a, fillGrid.Origin.U.ToMM(scale) , -fillGrid.Origin.V.ToMM(scale)));

                double cumulativeShift = 0;

                while (Math.Abs(dy) < maxLength * 2)
                {
                    b++;
                    if (b > 100) break;
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
            drawingContext.Close();
            return drawingVisual;
        }
    }
}
