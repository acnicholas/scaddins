
using Caliburn.Micro;

namespace SCaddins.RoomConverter.Views
{
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
            var vm = Common.ViewModels.SettingsViewModel.RoomConverterViewModel;
            ViewModelBinder.Bind(vm, this, null);
        }
    }
}
