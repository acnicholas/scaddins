// (C) Copyright 2018-2020 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.RoomConverter.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;

    public class RoomConvertorViewModel : Screen
    {
        private RoomConversionCandidate selectedRoom;
        private RoomFilter filter;
        private RoomConversionManager manager;
        private bool massCreationMode;
        private List<RoomConversionCandidate> rooms;
        private List<RoomConversionCandidate> selectedRooms = new List<RoomConversionCandidate>();
        private bool sheetCreationMode;

        public RoomConvertorViewModel(RoomConversionManager manager)
        {
            this.manager = manager;
            rooms = new List<RoomConversionCandidate>(manager.Candidates);
            MassCreationMode = true;
            SheetCreationMode = false;
            filter = new RoomFilter();
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 480;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                  new System.Uri("pack://application:,,,/SCaddins;component/Assets/scasfar.png"));
                settings.Width = 768;
                settings.Title = "Room Converter - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                return settings;
            }
        }

        public bool MassCreationMode
        {
            get => massCreationMode;

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

        public bool RoomInformationIsAvailable => SelectedRoom != null;

        public List<RoomParameter> RoomParameters => SelectedRoom.RoomParameters;

        public ObservableCollection<RoomConversionCandidate> Rooms
        {
            get { return new ObservableCollection<RoomConversionCandidate>(rooms.Where(r => filter.PassesFilter(r.Room))); }
        }

        public string RunButtonText
        {
            get
            {
                if (MassCreationMode)
                {
                    return "Create Masses";
                }
                else
                {
                    return "Create Sheets";
                }
            }
        }

        public RoomConversionCandidate SelectedRoom
        {
            get => selectedRoom;

            set
            {
                if (value != selectedRoom)
                {
                    selectedRoom = value;
                    NotifyOfPropertyChange(() => RoomParameters);
                    NotifyOfPropertyChange(() => RoomInformationIsAvailable);
                    NotifyOfPropertyChange(() => SelectionInformation);
                }
            }
        }

        public string SelectionInformation
        {
            get
            {
                return Rooms.Count + " Rooms, " + selectedRooms.Count + " Selected";
            }
        }

        public bool SheetCreationMode
        {
            get => sheetCreationMode;

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

        public void AddFilter()
        {
            var vm = new RoomFilterViewModel(manager, filter);
            SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, RoomFilterViewModel.DefaultWindowSettings);
            NotifyOfPropertyChange(() => Rooms);
            NotifyOfPropertyChange(() => SelectionInformation);
        }

        public void PushDataToRooms()
        {
            manager.SynchronizeMassesToRooms();
            NotifyOfPropertyChange(() => Rooms);
        }

        public void RemoveFilter()
        {
            filter.Clear();
            NotifyOfPropertyChange(() => Rooms);
            NotifyOfPropertyChange(() => SelectionInformation);
        }

        public void RenameSelectedRooms()
        {
            var renameManager = new RenameUtilities.RenameManager(
              manager.Doc,
              selectedRooms.Select(s => s.Room.Id).ToList());
            var renameSheetModel = new SCaddins.RenameUtilities.ViewModels.RenameUtilitiesViewModel(renameManager);
            renameSheetModel.SelectedParameterCategory = "Rooms";
            renameSheetModel.ParameterCategoryEnabled = false;
            SCaddinsApp.WindowManager.ShowDialogAsync(renameSheetModel, null, RenameUtilities.ViewModels.RenameUtilitiesViewModel.DefaultWindowSettings);

            NotifyOfPropertyChange(() => Rooms);
        }

        public void RowSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs eventArgs)
        {
            selectedRooms.AddRange(eventArgs.AddedItems.Cast<RoomConversionCandidate>());
            eventArgs.RemovedItems.Cast<RoomConversionCandidate>().ToList().ForEach(w => selectedRooms.Remove(w));
            NotifyOfPropertyChange(() => SelectionInformation);
        }

        public void Run()
        {
            if (MassCreationMode)
            {
                manager.CreateRoomMasses(selectedRooms);
            }
            else
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 480;
                settings.Width = 480;
                settings.Title = "Sheet Creation Options";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Height;
                var vm = new RoomToSheetWizardViewModel(manager);
                var task = SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, settings);
                bool newBool = task.Result ?? false;
                if (newBool)
                {
                    manager.CreateViewsAndSheets(selectedRooms);
                }
            }
        }
    }
}