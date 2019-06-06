namespace SCaddins.ModelSetupWizard
{
    public class NominatedArchitect
    {
        public NominatedArchitect(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Id {
            get; set;
        }

        public string Name {
            get; set;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0};{1}", Name, Id);
        }
    }
}
