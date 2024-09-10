namespace SCaddins.ViewUtilities.Views
{
    using Caliburn.Micro;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ViewUtilitiesSettingsView.xaml
    /// </summary>
    public partial class ViewUtilitiesSettingsView : UserControl
    {
        public ViewUtilitiesSettingsView()
        {
            InitializeComponent();
            var vm = Common.ViewModels.SettingsViewModel.ViewUtilitiesViewModel;
            ViewModelBinder.Bind(vm, this, null);
        }
    }
}
