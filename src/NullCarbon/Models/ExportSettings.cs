namespace SCaddins.ExportSchedules.Models
{
    public class ExportSettings
    {
        public bool IncludeStructureLayers { get; set; }
        public bool FillEmptyKeynote { get; set; }
        public string EmptyKeynotePlaceholder { get; set; } = "000.000";
    }
}
