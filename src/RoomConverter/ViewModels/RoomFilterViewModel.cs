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
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;
    using Caliburn.Micro;

    public class RoomFilterViewModel : Screen
    {
        private string comparisonFieldOne = string.Empty;
        private string comparisonFieldTwo = string.Empty;
        private RoomFilter filter;
        private RoomConversionManager manager;
        private Autodesk.Revit.DB.Parameter[] roomParameters;

        public RoomFilterViewModel(RoomConversionManager manager, RoomFilter filter)
        {
            this.manager = manager;
            this.filter = filter;
            roomParameters = new Autodesk.Revit.DB.Parameter[3];
        }

        public static ObservableCollection<ComparisonOperator> ComparisonOperators
        {
            get
            {
                return new ObservableCollection<ComparisonOperator>(Enum.GetValues(typeof(ComparisonOperator)).Cast<ComparisonOperator>().ToList());
            }
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 480;
                settings.Width = 768;
                settings.Title = "Filter Rooms";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                return settings;
            }
        }

        public static ObservableCollection<LogicalOperator> LogicalOperators
        {
            get
            {
                return new ObservableCollection<LogicalOperator>(Enum.GetValues(typeof(LogicalOperator)).Cast<LogicalOperator>().ToList());
            }
        }

        public string ComparisonFieldOne
        {
            get
            {
                return comparisonFieldOne;
            }

            set
            {
                if (value != comparisonFieldOne)
                {
                    comparisonFieldOne = value;
                    var f = new RoomFilterItem(
                            LogicalOperator.And,
                            FirstSelectedComparisonOperator,
                            RoomParameterOne.Definition.Name,
                            comparisonFieldOne);
                    filter.AddFilterItem(f, 0);
                    NotifyOfPropertyChange(() => ComparisonFieldOne);
                }
            }
        }

        public string ComparisonFieldThree
        {
            get; set;
        }

        public string ComparisonFieldTwo
        {
            get
            {
                return comparisonFieldTwo;
            }

            set
            {
                if (value != comparisonFieldTwo)
                {
                    comparisonFieldOne = value;
                    NotifyOfPropertyChange(() => ComparisonFieldTwo);
                }
            }
        }

        public bool EnableSecondFilter
        {
            get
            {
                return string.IsNullOrEmpty(ComparisonFieldOne);
            }
        }

        public bool EnableThirdFilter
        {
            get
            {
                return string.IsNullOrEmpty(ComparisonFieldTwo);
            }
        }

        public ComparisonOperator FirstSelectedComparisonOperator
        {
            get; set;
        }

        public Autodesk.Revit.DB.Parameter RoomParameterOne
        {
            get
            {
                return roomParameters[0];
            }

            set
            {
                roomParameters[0] = value;
                NotifyOfPropertyChange(() => RoomParameterOne);
                if (value.Definition.Name == "Department")
                {
                    dynamic settings = new System.Dynamic.ExpandoObject();
                    settings.Height = 320;
                    settings.Width = 640;
                    settings.Title = "Select Department";
                    settings.ShowInTaskbar = false;
                    settings.SizeToContent = System.Windows.SizeToContent.Height;
                    var windowManager = SCaddinsApp.WindowManager;
                    var vm = new ListSelectionViewModel(manager.GetAllDepartments());
                    var task = windowManager.ShowDialogAsync(vm, null, settings);
                    if (task.Result ?? false)
                    {
                        ComparisonFieldOne = vm.SelectedItem;
                    }
                }
                if (value.Definition.Name == "Design Option")
                {
                    dynamic settings = new System.Dynamic.ExpandoObject();
                    settings.Height = 320;
                    settings.Width = 640;
                    settings.MaxHeight = 800;
                    settings.Title = "Select Deisgn Option";
                    settings.ShowInTaskbar = false;
                    settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                    var windowManager = SCaddinsApp.WindowManager;
                    var vm = new ListSelectionViewModel(RoomConversionManager.GetAllDesignOptionNames(manager.Doc));
                    var task = windowManager.ShowDialogAsync(vm, null, settings);
                    if (task.Result ?? false)
                    {
                        ComparisonFieldOne = vm.SelectedItem;
                    }
                }
            }
        }

        public ICollectionView RoomParameters
        {
            get
            {
                var result = new CollectionViewSource();
                var collection = new ObservableCollection<Autodesk.Revit.DB.Parameter>(manager.GetRoomParameters());
                result.Source = collection;
                result.SortDescriptions.Add(new SortDescription("Definition.Name", ListSortDirection.Ascending));
                return result.View;
            }
        }

        public Autodesk.Revit.DB.Parameter RoomParameterThree
        {
            get; set;
        }

        public Autodesk.Revit.DB.Parameter RoomParameterTwo
        {
            get; set;
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void OK()
        {
            TryCloseAsync(true);
        }
    }
}