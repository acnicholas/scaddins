using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.UI;
using Caliburn.Micro;

namespace SCaddins.SheetCopier.ViewModels
{
    class SheetCopierViewModel : PropertyChangedBase
    {

        private SheetCopierManager copyManager;
        private SheetCopierSheet selectedSheet;
        List<SheetCopierSheet> selectedSheets = new List<SheetCopierSheet>();
        List<SheetCopierViewOnSheet> selectedViews = new List<SheetCopierViewOnSheet>();
        List<SheetInformation> selectedSheetInformation = new List<SheetInformation>();
        List<string> levelsInModel = new List<string>();

        public ObservableCollection<SheetCopierSheet> Sheets
        {
            get { return copyManager.Sheets; }
        }

        public ObservableCollection<SheetCopierViewOnSheet> ViewsOnSheet
        {
            get { return SelectedSheet.ViewsOnSheet; }
        }

        public SheetCopierSheet SelectedSheet
        {
            get
            {
                return selectedSheet;
            }
            set
            {
                if (value != selectedSheet)
                {
                    selectedSheet = value;
                    NotifyOfPropertyChange(() => SelectedSheetInformation);
                    NotifyOfPropertyChange(() => ViewsOnSheet);
                }
            }
        }

        public List<SheetInformation> SelectedSheetInformation
        {
            get
            {
                selectedSheetInformation.Clear();
                if (selectedSheet != null) {
                    foreach (Autodesk.Revit.DB.Parameter param in selectedSheet.SourceSheet.Parameters) {
                        selectedSheetInformation.Add(new SheetInformation(param));
                    }
                    return selectedSheetInformation.Where(s => !string.IsNullOrEmpty(s.ParameterValue)).ToList<SheetInformation>();
                } else {
                    return null;
                }
            }
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

        public SheetCopierViewModel(UIDocument uidoc)
        {
            copyManager = new SheetCopierManager(uidoc);
            levelsInModel = copyManager.Levels.Select(k => k.Key).ToList();
        }

        public void AddCurrentSheet()
        {
            copyManager.AddCurrentSheet();
        }

        public void RemoveSheetSelection()
        {
            foreach (var s in selectedSheets.ToList())
                Sheets.Remove(s);
        }

        public void RemoveSelectedViews()
        {
            foreach (var s in selectedViews.ToList())
                ViewsOnSheet.Remove(s);
        }

        public void CopySheetSelection()
        {
        }

        public void Go()
        {
            copyManager.CreateSheets();
        }

        public void AddSheets(List<SCaddins.ExportManager.ExportSheet> selectedSheets)
        {
            foreach (var sheet in selectedSheets) {
                copyManager.AddSheet(sheet.Sheet);
            }
        }
    }
}
