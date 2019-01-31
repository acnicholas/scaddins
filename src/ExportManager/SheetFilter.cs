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
            return @"Filter Similar [" + FilterValue + @"]";
        }

        private int LastNumberInString(string s)
        {
            var onlyNumbers = Regex.Replace(s, @"[\D-]", string.Empty);
            string[] numberParts = onlyNumbers.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            int lastNumber = -1;
            if (int.TryParse(numberParts.Last(), out lastNumber))
            {
                return lastNumber;
            }
            return lastNumber;
        }

        private int RoundDown(int i)
        {
            return i / 10 ^ (int)Math.Log10(i);
        }

        public Predicate<object> GetFilter()
        {
            //FIXME - do this some other way...
            string properyName = "SheetDescription";
            switch (FilterPropertyName)
            {
                case "Export Name":
                    properyName = "FullExportName";
                    break;
                //case "Number":
                //    //properyName = "SheetNumber";
                //    //get number only
                //    int n1 = LastNumberInString(FilterValue);
                //    return new Predicate<object>(item => (ExportSheet)item.SheetNumber
                case "Name":
                    properyName = "SheetDescription";
                    //remove numbers
                    var noNumbers = Regex.Replace(FilterValue, @"[\d-]", string.Empty);
                    string[] parts = noNumbers.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    return new Predicate<object>(item => parts.Any(item.GetType().GetProperty(properyName).GetValue(item, null).ToString().Contains));
                case "Revision":
                    properyName = "SheetRevision";
                    break;
                case "Revision Description":
                    properyName = "SheetRevisionDescription";
                    break;
                case "Revision Date":
                    properyName = "SheetRevisionDate";
                    break;
                case "Scale":
                    properyName = "Scale";
                    break;
                case "North Point":
                    properyName = "NorthPointVisible";
                    break;
                default:
                    return null;
            }
            return new Predicate<object>(item => item.GetType().GetProperty(properyName).GetValue(item, null).ToString().Equals(FilterValue));
        }
    }
}
