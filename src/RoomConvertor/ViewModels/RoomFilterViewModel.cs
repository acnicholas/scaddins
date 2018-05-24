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
        string comparisonFieldOne = string.Empty;
        string comparisonFieldTwo = string.Empty;
        string comparisonFieldThree = string.Empty;

        public RoomFilterViewModel(RoomConversionManager manager, RoomFilter filter)
        {
            this.manager = manager;
            this.filter = filter;
        }

        public ObservableCollection<Autodesk.Revit.DB.Parameter> RoomParameters
        {
            get { return new ObservableCollection<Autodesk.Revit.DB.Parameter>(manager.GetRoomParameters()); }
        }

        public Autodesk.Revit.DB.Parameter RoomParameterOne
        {
            get; set;
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

        public LogicalOperator FirstSelectedLogicalOperator
        {
            get; set;
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
                            RoomParameterOne,
                            value
                            );
                    if (filter.Size < 1)
                    {
                        filter.AddFilterItem(f);
                    } else {
                        filter.Filters[0] = f;
                    }
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
