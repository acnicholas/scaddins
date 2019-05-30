namespace SCaddins.ModelSetupWizard
{
    class ProjectInformationReplacement
    {
        public ProjectInformationReplacement(string parameterName, string replacement)
        {
            ParamaterName = parameterName;
            ReplacementValue = replacement;
        }

        public ProjectInformationReplacement() : this(string.Empty, string.Empty)
        {
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2}", ParamaterName, ReplacementValue, ReplacementFormat);
        }

        public string ParamaterName
        {
            get; set;
        }

        public string ReplacementValue
        {
            get; set;
        }

        public string ReplacementFormat
        {
            get; set;
        }
    }
}
