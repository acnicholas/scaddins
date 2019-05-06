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

            double maxLength = Math.Sqrt(((width / canvasScale) * (width / canvasScale)) + ((height / canvasScale) * (height / canvasScale)));

            foreach (var l in hatch.HatchPattern.GetFillGrids())
            {
                double dl = GetDashedLineLength(l, 1, scale);
                double sx = dl > 0 ? (int)(Math.Floor(maxLength / dl)) * dl: maxLength;
                if (dl > maxLength / 4) {
                    sx = dl * 24;
                }

                var segsInMM = new List<double>();
                foreach (var s in l.GetSegments()) {
                    segsInMM.Add(s.ToMM(scale));
                }

                if (l.Offset == 0) continue;

                int b = 0;

                var pen = new Pen(Brushes.Black, 1);
                pen.DashStyle = new DashStyle(segsInMM, sx * scale);

                double a = l.Angle.ToDeg();
                drawingContext.PushTransform(new RotateTransform(-a, l.Origin.U.ToMM(scale) , -l.Origin.V.ToMM(scale)));

                while (dy < maxLength)
                {
                    b++;
                    if (b > 300) break;
                    double x = l.Origin.U.ToMM(scale) - sx;
                    double y = -l.Origin.V.ToMM(scale);
                    drawingContext.PushTransform(new TranslateTransform(x + dx, y - dy));
                    drawingContext.DrawLine(pen, new Point(0, 0), new Point(sx * 2, 0));
                    drawingContext.Pop();
                    drawingContext.PushTransform(new TranslateTransform(x - dx, y + dy));
                    drawingContext.DrawLine(pen, new Point(0, 0), new Point(sx * 2, 0));
                    drawingContext.Pop();
                    dx += l.Shift.ToMM(scale);
                    dy += l.Offset.ToMM(scale);
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
