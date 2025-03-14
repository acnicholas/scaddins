namespace SCaddins.SolarAnalysis.Views
{
    using Caliburn.Micro;
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for SpellCheckerOptionsView.xaml
    /// </summary>
    public partial class SolarAnalysisOptionsView : UserControl
    {
        public SolarAnalysisOptionsView()
        {
            InitializeComponent();
            var vm = Common.ViewModels.SettingsViewModel.SolarAnalysisOptonsViewModel;
            ViewModelBinder.Bind(vm, this, null);
        }
    }
}
