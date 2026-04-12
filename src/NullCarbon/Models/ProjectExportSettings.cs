using System.Collections.Generic;

namespace SCaddins.ExportSchedules.Models
{
    public class ProjectExportSettings
    {
        public string ExportDirectory { get; set; }
        public string DecimalSeparator { get; set; }
        public bool IncludeWithoutKeynote { get; set; }
        public bool IncludeStructureLayers { get; set; }
        public bool FillEmptyKeynote { get; set; }
        public List<string> SelectedScheduleIds { get; set; } = new List<string>();
    }
}
