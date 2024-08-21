using Autodesk.Revit.DB;
using Caliburn.Micro;
using SCaddins.ParameterUtilities.ViewModels;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace SCaddins.ParameterUtilities.Views
{
    public partial class SCincrementView
    {
        public SCincrementView()
        {
            InitializeComponent();
            var test = Common.ViewModels.SettingsViewModel.IncrementViewModel;
            ViewModelBinder.Bind(test, this, null);
        }

        private void UserControl_Initialized(object sender, System.EventArgs e)
        {

        }
    }
}