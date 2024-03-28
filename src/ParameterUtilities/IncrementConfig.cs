namespace SCaddins.ParameterUtilities
{
    public class IncrementConfig
    {
        /*public IncrementConfig(
            string name,
            string sourceSearchPattern,
            string sourceReplacementPattern,
            string destinationSearchPattern,
            string destinationReplacementPattern,
            int incrementValue,
            int offsetValue,
            bool keepLeadingZeros,
            bool useDestinationSerachPattern)
        {
            ///TODO
        }*/

        public string Name { get; set; }

        public string SourceSearchPattern { get; set; }

        public string SourceReplacementPattern { get; set; }

        public string DestinationSearchPattern { get; set; }

        public string DestinationReplacementPattern { get; set; }

        public string IncrementValue { get; set; }

        public string OffsetValue { get; set; }

        public bool KeepLeadingZeros { get; set; }

        public bool UseDestinationSerachPattern { get; set; }
    }
}
