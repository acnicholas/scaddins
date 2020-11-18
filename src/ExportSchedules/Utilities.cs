namespace SCaddins.ExportSchedules
{
    using System.Collections.Generic;
    using System.IO;
    using Autodesk.Revit.DB;
    using OfficeOpenXml;        

    public class Utilities
    {
        public static void AddDelimitedDataToExcelWorkbook(string excelFileName, string worksheetsName, string csvFileName)
        {
            bool firstRowIsHeader = false;

            var format = new ExcelTextFormat();
            format.Delimiter = Settings.Default.FieldDelimiter.ToCharArray()[0];
            format.EOL = "\r";              // DEFAULT IS "\r\n";
            //// format.TextQualifier = Settings.Default.TextQualifier.ToCharArray()[0];
            //// SCaddinsApp.WindowManager.ShowMessageBox(format.TextQualifier.ToString());

            using (ExcelPackage package = new ExcelPackage(new FileInfo(excelFileName)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetsName);
                worksheet.Cells["A1"].LoadFromText(new FileInfo(csvFileName), format, OfficeOpenXml.Table.TableStyles.Medium27, firstRowIsHeader);
                package.Save();
            }
        }

        public static string Export(List<Schedule> schedules, string exportPath)
        {
            System.Text.StringBuilder exportMsg = new System.Text.StringBuilder();
            int successes = 0;
            int attempts = 0;
            using (var options = LoadSavedExportOptions())
            {
                // FIXME - Let user choose export name.
                var excelFileName = "RevitSchedules_" + Common.MiscUtilities.GetVerboseDateString + ".xlsx";
                foreach (var schedule in schedules)
                {
                    attempts++;
                    if (!Directory.Exists(exportPath))
                    {
                        exportMsg.AppendLine("[Error] " + schedule.ExportName + ". Directory not found: " + exportPath);
                        continue;
                    }
                    if (schedule.Export(options, exportPath))
                    {
                        exportMsg.AppendLine("[Success} " + schedule.ExportName);
                        successes++;
                        if (Settings.Default.ExportExcel)
                        {
                            AddDelimitedDataToExcelWorkbook(
                                Path.Combine(exportPath, excelFileName),
                                schedule.ExportName,
                                Path.Combine(exportPath, schedule.ExportName));
                        }
                    } else
                    {
                        exportMsg.AppendLine("[Error] " + schedule.ExportName);
                    }
                }
            }
            var fails = attempts - successes;
            var summaryString = string.Format(
                "Export Summary:" + System.Environment.NewLine +
                "{0} Export(s) attempted with {1} successe(s) and {2} fail(s))" + System.Environment.NewLine +
                System.Environment.NewLine, 
                attempts, 
                successes, 
                fails);
            exportMsg.Insert(0, summaryString);
            return exportMsg.ToString();
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

        private static string SelectExcelFileName()
        {
            string filePath = string.Empty;
            SCaddinsApp.WindowManager.ShowFileSelectionDialog(null, out filePath);
            return filePath;
        }
    }
}
