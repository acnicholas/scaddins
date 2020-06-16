namespace SCaddins.ViewUtilities.ViewModels
{
    using Caliburn.Micro;
    using System.ComponentModel;

    class ViewUtilitiesSettingsViewModel : PropertyChangedBase
    {
        public string UserViewNameFormat
        {
            get
            {
                return ViewUtilitiesSettings.Default.UserViewNameFormat;
            }
            set
            {
                if (ViewUtilitiesSettings.Default.UserViewNameFormat != value)
                {
                    ViewUtilitiesSettings.Default.UserViewNameFormat = value;
                    ViewUtilitiesSettings.Default.Save();
                    NotifyOfPropertyChange(() => UserViewNameFormat);
                }
            }
        }
    }
}
