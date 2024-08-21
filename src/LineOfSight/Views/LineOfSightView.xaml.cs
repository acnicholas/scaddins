using Wpf.Ui.Appearance;

namespace SCaddins.LineOfSight.Views
{
    public partial class LineOfSightView
    {
        public LineOfSightView()
        {
            InitializeComponent();
            ApplicationThemeManager.Apply(this);
        }
    }
}