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
        private bool massCreationMode;
        private bool sheetCreationMode;
        List<RoomConversionCandidate> selectedRooms = new List<RoomConversionCandidate>();

        public RoomConvertorViewModel(RoomConversionManager manager)
        {
            this.manager = manager;
            this.rooms = new ObservableCollection<RoomConversionCandidate>(manager.Candidates);
            MassCreationMode = true;
            SheetCreationMode = false;
        }

        public bool SheetCreationMode
        {
            get
            {
                return sheetCreationMode;
            }
            set
            {
                if (value != sheetCreationMode)
                {
                    sheetCreationMode = value;
                    NotifyOfPropertyChange(() => SheetCreationMode);
                    NotifyOfPropertyChange(() => RunButtonText);
                }
            }
        }

        public bool MassCreationMode
        {
            get
            {
                return massCreationMode;
            }
            set
            {
                if (value != massCreationMode)
                {
                    massCreationMode = value;
                    NotifyOfPropertyChange(() => MassCreationMode);
                    NotifyOfPropertyChange(() => RunButtonText);
                }
            }
        }

        public ObservableCollection<RoomConversionCandidate> Rooms
        {
            get { return rooms; }
        }

        public void RowSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            selectedRooms.AddRange(obj.AddedItems.Cast<RoomConversionCandidate>());
            obj.RemovedItems.Cast<RoomConversionCandidate>().ToList().ForEach(w => selectedRooms.Remove(w));
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

        public void run()
        {
            if (MassCreationMode) {
                manager.CreateRoomMasses(selectedRooms);
            } else {
                //Sheet creation mode.
                //Get some parameters first
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 480;
                settings.Width = 480;
                settings.Title = "Sheet Creation Options";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Height;
                var bs = SCaddinsApp.Bootstrapper;
                var windowManager = SCaddinsApp.WindowManager;
                var vm = new ViewModels.RoomToSheetWizardViewModel(manager);
                windowManager.ShowDialog(vm, null, settings);

                manager.CreateViewsAndSheets(selectedRooms);
            }
        }

        public string RunButtonText
        {
            get
            {
                if (MassCreationMode) {
                    return "Create Masses";
                } else {
                    return "Create Sheets";
                }
            }
        }

    }
}

