namespace SCaddins.ExportSchedules
{
    using Autodesk.Revit.DB;
    using ClosedXML;
    using ClosedXML.Excel;
    using CsvHelper;
    using SCaddins.ExportManager;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;

    public class Utilities
    {
        public static void AddDelimitedDataToExcelWorkbook(string fileName, string worksheetsName, string csvFileName)
        {
            SCaddinsApp.WindowManager.ShowMessageBox(fileName);

            XLWorkbook workbook;
            if (File.Exists(fileName))
            {
                workbook = new XLWorkbook(fileName);
            }
            else
            {
                workbook = new XLWorkbook();
            }

            //don't allow worksheet names with a length > 31
            if (worksheetsName.Length > 31) worksheetsName = worksheetsName.Substring(0, 30);

            var worksheet = workbook.Worksheets.Add(worksheetsName);
            var dt = GetDataTableFromCsv(csvFileName);
            worksheet.FirstCell().InsertTable(dt);
            workbook.SaveAs(fileName);
            workbook.Dispose();
        }

        public static DataTable GetDataTableFromCsv(string filePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string[] headers = sr.ReadLine().Split(ExportSchedules.Settings.Default.FieldDelimiter[0]);
                foreach (string header in headers)
                {
                    dt.Columns.Add(header.Trim());
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(ExportSchedules.Settings.Default.FieldDelimiter[0]);
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i].Trim();
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
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
                    }
                    else
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
