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
    using System;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Windows.Data;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    public class OpenSheetViewModel : Screen
    {
        private bool ctrlDown;
        private string searchInput;
        private CollectionViewSource searchResults;
        private OpenableView selectedSearchResult;
        private ViewType viewType;

        public OpenSheetViewModel(Document doc)
        {
            searchResults = new CollectionViewSource();
            searchResults.Source = OpenSheet.ViewsInModel(doc, true);
            viewType = ViewType.Undefined;
            SearchResults.Filter = v =>
            {
                OpenableView ov = v as OpenableView;
                if (string.IsNullOrEmpty(searchInput))
                {
                    return false;
                }
                return ov == null || ov.IsMatch(searchInput, viewType);
            };
            selectedSearchResult = null;
            ctrlDown = false;
            SearchInput = string.Empty;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Width = 480;
                settings.MaxHeight = 320;
                settings.WindowStyle = System.Windows.WindowStyle.None;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                  new Uri("pack://application:,,,/SCaddins;component/Assets/scexport.png"));
                settings.ShowInTaskbar = false;
                settings.ResizeMode = System.Windows.ResizeMode.NoResize;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                return settings;
            }
        }

        public string SearchInput
        {
            get
            {
                return searchInput;
            }

            set
            {
                if (value != searchInput)
                {
                    searchInput = value;
                    SearchResults.Refresh();
                }
            }
        }

        public ICollectionView SearchResults
        {
            get { return searchResults.View; }
        }

        public OpenableView SelectedSearchResult
        {
            get => selectedSearchResult;
            set => selectedSearchResult = value;
        }

        public string StatusText
        {
            get { return string.Format(System.Globalization.CultureInfo.CurrentCulture, "Type 'ctrl + h' for help. View Type Filter: {0}", viewType); }
        }

        public void Exit()
        {
            TryCloseAsync();
        }

        public void KeyDown(System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Escape)
            {
                TryCloseAsync(false);
            }
            if (args.Key == System.Windows.Input.Key.Enter)
            {
                if (SearchResults.IsEmpty)
                {
                    return;
                }
                if (selectedSearchResult == null)
                {
                    SelectNext();
                    selectedSearchResult.Open();
                    TryCloseAsync(true);
                }
                else
                {
                    selectedSearchResult.Open();
                    TryCloseAsync(true);
                }
            }
            if (args.Key == System.Windows.Input.Key.Tab)
            {
                SelectNext();
            }
            if (ctrlDown && args.Key == System.Windows.Input.Key.J)
            {
                SelectNext();
            }
            if (ctrlDown && args.Key == System.Windows.Input.Key.K)
            {
                SelectPrevious();
            }
            if (args.Key == System.Windows.Input.Key.LeftCtrl)
            {
                ctrlDown = true;
            }
            if (ctrlDown && args.Key == System.Windows.Input.Key.S)
            {
                ToggleFilterFlag(ViewType.DrawingSheet);
                NotifyOfPropertyChange(() => StatusText);
            }
            if (ctrlDown && args.Key == System.Windows.Input.Key.P)
            {
                ToggleFilterFlag(ViewType.FloorPlan);
                NotifyOfPropertyChange(() => StatusText);
            }
            if (ctrlDown && args.Key == System.Windows.Input.Key.E)
            {
                ToggleFilterFlag(ViewType.Elevation);
                NotifyOfPropertyChange(() => StatusText);
            }
            if (ctrlDown && args.Key == System.Windows.Input.Key.T)
            {
                ToggleFilterFlag(ViewType.Section);
                NotifyOfPropertyChange(() => StatusText);
            }

            if (ctrlDown && args.Key == System.Windows.Input.Key.H)
            {
                var helpMessage = "[Tab]\t-\tSelect Next" + Environment.NewLine +
                                  "[ctrl + j]\t-\tSelect Next" + Environment.NewLine +
                                  "[ctrl + k]\t-\tSelectPrevious" + Environment.NewLine +
                                  "[Esc]\t-\tExit" + Environment.NewLine +
                                  Environment.NewLine +
                                  "[ctrl + e]\t-\tOnly Search for Elevations" + Environment.NewLine +
                                  "[ctrl + p]\t-\tOnly Search for Plans" + Environment.NewLine +
                                  "[ctrl + s]\t-\tOnly Search for Sheets" + Environment.NewLine +
                                  "[ctrl + t]\t-\tOnly Search for Sections";

                SCaddinsApp.WindowManager.ShowMessageBox(helpMessage);
            }
        }

        public void KeyUp(System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.LeftCtrl)
            {
                ctrlDown = false;
            }
        }

        public void MouseDoubleClick()
        {
            selectedSearchResult.Open();
            TryCloseAsync();
        }

        public void SelectNext()
        {
            SearchResults.MoveCurrentToNext();
            if (SearchResults.IsCurrentAfterLast)
            {
                SearchResults.MoveCurrentToFirst();
            }
        }

        public void SelectPrevious()
        {
            SearchResults.MoveCurrentToPrevious();
            if (SearchResults.IsCurrentBeforeFirst)
            {
                SearchResults.MoveCurrentToLast();
            }
        }

        private void ToggleFilterFlag(ViewType vt)
        {
            viewType = viewType != vt ? vt : ViewType.Undefined;
            SearchResults.Refresh();
        }
    }
}