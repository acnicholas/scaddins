using Autodesk.Revit.DB;
using Caliburn.Micro;
using SCaddins.ParameterUtilities.ViewModels;
using System.Windows.Controls;

namespace SCaddins.ParameterUtilities.Views
{
    public partial class SCincrementView
    {
        public SCincrementView()
        {
            InitializeComponent();
            var vm = Common.ViewModels.SettingsViewModel.IncrementViewModel;
            ViewModelBinder.Bind(vm, this, null);
        }

        private void UserControl_Initialized(object sender, System.EventArgs e)
        {

        }
    }
}