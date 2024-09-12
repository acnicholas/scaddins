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

namespace SCaddins.SheetCopier.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;

    internal class SheetCopierViewModel : Screen
    {
        private SheetCopierManager copyManager;
        private SheetCopierViewHost selectedViewHost;
        private BindableCollection<SheetInformation> selectedSheetInformation = new BindableCollection<SheetInformation>();
        private List<SheetCopierViewHost> selectedSheets = new List<SheetCopierViewHost>();
        private List<SheetCopierView> selectedViews = new List<SheetCopierView>();

        public SheetCopierViewModel(UIDocument uidoc)
        {
            copyManager = new SheetCopierManager(uidoc);
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 600;
                settings.Width = 1024;
                settings.Title = "Sheet Copier - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                return settings;
            }
        }

        public bool AddCurrentSheetIsEnabled => true;

        public string AddCurrentSheetLabel
        {
            get
            {
                switch (copyManager.ActiveViewType)
                {
                    case Autodesk.Revit.DB.ViewType.ProjectBrowser:
                        return "Add Current Project Browser Selction";
                    case Autodesk.Revit.DB.ViewType.DrawingSheet:
                        return "Add Current Sheet";
                    default:
                        return "Add Current View";
                }
            }
        }

        public string ChildViewsTitleLabel
        {
            get
            {
                if (SelectedViewHost != null)
                {
                    switch (SelectedViewHost.Type)
                    {
                        case ViewHostType.Model:
                            return "Independent Views (no parent sheet)";
                        case ViewHostType.Sheet:
                            return "Views on sheet: " + SelectedViewHost.Number + "-" + SelectedViewHost.Title;
                    }
                }
                return "Views";
            }
        }

        public bool CopySheetSelectionIsEnabled
        {
            get
            {
                return selectedViewHost != null && this.selectedViewHost.SourceSheet != null;
            }
        }

        public bool CustomSheetParameterOneIsVisible => copyManager.CustomSheetParametersOne.Count > 1;

        public bool CustomSheetParameterTwoIsVisible => copyManager.CustomSheetParametersTwo.Count > 1;

        public bool CustomSheetParameterThreeIsVisible => copyManager.CustomSheetParametersThree.Count > 1;

        public string PrimaryCustomSheetParameterName => SheetCopierManager.PrimaryCustomSheetParameterName;

        public string SecondaryCustomSheetParameterName => SheetCopierManager.SecondaryCustomSheetParameterName;

        public string TertiaryCustomSheetParameterName => SheetCopierManager.TertiaryCustomSheetParameterName;

        public string PrimaryCustomSheetParameterColumnHeader => PrimaryCustomSheetParameterName.Replace("_", "__");

        public string SecondaryCustomSheetParameterColumnHeader => SecondaryCustomSheetParameterName.Replace("_", "__");

        public string TertiaryCustomSheetParameterColumnHeader => TertiaryCustomSheetParameterName.Replace("_", "__");

        public string GoLabel
        {
            get
            {
                var independentViewCount = copyManager.IndependentViewCount;
                var sheetsCount = independentViewCount == 0 ? ViewHosts.Count : ViewHosts.Count - 1;
                if (GoLabelIsEnabled)
                {
                    return "Copy " + sheetsCount + " Sheet(s) and " + independentViewCount + " Independent Views";
                }
                else
                {
                    return "Copy";
                }
            }
        }

        public bool GoLabelIsEnabled => ViewHosts.Count > 0;

        public bool RemoveSelectedViewsIsEnabled => selectedViews.Count > 0;

        public bool RemoveSheetSelectionIsEnabled => selectedViewHost != null;

        public string RemoveViewsLabel => selectedViews.Count < 2 ? "Remove View" : "Remove Views";

        public SheetCopierViewHost SelectedViewHost
        {
            get
            {
                return selectedViewHost;
            }

            set
            {
                if (value != selectedViewHost)
                {
                    selectedViewHost = value;
                    NotifyOfPropertyChange(() => SelectedSheetInformationView);
                    NotifyOfPropertyChange(() => ChildViews);
                    NotifyOfPropertyChange(() => SelectedViewHostName);
                }
            }
        }

        public BindableCollection<SheetInformation> SelectedSheetInformation
        {
            get
            {
                selectedSheetInformation.Clear();
                if (selectedViewHost != null && selectedViewHost.Type == ViewHostType.Sheet)
                {
                    selectedSheetInformation.Add(new SheetInformation(selectedViewHost.SourceSheet));
                    foreach (Autodesk.Revit.DB.ElementId id in selectedViewHost.SourceSheet.GetAllPlacedViews())
                    {
                        Autodesk.Revit.DB.Element element = copyManager.Doc.GetElement(id);
                        selectedSheetInformation.Add(new SheetInformation(element));
                    }
                    foreach (Autodesk.Revit.DB.Parameter param in selectedViewHost.SourceSheet.Parameters)
                    {
                        selectedSheetInformation.Add(new SheetInformation(param));
                    }
                    return selectedSheetInformation;
                }
                else
                {
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

        public string SelectedViewHostName => SelectedViewHost.Number + " - " + SelectedViewHost.Title;

        public ObservableCollection<SheetCopierViewHost> ViewHosts => copyManager.ViewHosts;

        /// <summary>
        /// A child view is a view that is belongs to either a sheet or the model.
        /// </summary>
        public ObservableCollection<SheetCopierView> ChildViews
        {
            get { return SelectedViewHost.ChildViews; }
        }

        public void AddCurrentView()
        {
            copyManager.AddCurrentView();
            NotifyOfPropertyChange(() => GoLabel);
            NotifyOfPropertyChange(() => GoLabelIsEnabled);
        }

        public void AddSheets()
        {
            var vm = new ViewSelectionViewModel(copyManager);
            var task = SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, ViewSelectionViewModel.DefaultWindowSettings);
            bool newBool = task.Result ?? false;
            if (newBool)
            {
                AddSheets(vm.SelectedViews);
                NotifyOfPropertyChange(() => GoLabel);
                NotifyOfPropertyChange(() => GoLabelIsEnabled);
            }
        }

        public void AddSheets(List<ExportManager.ExportSheet> sheetSelection)
        {
            foreach (var sheet in sheetSelection)
            {
                copyManager.AddSheet(sheet.Sheet);
            }
        }

        public void AddSheets(List<Autodesk.Revit.DB.View> viewSelection)
        {
            foreach (var view in viewSelection)
            {
                if (view is Autodesk.Revit.DB.ViewSheet)
                {
                    copyManager.AddSheet(view as Autodesk.Revit.DB.ViewSheet);
                }
                else
                {
                    copyManager.AddView(view as Autodesk.Revit.DB.View);
                }
            }
        }

        public void CopySheetSelection()
        {
            if (SelectedViewHost.SourceSheet != null)
            {
                copyManager.AddSheet(SelectedViewHost.SourceSheet);
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
            TryCloseAsync(true);
        }

        public void RemoveSelectedViews()
        {
            foreach (var s in selectedViews.ToList())
            {
                ChildViews.Remove(s);
            }
            if (SelectedViewHost.Type == ViewHostType.Model)
            {
                if (SelectedViewHost.ChildViews.Count == 0)
                {
                    ViewHosts.Remove(SelectedViewHost);
                }
                else
                {
                    SelectedViewHost.Title = "<" + SelectedViewHost.ChildViews.Count.ToString() + " Independent Views>";
                }
            }
        }

        public void RemoveSheetSelection()
        {
            foreach (var s in selectedSheets.ToList())
            {
                ViewHosts.Remove(s);
            }
            NotifyOfPropertyChange(() => GoLabel);
            NotifyOfPropertyChange(() => GoLabelIsEnabled);
        }

        public void RowSheetSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            try
            {
                selectedSheets.AddRange(obj.AddedItems.Cast<SheetCopierViewHost>());
                obj.RemovedItems.Cast<SheetCopierViewHost>().ToList().ForEach(w => selectedSheets.Remove(w));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            NotifyOfPropertyChange(() => GoLabel);
            NotifyOfPropertyChange(() => ChildViewsTitleLabel);
            NotifyOfPropertyChange(() => CopySheetSelectionIsEnabled);
            NotifyOfPropertyChange(() => RemoveSheetSelectionIsEnabled);
            NotifyOfPropertyChange(() => GoLabelIsEnabled);
        }

        public void ChildViewsRowSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            try
            {
                selectedViews.AddRange(obj.AddedItems.Cast<SheetCopierView>());
                obj.RemovedItems.Cast<SheetCopierView>().ToList().ForEach(w => selectedViews.Remove(w));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            NotifyOfPropertyChange(() => RemoveSelectedViewsIsEnabled);
            NotifyOfPropertyChange(() => RemoveViewsLabel);
        }
    }
}
