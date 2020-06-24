namespace SCaddins.Common.ViewModels
{
    using System;
    using Caliburn.Micro;
    using ParameterUtilities.ViewModels;
    using SCaddins.SheetCopier.ViewModels;
    using SCaddins.ViewUtilities.ViewModels;
    using SpellChecker.ViewModels;

    public class SettingsViewModel : Screen
    {
        private SCincrementViewModel incrementViewModel;
        private RoomConverter.ViewModels.SettingsViewModel roomConverterViewModel;
        private SheetCopierSettingsViewModel sheetCopierViewModel;
        private ViewUtilitiesSettingsViewModel viewUtilitiesViewModel;
        private SpellCheckerOptionsViewModel spellCheckerOptionsViewModel;

        public SettingsViewModel(
            SCincrementViewModel svm,
            RoomConverter.ViewModels.SettingsViewModel roomConverterViewModel,
            SheetCopierSettingsViewModel sheetCopierViewModel,
            ViewUtilitiesSettingsViewModel viewUtilitiesViewModel,
            SpellCheckerOptionsViewModel spellCheckerOptionsViewModel)
        {
            IncrementViewModel = svm;
            RoomConverterViewModel = roomConverterViewModel;
            SheetCopierViewModel = sheetCopierViewModel;
            ViewUtilitiesViewModel = viewUtilitiesViewModel;
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
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public RoomConverter.ViewModels.SettingsViewModel RoomConverterViewModel
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

        public SCincrementViewModel IncrementViewModel
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

        public SheetCopierSettingsViewModel SheetCopierViewModel
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

        public SpellCheckerOptionsViewModel SpellCheckerOptionsViewModel
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

        public ViewUtilitiesSettingsViewModel ViewUtilitiesViewModel
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
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(true);
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
