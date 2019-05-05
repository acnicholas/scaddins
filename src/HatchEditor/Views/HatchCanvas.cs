namespace SCaddins.HatchEditor.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    public class HatchCanvas : FrameworkElement
    {
        private double width;
        private double height;

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
            this.SizeChanged += HatchCanvas_SizeChanged;
        }

        private void HatchCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;
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
            h.Update(v);
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
            if (hatch == null)
            {
                return drawingVisual;
            }

            DrawingContext drawingContext = drawingVisual.RenderOpen();

            var p = hatch.HatchPattern;
            //p.ExpandDots();

            double dx = 0;
            double dy = 0;

            var scale = hatch.IsDrafting ? 10 : 1;

            drawingContext.PushClip(new RectangleGeometry(new Rect(0,0, width, height)));
            drawingContext.PushTransform(new TranslateTransform(width / 2, height / 2));
            drawingContext.DrawEllipse(Brushes.Azure, new Pen(), new Point(0,0), 100,100);
            drawingContext.PushTransform(new ScaleTransform(hatch.Scale, hatch.Scale));

            double maxLength = Math.Sqrt(width * width + height * height);

            foreach (var l in p.GetFillGrids())
            {
                double dl = GetDashedLineLength(l, 1, scale);
                double sx = dl > 0 ? (int)(Math.Floor(maxLength / dl)) * dl: maxLength;

                var segsInMM = new List<double>();
                foreach (var s in l.GetSegments()) {
                    segsInMM.Add(s.ToMM(scale));
                }

                if (l.Offset == 0) continue;

                int b = 0;

                var pen = new Pen(Brushes.Black, 1);
                pen.DashStyle = new DashStyle(segsInMM, sx * scale);

                double a = l.Angle.ToDeg();
                //var uv = l.GetSegmentDirection();
                //double a = Math.Atan2(uv.V, uv.U).ToDeg();
                //a = hatch.IsModel ? a : -a;
                drawingContext.PushTransform(new RotateTransform(-a, l.Origin.U.ToMM(scale) , -l.Origin.V.ToMM(scale)));

                while (dx < width && dy < height)
                {
                    b++;
                    if (b > 300) break;
                    double x = l.Origin.U.ToMM(scale) - sx;
                    double y = -l.Origin.V.ToMM(scale);
                    //drawingContext.PushTransform(new TranslateTransform(dx - sx, -dy));
                    drawingContext.PushTransform(new TranslateTransform(x + dx, y - dy));
                    drawingContext.DrawLine(pen, new Point(0, 0), new Point(maxLength * 2, 0));
                    drawingContext.Pop();
                    //drawingContext.PushTransform(new TranslateTransform(-dx - sx, dy));
                    drawingContext.PushTransform(new TranslateTransform(x - dx, y + dy));
                    drawingContext.DrawLine(pen, new Point(0, 0), new Point(maxLength * 2, 0));
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
            //drawingContext.Pop();

            drawingContext.Close();
            return drawingVisual;
        }
    }
}
