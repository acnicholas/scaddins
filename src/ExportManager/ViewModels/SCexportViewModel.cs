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

namespace SCaddins.ExportManager.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Data;
    using System.Windows.Input;
    using Caliburn.Micro;

    internal class SCexportViewModel : Screen
    {
        private readonly Manager exportManager;
        private CloseMode closeMode;
        private bool isClosing;
        private List<string> printTypes;
        private string searchText;
        private string selectedPrintType;
        private List<ExportSheet> selectedSheets = new List<ExportSheet>();
        private SheetFilter sheetFilter;
        private ObservableCollection<ExportSheet> sheets;
        private ICollectionView sheetsCollection;

        public SCexportViewModel(Manager exportManager)
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
                 exportManager.HasExportOption(ExportOptions.GhostscriptPDF));
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

        public bool IsSearchTextFocused
        {
            get; set;
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

        public bool SheetFilterEnabled { get; set; }

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
                if (exportManager.HasExportOption(ExportOptions.GhostscriptPDF))
                {
                    list.Add("gPDF");
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
            if (e == null || sender == null) {
                return;
            }
            if (e.OriginalSource.GetType() != typeof(System.Windows.Controls.TextBlock))
            {
                return;
            }
            var menuItem = (System.Windows.Controls.TextBlock)e.OriginalSource;
            if (menuItem.DataContext.GetType() != typeof(ExportSheet))
            {
                SheetFilterEnabled = false;
                return;
            }
            SheetFilterEnabled = true;
            ExportSheet myItem = (ExportSheet)menuItem.DataContext;
            if (!SelectedSheets.Contains(myItem))
            {
                SelectedSheets.Add(myItem);
            }
            SelectedSheet = myItem;
            var grid = (System.Windows.Controls.DataGrid)e.Source;
            var col = grid.CurrentColumn;
            var header = col.Header.ToString();
            var cell = ((System.Windows.Controls.TextBlock)e.OriginalSource).Text;
            SheetFilter = new SheetFilter(header, cell);
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

                case Key.G:
                    exportManager.ToggleExportOption(ExportOptions.GhostscriptPDF);
                    NotifyOfPropertyChange(() => StatusText);
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
                        FilterByNumber(((int)keyArgs.Key - (int)Key.D0).ToString(System.Globalization.CultureInfo.CurrentCulture));
                    }
                    break;
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
            SCaddinsApp.WindowManager.ShowMessageBox("Options");
            var optionsModel = new OptionsViewModel(exportManager);
            SCaddinsApp.WindowManager.ShowWindow(optionsModel, null, ViewModels.OptionsViewModel.DefaultWindowSettings);
            SCaddinsApp.WindowManager.ShowMessageBox("Options Success");
            NotifyOfPropertyChange(() => StatusText);
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

        public void MouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            OpenSheet.OpenViews(selectedSheets);
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
            NotifyOfPropertyChange(() => Sheets);
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