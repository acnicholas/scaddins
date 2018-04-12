namespace SCaddins.DestructivePurge.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    class DestructivePurgeViewModel : PropertyChangedBase
    {
        private ObservableCollection<CheckableItem> checkableItems;
        private CheckableItem selectedItem;
        //System.Windows.Media.Imaging.BitmapImage previewImage;

        public DestructivePurgeViewModel(Autodesk.Revit.DB.Document doc)
        {
            checkableItems = new ObservableCollection<CheckableItem>();
            selectedItem = null;
            //previewImage = null;

            var viewNotOnSheets = new CheckableItem(new DeletableItem("Views NOT On Sheets"));
            foreach (ViewType enumValue in Enum.GetValues(typeof(ViewType)))
            {
                if (enumValue == ViewType.DrawingSheet)
                    continue;
                var i = new CheckableItem(new DeletableItem(enumValue.ToString()));
                i.AddChildren(SCwashUtilities.Views(doc, false, enumValue));
                if (i.Children.Count > 0)
                    viewNotOnSheets.AddChild(i);
            }
            checkableItems.Add(viewNotOnSheets);

            var viewOnSheets = new CheckableItem(new DeletableItem("Views On Sheets"));
            foreach (ViewType enumValue in Enum.GetValues(typeof(ViewType)))
            {
                if (enumValue == ViewType.DrawingSheet)
                    continue;
                var i = new CheckableItem(new DeletableItem(enumValue.ToString()));
                i.AddChildren(SCwashUtilities.Views(doc, true, enumValue));
                if(i.Children.Count > 0)
                    viewOnSheets.AddChild(i);
            }
            checkableItems.Add(viewOnSheets);

            var sheets = new CheckableItem(new DeletableItem("Sheets"));
            sheets.AddChildren(SCwashUtilities.Views(doc, true, ViewType.DrawingSheet));
            checkableItems.Add(sheets);

            var images = new CheckableItem(new DeletableItem("Images"));
            images.AddChildren(SCwashUtilities.Images(doc));
            checkableItems.Add(images);
            var imports = new CheckableItem(new DeletableItem("CAD Imports"));
            imports.AddChildren(SCwashUtilities.Imports(doc, false));
            checkableItems.Add(imports);
            var links = new CheckableItem(new DeletableItem("CAD Links"));
            links.AddChildren(SCwashUtilities.Imports(doc, true));
            checkableItems.Add(links);
            var revisions = new CheckableItem(new DeletableItem("Revisions"));
            revisions.AddChildren(SCwashUtilities.Revisions(doc));
            checkableItems.Add(revisions);
            var uvf = new CheckableItem(new DeletableItem("Unused View Filters"));
            uvf.AddChildren(SCwashUtilities.UnusedViewFilters(doc));
            checkableItems.Add(uvf);
            var ubr = new CheckableItem(new DeletableItem("Unbound Rooms"));
            ubr.AddChildren(SCwashUtilities.UnboundRooms(doc));
            checkableItems.Add(ubr);
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
                NotifyOfPropertyChange(() => CheckableItems);
            }
        }

        public void SelectedItemChanged(CheckableItem item)
        {
            selectedItem = item;
            NotifyOfPropertyChange(() => Details);
            //PreviewImage = SCwashUtilities.ToBitmapImage(item.Deletable.PreviewImage);
        }

        //public System.Windows.Media.Imaging.BitmapImage PreviewImage
        //{
        //    set
        //    {
        //        //if (previewImage != value)
        //        //{
        //            previewImage = value;
        //            Autodesk.Revit.UI.TaskDialog.Show("test", previewImage.Width.ToString());
        //            NotifyOfPropertyChange(() => PreviewImage);
        //        //}
        //    }
        //    get { return previewImage; }
        //}

        public string Details
        {
            get { return selectedItem.Deletable.Info;  }
        }

        public void ShowElement()
        {

        }

        public void DeleteElements()
        {

        }


    }
}
