namespace SCaddins.DestructivePurge.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    class DestructivePurgeViewModel : PropertyChangedBase
    {
        private ObservableCollection<CheckableItem> checkableItems;
        private CheckableItem selectedItem;
        private Document doc;
        System.Windows.Media.Imaging.BitmapImage previewImage;

        public DestructivePurgeViewModel(Autodesk.Revit.DB.Document doc)
        {
            checkableItems = new ObservableCollection<CheckableItem>();
            selectedItem = null;
            this.doc = doc;
            previewImage = null;

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
            selectedItem = checkableItems[0];
            NotifyOfPropertyChange(() => ShowButtonLabel);
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
            NotifyOfPropertyChange(() => ShowButtonLabel);
            PreviewImage = SCwashUtilities.ToBitmapImage(item.Deletable.PreviewImage);
        }

        public System.Windows.Media.Imaging.BitmapImage PreviewImage
        {
            set
            {
                if (previewImage != value) {
                    previewImage = value;
                    NotifyOfPropertyChange(() => PreviewImage);
                    NotifyOfPropertyChange(() => ImageHeight);
                    NotifyOfPropertyChange(() => ImageWidth);
                    NotifyOfPropertyChange(() => ImageMargin);
                }
            }
            get { return previewImage; }
        }

        public int ImageWidth
        {
            get
            {
                return PreviewImage != null ? 196 : 0;
            }
        }


        public int ImageHeight
        {
            get
            {
                return PreviewImage != null ? 196 : 0;
            }
        }


        public int ImageMargin
        {
            get
            {
                return PreviewImage != null ? 5 : 0;
            }
        }


        public string Details
        {
            get
            {
                if (selectedItem.Deletable.Info == "-") {
                    return "Select an element to view additional properties";
                } else {
                    return selectedItem.Deletable.Info;
                }
            }
        }

        public void ShowElement()
        {
        }

        public string ShowButtonLabel
        {
            get
            {
                return selectedItem == null ? "Show Element" : "Show Element " + selectedItem.Deletable.Id.ToString();
            }
        }

        public void RecurseItems(List<ElementId> list, CheckableItem item)
        {
            foreach (var child in item.Children)
            {
                if (child.IsChecked)
                {
                    if (child.Deletable.Id != null)
                        list.Add(child.Deletable.Id);
                    RecurseItems(list, child);
                }
            }
        }

        public void DeleteElements()
        {
            List<ElementId> toDelete = new List<ElementId>();
            foreach (var item in CheckableItems)
            {
                if (item.IsChecked)
                {
                    if (item.Deletable.Id != null)
                        toDelete.Add(item.Deletable.Id);
                    RecurseItems(toDelete, item);
                }
            }
            SCwashUtilities.RemoveElements(doc,toDelete);
        }




    }
}
