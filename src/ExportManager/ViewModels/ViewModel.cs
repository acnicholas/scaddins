using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using SCaddins.ExportManager;
using Autodesk.Revit.DB;
using SCaddins.Common;

namespace SCaddins.ExportManager.ViewModels
{
    public class ViewModel : ObservableObject
    {
        private readonly ExportManager exportManager;
        private ObservableCollection<ExportSheet> sheets;
        private ViewSheetSetCombo selectedViewSheetSet;
        private readonly ObservableCollection<string> _history = new ObservableCollection<string>();

        public ViewModel(ExportManager exportManager)
        {
            this.exportManager = exportManager;
            Sheets = exportManager.AllSheets;
            this.selectedViewSheetSet = null;
        }

        public ObservableCollection<ExportSheet> Sheets
        {
            get { return this.sheets; }
            set {
                if (sheets != value) {
                    this.sheets = value;
                    RaisePropertyChangedEvent("Sheets");
                    SheetCollection = CollectionViewSource.GetDefaultView(this.sheets);
                }
            }
        }

        private IList _selectedModels = new ArrayList();

        public IList TestSelected
        {
            get { return _selectedModels; }
            set
            {
                _selectedModels = value;
                RaisePropertyChangedEvent("TestSelected");
            }
        }

        public ICollectionView SheetCollection
        {
            get; set;
        }
        
        public IEnumerable<ViewSheetSetCombo> ViewSheetSets
        {
            get { return exportManager.AllViewSheetSets; }    
        }

        public ViewSheetSetCombo SelectedViewSheetSet
        {
            get
            {
                return this.selectedViewSheetSet;
            }
            set
            {
                this.selectedViewSheetSet = value;
                System.Windows.Forms.MessageBox.Show(value.ToString());
                RaisePropertyChangedEvent("SelectedViewSheetSet");
            }
        }

        public ICommand OKCommand
        {
            get { return new DelegateCommand2(OKPressed); }
        }
        
        public ICommand VerifyCommand
        {
            get { return new DelegateCommand2(VerifyPressed); }
        }

        public ICommand OpenViewsCommand
        {
            get { return new DelegateCommand2(OpenViewsPressed); }
        }

        private void OKPressed()
        {
            string list = string.Empty;
            foreach (var item in TestSelected) {
                var sheet = item as SCaddins.ExportManager.ExportSheet;
                list += sheet.SheetNumber + System.Environment.NewLine;
            }
            System.Windows.Forms.MessageBox.Show(list);
        }

        private void OpenViewsPressed()
        {
            OpenSheet.OpenViews(TestSelected);
        }

        private void CopySheetsPressed()
        {
            SCaddins.SheetCopier.MainForm form = new SheetCopier.MainForm()
        }


        public void VerifyPressed()
        {
            exportManager.Update();
            Sheets = exportManager.AllSheets;
        }


    }
}

