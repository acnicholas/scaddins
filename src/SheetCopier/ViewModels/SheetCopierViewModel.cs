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

namespace SCaddins.SheetCopier.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq;
    using System.Windows.Data;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;

    internal class SheetCopierViewModel : Screen
    {
        private SheetCopierManager copyManager;
        private List<string> levelsInModel = new List<string>();
        private SheetCopierSheet selectedSheet;
        private BindableCollection<SheetInformation> selectedSheetInformation = new BindableCollection<SheetInformation>();
        private List<SheetCopierSheet> selectedSheets = new List<SheetCopierSheet>();
        private List<SheetCopierViewOnSheet> selectedViews = new List<SheetCopierViewOnSheet>();

        public SheetCopierViewModel(UIDocument uidoc)
        {
            copyManager = new SheetCopierManager(uidoc);
            levelsInModel = copyManager.Levels.Select(k => k.Key).ToList();
            RunAfterClose = false;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 480;
                settings.Width = 768;
                settings.Title = "Sheet Copier - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public string GoLabel
        {
            get { return "Copy " + Sheets.Count + " Sheets"; }
        }

        public bool RunAfterClose
        {
            get; set;
        }

        public SheetCopierSheet SelectedSheet
        {
            get
            {
                return selectedSheet;
            }

            set
            {
                if (value != selectedSheet) {
                    selectedSheet = value;
                    NotifyOfPropertyChange(() => SelectedSheetInformationView);
                    NotifyOfPropertyChange(() => ViewsOnSheet);
                    NotifyOfPropertyChange(() => SelectedSheetName);
                }
            }
        }

        public BindableCollection<SheetInformation> SelectedSheetInformation
        {
            get
            {
                selectedSheetInformation.Clear();
                if (selectedSheet != null) {
                    selectedSheetInformation.Add(new SheetInformation(selectedSheet.SourceSheet));
                    foreach (Autodesk.Revit.DB.ElementId id in selectedSheet.SourceSheet.GetAllPlacedViews()) {
                        Autodesk.Revit.DB.Element element = copyManager.Doc.GetElement(id);
                        selectedSheetInformation.Add(new SheetInformation(element));
                    }
                    foreach (Autodesk.Revit.DB.Parameter param in selectedSheet.SourceSheet.Parameters) {
                        selectedSheetInformation.Add(new SheetInformation(param));
                    }
                    return selectedSheetInformation;
                } else {
                    return null;
                }
            }
        }

        public CollectionView SelectedSheetInformationView
        {
            get
            {
                CollectionView result = (CollectionView)CollectionViewSource.GetDefaultView(SelectedSheetInformation);
                PropertyGroupDescription gd = new PropertyGroupDescription("IndexType");
                result.GroupDescriptions.Clear();
                result.GroupDescriptions.Add(gd);
                return result;
            }
        }

        public string SelectedSheetName
        {
            get
            {
                return SelectedSheet.Number + " - " + SelectedSheet.Title;
            }
        }

        public ObservableCollection<SheetCopierSheet> Sheets
        {
            get { return copyManager.Sheets; }
        }

        public ObservableCollection<SheetCopierViewOnSheet> ViewsOnSheet
        {
            get { return SelectedSheet.ViewsOnSheet; }
        }

        public void AddCurrentSheet()
        {
            copyManager.AddCurrentSheet();
            NotifyOfPropertyChange(() => GoLabel);
        }

        public void AddSheetSelection()
        {
            var vm = new ViewModels.SheetSelectionViewModel(copyManager.Doc);
            SCaddinsApp.WindowManager.ShowDialog(vm, null, ViewModels.SheetSelectionViewModel.DefaultWindowSettings);
        }

        public void AddSheets(List<SCaddins.ExportManager.ExportSheet> selectedSheets)
        {
            foreach (var sheet in selectedSheets) {
                copyManager.AddSheet(sheet.Sheet);
            }
        }

        public void CopySheetSelection()
        {
            if (SelectedSheet.SourceSheet != null) {
                copyManager.AddSheet(SelectedSheet.SourceSheet);
                NotifyOfPropertyChange(() => GoLabel);
            }
        }

        public void CreateSheets()
        {
            copyManager.CreateSheets();
        }

        public void Go()
        {
            copyManager.CreateSheets();
        }

        public void RemoveSelectedViews()
        {
            foreach (var s in selectedViews.ToList()) {
                ViewsOnSheet.Remove(s);
            }
        }

        public void RemoveSheetSelection()
        {
            foreach (var s in selectedSheets.ToList()) {
                Sheets.Remove(s);
            }
            NotifyOfPropertyChange(() => GoLabel);
        }

        public void RowSheetSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            try {
                selectedSheets.AddRange(obj.AddedItems.Cast<SheetCopierSheet>());
                obj.RemovedItems.Cast<SheetCopierSheet>().ToList().ForEach(w => selectedSheets.Remove(w));
            } catch {
            }
        }

        public void RowViewsOnSheetSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            try {
                selectedViews.AddRange(obj.AddedItems.Cast<SheetCopierViewOnSheet>());
                obj.RemovedItems.Cast<SheetCopierViewOnSheet>().ToList().ForEach(w => selectedViews.Remove(w));
            } catch {
            }
        }
    }
}
