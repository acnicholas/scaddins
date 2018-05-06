using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
using Caliburn.Micro;
using System.Linq;
using System.Dynamic;

namespace SCaddins.ExportManager.ViewModels
{
    public class SCexportViewModel : PropertyChangedBase
    {
        private readonly ExportManager exportManager;
        private ObservableCollection<ExportSheet> sheets;
        private CollectionViewSource sheetsCollection;
        private WindowManager windowManager;
        private ViewSheetSetCombo selectedViewSheetSet;
        List<ExportSheet> selectedSheets = new List<ExportSheet>();
        private System.Windows.Visibility searchFieldIsVisible;
        private System.Windows.Visibility progessBarIsVisible;

        public SCexportViewModel(WindowManager windowManager, ExportManager exportManager)
        {
            this.windowManager = windowManager;
            this.exportManager = exportManager;
            this.sheets = new ObservableCollection<ExportSheet>(exportManager.AllSheets);
            this.sheetsCollection = new CollectionViewSource();
            this.sheetsCollection.Source = this.sheets;
            this.selectedViewSheetSet = null;
            SearchFieldIsVisible = System.Windows.Visibility.Hidden;
            ProgessBarIsVisible = System.Windows.Visibility.Hidden;
        }

        public ICollectionView Sheets
        {
            get { return this.sheetsCollection.View; }
        }

        public ExportSheet SelectedSheet
        {
            get; set;
        }

        public System.Windows.Visibility SearchFieldIsVisible
        {
            get { return searchFieldIsVisible; }
            set
            {
                if (value != searchFieldIsVisible)
                {
                    searchFieldIsVisible = value;
                    NotifyOfPropertyChange(() => SearchFieldIsVisible);
                }
            }
        }

        public System.Windows.Visibility ProgessBarIsVisible
        {
            get { return progessBarIsVisible; }
            set
            {
                if (value != progessBarIsVisible)
                {
                    progessBarIsVisible = value;
                    NotifyOfPropertyChange(() => ProgessBarIsVisible);
                }
            }
        }

        public void Row_SelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            selectedSheets.AddRange(obj.AddedItems.Cast<ExportSheet>());
            obj.RemovedItems.Cast<ExportSheet>().ToList().ForEach(w => selectedSheets.Remove(w));
        }

        public void RemoveViewFilter()
        {
            Sheets.Filter = null;
            NotifyOfPropertyChange(() => Sheets);
        }

        public void ExecuteFilterView(KeyEventArgs keyArgs)
        {

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

            if (keyArgs.Key == Key.Y)
            {
                SearchFieldIsVisible = System.Windows.Visibility.Visible;
                ProgessBarIsVisible = System.Windows.Visibility.Hidden;
            }
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
            var optionsModel = new OptionsViewModel(exportManager);
            windowManager.ShowDialog(optionsModel, null, settings);
        }

        public void Export()
        {
            //System.Windows.MessageBox.Show(selectedSheets.Count.ToString());
            exportManager.ExportSheets(selectedSheets);
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
            //ExportManager.CreateUserViews(selectedSheets, exportManager.Doc);
        }

        public void AddRevision()
        {
            var revisionSelectionViewModel = new RevisionSelectionViewModel();
            var result = windowManager.ShowDialog(revisionSelectionViewModel, null, null);

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
            var sheetCopierModel = new SCaddins.SheetCopier.ViewModels.SheetCopierViewModel(exportManager.UIDoc);
            sheetCopierModel.AddSheets(selectedSheets);
            windowManager.ShowDialog(sheetCopierModel, null, null);
        }

        public void RenameSheets()
        {
            var renameManager = new SCaddins.RenameUtilities.RenameManager(
                exportManager.Doc,
                selectedSheets.Select(s => s.Id).ToList()
                );
            var renameSheetModel = new SCaddins.RenameUtilities.ViewModels.RenameUtilitiesViewModel(renameManager);
            renameSheetModel.SelectedParameterCategory = "Sheets";
            windowManager.ShowDialog(renameSheetModel, null, null);
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

