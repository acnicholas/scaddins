using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace SCaddins.RoomConvertor.ViewModels
{

    public class RoomConvertorViewModel : Screen
    {
        private RoomConversionManager manager;
        private ObservableCollection<RoomConversionCandidate> rooms;

        public RoomConvertorViewModel(RoomConversionManager manager)
        {
            this.manager = manager;
            this.rooms = new ObservableCollection<RoomConversionCandidate>(manager.Candidates);
        }

        public bool SheetCreationMode
        {
            get; set;
        }

        public bool MassCreationMode
        {
            get; set;
        }

        public ObservableCollection<RoomConversionCandidate> Rooms
        {
            get { return rooms; }
        }

        public void AddFilter()
        {
            dynamic settings = new System.Dynamic.ExpandoObject();
            settings.Height = 480;
            settings.Width = 768;
            settings.Title = "Filter Rooms";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            var bs = SCaddinsApp.Bootstrapper;
            var windowManager = SCaddinsApp.WindowManager;
            var vm = new ViewModels.RoomFilterViewModel();
            windowManager.ShowDialog(vm, null, settings);
        }

    }
}

