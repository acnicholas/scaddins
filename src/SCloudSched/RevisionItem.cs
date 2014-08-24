namespace SCaddins.SCloudSChed
{
    public class RevisionItem
    {
        private bool export;
        private string description;
        private string date;
        
        public RevisionItem(string date, string description, bool issued, int sequence)
        {
            this.description = description;
            this.date = date;
            this.Issued = issued;
            this.Sequence = sequence;
        }
        
        public bool Export {
            get { return this.export; }
            set { this.export = value; }
        }
        
        public string Description {
            get { return this.description; }
        }
        
        public string Date {
            get { return this.date; }
        }
    
        public bool Issued {
            get;
            set;
        }

        public int Sequence {
            get;
            set;
        }
    }
}