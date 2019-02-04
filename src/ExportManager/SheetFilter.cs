namespace SCaddins.ExportManager
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class SheetFilter
    {
        public SheetFilter(string filterPropertyName, string filterValue)
        {
            FilterPropertyName = filterPropertyName;
            FilterValue = filterValue;
        }

        public string FilterPropertyName { get; set; }

        public string FilterValue { get; set; }

        public Predicate<object> GetFilter()
        {
            string properyName = "SheetDescription";
            switch (FilterPropertyName)
            {
                case "Export Name":
                    var m = FirstDigitOfLastNumberInString(FilterValue);
                    if (m == null)
                    {
                        return null;
                    }
                    return new System.Predicate<object>(item => m == FirstDigitOfLastNumberInString((item as ExportSheet).FullExportName));
                case "Number":
                    var n = FirstDigitOfLastNumberInString(FilterValue);
                    if (n == null)
                    {
                        return null;
                    }
                    return new System.Predicate<object>(item => n == FirstDigitOfLastNumberInString((item as ExportSheet).SheetNumber));
                case "Name":
                    properyName = "SheetDescription";
                    var noNumbers = Regex.Replace(FilterValue, "[0-9-]", @" ");
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
                    properyName = "NorthPointVisibilityString";
                    break;
                default:
                    return null;
            }    
            return new Predicate<object>(item => item.GetType().GetProperty(properyName).GetValue(item, null).ToString().Equals(FilterValue, StringComparison.InvariantCulture));
        }

        public override string ToString()
        {
            return @"Filter Similar [" + FilterValue + @"]";
        }

        private static string FirstDigitOfLastNumberInString(string s)
        {
            var onlyNumbers = Regex.Replace(s, "[^0-9]", @" ");
            string[] numberParts = onlyNumbers.Split(new string[] { @" " }, StringSplitOptions.RemoveEmptyEntries);
            var n = numberParts.Where(v => v.Length > 1);
            if (n.Count() > 0) {
                return n.Last().Substring(0, 1);
            }
            return null;
        }
    }
}
