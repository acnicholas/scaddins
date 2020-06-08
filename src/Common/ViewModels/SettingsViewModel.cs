using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.Common.ViewModels
{

    using Caliburn.Micro;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Reflection;
    using ParameterUtilities.ViewModels;

    class SettingsViewModel : PropertyChangedBase
    {
        private SCincrementViewModel incrementViewModel;

        public SettingsViewModel(SCincrementViewModel svm)
        {
            IncrementViewModel = svm;
        }

        public SCincrementViewModel IncrementViewModel
        {
            get { return incrementViewModel; }
            set
            {
                incrementViewModel = value;
                NotifyOfPropertyChange(() => IncrementViewModel);
            }
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 400;
                settings.Title = "SCaddins Global Settings";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                settings.ResizeMode = System.Windows.ResizeMode.CanResize;
                return settings;
            }
        }
    }
}
