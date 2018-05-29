using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Caliburn.Micro;

namespace SCaddins.RoomConvertor.ViewModels
{
    public class RoomFilterViewModel : Screen
    {
        private RoomConversionManager manager;
        private RoomFilter filter;
        public Autodesk.Revit.DB.Parameter[] roomParameters;
        string comparisonFieldOne = string.Empty;
        string comparisonFieldTwo = string.Empty;
        string comparisonFieldThree = string.Empty;

        public RoomFilterViewModel(RoomConversionManager manager, RoomFilter filter)
        {
            this.manager = manager;
            this.filter = filter;
            roomParameters = new Autodesk.Revit.DB.Parameter[3];
        }

        public ObservableCollection<Autodesk.Revit.DB.Parameter> RoomParameters
        {
            get { return new ObservableCollection<Autodesk.Revit.DB.Parameter>(manager.GetRoomParameters()); }
        }

        public Autodesk.Revit.DB.Parameter RoomParameterOne
        {
            get
            {
                return roomParameters[0];
            } set
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
                    var vm = new ViewModels.ListSelectionViewModel(manager.GetAllDepartments());
                    bool? r = windowManager.ShowDialog(vm, null, settings);
                    //if (r.HasValue && r.Value)
                    //{
                        ComparisonFieldOne = vm.SelectedItem;
                    //} 
                }
                if (value.Definition.Name == "Design Option")
                {
                    dynamic settings = new System.Dynamic.ExpandoObject();
                    settings.Height = 320;
                    settings.Width = 640;
                    settings.Title = "Select Deisgn Option";
                    settings.ShowInTaskbar = false;
                    settings.SizeToContent = System.Windows.SizeToContent.Height;
                    var windowManager = SCaddinsApp.WindowManager;
                    var vm = new ViewModels.ListSelectionViewModel(RoomConversionManager.GetAllDesignOptionNames(manager.Doc));
                    bool? r = windowManager.ShowDialog(vm, null, settings);
                    //if (r.HasValue && r.Value)
                    //{
                    ComparisonFieldOne = vm.SelectedItem;
                    //} 
                }
            }
        }

        public Autodesk.Revit.DB.Parameter RoomParameterTwo
        {
            get; set;
        }

        public Autodesk.Revit.DB.Parameter RoomParameterThree
        {
            get; set;
        }

        public ObservableCollection<ComparisonOperator> ComparisonOperators
        {
            get
            {
                return new ObservableCollection<ComparisonOperator>(Enum.GetValues(typeof(ComparisonOperator)).Cast<ComparisonOperator>().ToList());
            }
        }


        public ComparisonOperator FirstSelectedComparisonOperator
        {
            get; set;
        }

        public ObservableCollection<LogicalOperator> LogicalOperators
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
                            comparisonFieldOne
                            );
                    filter.AddFilterItem(f, 0);
                    NotifyOfPropertyChange(() => ComparisonFieldOne);
                }
            }
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

        public string ComparisonFieldThree
        {
            get; set;
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
    }
}
