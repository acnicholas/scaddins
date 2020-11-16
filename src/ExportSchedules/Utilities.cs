namespace SCaddins.ExportSchedules
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;

    public class Utilities
    {
        public static void Export(List<Schedule> schedules, string exportPath)
        {
            int successes = 0;
            int attempts = 0;
            using (var options = LoadSavedExportOptions())
            {
                foreach (var schedule in schedules)
                {
                    attempts++;
                    if (!System.IO.Directory.Exists(exportPath))
                    {
                        continue;
                    }
                    if (schedule.Export(options, exportPath))
                    {
                        successes++;
                    }
                }
            }
        }

        public static List<Schedule> GetAllSchedules(Document doc)
        {
            var result = new List<Schedule>();
            var collector = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule));
            foreach (var elem in collector)
            {
                if (elem is ViewSchedule)
                {
                    var schedule = elem as ViewSchedule;
                    if (schedule.IsTitleblockRevisionSchedule || schedule.IsInternalKeynoteSchedule)
                    {
                        continue;
                    }
                    result.Add(new Schedule(elem as ViewSchedule));
                }
            }
            return result;
        }

        public static Dictionary<string, string> GetFieldDelimiters()
        {
            var result = new Dictionary<string, string>();
            result.Add("Comma", ",");
            result.Add("Semi-Colon", ";");
            result.Add("Tab", "\t");
            result.Add("Sapce", " ");
            return result;
        }

        public static Dictionary<string, string> GetFieldTextQualifiers()
        {
            var result = new Dictionary<string, string>();
            result.Add("Double Quote (\")", "\"");
            result.Add("Quote) (\')", "\'");
            result.Add("None", string.Empty);
            return result;
        }

        private static ViewScheduleExportOptions LoadSavedExportOptions()
        {
            var options = new ViewScheduleExportOptions();
            var headerExportType = Settings.Default.ExportColumnHeader ? ExportColumnHeaders.OneRow : ExportColumnHeaders.None;
            if (Settings.Default.IncludeGroupedColumnHeaders)
            {
                headerExportType = ExportColumnHeaders.MultipleRows;
            }
            options.ColumnHeaders = headerExportType;
            options.FieldDelimiter = Settings.Default.FieldDelimiter != null ? Settings.Default.FieldDelimiter : ",";
            options.HeadersFootersBlanks = Settings.Default.ExportGrouppHeaderAndFooters;
            var textQualifier = Settings.Default.TextQualifier;
            if (textQualifier == "\"")
            {
                options.TextQualifier = ExportTextQualifier.DoubleQuote;
            }
            if (textQualifier == "\'")
            {
                options.TextQualifier = ExportTextQualifier.Quote;
            }
            if (textQualifier == string.Empty)
            {
                options.TextQualifier = ExportTextQualifier.None;
            }
            options.Title = Settings.Default.ExportTitle;
            return options;
        }
    }
}
