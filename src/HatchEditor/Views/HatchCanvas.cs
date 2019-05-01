namespace SCaddins.HatchEditor.Views
{
    using System;
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
            //SCaddinsApp.WindowManager.ShowMessageBox("Pattern changed");
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

        private DrawingVisual CreateDrawingVisualHatch(Hatch hatch)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            if (hatch == null)
            {
                return drawingVisual;
            }

            DrawingContext drawingContext = drawingVisual.RenderOpen();

            var p = hatch.HatchPattern;

            double dx = 0;
            double dy = 0;

            drawingContext.PushTransform(new TranslateTransform(width / 2, height / 2));

            foreach (var l in p.GetFillGrids())
            {
                while (dx < width / 2 && dy < height)
                {        
                    var pen = new Pen(Brushes.Black, 1);
                    pen.DashStyle = new DashStyle(l.GetSegments(), 0);
                    drawingContext.PushTransform(new TranslateTransform(dx, -dy));
                    drawingContext.PushTransform(new RotateTransform(-l.Angle.ToDeg(), l.Origin.U.ToMM(), l.Origin.V.ToMM()));
                    //drawingContext.PushTransform(new TranslateTransform(width / 2, height / 2));
                    drawingContext.DrawLine(pen, new Point(l.Origin.U.ToMM(), l.Origin.V.ToMM()), new Point(l.Origin.U.ToMM() + width / 2, l.Origin.V.ToMM()));
                    //drawingContext.Pop();
                    drawingContext.Pop();
                    drawingContext.Pop();
                    dx += l.Shift.ToMM();
                    dy += l.Offset.ToMM();
                }
                dx = 0;
                dy = 0;
            }
            drawingContext.Pop();

            drawingContext.Close();
            return drawingVisual;
        }
    }
}
