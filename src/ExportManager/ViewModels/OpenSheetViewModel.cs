using System.Linq;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace SCaddins.ExportManager.ViewModels 
{
    class OpenSheetViewModel : Screen
    {
        private OpenableView selectedSearchResult;
        private ObservableCollection<OpenableView> viewsInDoc;
        private CollectionViewSource searchResults;
        private string searchInput;

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
                    this.searchResults.Source = this.viewsInDoc.Where(v => v.IsMatch(searchInput)).Take(searchInput.Length > 3 ? 20 : 10);
                    NotifyOfPropertyChange(() => SearchInput);
                    NotifyOfPropertyChange(() => SearchResults);
                }
            }
        }

        public OpenableView SelectedSearchResult
        {
            get
            {
                return selectedSearchResult;
            }
            set
            {
                if (value != selectedSearchResult) {
                    selectedSearchResult = value;
                }
            }
        }

        public ICollectionView SearchResults
        {
            get { return this.searchResults.View; }
        }

        public void SelectNext()
        {
            SearchResults.MoveCurrentToNext();
            if (SearchResults.IsCurrentAfterLast) SearchResults.MoveCurrentToFirst();
        }

        public void SelectPrevious()
        {
            SearchResults.MoveCurrentToPrevious();
            if (SearchResults.IsCurrentBeforeFirst) SearchResults.MoveCurrentToLast();
        }

        public void KeyDown(System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Escape) {
                TryClose();
            }
            if (args.Key == System.Windows.Input.Key.Enter) {
                selectedSearchResult.Open();
            }
        }

        public void Exit()
        {
            TryClose();
        }

        public OpenSheetViewModel(Autodesk.Revit.DB.Document doc)
        {
            viewsInDoc = new ObservableCollection<OpenableView>(OpenSheet.ViewsInModel(doc));
            this.searchResults = new CollectionViewSource();
            this.searchResults.Source = this.viewsInDoc;
            selectedSearchResult = null;
            SearchInput = string.Empty;
        }
    }
}
