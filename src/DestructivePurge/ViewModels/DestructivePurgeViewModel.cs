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

namespace SCaddins.DestructivePurge.ViewModels
{
    using Autodesk.Revit.DB;
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal class DestructivePurgeViewModel : PropertyChangedBase
    {
        private ObservableCollection<CheckableItem> checkableItems;
        private Document doc;
        private System.Windows.Media.Imaging.BitmapImage previewImage;
        private CheckableItem selectedItem;

        public DestructivePurgeViewModel(Autodesk.Revit.DB.Document doc)
        {
            checkableItems = new ObservableCollection<CheckableItem>();
            selectedItem = null;
            this.doc = doc;
            previewImage = null;

            var viewNotOnSheets = new CheckableItem(new DeletableItem("Views NOT On Sheets"), null);
            foreach (ViewType enumValue in Enum.GetValues(typeof(ViewType)))
            {
                if (enumValue == ViewType.DrawingSheet) {
                    continue;
                }
                var i = new CheckableItem(new DeletableItem(enumValue.ToString()), viewNotOnSheets);
                i.AddChildren(SCwashUtilities.Views(doc, false, enumValue));
                if (i.Children.Count > 0) {
                    viewNotOnSheets.AddChild(i);
                }
            }
            checkableItems.Add(viewNotOnSheets);

            var viewOnSheets = new CheckableItem(new DeletableItem("Views On Sheets"), null);
            foreach (ViewType enumValue in Enum.GetValues(typeof(ViewType)))
            {
                if (enumValue == ViewType.DrawingSheet) {
                    continue;
                }
                var i = new CheckableItem(new DeletableItem(enumValue.ToString()), viewOnSheets);
                i.AddChildren(SCwashUtilities.Views(doc, true, enumValue));
                if (i.Children.Count > 0)
                    viewOnSheets.AddChild(i);
            }
            checkableItems.Add(viewOnSheets);

            var sheets = new CheckableItem(new DeletableItem("Sheets"), null);
            sheets.AddChildren(SCwashUtilities.Views(doc, true, ViewType.DrawingSheet));
            checkableItems.Add(sheets);
            var images = new CheckableItem(new DeletableItem("Images"), null);
            images.AddChildren(SCwashUtilities.Images(doc));
            checkableItems.Add(images);
            var imports = new CheckableItem(new DeletableItem("CAD Imports"), null);
            imports.AddChildren(SCwashUtilities.Imports(doc, false));
            checkableItems.Add(imports);
            var links = new CheckableItem(new DeletableItem("CAD Links"), null);
            links.AddChildren(SCwashUtilities.Imports(doc, true));
            checkableItems.Add(links);
            var revisions = new CheckableItem(new DeletableItem("Revisions"), null);
            revisions.AddChildren(SCwashUtilities.Revisions(doc));
            checkableItems.Add(revisions);
            var uvf = new CheckableItem(new DeletableItem("Unused View Filters"), null);
            uvf.AddChildren(SCwashUtilities.UnusedViewFilters(doc));
            checkableItems.Add(uvf);
            var ubr = new CheckableItem(new DeletableItem("Unbound Rooms"), null);
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

        public string Details
        {
            get
            {
                if (selectedItem.Deletable.Info == "-")
                {
                    return "Select an element to view additional properties";
                }
                else
                {
                    return selectedItem.Deletable.Info;
                }
            }
        }

        public bool EnableShowElemant
        {
            get { return selectedItem.Deletable.Id != null; }
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

        public int ImageWidth
        {
            get
            {
                return PreviewImage != null ? 196 : 0;
            }
        }

        public System.Windows.Media.Imaging.BitmapImage PreviewImage
        {
            set
            {
                if (previewImage != value)
                {
                    previewImage = value;
                    NotifyOfPropertyChange(() => PreviewImage);
                    NotifyOfPropertyChange(() => ImageHeight);
                    NotifyOfPropertyChange(() => ImageWidth);
                    NotifyOfPropertyChange(() => ImageMargin);
                }
            }

            get
            {
                return previewImage;
            }
        }

        public string ShowButtonLabel
        {
            get
            {
                return selectedItem.Deletable.Id == null ? "Select Element" : "Show Element " + selectedItem.Deletable.Id.ToString();
            }
        }

        public void DeleteElements()
        {
            List<ElementId> toDelete = new List<ElementId>();
            foreach (var item in CheckableItems)
            {
                if (item.IsChecked.Value == true)
                {
                    if (item.Deletable.Id != null)
                        toDelete.Add(item.Deletable.Id);
                    RecurseItems(toDelete, item);
                }
            }
            SCwashUtilities.RemoveElements(doc, toDelete);
        }

        public void RecurseItems(List<ElementId> list, CheckableItem item)
        {
            foreach (var child in item.Children)
            {
                if (child.IsChecked.Value == true) {
                    if (child.Deletable.Id != null) {
                        list.Add(child.Deletable.Id);
                    }
                    RecurseItems(list, child);
                }
            }
        }

        public void SelectedItemChanged(CheckableItem item)
        {
            selectedItem = item;
            NotifyOfPropertyChange(() => Details);
            NotifyOfPropertyChange(() => ShowButtonLabel);
            NotifyOfPropertyChange(() => EnableShowElemant);
            PreviewImage = SCwashUtilities.ToBitmapImage(item.Deletable.PreviewImage);
        }

        public void ShowElement()
        {
            if (selectedItem.Deletable.Id != null)
            {
                var uiapp = new Autodesk.Revit.UI.UIApplication(doc.Application);
                Element e = doc.GetElement(selectedItem.Deletable.Id);
                Type t = e.GetType();
                if (e is Autodesk.Revit.DB.View)
                {
                    uiapp.ActiveUIDocument.ActiveView = (Autodesk.Revit.DB.View)e;
                }
                else
                {
                    uiapp.ActiveUIDocument.ShowElements(selectedItem.Deletable.Id);
                }
            }
        }
    }
}