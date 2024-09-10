namespace SCaddins.SpellChecker.Views
{
    using Caliburn.Micro;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for SpellCheckerOptionsView.xaml
    /// </summary>
    public partial class SpellCheckerOptionsView : UserControl
    {
        public SpellCheckerOptionsView()
        {
            InitializeComponent();
            var vm = Common.ViewModels.SettingsViewModel.SpellCheckerOptionsViewModel;
            ViewModelBinder.Bind(vm, this, null);
        }
    }
}
