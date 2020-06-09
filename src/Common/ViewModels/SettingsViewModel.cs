using System;
using System.Collections.Generic;
namespace SCaddins.Common.ViewModels
{

    using Caliburn.Micro;
    using ParameterUtilities.ViewModels;
    using SCaddins.SheetCopier.ViewModels;
    using SCaddins.ViewUtilities.ViewModels;
    using SpellChecker.ViewModels;

    class SettingsViewModel : Screen
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
            SpellCheckerOptionsViewModel spellCheckerOptionsViewModel
            )
        {
            IncrementViewModel = svm;
            RoomConverterViewModel = roomConverterViewModel;
            SheetCopierViewModel = sheetCopierViewModel;
            ViewUtilitiesViewModel = viewUtilitiesViewModel;
            SpellCheckerOptionsViewModel = spellCheckerOptionsViewModel;
        }

        public RoomConverter.ViewModels.SettingsViewModel RoomConverterViewModel
        {
            get { return roomConverterViewModel; }
            set
            {
                roomConverterViewModel = value;
                NotifyOfPropertyChange(() => RoomConverterViewModel);
            }

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

        public SheetCopierSettingsViewModel SheetCopierViewModel
        {
            get { return sheetCopierViewModel; }
            set
            {
                sheetCopierViewModel = value;
                NotifyOfPropertyChange(() => SheetCopierViewModel); 
                    
            }
        }

        public SpellCheckerOptionsViewModel SpellCheckerOptionsViewModel
        {
            get { return spellCheckerOptionsViewModel; }
            set
            {
                spellCheckerOptionsViewModel = value;
                NotifyOfPropertyChange(() => SpellCheckerOptionsViewModel);
            }
        }

        public ViewUtilitiesSettingsViewModel ViewUtilitiesViewModel
        {
            get { return viewUtilitiesViewModel; }
            set
            {
                viewUtilitiesViewModel = value;
                NotifyOfPropertyChange(() => ViewUtilitiesViewModel);

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
