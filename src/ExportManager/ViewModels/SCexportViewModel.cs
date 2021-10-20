﻿// (C) Copyright 2018-2021 by Andrew Nicholas
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

namespace SCaddins.ExportManager.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class SCexportViewModel : Screen
    {
        private readonly Manager exportManager;
        private CloseMode closeMode;
        private string currentColumnHeader;
        private bool isClosing;
        private List<string> printTypes;
        private List<ViewSetItem> recentExportSets;
        private string searchText;
        private string selectedPrintType;
        private List<ExportSheet> selectedSheets = new List<ExportSheet>();
        private SheetFilter sheetFilter;
        private ObservableCollection<ExportSheet> sheets;
        private ICollectionView sheetsCollection;

        public SCexportViewModel(Manager exportManager, List<Autodesk.Revit.DB.ViewSheet> preSelectedViews)
        {
            printTypes = (new[] { "Print A3", "Print A2", "Print Full Size" }).ToList();
            selectedPrintType = "Print A3";
            this.exportManager = exportManager;
            isClosing = false;
            closeMode = CloseMode.Exit;
            sheets = new ObservableCollection<ExportSheet>(exportManager.AllSheets);
            Sheets = CollectionViewSource.GetDefaultView(sheets);
            Sheets.SortDescriptions.Add(new SortDescription("FullExportName", ListSortDirection.Ascending));
            ShowSearchHint = true;
            sheetFilter = null;
            recentExportSets = RecentExport.GetAllUserViewSets(exportManager.AllViewSheetSets);
            recentExportSets = recentExportSets.OrderByDescending(v => v.CreationDate).ToList();
            PreSelectedViews = preSelectedViews;

            foreach (var viewSheet in preSelectedViews)
            {
                SelectedSheets.Add(sheets.Where(s => s.SheetNumber == viewSheet.SheetNumber).First());
            }
        }

        public enum CloseMode
        {
            Exit,
            Print,
            PrintA3,
            PrintA2,
            Export
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 480;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/SCaddins;component/Assets/scexport.png"));
                settings.Width = 768;
                settings.Title = "SCexport - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                return settings;
            }
        }

        public static string ExportButtonLabel => @"Export";

        public bool CanExport
        {
            get
            {
                return CanPrint &&
                (exportManager.HasExportOption(ExportOptions.DWG) ||
                 exportManager.HasExportOption(ExportOptions.PDF) ||
                 exportManager.HasExportOption(ExportOptions.DirectPDF));
            }
        }

        public bool CanPrint
        {
            get
            {
                return SelectedSheets.Count > 0;
            }
        }

        public CloseMode CloseStatus
        {
            get
            {
                return closeMode;
            }
        }

        public string ExportButtonToolTip
        {
            get
            {
                return CanExport ? "Export selected drawings. For further settings goto options." : "Select sheets to enable exporting.";
            }
        }

        public List<Autodesk.Revit.DB.ViewSheet> PreSelectedViews { get; private set; }

        public string InvlaidFileNamingStatusText
        {
            get
            {
                var invalidFileNames = exportManager.AllSheets.Count(s => s.ValidExportName != true);
                if (invalidFileNames > 0)
                {
                    return @" [Invalid file names: " + invalidFileNames + @"]";
                }

                return string.Empty;
            }
        }

        public string InvlaidPrintSettingsStatusText
        {
            get
            {
                var invalidPrintSettings = exportManager.AllSheets.Count(s => s.ValidPrintSettingIsAssigned != true);
                if (invalidPrintSettings > 0)
                {
                    return @" [Invalid print settings: " + invalidPrintSettings + @"]";
                }

                return string.Empty;
            }
        }

        public bool IsSearchTextFocused
        {
            get; set;
        }

        public bool PreviousExportFiveIsEnabled
        {
            get { return recentExportSets.Count > 4; }
        }

        public string PreviousExportFiveName
        {
            get
            {
                return PreviousExportFiveIsEnabled ? recentExportSets[4].DescriptiveName : "N/A";
            }
        }

        public bool PreviousExportFourIsEnabled
        {
            get { return recentExportSets.Count > 3; }
        }

        public string PreviousExportFourName
        {
            get
            {
                return PreviousExportFourIsEnabled ? recentExportSets[3].DescriptiveName : "N/A";
            }
        }

        public bool PreviousExportOneIsEnabled
        {
            get { return recentExportSets.Count > 0; }
        }

        public string PreviousExportOneName
        {
            get
            {
                return PreviousExportOneIsEnabled ? recentExportSets[0].DescriptiveName : "N/A";
            }
        }

        public bool PreviousExportThreeIsEnabled
        {
            get { return recentExportSets.Count > 2; }
        }

        public string PreviousExportThreeName
        {
            get
            {
                return PreviousExportThreeIsEnabled ? recentExportSets[2].DescriptiveName : "N/A";
            }
        }

        public bool PreviousExportTwoIsEnabled
        {
            get { return recentExportSets.Count > 1; }
        }

        public string PreviousExportTwoName
        {
            get
            {
                return PreviousExportTwoIsEnabled ? recentExportSets[1].DescriptiveName : "N/A";
            }
        }

        public string PrintButtonToolTip
        {
            get
            {
                return CanPrint ? "Print selected drawings. For further settings goto options." : "Select sheets to enable printing.";
            }
        }

        public BindableCollection<string> PrintTypes
        {
            get
            {
                return new BindableCollection<string>(printTypes);
            }
        }

        public string SearchText
        {
            get => searchText;

            set
            {
                if (value != searchText)
                {
                    searchText = value;
                }
            }
        }

        public string SelectedPrintType
        {
            get
            {
                return selectedPrintType;
            }

            set
            {
                selectedPrintType = value;
                NotifyOfPropertyChange(() => SelectedPrintType);
                PrintButton();
            }
        }

        public ExportSheet SelectedSheet
        {
            get; set;
        }

        public List<ExportSheet> SelectedSheets
        {
            get => selectedSheets;

            set
            {
                selectedSheets = value;
                NotifyOfPropertyChange(() => Sheets);
                NotifyOfPropertyChange(() => SelectedSheets);
                NotifyOfPropertyChange(() => CanPrint);
                NotifyOfPropertyChange(() => CanExport);
                NotifyOfPropertyChange(() => StatusText);
            }
        }

        public SheetFilter SheetFilter
        {
            get => sheetFilter;

            set
            {
                sheetFilter = value;
                NotifyOfPropertyChange(() => SheetFilter);
                NotifyOfPropertyChange(() => Sheets);
            }
        }

        public bool SheetFilterEnabled => true;

        public ICollectionView Sheets
        {
            get => sheetsCollection;

            set
            {
                sheetsCollection = value;
                NotifyOfPropertyChange(() => Sheets);
            }
        }

        public bool ShowSearchHint
        {
            get; set;
        }

        public string StatusText
        {
            get
            {
                var numberOfSheets = SelectedSheets.Count;
                return numberOfSheets + @" Sheet[s] Selected To Export/Print " + SelectedExportTypesAsString;
            }
        }

        public ObservableCollection<ViewSetItem> ViewSheetSets => exportManager.AllViewSheetSets;

        private string SelectedExportTypesAsString
        {
            get
            {
                List<string> list = new List<string>();
                if (exportManager.HasExportOption(ExportOptions.PDF))
                {
                    list.Add("PDF");
                }
                if (exportManager.HasExportOption(ExportOptions.DirectPDF))
                {
                    list.Add("rPDF");
                }
                if (exportManager.HasExportOption(ExportOptions.DWG))
                {
                    list.Add("DWG");
                }
                NotifyOfPropertyChange(() => CanExport);
                return @"[" + string.Join(",", list.ToArray()) + @"]";
            }
        }

        public void AddRevision()
        {
            var revisionSelectionViewModel = new RevisionSelectionViewModel(exportManager.Doc);
            bool? result = SCaddinsApp.WindowManager.ShowDialog(revisionSelectionViewModel, null, RevisionSelectionViewModel.DefaultWindowSettings);
            bool newBool = result.HasValue ? result.Value : false;
            if (newBool)
            {
                if (revisionSelectionViewModel.SelectedRevision != null)
                {
                    Manager.AddRevisions(selectedSheets, revisionSelectionViewModel.SelectedRevision.Id, exportManager.Doc);
                    NotifyOfPropertyChange(() => Sheets);
                }
            }
        }

        public void ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            if (e == null || sender == null)
            {
                return;
            }
            if (e.OriginalSource.GetType() != typeof(System.Windows.Controls.TextBlock))
            {
                return;
            }
            var menuItem = (System.Windows.Controls.TextBlock)e.OriginalSource;

            try
            {
                if (menuItem.DataContext.GetType() != typeof(ExportSheet))
                {
                    return;
                }
                ExportSheet myItem = (ExportSheet)menuItem.DataContext;
                if (!SelectedSheets.Contains(myItem))
                {
                    SelectedSheets.Add(myItem);
                }
                SelectedSheet = myItem;
                var element = (System.Windows.Controls.TextBlock)e.OriginalSource;
                var cell = element.Text;
                SheetFilter = new SheetFilter(currentColumnHeader, cell);
            }
            catch
            {
                //// FIXME
            }
        }

        public void CopySheets()
        {
            var sheetCopierModel = new SCaddins.SheetCopier.ViewModels.SheetCopierViewModel(exportManager.UIDoc);
            sheetCopierModel.AddSheets(selectedSheets);
            IsNotifying = false;
            SCaddinsApp.WindowManager.ShowDialog(
                sheetCopierModel,
                null,
                SheetCopier.ViewModels.SheetCopierViewModel.DefaultWindowSettings);
            IsNotifying = true;
        }

        public void CreateUserViews()
        {
            ViewUtilities.UserView.ShowSummaryDialog(
                ViewUtilities.UserView.Create(selectedSheets, exportManager.Doc));
        }

        public void DeleteHistory()
        {
            var result = RecentExport.DeleteAll(exportManager.Doc, exportManager.AllViewSheetSets);
            exportManager.UpdateAllViewSheetSets();
            recentExportSets = RecentExport.GetAllUserViewSets(exportManager.AllViewSheetSets);
            if (!result)
            {
                SCaddinsApp.WindowManager.ShowErrorMessageBox("Error deleteing history.", "Error deleteing history, maybe try deleting manually?...");
            }
        }

        public void Export()
        {
            isClosing = true;
            closeMode = CloseMode.Export;
            TryClose(true);
        }

        public void FixScaleBars()
        {
            Manager.FixScaleBars(selectedSheets, exportManager.Doc);
        }

        public void HideInSheetList()
        {
            Manager.HideSheetsInSheetList(selectedSheets, exportManager.Doc);
        }

        public void KeyPressed(KeyEventArgs keyArgs)
        {
            //// only execute search if in the search text box
            if (keyArgs.OriginalSource.GetType() == typeof(System.Windows.Controls.TextBox))
            {
                if (keyArgs.Key == Key.Enter)
                {
                    ExecuteSearch();
                    NotifyOfPropertyChange(() => Sheets);
                }
                return;
            }

            switch (keyArgs.Key)
            {
                case Key.C:
                    RemoveViewFilter();
                    break;

                case Key.D:
                    exportManager.ToggleExportOption(ExportOptions.DWG);
                    NotifyOfPropertyChange(() => StatusText);
                    break;

                case Key.N:
#if REVIT2022
                    exportManager.ToggleExportOption(ExportOptions.DirectPDF);
                    NotifyOfPropertyChange(() => StatusText);
#endif
                    break;

                case Key.L:
                    ShowLatestRevision();
                    break;

                case Key.O:
                    OpenViewsCommand();
                    break;

                case Key.P:
                    exportManager.ToggleExportOption(ExportOptions.PDF);
                    NotifyOfPropertyChange(() => StatusText);
                    break;

                case Key.S:
                    var activeSheetNumber = Manager.CurrentViewNumber(exportManager.Doc);
                    if (activeSheetNumber == null)
                    {
                        return;
                    }
                    ExportSheet ss = sheets.Where(item => item.SheetNumber.Equals(activeSheetNumber, StringComparison.CurrentCulture)).First();
                    SelectedSheet = ss;
                    NotifyOfPropertyChange(() => SelectedSheet);
                    break;

                case Key.Q:
                    OpenViewSet();
                    break;

                case Key.V:
                    VerifySheets();
                    break;

                case Key.W:
                    SaveViewSet();
                    break;

                case Key.Escape:
                    TryClose();
                    break;

                default:
                    if (keyArgs.Key >= Key.D0 && keyArgs.Key <= Key.D9)
                    {
                        int index = (int)keyArgs.Key - (int)Key.D0;
                        if (keyArgs.KeyboardDevice.IsKeyDown(Key.LeftShift) || keyArgs.KeyboardDevice.IsKeyDown(Key.RightShift))
                        {
                            if (index < recentExportSets.Count)
                            {
                                SelectPrevious(recentExportSets[index]);
                            }
                        }
                        else
                        {
                            FilterByNumber(index.ToString(System.Globalization.CultureInfo.CurrentCulture));
                        }
                    }
                    break;
            }
        }

        public void MouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            OpenSheet.OpenViews(selectedSheets);
        }

        public void MouseEnteredDataGrid(object sender, MouseEventArgs e)
        {
            try
            {
                if (e == null || sender == null)
                {
                    return;
                }
                if (e.OriginalSource.GetType() != typeof(TextBlock))
                {
                    return;
                }
                var menuItem = (TextBlock)e.OriginalSource;
                DataGridCell cell = FindVisualParent<DataGridCell>(menuItem);
                DataGridCellsPanel cellPanel = FindVisualParent<DataGridCellsPanel>(menuItem);
                DataGrid grid = FindVisualParent<DataGrid>(menuItem);
                int index = cellPanel.Children.IndexOf(cell);
                currentColumnHeader = grid.Columns[index].Header.ToString();
            }
            catch
            {
                //// FIXME
            }
        }

        public void OpenViewsCommand()
        {
            OpenSheet.OpenViews(selectedSheets);
        }

        public void OpenViewSet()
        {
            var viewSetSelectionViewModel = new ViewSetSelectionViewModel(exportManager.AllViewSheetSets);
            bool? result = SCaddinsApp.WindowManager.ShowDialog(viewSetSelectionViewModel, null, ViewSetSelectionViewModel.DefaultWindowSettings);
            bool newBool = result.HasValue ? result.Value : false;
            if (newBool && viewSetSelectionViewModel.SelectedSet != null)
            {
                IsNotifying = false;
                try
                {
                    var filter = new Predicate<object>(item => viewSetSelectionViewModel
                            .SelectedSet
                            .ViewIds.Contains(((ExportSheet)item).Sheet.Id.IntegerValue));
                    Sheets.Filter = filter;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
                IsNotifying = true;
            }
        }

        public void OptionsButton()
        {
            var optionsModel = new OptionsViewModel(exportManager);
            SCaddinsApp.WindowManager.ShowWindow(optionsModel, null, ViewModels.OptionsViewModel.DefaultWindowSettings);
            NotifyOfPropertyChange(() => StatusText);
        }

        public void HideFamilyOnSheet()
        {
            SCaddinsApp.WindowManager.ShowMessageBox("Hide Family On Sheet");

            List<FamilyInstance> familes = new List<FamilyInstance>();

            using (var collector = new FilteredElementCollector(exportManager.Doc, exportManager.Doc.ActiveView.Id))
            {
                collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
                collector.OfClass(typeof(FamilyInstance));
                foreach (var element in collector)
                {
                    var fi = (FamilyInstance)element;
                    SCaddinsApp.WindowManager.ShowMessageBox(fi.Name);
                    familes.Add(fi);
                }
            }

            if (familes.Count == 0)
            {
                return;
            }

            var list = new List<ElementId>();
            list.Add(familes[1].Id);

            using (var t = new Transaction(exportManager.Doc, "Hide Family"))
            {
                if (t.Start() == TransactionStatus.Started)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("Hiding");
                    exportManager.Doc.ActiveView.HideElements(list);
                    exportManager.Doc.Regenerate();
                    t.Commit();
                }
            }
        }

            public void PlaceFamilyOnSheet()
        {
            SCaddinsApp.WindowManager.ShowMessageBox("Place Family On Sheet");

            var family = TryGetDefaultSpotCoordFamily(exportManager.Doc,"RDR");
            var family2 = TryGetDefaultSpotCoordFamily(exportManager.Doc, "Family1");
            //// return;

            if (family == null || family2 == null)
            {
                return;
            }

            SCaddinsApp.WindowManager.ShowMessageBox("still going");

            using (var t = new Transaction(exportManager.Doc, "Place Family"))
            {
                if (t.Start() == TransactionStatus.Started)
                {
                    if (!family.IsActive)
                    {
                        family.Activate();
                        exportManager.Doc.Regenerate();
                    }

                    var box = family2.get_BoundingBox(exportManager.Doc.ActiveView);

                    XYZ newLocation = new XYZ(box.Min.X, box.Max.Y, 0);
                    exportManager.Doc.Create.NewFamilyInstance(
                        newLocation,
                        family, exportManager.Doc.ActiveView);
                    t.Commit();
                }
            }
            //ViewUtilities.Pin.PinSheetContents(selectedSheets, exportManager.Doc);
        }




        //// FIXME move this to utility class
        public static FamilySymbol TryGetDefaultSpotCoordFamily(Document doc, string name)
        {
            List<FamilySymbol> familes = new List<FamilySymbol>();

            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
                collector.OfClass(typeof(FamilySymbol));
                foreach (var element in collector)
                {
                    var fs = (FamilySymbol)element;
                    familes.Add(fs);
                }
            }

            SCaddinsApp.WindowManager.ShowMessageBox(familes.Count.ToString());


            foreach (FamilySymbol f in familes)
            {
                if (f.Name.ToUpper(CultureInfo.InvariantCulture).Contains(name.ToUpper(CultureInfo.InvariantCulture)))
                {
                    return f;
                }
            }
            SCaddinsApp.WindowManager.ShowMessageBox("Null");
            return null;
        }

        public void PinSheetContents()
        {
            ViewUtilities.Pin.PinSheetContents(selectedSheets, exportManager.Doc);
        }

        public void PrintButton()
        {
            isClosing = true;
            switch (selectedPrintType)
            {
                case "Print A3":
                    closeMode = CloseMode.PrintA3;
                    break;
                case "Print A2":
                    closeMode = CloseMode.PrintA2;
                    break;
                case "Print Full Size":
                    closeMode = CloseMode.Print;
                    break;
            }

            TryClose(true);
        }

        public void RemoveUnderlays()
        {
            ViewUtilities.ViewUnderlays.RemoveUnderlays(selectedSheets, exportManager.Doc);
        }

        public void RemoveViewFilter()
        {
            Sheets.Filter = null;
            SearchText = string.Empty;
            NotifyOfPropertyChange(() => Sheets);
            NotifyOfPropertyChange(() => SearchText);
            NotifyOfPropertyChange(() => CanPrint);
            NotifyOfPropertyChange(() => CanExport);
        }

        public void RenameSheets()
        {
            var renameManager = new RenameUtilities.RenameManager(
                exportManager.Doc,
                selectedSheets.Select(s => s.Id).ToList());
            var renameSheetModel = new SCaddins.RenameUtilities.ViewModels.RenameUtilitiesViewModel(renameManager);
            renameSheetModel.SelectedParameterCategory = "Sheets";
            SCaddinsApp.WindowManager.ShowDialog(renameSheetModel, null, RenameUtilities.ViewModels.RenameUtilitiesViewModel.DefaultWindowSettings);
            foreach (ExportSheet exportSheet in selectedSheets)
            {
                exportSheet.UpdateName();
                exportSheet.UpdateNumber();
            }
            NotifyOfPropertyChange(() => Sheets);
            NotifyOfPropertyChange(() => InvlaidFileNamingStatusText);
        }

        public void SaveViewSet()
        {
            var saveAsVm = new ViewSetSaveAsViewModel("Select name for new view sheet set", exportManager.AllViewSheetSets);
            bool? result = SCaddinsApp.WindowManager.ShowDialog(saveAsVm, null, ViewSetSaveAsViewModel.DefaultWindowSettings);
            bool newBool = result.HasValue ? result.Value : false;
            if (newBool)
            {
                exportManager.SaveViewSet(saveAsVm.SaveName, selectedSheets);
            }
        }

        public void SearchButton()
        {
            ExecuteSearch();
            NotifyOfPropertyChange(() => Sheets);
        }

        public void SearchFieldEntered()
        {
            ShowSearchHint = false;
            NotifyOfPropertyChange(() => ShowSearchHint);
        }

        public void SearchLabelMouseEnter()
        {
            ShowSearchHint = false;
            NotifyOfPropertyChange(() => ShowSearchHint);
        }

        public void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            if (!isClosing)
            {
                IsNotifying = false;
                List<ExportSheet> list = ((System.Windows.Controls.DataGrid)sender).SelectedItems.Cast<ExportSheet>().ToList();
                IsNotifying = true;
                SelectedSheets = list;
            }
        }

        public void SelectPrevious(int i)
        {
            SelectPrevious(recentExportSets[i]);
        }

        public void SelectPrevious(ViewSetItem viewSet)
        {
            if (viewSet == null)
            {
                return;
            }

            IsNotifying = false;
            try
            {
                var filter = new Predicate<object>(item => viewSet.ViewIds.Contains(((ExportSheet)item).Sheet.Id.IntegerValue));
                Sheets.Filter = filter;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            IsNotifying = true;
        }

        public void SheetFilterSelected()
        {
            if (SheetFilter != null)
            {
                Sheets.Filter = SheetFilter.GetFilter();
            }
        }

        public void ShowInSheetList()
        {
            Manager.ShowSheetsInSheetList(selectedSheets, exportManager.Doc);
        }

        public void ShowLatestRevision()
        {
            var revDate = Manager.LatestRevisionDate(exportManager.Doc);
            IsNotifying = false;
            try
            {
                var filter = new Predicate<object>(item => ((ExportSheet)item).SheetRevisionDate.Equals(revDate, StringComparison.CurrentCulture));
                Sheets.Filter = filter;
                NotifyOfPropertyChange(() => Sheets);
            }
            catch (Exception exception)
            {
                SCaddinsApp.WindowManager.ShowMessageBox(exception.Message);
            }
            IsNotifying = true;
        }

        public void TurnNorthPointsOff()
        {
            Manager.ToggleNorthPoints(selectedSheets, exportManager.Doc, false);
        }

        public void TurnNorthPointsOn()
        {
            Manager.ToggleNorthPoints(selectedSheets, exportManager.Doc, true);
        }

        public void VerifySheets()
        {
            exportManager.Update();
        }

        private static T FindVisualParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent == null)
            {
                return null;
            }

            var parentT = parent as T;
            return parentT ?? FindVisualParent<T>(parent);
        }

        private void ExecuteSearch()
        {
            if (SearchText == null)
            {
                return;
            }

            IsNotifying = false;
            try {
                var filter = new Predicate<object>(
                    item =>
                        ((item != null) &&
                         (-1 < ((ExportSheet)item).SheetDescription.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase)))
                        ||
                        (item != null &&
                         -1 < ((ExportSheet)item).SheetNumber.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase)));
                if (Sheets.CanFilter) {
                    Sheets.Filter = filter;
                }
            }
            catch (Exception exception) {
                SCaddinsApp.WindowManager.ShowMessageBox(exception.Message);
            }

            IsNotifying = true;      
        }

        private void FilterByNumber(string number)
        {
            Manager.CurrentViewNumber(exportManager.Doc);
            try {
                var filter = new Predicate<object>(item =>
                    Regex.IsMatch(((ExportSheet)item).SheetNumber, @"^\D*" + number));
                Sheets.Filter = filter;
            }
            catch (Exception exception) {
                SCaddinsApp.WindowManager.ShowMessageBox(exception.Message);
            }
        }
    }
}