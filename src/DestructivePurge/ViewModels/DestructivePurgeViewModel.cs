namespace SCaddins.DestructivePurge.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    class DestructivePurgeViewModel : PropertyChangedBase
    {
        private ObservableCollection<CheckableItem> checkableItems;

        public DestructivePurgeViewModel(Autodesk.Revit.DB.Document doc)
        {
            checkableItems = new ObservableCollection<CheckableItem>();
            var images = new CheckableItem(new DeletableItem("Images"));
            images.AddChildren(SCwashUtilities.Images(doc));
            checkableItems.Add(images);
            var imports = new CheckableItem(new DeletableItem("Imports"));
            imports.AddChildren(SCwashUtilities.Imports(doc, false));
            checkableItems.Add(imports);
            var revisions = new CheckableItem(new DeletableItem("Revisions"));
            revisions.AddChildren(SCwashUtilities.Revisions(doc));
            checkableItems.Add(revisions);
            var revisions = new CheckableItem(new DeletableItem("Revisions"));
            revisions.AddChildren(SCwashUtilities.Revisions(doc));
            checkableItems.Add(revisions);
        }

        public ObservableCollection<CheckableItem> CheckableItems
        {
            get
            {
                return checkableItems;
            }
            set
            {
                checkableItems = value;
            }
        }
    }
}
