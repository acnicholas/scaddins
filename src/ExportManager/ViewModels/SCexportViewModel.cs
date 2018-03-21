﻿using System.Collections.Generic;
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
            dynamic settings = new ExpandoObject();
            settings.Height = 640;
            settings.Width = 480;
            //settings.WindowStyle = System.Windows.WindowStyle.ToolWindow;
            settings.Title = "SCexport - Options";
            settings.ShowInTaskbar = false;
            settings.ResizeMode = System.Windows.ResizeMode.NoResize;
            settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            var optionsModel = new OptionsViewModel();
            windowManager.ShowDialog(optionsModel, null, settings);
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

        public void AddRevision()
        {
            var revisionSelectionViewModel = new RevisionSelectionViewModel();
            var result = windowManager.ShowDialog(revisionSelectionViewModel, null, null);

        }
        
        public void Info()
        {
            if (SelectedSheet != null) {
                System.Windows.MessageBox.Show(SelectedSheet.FullExportName);
            }
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
            //RenameUtilities.RenameParameter v = renameSheetModel.RenameParameters.Select(p => p.Name.Equals("Sheet Number"));
            //renameSheetModel.SelectedRenameParameter = v;
            //nameSheetModel.SelectedRenameMode = 
            //  renameSheetModel.RenameModes.Select(m => m.Name == "Custom Replace") as RenameUtilities.RenameCommand;
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
