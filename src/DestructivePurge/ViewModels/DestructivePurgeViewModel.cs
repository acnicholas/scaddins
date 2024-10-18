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

namespace SCaddins.DestructivePurge.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    public class DestructivePurgeViewModel : Screen
    {
        private readonly Document doc;
        private readonly bool isFamily;
        private ObservableCollection<CheckableItem> checkableItems;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "doc")]
        private System.Windows.Media.Imaging.BitmapImage previewImage;
        private CheckableItem selectedItem;

        public DestructivePurgeViewModel(Document doc)
        {
            this.doc = doc;
            isFamily = doc.IsFamilyDocument;
            CheckableItems = GetPurgableItems();
            selectedItem = null;
            previewImage = null;
            selectedItem = CheckableItems[0];
            NotifyOfPropertyChange(() => ShowButtonLabel);
        }

        public ObservableCollection<CheckableItem> CheckableItems
        {
            get => checkableItems;

            set
            {
                checkableItems = value;
                NotifyOfPropertyChange(() => CheckableItems);
            }
        }

        public int CheckedCount
        {
            get
            {
                int n = 0;
                foreach (CheckableItem ci in CheckableItems)
                {
                    if (ci.IsYesOrMaybe)
                    {
                        n += ci.CheckedCount;
                    }
                }
                return n;
            }
        }

        public string DeleteButtonLabel
        {
            get
            {
                return "Delete " + CheckedCount;
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

        public bool EnableShowElemant => selectedItem.Deletable.Id != null;

        public int ImageHeight => PreviewImage != null ? 196 : 0;

        public int ImageMargin => PreviewImage != null ? 5 : 0;

        public int ImageWidth => PreviewImage != null ? 196 : 0;

        public System.Windows.Media.Imaging.BitmapImage PreviewImage
        {
            get => previewImage;

            set
            {
                if (previewImage == value)
                {
                    return;
                }
                previewImage = value;
                NotifyOfPropertyChange(() => PreviewImage);
                NotifyOfPropertyChange(() => ImageHeight);
                NotifyOfPropertyChange(() => ImageWidth);
                NotifyOfPropertyChange(() => ImageMargin);
            }
        }

        public string ShowButtonLabel => ShowButtonIsVisible ? "Select Element" : "Show Element " + selectedItem.Deletable.Id;

        public bool ShowButtonIsVisible => selectedItem.Deletable.Id != null;

        public void CollapseAll(object sender)
        {
            if (!(sender is TreeView treeView))
            {
                return;
            }
            foreach (var item in treeView.Items)
            {
                if (item is TreeViewItem treeViewItem)
                {
                    treeViewItem.IsExpanded = false;
                }
            }
        }

        public void DeleteElements()
        {
            List<DeletableItem> toDelete = new List<DeletableItem>();
            foreach (var item in CheckableItems)
            {
                if (item.IsYes)
                {
                    if (item.Deletable.Id != null && doc.GetElement(item.Deletable.Id).IsValidObject)
                    {
                        toDelete.Add(item.Deletable);
                    }
                    RecurseItems(toDelete, item);
                }
                if (item.IsMaybe)
                {
                    RecurseItems(toDelete, item);
                }
            }

            IsNotifying = false;
            DestructivePurgeUtilitiles.RemoveElements(doc, toDelete);
            IsNotifying = true;
            CheckableItems = GetPurgableItems();
        }

        public ObservableCollection<CheckableItem> GetPurgableItems()
        {
            var result = new ObservableCollection<CheckableItem>();
            var viewNotOnSheets = new CheckableItem(new DeletableItem("Views NOT On Sheets"), null);
            foreach (ViewType enumValue in Enum.GetValues(typeof(ViewType)))
            {
                if (enumValue == ViewType.DrawingSheet)
                {
                    continue;
                }
                var i = new CheckableItem(new DeletableItem(enumValue.ToString()), viewNotOnSheets);
                i.AddChildren(DestructivePurgeUtilitiles.Views(doc, false, enumValue));
                if (i.Children.Count > 0)
                {
                    viewNotOnSheets.AddChild(i);
                }
            }
            result.Add(viewNotOnSheets);

            var viewOnSheets = new CheckableItem(new DeletableItem("Views On Sheets"), null);
            foreach (ViewType enumValue in Enum.GetValues(typeof(ViewType)))
            {
                if (enumValue == ViewType.DrawingSheet)
                {
                    continue;
                }
                var i = new CheckableItem(new DeletableItem(enumValue.ToString()), viewOnSheets);
                i.AddChildren(DestructivePurgeUtilitiles.Views(doc, true, enumValue));
                if (i.Children.Count > 0)
                {
                    viewOnSheets.AddChild(i);
                }
            }
            result.Add(viewOnSheets);

            var sheets = new CheckableItem(new DeletableItem("Sheets"), null);
            sheets.AddChildren(DestructivePurgeUtilitiles.Views(doc, true, ViewType.DrawingSheet));
            result.Add(sheets);
            var images = new CheckableItem(new DeletableItem("Images"), null);
            images.AddChildren(DestructivePurgeUtilitiles.Images(doc));
            result.Add(images);
            var imports = new CheckableItem(new DeletableItem("CAD Imports"), null);
            imports.AddChildren(DestructivePurgeUtilitiles.Imports(doc, false));
            result.Add(imports);
            var links = new CheckableItem(new DeletableItem("CAD Links"), null);
            links.AddChildren(DestructivePurgeUtilitiles.Imports(doc, true));
            result.Add(links);
            var revisions = new CheckableItem(new DeletableItem("Revisions"), null);
            revisions.AddChildren(DestructivePurgeUtilitiles.Revisions(doc));
            result.Add(revisions);
            if (!isFamily)
            {
                var uvf = new CheckableItem(new DeletableItem("Unused View Filters"), null);
                uvf.AddChildren(DestructivePurgeUtilitiles.UnusedViewFilters(doc));
                result.Add(uvf);
            }
            var ubr = new CheckableItem(new DeletableItem("Unbound Rooms"), null);
            ubr.AddChildren(DestructivePurgeUtilitiles.UnboundRooms(doc));
            result.Add(ubr);
            return result;
        }

        public void RecurseItems(List<DeletableItem> list, CheckableItem item)
        {
            foreach (var child in item.Children)
            {
                if (child.IsYes)
                {
                    if (child.Deletable.Id != null && doc.GetElement(child.Deletable.Id).IsValidObject)
                    {
                        list.Add(child.Deletable);
                    }
                    RecurseItems(list, child);
                }
                if (child.IsMaybe)
                {
                    RecurseItems(list, child);
                }
            }
        }

        public void SelectAll()
        {
            SelectAllOrNone(true);
        }

        public void SelectedItemChanged(CheckableItem item)
        {
            selectedItem = item;
            NotifyOfPropertyChange(() => Details);
            NotifyOfPropertyChange(() => ShowButtonLabel);
            NotifyOfPropertyChange(() => ShowButtonIsVisible);
            NotifyOfPropertyChange(() => EnableShowElemant);
            NotifyOfPropertyChange(() => DeleteButtonLabel);
            PreviewImage = DestructivePurgeUtilitiles.ToBitmapImage(item.Deletable.PreviewImage);
        }

        public void SelectNone()
        {
            SelectAllOrNone(false);
        }

        public void ShowElement()
        {
            if (selectedItem.Deletable.Id != null)
            {
                var uiapp = new Autodesk.Revit.UI.UIApplication(doc.Application);
                Element e = doc.GetElement(selectedItem.Deletable.Id);
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

        public void TreeViewSourceUpdated()
        {
            NotifyOfPropertyChange(() => DeleteButtonLabel);
        }

        private void SelectAllOrNone(bool selectAll)
        {
            foreach (var item in CheckableItems)
            {
                item.IsChecked = selectAll;
            }
        }
    }
}