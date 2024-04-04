// (C) Copyright 2018-2020 by Andrew Nicholas
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

namespace SCaddins.ExportManager.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq;
    using Caliburn.Micro;

    public class ViewSetSaveAsViewModel : Screen
    {
        private string label;
        private string name;

        public ViewSetSaveAsViewModel(string label, ObservableCollection<ViewSetItem> allViewSheetSets)
        {
            this.label = label;
            ViewSheetSets = allViewSheetSets;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 160;
                settings.Width = 320;
                settings.Title = "Save View Set";
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                  new System.Uri("pack://application:,,,/SCaddins;component/Assets/scexport.png"));
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public ObservableCollection<ViewSetItem> ViewSheetSets
        {
            get; private set;
        }

        public bool CanSave
        {
            get { return !ViewSheetSets.Select(n => n.Name).Contains(SaveName) && !string.IsNullOrEmpty(SaveName); }
        }

        public string Label
        {
            get
            {
                if (CanSave)
                {
                    return label;
                }
                else
                {
                    return string.IsNullOrEmpty(SaveName) ? label : "ERROR: Name is in use";
                }
            }

            set
            {
                if (value != label)
                {
                    label = value;
                    NotifyOfPropertyChange(() => Label);
                }
            }
        }

        public string SaveName
        {
            get
            {
                return name;
            }

            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyOfPropertyChange(() => CanSave);
                    NotifyOfPropertyChange(() => Label);
                }
            }
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void Save()
        {
            TryCloseAsync(true);
        }
    }
}
