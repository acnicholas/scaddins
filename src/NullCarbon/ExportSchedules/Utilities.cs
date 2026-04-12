namespace SCaddins.ExportSchedules
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.DB;
    using OfficeOpenXml;

    public class Utilities
    {
        public class ExportResult
        {
            public string Message { get; set; }
            public string MergedExcelFilePath { get; set; }
            public string MergedExcelFileName { get; set; }
        }

        /// <summary>
        /// (Optional) Imports CSV data into an Excel workbook.
        /// This method is kept for reference if needed.
        /// </summary>
        public static void AddDelimitedDataToExcelWorkbook(string excelFileName, string worksheetName, string csvFileName)
        {
            bool firstRowIsHeader = false;
            var format = new ExcelTextFormat
            {
                // Use tab as the delimiter.
                Delimiter = '\t',
                EOL = "\r\n",
                // Default text qualifier (double quote).
                TextQualifier = '\"'
            };

            using (ExcelPackage package = new ExcelPackage(new FileInfo(excelFileName)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);
                worksheet.Cells["A1"].LoadFromText(
                    new FileInfo(csvFileName),
                    format,
                    OfficeOpenXml.Table.TableStyles.Medium27,
                    firstRowIsHeader
                );
                package.Save();
            }
        }

        /// <summary>
        /// Exports the given schedules to a single Excel workbook named after the Revit project plus " nullCarbon export",
        /// then deletes any temporary CSV/TXT files that Revit generates during export.
        /// ENHANCED: Preserves exact decimal formatting with PERIOD as decimal separator (223.001 stays as 223.001)
        /// </summary>
        /// <param name="schedules">The list of schedules to export.</param>
        /// <param name="exportPath">The directory to export to.</param>
        /// <returns>A message summarizing the export results.</returns>
        public static ExportResult Export(List<Schedule> schedules, string exportPath, bool includeStructureLayers)
        {
            // If nothing is selected or no schedules found, bail out early.
            if (schedules == null || schedules.Count == 0)
            {
                return new ExportResult
                {
                    Message = "No schedules to export.",
                    MergedExcelFilePath = null,
                    MergedExcelFileName = null
                };
            }

            // Attempt to get the Revit project's title from the first schedule.
            // Fall back if it's somehow null/empty.
            string docTitle = schedules[0]?.RevitViewSchedule?.Document?.Title;
            if (string.IsNullOrWhiteSpace(docTitle))
            {
                docTitle = "UnknownProject";
            }

            // Build the final Excel export file name:
            string mergedExcelFilePath = BuildMergedExcelFilePath(docTitle, exportPath);
            string mergedExcelFileName = Path.GetFileName(mergedExcelFilePath);

            StringBuilder exportMsg = new StringBuilder();
            int successes = 0;
            int attempts = 0;

            // Create a new ExcelPackage for the final merged Excel file.
            using (ExcelPackage mergedPackage = new ExcelPackage())
            {
                using (var options = LoadSavedExportOptions())
                {
                    foreach (var schedule in schedules)
                    {
                        attempts++;

                        // Make sure the export path actually exists.
                        if (!Directory.Exists(exportPath))
                        {
                            exportMsg.AppendLine($"[Error] {schedule.ExportName}. Directory not found: {exportPath}");
                            continue;
                        }

                        // Export the schedule to a temporary CSV/TXT file via Revit's built-in "schedule.Export" method.
                        if (schedule.Export(options, exportPath))
                        {
                            string csvFilePath = Path.Combine(exportPath, schedule.ExportName);
                            try
                            {
                                // Use the name (minus extension) as the worksheet name.
                                string requestedSheetName = Path.GetFileNameWithoutExtension(schedule.ExportName);
                                string sheetName = GetUniqueWorksheetName(mergedPackage, requestedSheetName);

                                if (!string.Equals(sheetName, requestedSheetName, StringComparison.Ordinal))
                                {
                                    exportMsg.AppendLine($"[Info] Worksheet name adjusted from '{requestedSheetName}' to '{sheetName}' due to name conflict.");
                                }

                                // Create a new worksheet in the merged Excel file.
                                ExcelWorksheet worksheet = mergedPackage.Workbook.Worksheets.Add(sheetName);

                                // Use the user-selected field delimiter from options.
                                char delimiterChar = options.FieldDelimiter != null && options.FieldDelimiter.Length > 0
                                    ? options.FieldDelimiter[0]
                                    : '\t';

                                // Determine the text qualifier based on the user-selected option.
                                char textQualifierChar = '\"'; // default to double quote
                                switch (options.TextQualifier)
                                {
                                    case ExportTextQualifier.DoubleQuote:
                                        textQualifierChar = '\"';
                                        break;
                                    case ExportTextQualifier.Quote:
                                        textQualifierChar = '\'';
                                        break;
                                    case ExportTextQualifier.None:
                                        textQualifierChar = '\0'; // no text qualifier
                                        break;
                                    default:
                                        textQualifierChar = '\"';
                                        break;
                                }

                                // Use InvariantCulture to preserve formatting
                                var format = new ExcelTextFormat
                                {
                                    Delimiter = delimiterChar,
                                    EOL = "\r\n",
                                    TextQualifier = textQualifierChar,
                                    Culture = CultureInfo.InvariantCulture
                                };

                                // Read the CSV file and manually parse it to avoid any automatic conversion
                                string csvContent = File.ReadAllText(csvFilePath, Encoding.UTF8);

                                // MANUAL CSV PARSING - bypasses all EPPlus automatic formatting
                                LoadCsvAsTextManually(worksheet, csvContent, delimiterChar, textQualifierChar);

                                exportMsg.AppendLine($"[Success] {schedule.ExportName}");
                                successes++;
                            }
                            catch (Exception ex)
                            {
                                exportMsg.AppendLine($"[Error] Merging {schedule.ExportName}: {ex.Message}");
                            }
                            finally
                            {
                                // Delete the temporary CSV/TXT file so only the single merged Excel remains.
                                if (File.Exists(csvFilePath))
                                {
                                    File.Delete(csvFilePath);
                                }
                            }
                        }
                        else
                        {
                            exportMsg.AppendLine($"[Error] {schedule.ExportName}");
                        }
                    }
                }

                if (includeStructureLayers)
                {
                    AppendStructureLayersWorksheet(mergedPackage, schedules, exportMsg);
                }

                // Save the merged Excel workbook.
                mergedPackage.SaveAs(new FileInfo(mergedExcelFilePath));
            }

            int fails = attempts - successes;
            string summaryString = string.Format(
                "Export Summary:" + Environment.NewLine +
                "{0} Export(s) attempted with {1} success(es) and {2} fail(s)" + Environment.NewLine + Environment.NewLine,
                attempts,
                successes,
                fails);
            exportMsg.Insert(0, summaryString);

            // We only keep the single Excel file, no CSV/TXT are retained.
            exportMsg.AppendLine($"[Excel Export] Merged Excel file created at {mergedExcelFilePath}");

            return new ExportResult
            {
                Message = exportMsg.ToString(),
                MergedExcelFilePath = mergedExcelFilePath,
                MergedExcelFileName = mergedExcelFileName
            };
        }

        /// <summary>
        /// MANUAL CSV LOADING: Bypasses all EPPlus automatic formatting to preserve exact text
        /// This ensures 223.001 displays exactly as 223.001 (never 223,001)
        /// </summary>
        /// <param name="worksheet">The Excel worksheet to populate</param>
        /// <param name="csvContent">The raw CSV content as string</param>
        /// <param name="delimiter">Field delimiter character</param>
        /// <param name="textQualifier">Text qualifier character</param>
        private static void LoadCsvAsTextManually(ExcelWorksheet worksheet, string csvContent, char delimiter, char textQualifier)
        {
            var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];
                if (string.IsNullOrEmpty(line)) continue;

                var fields = ParseCsvLine(line, delimiter, textQualifier);

                for (int fieldIndex = 0; fieldIndex < fields.Count; fieldIndex++)
                {
                    var cell = worksheet.Cells[lineIndex + 1, fieldIndex + 1];
                    string fieldValue = fields[fieldIndex];

                    // FORCE EVERYTHING TO BE PURE TEXT - no conversion whatsoever
                    cell.Value = fieldValue;
                    cell.Style.Numberformat.Format = "@"; // Text format

                    // Apply alignment for better appearance
                    if (IsNumericLooking(fieldValue))
                    {
                        cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }
                    else
                    {
                        cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                }
            }
        }

        /// <summary>
        /// Parse a single CSV line respecting delimiters and text qualifiers
        /// </summary>
        /// <param name="line">CSV line to parse</param>
        /// <param name="delimiter">Field delimiter</param>
        /// <param name="textQualifier">Text qualifier</param>
        /// <returns>List of field values</returns>
        private static List<string> ParseCsvLine(string line, char delimiter, char textQualifier)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;
            bool useTextQualifier = textQualifier != '\0';

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (useTextQualifier && c == textQualifier)
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == textQualifier)
                    {
                        // Escaped quote - add one quote to field
                        currentField.Append(textQualifier);
                        i++; // Skip next quote
                    }
                    else
                    {
                        // Toggle quote state
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == delimiter && !inQuotes)
                {
                    // End of field
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    // Regular character
                    currentField.Append(c);
                }
            }

            // Add the last field
            fields.Add(currentField.ToString());
            return fields;
        }

        /// <summary>
        /// Check if a string looks like a number (for alignment purposes only)
        /// </summary>
        /// <param name="text">Text to check</param>
        /// <returns>True if it looks numeric</returns>
        private static bool IsNumericLooking(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            // Just check if it starts with a digit or negative sign followed by digit
            text = text.Trim();
            if (text.Length == 0) return false;

            return char.IsDigit(text[0]) ||
                   (text.Length > 1 && text[0] == '-' && char.IsDigit(text[1]));
        }

        /// <summary>
        /// Helper method to count decimal places in a numeric string
        /// </summary>
        /// <param name="numericString">String representation of a number</param>
        /// <returns>Number of decimal places</returns>
        private static int GetDecimalPlaces(string numericString)
        {
            if (string.IsNullOrEmpty(numericString)) return 0;

            int decimalIndex = numericString.IndexOf('.');
            if (decimalIndex == -1) return 0;

            return numericString.Length - decimalIndex - 1;
        }

        /// <summary>
        /// Retrieves all schedules from the given Revit document.
        /// </summary>
        public static List<Schedule> GetAllSchedules(Document doc)
        {
            List<Schedule> result = new List<Schedule>();
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule));
            foreach (var elem in collector)
            {
                if (elem is ViewSchedule schedule)
                {
                    if (schedule.IsTitleblockRevisionSchedule || schedule.IsInternalKeynoteSchedule)
                    {
                        continue;
                    }
                    result.Add(new Schedule(schedule));
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a dictionary of available field delimiters.
        /// </summary>
        public static Dictionary<string, string> GetFieldDelimiters()
        {
            // Key = Display text, Value = Actual delimiter string
            return new Dictionary<string, string>
            {
                { "Comma", "," },
                { "Semi-Colon", ";" },
                { "Tab", "\t" },
                { "Space", " " }
            };
        }

        /// <summary>
        /// Returns a dictionary of available text qualifiers.
        /// </summary>
        public static Dictionary<string, string> GetFieldTextQualifiers()
        {
            // Key = Display text, Value = Actual qualifier string
            return new Dictionary<string, string>
            {
                { "Double Quote (\")", "\"" },
                { "Quote (\')", "\'" },
                { "None", string.Empty }
            };
        }

        /// <summary>
        /// Loads saved export options from your application settings.
        /// </summary>
        private static ViewScheduleExportOptions LoadSavedExportOptions()
        {
            ViewScheduleExportOptions options = new ViewScheduleExportOptions();

            var headerExportType = Settings.Default.ExportColumnHeader ? ExportColumnHeaders.OneRow : ExportColumnHeaders.None;
            if (Settings.Default.IncludeGroupedColumnHeaders)
            {
                headerExportType = ExportColumnHeaders.MultipleRows;
            }
            options.ColumnHeaders = headerExportType;

            // Grab what's stored in Settings for the delimiter
            string fieldDelim = Settings.Default.FieldDelimiter;

            // Ensure we have a proper delimiter - handle legacy values and ensure actual tab character
            if (string.IsNullOrWhiteSpace(fieldDelim) ||
                fieldDelim == "\\t" ||
                fieldDelim == "/t" ||
                fieldDelim.Equals("tab", StringComparison.OrdinalIgnoreCase))
            {
                fieldDelim = "\t"; // Actual tab character
            }
            else
            {
                // Convert any escape sequences to actual characters (for robustness)
                fieldDelim = fieldDelim.Replace("\\t", "\t")
                                      .Replace("\\n", "\n")
                                      .Replace("\\r", "\r");
            }

            // Now set the final delimiter
            options.FieldDelimiter = fieldDelim;

            // Rest of the method stays the same...
            options.HeadersFootersBlanks = Settings.Default.ExportGrouppHeaderAndFooters;

            string textQualifier = Settings.Default.TextQualifier;
            if (textQualifier == "\"")
            {
                options.TextQualifier = ExportTextQualifier.DoubleQuote;
            }
            else if (textQualifier == "\'")
            {
                options.TextQualifier = ExportTextQualifier.Quote;
            }
            else if (textQualifier == string.Empty)
            {
                options.TextQualifier = ExportTextQualifier.None;
            }
            else
            {
                options.TextQualifier = ExportTextQualifier.DoubleQuote;
            }

            options.Title = Settings.Default.ExportTitle;

            return options;
        }

        /// <summary>
        /// Opens a file selection dialog to choose an Excel file name.
        /// </summary>
        private static string SelectExcelFileName()
        {
            string filePath = string.Empty;
            SCaddinsApp.WindowManager.ShowFileSelectionDialog(null, out filePath);
            return filePath;
        }

        private static string BuildMergedExcelFilePath(string docTitle, string exportPath)
        {
            string safeTitle = SanitizeFileName(docTitle);
            string baseName = $"{safeTitle} nullCarbon-LCA-Export {DateTime.Now:yyyyMMdd_HHmmss}";
            string fileName = $"{SanitizeFileName(baseName)}.xlsx";
            string filePath = Path.Combine(exportPath, fileName);

            return EnsureUniqueFilePath(filePath);
        }

        private static string EnsureUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return filePath;
            }

            string directory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            int counter = 1;

            string candidatePath;
            do
            {
                string candidateFileName = $"{fileNameWithoutExtension} ({counter}){extension}";
                candidatePath = Path.Combine(directory ?? string.Empty, candidateFileName);
                counter++;
            }
            while (File.Exists(candidatePath));

            return candidatePath;
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "Unnamed Export";
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName;
        }

        private static void AppendStructureLayersWorksheet(ExcelPackage mergedPackage, List<Schedule> schedules, StringBuilder exportMsg)
        {
            var document = schedules[0]?.RevitViewSchedule?.Document;
            if (document == null)
            {
                return;
            }

            var scheduleCategoryIds = schedules
                .Select(schedule => schedule.RevitViewSchedule?.Definition?.CategoryId)
                .Where(id => id != null && id != ElementId.InvalidElementId)
                .Select(GetElementIdValue)
                .ToHashSet();

            var hostTypes = new FilteredElementCollector(document)
                .OfClass(typeof(HostObjAttributes))
                .Cast<HostObjAttributes>();

            if (scheduleCategoryIds.Count > 0)
            {
                hostTypes = hostTypes.Where(type => scheduleCategoryIds.Contains(GetElementIdValue(type.Category.Id)));
            }

            const string requestedSheetName = "Structure Layers";
            string sheetName = GetUniqueWorksheetName(mergedPackage, requestedSheetName);

            if (!string.Equals(sheetName, requestedSheetName, StringComparison.Ordinal))
            {
                exportMsg?.AppendLine($"[Info] Worksheet name adjusted from '{requestedSheetName}' to '{sheetName}' due to name conflict.");
            }

            var worksheet = mergedPackage.Workbook.Worksheets.Add(sheetName);
            worksheet.Cells[1, 1].Value = "Category";
            worksheet.Cells[1, 2].Value = "Type Name";
            worksheet.Cells[1, 3].Value = "Layer Index";
            worksheet.Cells[1, 4].Value = "Function";
            worksheet.Cells[1, 5].Value = "Material";
            worksheet.Cells[1, 6].Value = "Thickness (ft)";

            int row = 2;
            foreach (var hostType in hostTypes)
            {
                var compound = hostType.GetCompoundStructure();
                if (compound == null)
                {
                    continue;
                }

                var layers = compound.GetLayers();
                if (layers == null || layers.Count == 0)
                {
                    continue;
                }

                for (int index = 0; index < layers.Count; index++)
                {
                    var layer = layers[index];
                    var material = layer.MaterialId != ElementId.InvalidElementId
                        ? document.GetElement(layer.MaterialId)
                        : null;

                    worksheet.Cells[row, 1].Value = hostType.Category?.Name ?? string.Empty;
                    worksheet.Cells[row, 2].Value = hostType.Name;
                    worksheet.Cells[row, 3].Value = index + 1;
                    worksheet.Cells[row, 4].Value = layer.Function.ToString();
                    worksheet.Cells[row, 5].Value = material?.Name ?? string.Empty;
                    worksheet.Cells[row, 6].Value = layer.Width.ToString("F4", CultureInfo.InvariantCulture);
                    row++;
                }
            }
        }

        private static string GetUniqueWorksheetName(ExcelPackage package, string preferredName)
        {
            const int maxWorksheetNameLength = 31;
            string baseName = string.IsNullOrWhiteSpace(preferredName) ? "Sheet" : preferredName.Trim();

            if (baseName.Length > maxWorksheetNameLength)
            {
                baseName = baseName.Substring(0, maxWorksheetNameLength);
            }

            if (package.Workbook.Worksheets[baseName] == null)
            {
                return baseName;
            }

            int counter = 2;
            while (true)
            {
                string suffix = $" ({counter})";
                int allowedBaseLength = Math.Max(1, maxWorksheetNameLength - suffix.Length);
                string truncatedBaseName = baseName.Length > allowedBaseLength
                    ? baseName.Substring(0, allowedBaseLength)
                    : baseName;

                string candidate = $"{truncatedBaseName}{suffix}";
                if (package.Workbook.Worksheets[candidate] == null)
                {
                    return candidate;
                }

                counter++;
            }
        }

        private static long GetElementIdValue(ElementId elementId)
        {
#if REVIT2024 || REVIT2025 || REVIT2026
            return elementId != null
                ? elementId.Value
                : ElementId.InvalidElementId.Value;
#else
            return elementId?.IntegerValue ?? ElementId.InvalidElementId.IntegerValue;
#endif
        }
    }
}
