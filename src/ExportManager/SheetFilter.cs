using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SCaddins.ExportManager
{
    class SheetFilter
    {
        string FilterPropertyName { get; set; }
        string FilterValue { get; set; }

        public SheetFilter(string filterPropertyName, string filterValue)
        {
            FilterPropertyName = filterPropertyName;
            FilterValue = filterValue;
        }

        public override string ToString()
        {
            return FilterPropertyName + @" - " + FilterValue;
        }

        public Predicate<object> GetFilter()
        {
            return new Predicate<object>(item => ((ExportSheet)item).GetType().GetProperty(FilterPropertyName, 0).ToString().Contains(FilterValue));
            //return new System.Predicate<object>(item => Regex.IsMatch(((ExportSheet)item).SheetNumber, @"^\D*" + number));
        }
    }
}
