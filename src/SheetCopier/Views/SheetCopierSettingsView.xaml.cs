namespace SCaddins.SheetCopier.Views
{
    using Caliburn.Micro;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SheetCopierSettingsView : UserControl
    {
        public SheetCopierSettingsView()
        {
            InitializeComponent();
            var vm = Common.ViewModels.SettingsViewModel.SheetCopierViewModel;
            ViewModelBinder.Bind(vm, this, null);
        }
    }
}
