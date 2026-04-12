namespace SCaddins.ExportSchedules
{
    using Autodesk.Revit.DB;
    using System;
    using System.IO;

    public class Schedule
    {
        public Schedule(Autodesk.Revit.DB.ViewSchedule viewSchedule)
        {
            if (viewSchedule == null)
            {
                throw new ArgumentNullException(nameof(viewSchedule), "ViewSchedule cannot be null");
            }

            RevitViewSchedule = viewSchedule;
            RevitName = RevitViewSchedule.Name;
            ExportName = SanitizeFileName(RevitName + ".txt");
        }

        public string ExportName { get; set; }
        public string RevitName { get; set; }

        public string Type
        {
            get
            {
                try
                {
                    var scheduleDefinition = RevitViewSchedule.Definition;
                    var scheduleCategory = scheduleDefinition.CategoryId;
                    var categories = RevitViewSchedule.Document.Settings.Categories;

                    if (!RevitViewSchedule.IsTitleblockRevisionSchedule)
                    {
                        foreach (Category category in categories)
                        {
                            if (category.Id == scheduleCategory)
                            {
                                return category.Name;
                            }
                        }
                        return "<Multi-Category>";
                    }
                    return string.Empty;
                }
                catch (Exception)
                {
                    // If we can't determine the type for any reason
                    return "<Unknown>";
                }
            }
        }

        public Autodesk.Revit.DB.ViewSchedule RevitViewSchedule { get; set; }

        public override string ToString()
        {
            return RevitName;
        }

        /// <summary>
        /// Exports the schedule to the specified path with the given options
        /// </summary>
        /// <param name="options">The export options</param>
        /// <param name="exportPath">The directory to export to</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Export(ViewScheduleExportOptions options, string exportPath)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Export options cannot be null");
            }

            if (string.IsNullOrEmpty(exportPath))
            {
                throw new ArgumentException("Export path cannot be empty", nameof(exportPath));
            }

            if (!Directory.Exists(exportPath))
            {
                return false;
            }

            try
            {
                // Build the full path for the file
                string filePath = Path.Combine(exportPath, ExportName);

                // If a file with the same name exists, delete it
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Use Revit's built-in export functionality
                RevitViewSchedule.Export(exportPath, ExportName, options);

                // Verify the file was created
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                // Log the exception or add to a diagnostic log
                System.Diagnostics.Debug.WriteLine($"Error exporting schedule {RevitName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Removes invalid characters from a filename
        /// </summary>
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "Unnamed Schedule.txt";
            }

            // Replace invalid filename characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName;
        }
    }
}