using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SCaddins.ExportManager;
using Autodesk.Revit.DB;
using SCaddins.Common;

namespace SCaddins.ExportManager.ViewModels
{
    public class ViewModel : ObservableObject
    {
        private readonly ExportManager exportManager;
        private ViewSheetSetCombo selectedViewSheetSet;
        private readonly ObservableCollection<string> _history = new ObservableCollection<string>();
        
        public ViewModel(ExportManager exportManager)
        {
            this.exportManager = exportManager;
            this.selectedViewSheetSet = null;
        }
        
        public IEnumerable<ExportSheet> Sheets
        {
            get { return exportManager.AllSheets; }
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
                //Autodesk.Revit.UI.TaskDialog.Show("test", value.ToString());
                RaisePropertyChangedEvent("SelectedViewSheetSet");
            }
        }

        public ICommand OKCommand
        {
            get { return new DelegateCommand2(OKPressed); }
        }

        private void OKPressed()
        {
            System.Windows.Forms.MessageBox.Show("OK Pressed");
        }


    }
}

