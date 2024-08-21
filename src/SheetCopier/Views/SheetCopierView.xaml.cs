using Wpf.Ui.Appearance;

namespace SCaddins.SheetCopier.Views
{
    /// <summary>
    /// Interaction logic for SheetCopierView.xaml
    /// </summary>
    public partial class SheetCopierView
    {
        public SheetCopierView()
        {
            InitializeComponent();
            ApplicationThemeManager.Apply(this);
        }
    }
}