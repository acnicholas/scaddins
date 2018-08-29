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
    using System.ComponentModel;
    using System.Dynamic;
    using System.Windows.Data;
    using Caliburn.Micro;

    public class OpenSheetViewModel : Screen
    {
        private bool ctrlDown;
        private string searchInput;
        private CollectionViewSource searchResults;
        private OpenableView selectedSearchResult;

        public OpenSheetViewModel(Autodesk.Revit.DB.Document doc)
        {
            this.searchResults = new CollectionViewSource();
            this.searchResults.Source = OpenSheet.ViewsInModel(doc, true);
            SearchResults.Filter  = v => {
                ////using (SearchResults.DeferRefresh()) {
                    OpenableView ov = v as OpenableView;
                    return ov == null || ov.IsMatch(searchInput);
                ////}
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
                settings.Width = 640;
                settings.MaxHeight = 480;
                settings.WindowStyle = System.Windows.WindowStyle.None;
                settings.ShowInTaskbar = false;
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
                if (value != searchInput) {
                    searchInput = value;
                    SearchResults.Refresh();
                }
                NotifyOfPropertyChange(() => ShowHelpText);
                NotifyOfPropertyChange(() => ShowExtendedHelpText);
                NotifyOfPropertyChange(() => ShowSearchresults);
            }
        }

        public ICollectionView SearchResults
        {
            get { return this.searchResults.View; }
        }

        public OpenableView SelectedSearchResult
        {
            get
            {
                return selectedSearchResult;
            }

            set
            {
                if (value != selectedSearchResult)
                {
                    selectedSearchResult = value;
                }
            }
        }

        public bool ShowExtendedHelpText
        {
            get
            {
                return searchInput == "?";
            }
        }

        public bool ShowHelpText
        {
            get
            {
                return searchInput.Length < 1;
            }
        }

        public bool ShowSearchresults
        {
            get
            {
                return !ShowExtendedHelpText;
            }
        }

        public void Exit()
        {
            TryClose();
        }

        public void KeyDown(System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Escape) {
                TryClose(false);
            }
            if (args.Key == System.Windows.Input.Key.Enter) {
                if (SearchResults.IsEmpty) {
                    return;
                }
                if (selectedSearchResult == null) {
                    SelectNext();
                    selectedSearchResult.Open();
                    TryClose(true);
                } else {
                    selectedSearchResult.Open();
                    TryClose(true);
                }
            }
            if (ctrlDown && args.Key == System.Windows.Input.Key.J) {
                SelectNext();
            }
            if (ctrlDown && args.Key == System.Windows.Input.Key.K) {
                SelectPrevious();
            }
            if (args.Key == System.Windows.Input.Key.LeftCtrl) {
                ctrlDown = true;
            }
        }

        public void KeyUp(System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.LeftCtrl) {
                ctrlDown = false;
            }
        }

        public void MouseDoubleClick()
        {
            selectedSearchResult.Open();
            TryClose();
        }

        public void SelectNext()
        {
            SearchResults.MoveCurrentToNext();
            if (SearchResults.IsCurrentAfterLast) {
                SearchResults.MoveCurrentToFirst();
            }
        }

        public void SelectPrevious()
        {
            SearchResults.MoveCurrentToPrevious();
            if (SearchResults.IsCurrentBeforeFirst) {
                SearchResults.MoveCurrentToLast();
            }
        }
    }
}