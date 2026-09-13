using Autodesk.Revit.DB;
using Caliburn.Micro;
using SCaddins.ColouredTabs.ViewModels;
using SCaddins.ParameterUtilities.ViewModels;
using System.Windows.Controls;

namespace SCaddins.ColouredTabs.Views
{
    public partial class ColouredTabsView
    {
        public ColouredTabsView()
        {
            InitializeComponent();
            var vm = new ColouredTabs.ViewModels.ColouredTabsViewModel();
            ViewModelBinder.Bind(vm, this, null);
        }

        private void UserControl_Initialized(object sender, System.EventArgs e)
        {

        }
    }
}