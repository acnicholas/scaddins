namespace SCaddins.Common.ViewModels
{
    using System;
    using Caliburn.Micro;

    public class SettingsViewModel : Screen
    {
        private static ISettingPanel incrementViewModel;
        private static ISettingPanel roomConverterViewModel;
        private static ISettingPanel viewUtilitiesViewModel;
        private static ISettingPanel spellCheckerOptionsViewModel;
        private static ISettingPanel sheetCopierViewModel;

        public SettingsViewModel(
            ISettingPanel incrementViewModel,
            ISettingPanel roomConverterViewModel,
            ISettingPanel viewUtilitiesViewModel,
            ISettingPanel sheetCopierViewModel,
            ISettingPanel spellCheckerOptionsViewModel)
        {
            IncrementViewModel = incrementViewModel;
            RoomConverterViewModel = roomConverterViewModel;
            ViewUtilitiesViewModel = viewUtilitiesViewModel;
            SheetCopierViewModel = sheetCopierViewModel;
            SpellCheckerOptionsViewModel = spellCheckerOptionsViewModel;
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
            if (IncrementViewModel == null) SCaddinsApp.WindowManager.ShowErrorMessageBox("NO","Bummer");
            ParameterUtilities.ViewModels.SCincrementViewModel svm = IncrementViewModel as ParameterUtilities.ViewModels.SCincrementViewModel; ;
            SCaddinsApp.WindowManager.ShowMessageBox(svm.SourceSearchPattern);
            IncrementViewModel.Apply();
            ViewUtilitiesViewModel.Apply();
            SpellCheckerOptionsViewModel.Apply();
            SheetCopierViewModel.Apply();
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
            SpellCheckerOptionsViewModel.Reset();
            SheetCopierViewModel.Reset();
        }
    }
}
