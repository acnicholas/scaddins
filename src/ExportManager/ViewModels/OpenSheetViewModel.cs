using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace SCaddins.ExportManager.ViewModels 
{
    class OpenSheetViewModel : Screen
    {
        private OpenableView selectedSearchResult;
        private List<OpenableView> viewsInDoc;
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

        public List<OpenableView> SearchResults
        {
            get
            {  
                if (searchInput.Length > 1) {
                    return viewsInDoc
                        .Where(ov => ov.Name.Contains(searchInput.ToUpper()) || ov.SheetNumber.Contains(searchInput.ToUpper()))
                        .Take(searchInput.Length > 3 ? 20 : 10)
                        .ToList();
                }
                return null;
            }
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

        public OpenSheetViewModel(Autodesk.Revit.DB.Document doc)
        {
            viewsInDoc = OpenSheet.ViewsInModel(doc);
            selectedSearchResult = null;
        }
    }
}
