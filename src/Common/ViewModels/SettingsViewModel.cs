namespace SCaddins.Common.ViewModels
{
    using System;
    using Caliburn.Micro;
    using SCaddins.RunScript.ViewModels;
    using SCaddins.SolarAnalysis.ViewModels;
    using SCaddins.SolarAnalysis.Views;

    public class SettingsViewModel : Screen
    {
        private static ISettingPanel incrementViewModel;
        private static ISettingPanel roomConverterViewModel;
        private static ISettingPanel viewUtilitiesViewModel;
        private static ISettingPanel solarAnalysisOptonsViewModel;
        private static ISettingPanel spellCheckerOptionsViewModel;
        private static ISettingPanel sheetCopierViewModel;
        private static ISettingPanel runScriptSettingsViewModel;

        public SettingsViewModel(
            ISettingPanel incrementViewModel,
            ISettingPanel roomConverterViewModel,
            ISettingPanel viewUtilitiesViewModel,
            ISettingPanel solarAnalysisOptonsViewModel,
            ISettingPanel sheetCopierViewModel,
            ISettingPanel spellCheckerOptionsViewModel,
            ISettingPanel runScriptSettingsViewModel)
        {
            IncrementViewModel = incrementViewModel;
            RoomConverterViewModel = roomConverterViewModel;
            ViewUtilitiesViewModel = viewUtilitiesViewModel;
            SolarAnalysisOptonsViewModel = solarAnalysisOptonsViewModel;
            SheetCopierViewModel = sheetCopierViewModel;
            SpellCheckerOptionsViewModel = spellCheckerOptionsViewModel;
            RunScriptSettingsViewModel = runScriptSettingsViewModel;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 480;
                settings.Width = 1024;
                settings.Title = "SCaddins Global Settings";
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                  new Uri("pack://application:,,,/SCaddins;component/Assets/gear.png"));
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResize;
                settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                return settings;
            }
        }

        public static ISettingPanel RoomConverterViewModel
        {
            get
            {
                return roomConverterViewModel;
            }

            set
            {
                roomConverterViewModel = value;
                //NotifyOfPropertyChange(() => RoomConverterViewModel);
            }
        }

        public static ISettingPanel IncrementViewModel
        {
            get
            {
                return incrementViewModel;
            }

            set
            {
                incrementViewModel = value;
                //NotifyOfPropertyChange(() => IncrementViewModel);
            }
        }

        public static ISettingPanel RunScriptSettingsViewModel
        {
            get
            {
                return runScriptSettingsViewModel;
            }

            set
            {
                runScriptSettingsViewModel = value;
                //NotifyOfPropertyChange(() => SheetCopierViewModel);
            }
        }

        public static ISettingPanel SheetCopierViewModel
        {
            get
            {
                return sheetCopierViewModel;
            }

            set
            {
                sheetCopierViewModel = value;
                //NotifyOfPropertyChange(() => SheetCopierViewModel);
            }
        }

        public static ISettingPanel SolarAnalysisOptonsViewModel
        {
            get
            {
                return solarAnalysisOptonsViewModel;
            }

            set
            {
                solarAnalysisOptonsViewModel = value;
                //NotifyOfPropertyChange(() => SpellCheckerOptionsViewModel);
            }
        }

        public static ISettingPanel SpellCheckerOptionsViewModel
        {
            get
            {
                return spellCheckerOptionsViewModel;
            }

            set
            {
                spellCheckerOptionsViewModel = value;
                //NotifyOfPropertyChange(() => SpellCheckerOptionsViewModel);
            }
        }

        public static ISettingPanel ViewUtilitiesViewModel
        {
            get
            {
                return viewUtilitiesViewModel;
            }

            set
            {
                viewUtilitiesViewModel = value;
                //NotifyOfPropertyChange(() => ViewUtilitiesViewModel);
            }
        }

        public void Apply()
        {
            ParameterUtilities.ViewModels.SCincrementViewModel svm = IncrementViewModel as ParameterUtilities.ViewModels.SCincrementViewModel; ;
            IncrementViewModel.Apply();
            ViewUtilitiesViewModel.Apply();
            SolarAnalysisOptonsViewModel.Apply();
            SpellCheckerOptionsViewModel.Apply();
            SheetCopierViewModel.Apply();
            RunScriptSettingsViewModel.Apply();
        }

        public void Cancel()
        {
            TryCloseAsync(true);
        }

        public void OK()
        {
            Apply();
            this.TryCloseAsync(true);
        }

        public void Reset()
        {
            IncrementViewModel.Reset();
            ViewUtilitiesViewModel.Reset();
            SolarAnalysisOptonsViewModel.Reset();
            SpellCheckerOptionsViewModel.Reset();
            SheetCopierViewModel.Reset();
            RunScriptSettingsViewModel.Reset();
        }
    }
}
