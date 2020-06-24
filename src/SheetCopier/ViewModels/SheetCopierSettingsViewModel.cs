namespace SCaddins.SheetCopier.ViewModels
{
    using Caliburn.Micro;

    public class SheetCopierSettingsViewModel : PropertyChangedBase
    {
        public SheetCopierSettingsViewModel()
        {
            Reset();
        }

        public string SheetParameterOne { get; set; }

        public string SheetParameterTwo { get; set; }

        public void Apply()
        {
            SheetCopierSettings.Default.SheetParameterOne = SheetParameterOne;
            SheetCopierSettings.Default.SheetParameterTwo = SheetParameterTwo;
            SheetCopierSettings.Default.Save();
        }

        public void Reset()
        {
            SheetParameterOne = SheetCopierSettings.Default.SheetParameterOne;
            SheetParameterTwo = SheetCopierSettings.Default.SheetParameterTwo;
            NotifyOfPropertyChange(() => SheetParameterOne);
            NotifyOfPropertyChange(() => SheetParameterTwo);
        }
    }
}
