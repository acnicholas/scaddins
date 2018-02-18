using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCaddins.SheetCopier;
using Autodesk.Revit.UI;
using Caliburn.Micro;

namespace SCaddins.SheetCopier.ViewModels
{
    class SheetCopierViewModel : PropertyChangedBase
    {

        private SheetCopierManager copyManager;
        private SheetCopierSheet selectedSheet;
        List<SheetCopierSheet> selectedSheets = new List<SheetCopierSheet>();
        List<string> selectedSheetInformation = new List<string>();

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

        public List<string> SelectedSheetInformation
        {
            get
            {
                if (selectedSheet != null) {
                    selectedSheetInformation.Clear();
                    selectedSheetInformation.Add("Title: " + selectedSheet.SourceSheet.Title);
                    selectedSheetInformation.Add("Sheet Number: " + selectedSheet.SourceSheet.SheetNumber);
                    //foreach (View view in selectedSheet.SourceSheet.GetAllPlacedViews()
                    //{
                    //    selectedSheetInformation.Add(view.ToString);
                    //}
                    return selectedSheetInformation;
                } else {
                    return null;
                }
            }
        }

        public void Row_SelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            selectedSheets.AddRange(obj.AddedItems.Cast<SheetCopierSheet>());
            obj.RemovedItems.Cast<SheetCopierSheet>().ToList().ForEach(w => selectedSheets.Remove(w));
        }

        public SheetCopierViewModel(UIDocument uidoc)
        {
            copyManager = new SheetCopierManager(uidoc);
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

        public bool CanRemoveSheetSelection()
        {
            return true;
        }
    }
}
