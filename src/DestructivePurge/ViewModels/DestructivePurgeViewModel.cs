namespace SCaddins.DestructivePurge.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Autodesk.Revit.DB;

    class DestructivePurgeViewModel
    {
        private ObservableCollection<CheckableItem> checkableItems;

        public DestructivePurgeViewModel(Autodesk.Revit.DB.Document doc)
        {
            checkableItems = new ObservableCollection<CheckableItem>();
            var images = new CheckableItem(new DeletableItem("Images"));
            images.AddChildren(SCwashUtilities.Images(doc));
            var imports = new CheckableItem(new DeletableItem("Imports"));
            imports.AddChildren(SCwashUtilities.Imports(doc, false));
        }

        public ObservableCollection<CheckableItem> CheckableItems
        {
            get
            {
                return checkableItems;
            }
        }
    }
}
