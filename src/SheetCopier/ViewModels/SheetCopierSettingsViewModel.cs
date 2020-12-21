namespace SCaddins.SheetCopier.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SheetCopierSettingsViewModel : Common.ViewModels.ISettingPanel
    {
        public SheetCopierSettingsViewModel()
        {
            Reset();
        }

        public string PrimaryCustomSheetParameter { get; set;  }

        public string SecondaryCustomSheetParameter { get; set; }

        public void Apply()
        {
            Settings.Default.CustomSheetParameterOne = PrimaryCustomSheetParameter;
            Settings.Default.CustomSheetParameterTwo = SecondaryCustomSheetParameter;
            Settings.Default.Save();
        }

        public void Reset()
        {
            PrimaryCustomSheetParameter = Settings.Default.CustomSheetParameterOne;
            SecondaryCustomSheetParameter = Settings.Default.CustomSheetParameterTwo;
        }

        public void ResetToDefault()
        {
            Settings.Default.Reset();
            Settings.Default.Save();
        }
    }
}
