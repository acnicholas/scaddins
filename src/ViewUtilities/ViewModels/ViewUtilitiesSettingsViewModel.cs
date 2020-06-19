namespace SCaddins.ViewUtilities.ViewModels
{
    using System.ComponentModel;
    using Caliburn.Micro;

    public class ViewUtilitiesSettingsViewModel : PropertyChangedBase
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

        public string FirstParamName
        {
            get
            {
                return ViewUtilitiesSettings.Default.FirstParamName;
            }

            set
            {
                if (ViewUtilitiesSettings.Default.FirstParamName != value)
                {
                    ViewUtilitiesSettings.Default.FirstParamName = value;
                    ViewUtilitiesSettings.Default.Save();
                    NotifyOfPropertyChange(() => FirstParamName);
                }
            }
        }

        public string FirstParamValue
        {
            get
            {
                return ViewUtilitiesSettings.Default.FirstParamValue;
            }

            set
            {
                if (ViewUtilitiesSettings.Default.FirstParamValue != value)
                {
                    ViewUtilitiesSettings.Default.FirstParamValue = value;
                    ViewUtilitiesSettings.Default.Save();
                    NotifyOfPropertyChange(() => FirstParamValue);
                }
            }
        }

        public string SecondParamName
        {
            get
            {
                return ViewUtilitiesSettings.Default.SecondParamName;
            }

            set
            {
                if (ViewUtilitiesSettings.Default.SecondParamName != value)
                {
                    ViewUtilitiesSettings.Default.SecondParamName = value;
                    ViewUtilitiesSettings.Default.Save();
                    NotifyOfPropertyChange(() => SecondParamName);
                }
            }
        }

        public string SecondParamValue
        {
            get
            {
                return ViewUtilitiesSettings.Default.SecondParamValue;
            }

            set
            {
                if (ViewUtilitiesSettings.Default.SecondParamValue != value)
                {
                    ViewUtilitiesSettings.Default.SecondParamValue = value;
                    ViewUtilitiesSettings.Default.Save();
                    NotifyOfPropertyChange(() => SecondParamValue);
                }
            }
        }

        public string ThirdParamName
        {
            get
            {
                return ViewUtilitiesSettings.Default.ThirdParamName;
            }

            set
            {
                if (ViewUtilitiesSettings.Default.ThirdParamName != value)
                {
                    ViewUtilitiesSettings.Default.ThirdParamName = value;
                    ViewUtilitiesSettings.Default.Save();
                    NotifyOfPropertyChange(() => ThirdParamName);
                }
            }
        }

        public string ThirdParamValue
        {
            get
            {
                return ViewUtilitiesSettings.Default.ThirdParamValue;
            }

            set
            {
                if (ViewUtilitiesSettings.Default.ThirdParamValue != value)
                {
                    ViewUtilitiesSettings.Default.ThirdParamValue = value;
                    ViewUtilitiesSettings.Default.Save();
                    NotifyOfPropertyChange(() => ThirdParamValue);
                }
            }
        }
    }
}
