using Wpf.Ui.Appearance;

namespace SCaddins.RenameUtilities.Views
{
    /// <summary>
    /// Interaction logic for RenameUtilitiesView.xaml
    /// </summary>
    public partial class RenameUtilitiesView
    {
        public RenameUtilitiesView()
        {
            InitializeComponent();
            ApplicationThemeManager.Apply(this);
        }
    }
}