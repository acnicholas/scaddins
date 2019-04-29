namespace SCaddins.HatchEditor.Views
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public class HatchCanvas : FrameworkElement
    {
        public static readonly DependencyProperty FillPatternProperty = DependencyProperty.Register(
            "FillPattern",
            typeof(string),
            typeof(HatchCanvas),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPatternChange)));

        private VisualCollection children;

        public HatchCanvas()
        {
            FillPattern = string.Empty;
            children = new VisualCollection(this);
        }

        public string FillPattern
        {
            get { return (string)this.GetValue(FillPatternProperty); }
            set { this.SetValue(FillPatternProperty, value); }
        }

        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }

        public static void OnPatternChange(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            string v = (string)e.NewValue;
            HatchCanvas h = (HatchCanvas)d;
            h.Update(v);
        }

        public void Update(string v)
        {
            if (children == null || string.IsNullOrEmpty(v))
            {
                return;
            }
            children.Add(CreateDrawingVisualHatch(v));
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            return children[index];
        }

        private DrawingVisual CreateDrawingVisualHatch(string hatchDefinition)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Rect rect = new Rect(new System.Windows.Point(160, 100), new System.Windows.Size(320, 80));
            drawingContext.DrawRectangle(System.Windows.Media.Brushes.LightBlue, (System.Windows.Media.Pen)null, rect);
            drawingContext.Close();
            return drawingVisual;
        }
    }
}
