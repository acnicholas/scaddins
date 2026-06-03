// (C) Copyright 2019-2020 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.ModelSetupWizard.ViewModels
{
    using Autodesk.Revit.DB;
    using Caliburn.Micro;
    using ColorPickerWPF;
    using ColorPickerWPF.Code;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    public class ModelSetupWizardViewModel : Screen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "doc")]
        private Document doc;
        private ModelSetupWizardOptionsViewModel optionsVm;
        private ColourScheme selectedColourScheme;
        private int tabControlSeelctedIndex;

        public ModelSetupWizardViewModel(Autodesk.Revit.UI.UIDocument uidoc)
        {
            doc = uidoc.Document;
            optionsVm = new ModelSetupWizardOptionsViewModel();
            ColourSchemes = optionsVm.ColourSchemes;
            selectedColourScheme = ColourSchemes[0];
            tabControlSeelctedIndex = 0;

            var iniFile = IniIO.GetIniFile(doc);
            if (iniFile.Length > 0)
            {
                var colors = IniIO.ReadColours(iniFile);
                Colours = new BindableCollection<System.Windows.Media.Color>(colors);
            }
            else
            {
                SCaddinsApp.WindowManager.ShowMessageBox(iniFile + " does not exist");
            }
        }

        public string ApplyButtonLabel
        {
            get
            {
            return "Write Colours to Revit.ini";
            }
        }


        public int TabControlSelectedIndex
        {
            get {
                return tabControlSeelctedIndex;
            }
            set
            {
                tabControlSeelctedIndex = value;
                NotifyOfPropertyChange(() => ApplyButtonLabel);
                NotifyOfPropertyChange(() => TabControlSelectedIndex);
            }
        }

        public BindableCollection<System.Windows.Media.Color> Colours
        {
            get; private set;
        }

        public BindableCollection<ColourScheme> ColourSchemes
        {
            get; private set;
        }

        public void Colour(int c)
        {
            if (ColorPickerWindow.ShowDialog(out var selected) == true)
            {
                Colours[c] = selected;
                NotifyOfPropertyChange(() => Colours);
            }
        }

        public void Colour01()
        {
            if (ColorPickerWindow.ShowDialog(out var selected) == true)
            {
                Colours[0] = selected;
                NotifyOfPropertyChange(() => Colours);
            }
        }

        public void Colour02()
        {
            if (ColorPickerWindow.ShowDialog(out var selected) == true)
            {
                Colours[1] = selected;
                NotifyOfPropertyChange(() => Colours);
            }
        }

        public void Colour03()
        {
            if (ColorPickerWindow.ShowDialog(out var selected) == true)
            {
                Colours[2] = selected;
                NotifyOfPropertyChange(() => Colours);
            }
        }

        public ColourScheme SelectedColourScheme
        {
            get
            {
                return selectedColourScheme;
            }

            set
            {
                selectedColourScheme = value;
                for (int i = 0; i < selectedColourScheme.Colors.Count; i++)
                {
                    Colours[i] = selectedColourScheme.Colors[i];
                }
                NotifyOfPropertyChange(() => SelectedColourScheme);
                NotifyOfPropertyChange(() => Colours);
            }
        }

        public void Apply()
        {
            if (TabControlSelectedIndex == 2)
            {
                var iniFile = IniIO.GetIniFile(doc);
                if (iniFile.Length > 0)
                {
                    IniIO.WriteColours(iniFile, Colours.ToList());
                }
                else
                {
                    SCaddinsApp.WindowManager.ShowMessageBox(iniFile + " does not exist");
                }
            }
            TryCloseAsync(true);
        }

        public void Cancel()
        {
            TryCloseAsync(true);
        }

        public void Options()
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 640;
            settings.Width = 800;
            settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                  new System.Uri("pack://application:,,,/SCaddins;component/Assets/checkdoc.png"));
            settings.Title = "Model Setup Wizard Options";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Width;
            SCaddinsApp.WindowManager.ShowDialogAsync(optionsVm, null, settings);
        }
    }
}
