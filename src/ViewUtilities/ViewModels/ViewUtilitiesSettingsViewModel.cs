namespace SCaddins.ViewUtilities.ViewModels
{
    using Caliburn.Micro;

    public class ViewUtilitiesSettingsViewModel : PropertyChangedBase, Common.ViewModels.ISettingPanel
    {
        public ViewUtilitiesSettingsViewModel()
        {
            Reset();
        }

        public string UserViewNameFormat { get; set; }

        public string FirstParamName { get; set; }

        public string FirstParamValue { get; set; }

        public string SecondParamName { get; set; }

        public string SecondParamValue { get; set; }

        public string ThirdParamName { get; set; }

        public string ThirdParamValue { get; set; }

        public void Apply()
        {
            ViewUtilitiesSettings.Default.UserViewNameFormat = UserViewNameFormat;
            ViewUtilitiesSettings.Default.FirstParamName = FirstParamName;
            ViewUtilitiesSettings.Default.SecondParamName = SecondParamName;
            ViewUtilitiesSettings.Default.ThirdParamName = ThirdParamName;
            ViewUtilitiesSettings.Default.FirstParamValue = FirstParamValue;
            ViewUtilitiesSettings.Default.SecondParamValue = SecondParamValue;
            ViewUtilitiesSettings.Default.ThirdParamValue = ThirdParamValue;
            ViewUtilitiesSettings.Default.Save();
        }

        public void Reset()
        {
            UserViewNameFormat = ViewUtilitiesSettings.Default.UserViewNameFormat;
            FirstParamName = ViewUtilitiesSettings.Default.FirstParamName;
            SecondParamName = ViewUtilitiesSettings.Default.SecondParamName;
            ThirdParamName = ViewUtilitiesSettings.Default.ThirdParamName;
            FirstParamValue = ViewUtilitiesSettings.Default.FirstParamValue;
            SecondParamValue = ViewUtilitiesSettings.Default.SecondParamValue;
            ThirdParamValue = ViewUtilitiesSettings.Default.ThirdParamValue;
            NotifyOfPropertyChange(() => UserViewNameFormat);
            NotifyOfPropertyChange(() => FirstParamName);
            NotifyOfPropertyChange(() => FirstParamValue);
            NotifyOfPropertyChange(() => SecondParamName);
            NotifyOfPropertyChange(() => SecondParamValue);
            NotifyOfPropertyChange(() => ThirdParamName);
            NotifyOfPropertyChange(() => ThirdParamValue);
        }

        public void ResetToDefault()
        {
            throw new System.NotImplementedException();
        }
    }
}
