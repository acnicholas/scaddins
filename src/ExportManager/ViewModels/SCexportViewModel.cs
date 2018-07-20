using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using Caliburn.Micro;
using System.Linq;
using System.Dynamic;
using System.Text.RegularExpressions;
using Hardcodet.Wpf.TaskbarNotification;

namespace SCaddins.ExportManager.ViewModels
{
    class SCexportViewModel : Screen
    {
        private readonly ExportManager exportManager;
        private ObservableCollection<ExportSheet> sheets;
        private ICollectionView sheetsCollection;
        private ViewSheetSetCombo selectedViewSheetSet;
        private double currentProgress;
        private List<string> printTypes;
        private int searchStringLength;
        private string selectedPrintType;
        private string sheetNameFilter;
        List<ExportSheet> selectedSheets = new List<ExportSheet>();

        public SCexportViewModel(ExportManager exportManager)
        {
            printTypes = (new string[] { "Print A3", "Print A2", "Print Full Size" }).ToList();
            selectedPrintType = "Print A3";
            this.exportManager = exportManager;
            this.searchStringLength = 0;
            this.sheets = new ObservableCollection<ExportSheet>(exportManager.AllSheets);
            Sheets = CollectionViewSource.GetDefaultView(this.sheets);
            this.selectedViewSheetSet = null;
            ShowSearchHelpText = true;
            CurrentProgress = 0;
            ProgressBarMaximum = 1;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 480;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri("pack://application:,,,/SCaddins;component/Assets/scexport.png")
                    );
                settings.Width = 768;
                settings.Title = "SCexport - By Andrew Nicholas";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                return settings;
            }
        }

        public ICollectionView Sheets
        {
            get
            {
                return this.sheetsCollection;
            }

            set
            {
                this.sheetsCollection = value;
                NotifyOfPropertyChange(() => Sheets);
            }
        }


        public ExportSheet SelectedSheet
        {
            get; set;
        }

        public BindableCollection<string> PrintTypes
        {
            get
            {
                return new BindableCollection<string>(printTypes);
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
                if (value != selectedPrintType) {
                    selectedPrintType = value;
                    NotifyOfPropertyChange(() => SelectedPrintType);
                }
            }
        }

        public void PrintButton()
        {
            if (selectedPrintType == "Print A3") {
                PrintA3();
            }
            if (selectedPrintType == "Print A2") {
                PrintA2();
            }
            if (selectedPrintType == "Print Full Size") {
                PrintFullsize();
            }
        }

        public double CurrentProgress
        {
            get { return currentProgress; }
            set
            {
                currentProgress = value;
                NotifyOfPropertyChange(() => CurrentProgress);
            }
        }

        public double ProgressBarMaximum
        {
            get;
            set;
        }

        public string ProgressBarText
        {
            get {
                var numberOfSheets = selectedSheets.Count;
                return numberOfSheets + @" Sheet[s] Selected To Export/Print";
            }
        }

        public void ShowLatestRevision()
        {
            var revDate = ExportManager.LatestRevisionDate(exportManager.Doc);
            var filter = new System.Predicate<object>(item => ((ExportSheet)item).SheetRevisionDate.Equals(revDate));
            Sheets.Filter = filter;
            NotifyOfPropertyChange(() => Sheets);
        }

        public void Row_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            var grid = sender as System.Windows.Controls.DataGrid;
            selectedSheets = grid.SelectedItems.Cast<ExportSheet>().ToList();
            NotifyOfPropertyChange(() => ProgressBarText);
            NotifyOfPropertyChange(() => Sheets);
        }

        public void RemoveViewFilter()
        {
            Sheets.Filter = null;
            NotifyOfPropertyChange(() => Sheets);
        }

        public void ExecuteFilterView(KeyEventArgs keyArgs)
        {

            if (keyArgs.OriginalSource.GetType() == typeof(System.Windows.Controls.TextBox)) return;

            switch (keyArgs.Key) {
                case Key.C:
                    RemoveViewFilter();
                    break;
                case Key.J:
                    Sheets.MoveCurrentToNext();
                    if (Sheets.IsCurrentAfterLast) Sheets.MoveCurrentToFirst();
                    break;
                case Key.K:
                    Sheets.MoveCurrentToPrevious();
                    if (Sheets.IsCurrentBeforeFirst) Sheets.MoveCurrentToLast();
                    break;
                case Key.L:
                    ShowLatestRevision();
                    break;
                default:
                    break;

            }

            if (keyArgs.Key == Key.O)
            {
                OpenViewsCommand();
            }

            if (keyArgs.Key == Key.S)
            {
                var activeSheetName = ExportManager.CurrentViewNumber(exportManager.Doc);
                var filter = new System.Predicate<object>(item => ((ExportSheet)item).SheetNumber.Equals(activeSheetName));
                Sheets.Filter = filter;
            }

            if (keyArgs.Key == Key.Escape)
            {
                TryClose();
            }

            if (keyArgs.Key == Key.D1) {
                FilterByNumber("1");
            }

            if (keyArgs.Key == Key.D2) {
                FilterByNumber("2");
            }

        }

        private void FilterByNumber(string number)
        {
            var activeSheetName = ExportManager.CurrentViewNumber(exportManager.Doc);
            var filter = new System.Predicate<object>(item => Regex.IsMatch(((ExportSheet)item).SheetNumber, @"^\D*" + number));
            Sheets.Filter = filter;
        }

        public ObservableCollection<ViewSheetSetCombo> ViewSheetSets
        {
            get { return exportManager.AllViewSheetSets; }
        }

        public void SelectViewSheetSet()
        {
            if (selectedViewSheetSet.ViewSheetSet != null)
            {
                var filter = new System.Predicate<object>(item => selectedViewSheetSet.ViewSheetSet.Views.Contains(((ExportSheet)item).Sheet));
                Sheets.Filter = filter;
            }
            else
            {
                Sheets.Filter = null;
            }
            NotifyOfPropertyChange(() => Sheets);
        }

        public void HideHelpLabel()
        {
            ShowSearchHelpText = false;
            NotifyOfPropertyChange(() => ShowSearchHelpText);
        }

        public void TryShowHelpLabel()
        {
            ShowSearchHelpText = searchStringLength < 1;
            NotifyOfPropertyChange(() => ShowSearchHelpText);
        }

        public bool ShowSearchHelpText
        {
            get; set;   
        }

        public string SearchText
        {
            set
            {
                searchStringLength = value.Length;
                ShowSearchHelpText = searchStringLength < 1; 
                NotifyOfPropertyChange(() => ShowSearchHelpText);
                if (ViewSheetSets.ToList().Where(v => v.ToString() == value).Count() < 1)
                {
                    var filter = new System.Predicate<object>
                        (
                        item =>
                            -1 < ((ExportSheet)item).SheetDescription.IndexOf(value, System.StringComparison.OrdinalIgnoreCase)
                                ||
                            -1 < ((ExportSheet)item).SheetNumber.IndexOf(value, System.StringComparison.OrdinalIgnoreCase)
                        );
                    Sheets.Filter = filter;
                    NotifyOfPropertyChange(() => Sheets);
                } else
                {
                    selectedViewSheetSet = ViewSheetSets.ToList().Where(v => v.ToString() == value).First();
                    SelectViewSheetSet();
                }
            }
        }

        public void OptionsButton()
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 640;
            settings.Width = 480;
            settings.Title = "SCexport - Options";
            settings.ShowInTaskbar = false;
            settings.ResizeMode = System.Windows.ResizeMode.NoResize;
            settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            var optionsModel = new OptionsViewModel(exportManager, this);
            SCaddinsApp.WindowManager.ShowDialog(optionsModel, null, settings);
        }

        public void Export()
        {
            ProgressBarMaximum = selectedSheets.Count;
            NotifyOfPropertyChange(() => ProgressBarMaximum);
            System.Windows.Forms.Application.DoEvents();
            foreach (ExportSheet sheet in selectedSheets)
            {
                CurrentProgress +=1;
                exportManager.ExportSheet(sheet);
                System.Windows.Forms.Application.DoEvents();
            }
            CurrentProgress = 0;
        }

        public void OpenViewsCommand()
        {
            OpenSheet.OpenViews(selectedSheets);
        }

        public void FixScaleBars()
        {
            ExportManager.FixScaleBars(selectedSheets, exportManager.Doc);
        }

        public void CreateUserViews()
        {
            ViewUtilities.UserView.ShowSummaryDialog(
                ViewUtilities.UserView.Create(selectedSheets, exportManager.Doc)
            );
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
                    ExportManager.AddRevisions(selectedSheets, revisionSelectionViewModel.SelectedRevision.Id, exportManager.Doc);
                    NotifyOfPropertyChange(() => Sheets);
                }
            }
        }

        public void PrintFullsize()
        {
            Print(exportManager.PrinterNameLargeFormat, 1);
        }

        public void PrintA3()
        {
            Print(exportManager.PrinterNameA3, 3);
        }

        public void PrintA2()
        {
            Print(exportManager.PrinterNameLargeFormat, 2);
        }

        public void Print(string PrinterName, int printMode)
        {
            ProgressBarMaximum = selectedSheets.Count;
            NotifyOfPropertyChange(() => ProgressBarMaximum);
            System.Windows.Forms.Application.DoEvents();
            foreach (ExportSheet sheet in selectedSheets.OrderBy(x => x.SheetNumber).ToList()) {
                CurrentProgress += 1;
                exportManager.Print(sheet, PrinterName, printMode);
                System.Windows.Forms.Application.DoEvents();
            }
            CurrentProgress = 0;
        }

        public void VerifySheets()
        {
            exportManager.Update();
            NotifyOfPropertyChange(() => Sheets);
        }

        public void RemoveUnderlays()
        {
            ViewUtilities.ViewUnderlays.RemoveUnderlays(selectedSheets, exportManager.Doc);
        }

        public void CopySheets()
        {
            var sheetCopierModel = new SCaddins.SheetCopier.ViewModels.SheetCopierViewModel(exportManager.UIDoc);
            sheetCopierModel.AddSheets(selectedSheets);
            SCaddinsApp.WindowManager.ShowDialog(
                sheetCopierModel,
                null,
                SheetCopier.ViewModels.SheetCopierViewModel.DefaultWindowSettings);
        }

        public void RenameSheets()
        { 
            var renameManager = new SCaddins.RenameUtilities.RenameManager(
                exportManager.Doc,
                selectedSheets.Select(s => s.Id).ToList()
                );
            var renameSheetModel = new SCaddins.RenameUtilities.ViewModels.RenameUtilitiesViewModel(renameManager);
            renameSheetModel.SelectedParameterCategory = "Sheets";
            SCaddinsApp.WindowManager.ShowDialog(renameSheetModel, null, RenameUtilities.ViewModels.RenameUtilitiesViewModel.DefaultWindowSettings);
            foreach (ExportSheet exportSheet in selectedSheets)
            {
                exportSheet.UpdateName();
                exportSheet.UpdateNumber();
            }
            NotifyOfPropertyChange(() => Sheets);
        }

        public void TurnNorthPointsOn()
        {
            ExportManager.ToggleNorthPoints(selectedSheets, exportManager.Doc, true);
        }

        public void TurnNorthPointsOff()
        {
            ExportManager.ToggleNorthPoints(selectedSheets, exportManager.Doc, false);
        }
    }
}

