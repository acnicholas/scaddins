using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
using Caliburn.Micro;
using System.Linq;

namespace SCaddins.ExportManager.ViewModels
{
    public class SCexportViewModel : PropertyChangedBase
    {
        private readonly ExportManager exportManager;
        private ObservableCollection<ExportSheet> sheets;
        private WindowManager windowManager;
        private ViewSheetSetCombo selectedViewSheetSet;
         List<ExportSheet> selectedSheets = new List<ExportSheet>();

        public SCexportViewModel(WindowManager windowManager, ExportManager exportManager)
        {
            this.windowManager = windowManager;
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
                }
            }
        }
        
        public ExportSheet SelectedSheet
        {
            get; set;
        }
        
        public void Row_SelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            selectedSheets.AddRange(obj.AddedItems.Cast<ExportSheet>());
            obj.RemovedItems.Cast<ExportSheet>().ToList().ForEach(w => selectedSheets.Remove(w));
        }

        public void OptionsButton()
        {
            var optionsModel = new OptionsViewModel();
            windowManager.ShowDialog(optionsModel, null, null);
        }
        
        public void Export()
        {
            System.Windows.MessageBox.Show(selectedSheets.Count.ToString());
        }

        public void OpenViewsCommand()
        {
            OpenSheet.OpenViews(selectedSheets);  
        }
        
        public void FixScaleBars()
        {
            ExportManager.FixScaleBars(selectedSheets, exportManager.Doc);
        }
        
        public void Info()
        {
            System.Windows.MessageBox.Show(SelectedSheet.FullExportName);
        }
        
        public void VerifySheets()
        {
            exportManager.Update();
        }
        
        public void RemoveUnderlays()
        {
            ViewUtilities.ViewUnderlays.RemoveUnderlays(selectedSheets, exportManager.Doc);
        }
        
        public void CopySheets()
        {
            
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

