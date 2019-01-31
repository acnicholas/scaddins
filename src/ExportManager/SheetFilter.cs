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

        private string LastNumberInString(string s)
        {
            var onlyNumbers = Regex.Replace(s, "[^0-9]", @" ");
            string[] numberParts = onlyNumbers.Split(new string[] { @" " }, StringSplitOptions.RemoveEmptyEntries);
            var n = numberParts.Where(v => v.Length > 1);
            if(n.Count() > 0) {
                return n.Last().Substring(0, 1);
            }
            return null;
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
                case "Number":
                    var n = LastNumberInString(FilterValue);
                    if (n == null) {
                        return null;
                    }
                    return new System.Predicate<object>(item => n == LastNumberInString((item as ExportSheet).SheetNumber));
                case "Name":
                    properyName = "SheetDescription";
                    var noNumbers = Regex.Replace(FilterValue, "[0-9]", @" ");
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
