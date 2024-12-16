// (C) Copyright 2018-2023 by Andrew Nicholas
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
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Autodesk.Internal.InfoCenter;
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
            printTypes = GetAvailablePrinters();
            if (printTypes.Count > 0)
            {
                selectedPrintType = printTypes[0];
            } else
            {
                printTypes.Add("No Valid Printers");
                selectedPrintType = printTypes[0];
            }
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
                settings.Height = 600;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/SCaddins;component/Assets/scexport.png"));
                settings.Width = 1024;
                settings.Title = "SCexport - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = SizeToContent.Manual;
                settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                return settings;
            }
        }

        // FIXME, export and print are confusing as you "print" a pdf.
        public string ExportButtonLabel
        {
                get
                {
                        var s = "Export ";  
                        if (ExportDirectPDF || ExportPDF || ExportPDF24)
                        {
                                s += "PDF";
                        }
                        if (ExportDWG)
                        {
                                if (s.Length > 7)
                                {
                                        s += " & DWG";
                                } else {
                                        s += "DWG";
                                }
                        }
                        return s;
                }
        }

        public bool ExportDWG
        {
            get
            {
                return exportManager.HasExportOption(ExportOptions.DWG);
            }

            set
            {
                if (value)
                {
                    exportManager.AddExportOption(ExportOptions.DWG);
                }
                else
                {
                    exportManager.RemoveExportOption(ExportOptions.DWG);
                }
                NotifyOfPropertyChange(() => ExportDWG);
                NotifyOfPropertyChange(() => ExportButtonLabel);
                NotifyOfPropertyChange(() => StatusText);
            }
        }

        public bool ExportDirectPDF
        {
            get
            {
                return exportManager.HasExportOption(ExportOptions.DirectPDF);
            }

            set
            {
                if (value)
                {
                    exportManager.AddExportOption(ExportOptions.DirectPDF);
                }
                else
                {
                    exportManager.RemoveExportOption(ExportOptions.DirectPDF);
                }
                NotifyOfPropertyChange(() => ExportDirectPDF);
                NotifyOfPropertyChange(() => ExportButtonLabel);
                NotifyOfPropertyChange(() => StatusText);
            }
        }

        public bool ExportPDF
        {
            get
            {
                return exportManager.HasExportOption(ExportOptions.PDF);
            }

            set
            {
                if (value)
                {
                    exportManager.AddExportOption(ExportOptions.PDF);
                }
                else
                {
                    exportManager.RemoveExportOption(ExportOptions.PDF);
                }
                NotifyOfPropertyChange(() => ExportPDF);
                NotifyOfPropertyChange(() => ExportButtonLabel);
                NotifyOfPropertyChange(() => StatusText);
            }
        }

        public bool ExportPDF24
        {
            get
            {
                return exportManager.HasExportOption(ExportOptions.PDF24);
            }

            set
            {
                if (value)
                {
                    exportManager.AddExportOption(ExportOptions.PDF24);
                }
                else
                {
                    exportManager.RemoveExportOption(ExportOptions.PDF24);
                }
                NotifyOfPropertyChange(() => ExportPDF24);
                NotifyOfPropertyChange(() => ExportButtonLabel);
                NotifyOfPropertyChange(() => StatusText);
            }
        }

        public bool CanExport
        {
            get
            {
                return (SelectedSheets.Count > 0) &&
                (exportManager.HasExportOption(ExportOptions.DWG) ||
                 exportManager.HasExportOption(ExportOptions.PDF) ||
                 exportManager.HasExportOption(ExportOptions.PDF24) ||
                 exportManager.HasExportOption(ExportOptions.DirectPDF));
            }
        }

        public string CustomParameter01Name
        {
            get
            {
                return Settings1.Default.CustomSCExportParameter01;
            }
        }

        public bool CanPrint
        {
            get
            {
                return SelectedSheets.Count > 0 && SelectedPrintType != "No Valid Printers";
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

        public string InvlaidScaleBarStatusText
        {
            get
            {
                var invalidScaleBars = exportManager.AllSheets.Count(s => s.ScaleBarError);
                if (invalidScaleBars > 0)
                {
                    return @" [Incorrect scalebars: " + invalidScaleBars + @"]";
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

        public string PreviousExportThreeName => PreviousExportThreeIsEnabled ? recentExportSets[2].DescriptiveName : "N/A";

        public bool PreviousExportTwoIsEnabled => recentExportSets.Count > 1;

        public string PreviousExportTwoName => PreviousExportTwoIsEnabled ? recentExportSets[1].DescriptiveName : "N/A";

        public string PrintButtonToolTip => CanPrint ? "Print selected drawings. For further settings goto options." : "Select sheets to enable printing.";

        public BindableCollection<string> PrintTypes => new BindableCollection<string>(printTypes);

        public string SearchText
        {
            get => searchText;

            set
            {
                if (value != searchText)
                {
                    searchText = value;
                }
                ExecuteSearch();
                NotifyOfPropertyChange(() => Sheets);
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
                NotifyOfPropertyChange(() => ExportButtonLabel);
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

        //public BindableCollection<string> SheetNumberAndName
        //{
        //    get => sheetsCollection;

        //    set
        //    {
        //        sheetsCollection = value;
        //        NotifyOfPropertyChange(() => Sheets);
        //    }
        //}


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
                if (ExportDirectPDF)
                {
                    list.Add("rPDF");
                }
                if (ExportPDF24)
                {
                    list.Add("PDF24");
                }
                if (ExportDWG)
                {
                    list.Add("DWG");
                }
                NotifyOfPropertyChange(() => CanExport);
                return @"[" + string.Join(",", list.ToArray()) + @"]";
            }
        }

        public static void NavigateTo(Uri url)
        {
            Process.Start(new ProcessStartInfo(url.AbsoluteUri));
        }

        public void AddRevision()
        {
            var revisionSelectionViewModel = new RevisionSelectionViewModel(exportManager.Doc);
            SCaddinsApp.WindowManager.ShowDialogAsync(revisionSelectionViewModel, null, RevisionSelectionViewModel.DefaultWindowSettings);
                if (revisionSelectionViewModel.SelectedCloseMode == RevisionSelectionViewModel.CloseMode.OK &&
                revisionSelectionViewModel.SelectedRevision != null)
                {
                    Manager.AddRevisions(selectedSheets, revisionSelectionViewModel.SelectedRevision.Id, exportManager.Doc);
                    NotifyOfPropertyChange(() => Sheets);
                }
        }

        public void AlignViews()
        {
            var message = "Warning, there are still some bugs in this." + Environment.NewLine +
                "Currently this will only work with views containing one sheet." + Environment.NewLine +
                Environment.NewLine +
                "Just in case, please save your model before use";

            SCaddinsApp.WindowManager.ShowWarningMessageBox("Align", message);

            var viewModel = new TemplateViewViewModel(this.SelectedSheets);
            SCaddinsApp.WindowManager.ShowDialogAsync(viewModel, null, TemplateViewViewModel.DefaultWindowSettings);
            if (viewModel.SelectedCloseMode == TemplateViewViewModel.CloseMode.OK)
            {
				// SCaddinsApp.WindowManager.ShowWarningMessageBox("Test", "Should align now");
                ViewUtilities.ViewAlignmentUtils.AlignViews(exportManager.Doc, this.SelectedSheets, viewModel.SelectedSheet);
            }
        }

        public void ContextMenuOpening(object sender, ContextMenuEventArgs e)
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
                var element = (TextBlock)e.OriginalSource;
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
            SCaddinsApp.WindowManager.ShowDialogAsync(
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
            TryCloseAsync(true);
        }

        public void FixScaleBars()
        {
            Manager.FixScaleBars(selectedSheets, exportManager.Doc);
        }

        public void Help()
        {
            //// Manager.HideSheetsInSheetList(selectedSheets, exportManager.Doc);
        }

        public void HideInSheetList()
        {
            Manager.HideSheetsInSheetList(selectedSheets, exportManager.Doc);
        }

        public void KeyPressed(KeyEventArgs keyArgs)
        {
            //// only execute search if in the search text box
            if (keyArgs.OriginalSource.GetType() == typeof(TextBox))
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
                    NotifyOfPropertyChange(() => ExportButtonLabel);
                    break;

                case Key.N:
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                    exportManager.ToggleExportOption(ExportOptions.DirectPDF);
                    NotifyOfPropertyChange(() => StatusText);
                    NotifyOfPropertyChange(() => ExportButtonLabel);
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
                    NotifyOfPropertyChange(() => ExportButtonLabel);
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
                    TryCloseAsync();
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
                if (currentColumnHeader == @"Custom Parameter 01")
                {
                    currentColumnHeader = CustomParameter01Name; // FIXME. there's a better way to do this.
                }
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
            var task = SCaddinsApp.WindowManager.ShowDialogAsync(viewSetSelectionViewModel, null, ViewSetSelectionViewModel.DefaultWindowSettings);
            bool newBool = task.Result ?? false;
            if (newBool && viewSetSelectionViewModel.SelectedSet != null)
            {
                IsNotifying = false;
                try
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    var filter = new Predicate<object>(item => viewSetSelectionViewModel
                            .SelectedSet
                            .ViewIds.Contains(((ExportSheet)item).Sheet.Id.IntegerValue));
#pragma warning restore CS0618 // Type or member is obsolete
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
            this.IsNotifying = false;
            var optionsModel = new OptionsViewModel(exportManager, this);
            SCaddinsApp.WindowManager.ShowDialogAsync(optionsModel, null, OptionsViewModel.DefaultWindowSettings);
            NotifyOfPropertyChange(() => ExportButtonLabel);
            NotifyOfPropertyChange(() => StatusText);
            this.IsNotifying = true;
            this.Refresh();
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

            TryCloseAsync(true);
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
            var renameSheetModel = new SCaddins.RenameUtilities.ViewModels.RenameUtilitiesViewModel(renameManager)
            {
                SelectedParameterCategory = "Sheets",
                ParameterCategoryEnabled = false
            };
            var settings = RenameUtilities.ViewModels.RenameUtilitiesViewModel.DefaultWindowSettings;
            settings.Title = "Rename <" + selectedSheets.Count.ToString() + @" Sheets from Selection>";
            SCaddinsApp.WindowManager.ShowDialogAsync(renameSheetModel, null, settings);
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
            var task = SCaddinsApp.WindowManager.ShowDialogAsync(saveAsVm, null, ViewSetSaveAsViewModel.DefaultWindowSettings);
            bool newBool = task.Result ?? false;
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

        public void SelectionChanged(object sender, SelectionChangedEventArgs obj)
        {
            if (!isClosing)
            {
                IsNotifying = false;
                List<ExportSheet> list = ((DataGrid)sender).SelectedItems.Cast<ExportSheet>().ToList();
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
#pragma warning disable CS0618 // Type or member is obsolete
                var filter = new Predicate<object>(item => viewSet.ViewIds.Contains(((ExportSheet)item).Sheet.Id.IntegerValue));
#pragma warning restore CS0618 // Type or member is obsolete

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

        public void ToggleSelectedSheetParameters()
        {
            var yesNoParameters = Manager.GetYesNoSheetParameters(selectedSheets, exportManager.Doc);

            var toggleSelectedSheetParametersViewModel = new ToggleSelectedSheetParametersViewModel(
                exportManager.Doc, yesNoParameters);
            var task = SCaddinsApp.WindowManager.ShowDialogAsync(
                toggleSelectedSheetParametersViewModel,
                null,
                ToggleSelectedSheetParametersViewModel.DefaultWindowSettings);
            bool newBool = task.Result ?? false;
            if (newBool)
            {
                foreach (var item in toggleSelectedSheetParametersViewModel.YesNoParameters)
                {
                    if (item.Value.HasValue)
                    {
                        Manager.ToggleBooleanParameter(selectedSheets, exportManager.Doc, item.Value.Value, item.Name, true);
                    }
                }
            }
        }

        public void ToggleSelectedTitleblockParameters()
        {
            var yesNoParameters = Manager.GetYesNoTitleblockParameters(selectedSheets, exportManager.Doc);

            var toggleSelectedSheetParametersViewModel = new ToggleSelectedSheetParametersViewModel(
                exportManager.Doc, yesNoParameters);
            var task = SCaddinsApp.WindowManager.ShowDialogAsync(
                toggleSelectedSheetParametersViewModel,
                null,
                ToggleSelectedSheetParametersViewModel.DefaultWindowSettings);
            bool newBool = task.Result ?? false;
            if (newBool)
            {
                foreach (var item in toggleSelectedSheetParametersViewModel.YesNoParameters)
                {
                    if (item.Value.HasValue)
                    {
                        Manager.ToggleBooleanParameter(selectedSheets, exportManager.Doc, item.Value.Value, item.Name, false);
                    }
                }
            }
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
            try
            {
                var filter = new Predicate<object>(
                    item =>
                        ((item != null) &&
                         (-1 < ((ExportSheet)item).SheetDescription.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase)))
                        ||
                         ((item != null) &&
                         (-1 < ((ExportSheet)item).FullExportName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase)))
                        ||
                        (item != null &&
                         -1 < ((ExportSheet)item).SheetNumber.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase)));
                if (Sheets.CanFilter)
                {
                    Sheets.Filter = filter;
                }
            }
            catch (Exception exception)
            {
                SCaddinsApp.WindowManager.ShowMessageBox(exception.Message);
            }

            IsNotifying = true;
        }

        private void FilterByNumber(string number)
        {
            Manager.CurrentViewNumber(exportManager.Doc);
            try
            {
                var filter = new Predicate<object>(item =>
                    Regex.IsMatch(((ExportSheet)item).SheetNumber, @"^\D*" + number));
                Sheets.Filter = filter;
            }
            catch (Exception exception)
            {
                SCaddinsApp.WindowManager.ShowMessageBox(exception.Message);
            }
        }

        private List<string> GetAvailablePrinters()
        {
            var result = new List<string>();
            var printers = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>();
            if (printers.Contains(Settings1.Default.A3PrinterDriver)) {
                result.Add("Print A3");
            }
            if (printers.Contains(Settings1.Default.LargeFormatPrinterDriver)) {
                result.Add("Print A2");
                result.Add("Print Full Size");
            }
            return result;
        }
    }
}
