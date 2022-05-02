namespace SCaddins.SheetCopier.ViewModels
{
    public class SheetCopierSettingsViewModel : Common.ViewModels.ISettingPanel
    {
        public SheetCopierSettingsViewModel()
        {
            Reset();
        }

        public bool DeleteRevisionClouds { get; set; }

        public string PrimaryCustomSheetParameter { get; set; }

        public string SecondaryCustomSheetParameter { get; set; }

        public string TertiaryCustomSheetParameter { get; set; }

        public void Apply()
        {
            Settings.Default.CustomSheetParameterOne = PrimaryCustomSheetParameter;
            Settings.Default.CustomSheetParameterTwo = SecondaryCustomSheetParameter;
            Settings.Default.CustomSheetParameterThree = TertiaryCustomSheetParameter;
            Settings.Default.DeleteRevisionClouds = DeleteRevisionClouds;
            Settings.Default.Save();
        }

        public void Reset()
        {
            PrimaryCustomSheetParameter = Settings.Default.CustomSheetParameterOne;
            SecondaryCustomSheetParameter = Settings.Default.CustomSheetParameterTwo;
            TertiaryCustomSheetParameter = Settings.Default.CustomSheetParameterThree;
            DeleteRevisionClouds = Settings.Default.DeleteRevisionClouds;
        }

        public void ResetToDefault()
        {
            Settings.Default.Reset();
            Settings.Default.Save();
        }
    }
}
