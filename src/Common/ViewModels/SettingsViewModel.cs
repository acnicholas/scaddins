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
        private ViewUtilitiesSettingsViewModel viewUtilitiesViewModel;
        private SpellCheckerOptionsViewModel spellCheckerOptionsViewModel;

        public SettingsViewModel(
            SCincrementViewModel svm,
            RoomConverter.ViewModels.SettingsViewModel roomConverterViewModel,
            ViewUtilitiesSettingsViewModel viewUtilitiesViewModel,
            SpellCheckerOptionsViewModel spellCheckerOptionsViewModel)
        {
            IncrementViewModel = svm;
            RoomConverterViewModel = roomConverterViewModel;
            ViewUtilitiesViewModel = viewUtilitiesViewModel;
            SpellCheckerOptionsViewModel = spellCheckerOptionsViewModel;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 400;
                settings.Title = "SCaddins Global Settings";
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                  new Uri("pack://application:,,,/SCaddins;component/Assets/gear.png"));
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                settings.ResizeMode = System.Windows.ResizeMode.CanResize;
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
        }
    }
}
