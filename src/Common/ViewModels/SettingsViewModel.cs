namespace SCaddins.Common.ViewModels
{
    using System;
    using Caliburn.Micro;

    public class SettingsViewModel : Screen
    {
        private ISettingPanel incrementViewModel;
        private ISettingPanel roomConverterViewModel;
        private ISettingPanel viewUtilitiesViewModel;
        private ISettingPanel spellCheckerOptionsViewModel;
        private ISettingPanel sheetCopierViewModel;

        public SettingsViewModel(
            ISettingPanel svm,
            ISettingPanel roomConverterViewModel,
            ISettingPanel viewUtilitiesViewModel,
            ISettingPanel sheetCopierViewModel,
            ISettingPanel spellCheckerOptionsViewModel)
        {
            IncrementViewModel = svm;
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
                settings.Width = 640;
                settings.Title = "SCaddins Global Settings";
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                  new Uri("pack://application:,,,/SCaddins;component/Assets/gear.png"));
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResize;
                return settings;
            }
        }

        public ISettingPanel RoomConverterViewModel
        {
            get
            {
                return roomConverterViewModel;
            }

            set
            {
                roomConverterViewModel = value;
                NotifyOfPropertyChange(() => RoomConverterViewModel);
            }
        }

        public ISettingPanel IncrementViewModel
        {
            get
            {
                return incrementViewModel;
            }

            set
            {
                incrementViewModel = value;
                NotifyOfPropertyChange(() => IncrementViewModel);
            }
        }

        public ISettingPanel SheetCopierViewModel
        {
            get
            {
                return sheetCopierViewModel;
            }

            set
            {
                sheetCopierViewModel = value;
                NotifyOfPropertyChange(() => SheetCopierViewModel);
            }
        }

        public ISettingPanel SpellCheckerOptionsViewModel
        {
            get
            {
                return spellCheckerOptionsViewModel;
            }

            set
            {
                spellCheckerOptionsViewModel = value;
                NotifyOfPropertyChange(() => SpellCheckerOptionsViewModel);
            }
        }

        public ISettingPanel ViewUtilitiesViewModel
        {
            get
            {
                return viewUtilitiesViewModel;
            }

            set
            {
                viewUtilitiesViewModel = value;
                NotifyOfPropertyChange(() => ViewUtilitiesViewModel);
            }
        }

        public void Apply()
        {
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
