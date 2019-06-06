namespace SCaddins.ModelSetupWizard
{
    public class ProjectInformationReplacement
    {
        public ProjectInformationReplacement(string parameterName, string replacement)
        {
            ParamaterName = parameterName;
            ReplacementValue = replacement;
        }

        public ProjectInformationReplacement() : this(string.Empty, string.Empty)
        {
        }

        public string ParamaterName
        {
            get; set;
        }

        public string ReplacementFormat
        {
            get; set;
        }

        public string ReplacementValue
        {
            get; set;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2}", ParamaterName, ReplacementValue, ReplacementFormat);
        }
    }
}
