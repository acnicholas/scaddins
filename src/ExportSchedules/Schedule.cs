namespace SCaddins.ExportSchedules
{
    using Autodesk.Revit.DB;

    public class Schedule
    {
        public Schedule(Autodesk.Revit.DB.ViewSchedule viewSchedule)
        {
            RevitViewSchedule = viewSchedule;
            RevitName = RevitViewSchedule.Name;
            ExportName = RevitName + ".txt";
        }

        public string ExportName
        {
            get; set;
        }

        public string RevitName
        {
            get; set;
        }

        public string Type
        {
            get
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
        }

        public Autodesk.Revit.DB.ViewSchedule RevitViewSchedule
        {
            get; set;
        }

        public override string ToString()
        {
            return RevitName;
        }

        public bool Export(ViewScheduleExportOptions options, string exportPath)
        {
            if (!System.IO.Directory.Exists(exportPath))
            {
                return false;
            }
            try
            {
                RevitViewSchedule.Export(exportPath, ExportName, options);
            } catch
            {
                return false;
            }
            return true;
        }
    }
}
