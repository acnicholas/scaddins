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

        public RoomFilterViewModel(RoomConversionManager manager)
        {
            this.manager = manager;
        }

        public ObservableCollection<Autodesk.Revit.DB.Parameter> RoomParameters
        {
            get { return new ObservableCollection<Autodesk.Revit.DB.Parameter>(manager.GetRoomParameters()); }
        }

        public ObservableCollection<ComparisonOperator> ComparisonOperators
        {
            get
            {
                return new ObservableCollection<ComparisonOperator>(Enum.GetValues(typeof(ComparisonOperator)).Cast<ComparisonOperator>().ToList());
            }
        }


        public ComparisonOperator SelectedComparisonOperator
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

        public ComparisonOperator SelectedLogicalOperator
        {
            get; set;
        }

        public string ComparisonFieldOne
        {
            get; set;
        }

        public string ComparisonFieldTwo
        {
            get; set;
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
