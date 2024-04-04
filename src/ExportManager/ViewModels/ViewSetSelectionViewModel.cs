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
    using Caliburn.Micro;

    internal class ViewSetSelectionViewModel : Screen
    {
        private ViewSetItem selectedSet;

        public ViewSetSelectionViewModel(ObservableCollection<ViewSetItem> sets)
        {
            Sets = sets;
            SelectedSet = null;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 480;
                settings.Title = "Select Template Sheet";
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                  new System.Uri("pack://application:,,,/SCaddins;component/Assets/scexport.png"));
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public bool OKEnabled
        {
            get
            {
                return SelectedSet != null;
            }
        }

        public ObservableCollection<ViewSetItem> Sets
        {
            get; set;
        }

        public ViewSetItem SelectedSet
        {
            get
            {
                return selectedSet;
            }

            set
            {
                selectedSet = value;
                NotifyOfPropertyChange(() => OKEnabled);
            }
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void OK()
        {
            TryCloseAsync(true);
        }

        public void RowDoubleClicked()
        {
            TryCloseAsync(true);
        }
    }
}
