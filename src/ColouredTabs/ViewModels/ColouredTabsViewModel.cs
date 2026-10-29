// (C) Copyright 2026 by Andrew Nicholas
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

namespace SCaddins.ColouredTabs.ViewModels
{
    using Caliburn.Micro;
    using ColorPickerWPF;
    using SCaddins.ModelSetupWizard;
    using System.Collections.Generic;

    public class ColouredTabsViewModel : PropertyChangedBase, Common.ViewModels.ISettingPanel
    {

        public ColouredTabsViewModel()
        {
            Colours = new BindableCollection<System.Windows.Media.Color>();
            Reset();
        }

        public bool ColorizeDocTabs
        {
            get
            {
                return Settings1.Default.ColorizeDocs;
            }
            set
            {
                Settings1.Default.ColorizeDocs = value;
                Settings1.Default.Save();
            }
        }
        public bool SortDocTabs
        {
            get
            {
                return Settings1.Default.SortColorizeDocs;
            }
            set
            {
                Settings1.Default.SortColorizeDocs = value;
                Settings1.Default.Save();
            }
        }
        public int TabStyleIndex
        {
            get
            {
                return Settings1.Default.TabstyleIndex;
            }
            set
            {
                Settings1.Default.TabstyleIndex = value;
                Settings1.Default.Save();
            }
        }
        public int FamilyTabStyleIndex
        {
            get
            {
                return Settings1.Default.FamilyTabstyleIndex;
            }
            set
            {
                Settings1.Default.FamilyTabstyleIndex = value;
                Settings1.Default.Save();
            }
        }



        public BindableCollection<System.Windows.Media.Color> Colours
        {
            get; private set;
        }

        public void Colour(int c)
        {
            if (ColorPickerWindow.ShowDialog(out var selected) == true)
            {
                Colours[c] = selected;
                Settings1.Default.Colours[c] = ColourUtils.HexFromColor(selected);
            }
            NotifyOfPropertyChange(() => Colours);
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 400;
                settings.Title = "ColouredTab Settings";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                settings.ResizeMode = System.Windows.ResizeMode.CanResize;
                return settings;
            }
        }

        public void ResetToDefault()
        {
            Reset();
            Settings1.Default.Reset();
        }

        public void Apply()
        {
            //Settings1.Default.Colours[0] =  ColourUtils.HexFromColor(Colours[0]);
            //Settings1.Default.Colours[1] =  ColourUtils.HexFromColor(Colours[1]);
            //Settings1.Default.Colours[2] =  ColourUtils.HexFromColor(Colours[2]);
            //Settings1.Default.Colours[3] =  ColourUtils.HexFromColor(Colours[3]);
            //Settings1.Default.Colours[4] =  ColourUtils.HexFromColor(Colours[4]);
            //Settings1.Default.Colours[5] =  ColourUtils.HexFromColor(Colours[5]);
            //Settings1.Default.Colours[6] =  ColourUtils.HexFromColor(Colours[6]);
            //Settings1.Default.Colours[7] =  ColourUtils.HexFromColor(Colours[7]);
            //Settings1.Default.Colours[8] =  ColourUtils.HexFromColor(Colours[8]);
            //Settings1.Default.Colours[9] =  ColourUtils.HexFromColor(Colours[9]);
            //Settings1.Default.Colours[10] = ColourUtils.HexFromColor(Colours[10]);
            //Settings1.Default.Colours[11] = ColourUtils.HexFromColor(Colours[11]);
            Settings1.Default.Save();
            NotifyOfPropertyChange(() => Colours);
        }

        public void Reset()
        {
            Colours.Clear();
            Colours.Insert(0,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[0]));
            Colours.Insert(1,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[1]));
            Colours.Insert(2,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[2]));
            Colours.Insert(3,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[3]));
            Colours.Insert(4,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[4]));
            Colours.Insert(5,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[5]));
            Colours.Insert(6,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[6]));
            Colours.Insert(7,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[7]));
            Colours.Insert(8,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[8]));
            Colours.Insert(9,  ColourUtils.HexToMediaColor(Settings1.Default.Colours[9]));
            Colours.Insert(10, ColourUtils.HexToMediaColor(Settings1.Default.Colours[10]));
            Colours.Insert(11, ColourUtils.HexToMediaColor(Settings1.Default.Colours[11]));
            NotifyOfPropertyChange(() => Colours);
        }
    }
}