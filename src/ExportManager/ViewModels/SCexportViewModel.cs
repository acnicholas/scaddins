using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using Caliburn.Micro;
using System.Linq;
using System.Dynamic;

namespace SCaddins.ExportManager.ViewModels
{
    class SCexportViewModel : Screen
    {
        private readonly ExportManager exportManager;
        private ObservableCollection<ExportSheet> sheets;
        private CollectionViewSource sheetsCollection;
        private WindowManager windowManager;
        private ViewSheetSetCombo selectedViewSheetSet;
        private double currentProgress;
        private string sheetNameFilter;
        List<ExportSheet> selectedSheets = new List<ExportSheet>();

        public SCexportViewModel(WindowManager windowManager, ExportManager exportManager)
        {
            this.windowManager = windowManager;
            this.exportManager = exportManager;
            this.sheets = new ObservableCollection<ExportSheet>(exportManager.AllSheets);
            this.sheetsCollection = new CollectionViewSource();
            this.sheetsCollection.Source = this.sheets;
            this.selectedViewSheetSet = null;
            SheetNameFilter = string.Empty;
            CurrentProgress = 0;
            ProgressBarMaximum = 1;
        }

        public ICollectionView Sheets
        {
            get { return this.sheetsCollection.View; }
        }

        public ExportSheet SelectedSheet
        {
            get; set;
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

        public string SheetNameFilter
        {
            get
            {
                return sheetNameFilter;
            }
            set
            {
                if (value != sheetNameFilter)
                {
                    sheetNameFilter = value;
                    var filter = new System.Predicate<object>(item => ((ExportSheet)item).SheetDescription.Contains(sheetNameFilter));
                    Sheets.Filter = filter;
                    NotifyOfPropertyChange(() => Sheets);
                }
            }
        }

        public void Row_SelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            selectedSheets.AddRange(obj.AddedItems.Cast<ExportSheet>());
            obj.RemovedItems.Cast<ExportSheet>().ToList().ForEach(w => selectedSheets.Remove(w));
            NotifyOfPropertyChange(() => ProgressBarText);
        }

        public void RemoveViewFilter()
        {
            Sheets.Filter = null;
            NotifyOfPropertyChange(() => Sheets);
        }

        public void ExecuteFilterView(KeyEventArgs keyArgs)
        {

            if (keyArgs.OriginalSource.GetType() == typeof(System.Windows.Controls.TextBox)) return;

            if (keyArgs.Key == Key.C)
            {
                RemoveViewFilter();
            }

            if (keyArgs.Key == Key.J)
            {
                Sheets.MoveCurrentToNext();
                if (Sheets.IsCurrentAfterLast) Sheets.MoveCurrentToFirst();
            }

            if (keyArgs.Key == Key.K)
            {
                Sheets.MoveCurrentToPrevious();
                if (Sheets.IsCurrentBeforeFirst) Sheets.MoveCurrentToLast();
            }

            if (keyArgs.Key == Key.L)
            {
                var latest = ExportManager.LatestRevisionDate(exportManager.Doc);
            }

            if (keyArgs.Key == Key.O)
            {
                OpenViewsCommand();
            }

            if (keyArgs.Key == Key.S)
            {
                var activeSheetName = ExportManager.CurrentViewNumber(exportManager.Doc);
                var toSelect = sheets.Where<ExportSheet>(sheet => (sheet.SheetNumber == activeSheetName)).ToList<ExportSheet>().First();
                Sheets.MoveCurrentTo(toSelect);
            }

            if (keyArgs.Key == Key.Escape)
            {
                    TryClose();
            }
        }

        public bool UserShouldEditValueNow
        {
            get; set;
        }

        public ObservableCollection<ViewSheetSetCombo> ViewSheetSets
        {
            get { return exportManager.AllViewSheetSets; }
        }

        public ViewSheetSetCombo SelectedViewSheetSet
        {
            get
            {
                return selectedViewSheetSet;
            }
            set
            {
                if (value != selectedViewSheetSet)
                {
                    selectedViewSheetSet = value;
                    if (selectedViewSheetSet.ViewSheetSet != null) {
                        var filter = new System.Predicate<object>(item => selectedViewSheetSet.ViewSheetSet.Views.Contains(((ExportSheet)item).Sheet));
                        Sheets.Filter = filter;
                    } else
                    {
                        Sheets.Filter = null;
                    }
                }
                NotifyOfPropertyChange(() => Sheets);
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
            var optionsModel = new OptionsViewModel(exportManager, windowManager);
            windowManager.ShowDialog(optionsModel, null, settings);
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
            dynamic settings = new ExpandoObject();
            settings.Height = 640;
            settings.Width = 480;
            settings.Title = "Select Revision to Assign";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            var revisionSelectionViewModel = new RevisionSelectionViewModel(exportManager.Doc);
            bool? result = windowManager.ShowDialog(revisionSelectionViewModel, null, settings);
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

        public void Info()
        {
            if (SelectedSheet != null)
            {
                System.Windows.MessageBox.Show(SelectedSheet.FullExportName);
            }
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
            dynamic settings = new ExpandoObject();
            settings.Height = 480;
            settings.Width = 768;
            settings.Title = "Sheet Copier - By Andrew Nicholas";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            var sheetCopierModel = new SCaddins.SheetCopier.ViewModels.SheetCopierViewModel(exportManager.UIDoc);
            sheetCopierModel.AddSheets(selectedSheets);
            windowManager.ShowDialog(sheetCopierModel, null, settings);
        }

        public void RenameSheets()
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 480;
            settings.Width = 768;
            settings.Title = "Rename Selected Sheet Parameters";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;

            var renameManager = new SCaddins.RenameUtilities.RenameManager(
                exportManager.Doc,
                selectedSheets.Select(s => s.Id).ToList()
                );
            var renameSheetModel = new SCaddins.RenameUtilities.ViewModels.RenameUtilitiesViewModel(renameManager);
            renameSheetModel.SelectedParameterCategory = "Sheets";
            windowManager.ShowDialog(renameSheetModel, null, settings);
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

