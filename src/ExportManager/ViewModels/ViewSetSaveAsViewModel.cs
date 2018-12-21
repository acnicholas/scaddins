// (C) Copyright 2018 by Andrew Nicholas
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

    class ViewSetSaveAsViewModel : Screen
    {
        private string label;

        public ViewSetSaveAsViewModel(string label, ObservableCollection<ViewSetItem> AllViewSheetSets)
        {
            Label = label;
            this.AllViewSheetSets = AllViewSheetSets;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 164;
                settings.Width = 640;
                settings.Title = "Save View Set";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public ObservableCollection<ViewSetItem> AllViewSheetSets
        {
            get; set;
        }

        public string Label
        {
            get
            {
                return label;
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

        public string Name
        {
            get; set;
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public bool SaveButtonEnabled()
        {
            return !AllViewSheetSets.Select(n => n.Name).Contains(Label);
        }

        public void Save()
        {
            TryClose(true);
        }
    }
}
