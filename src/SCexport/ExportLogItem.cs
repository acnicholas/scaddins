using System;

namespace SCaddins.SCexport
{
    public class ExportLogItem
    {
        public string Filename
        {
            get; set;
        }
        
        public string Description
        {
            get; set;
        }
        
        public ExportLogItem(string description, string filename)
        {
            Filename = filename;
            Description = description;
        }
    }
}
