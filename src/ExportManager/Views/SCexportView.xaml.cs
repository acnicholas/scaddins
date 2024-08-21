namespace SCaddins.ExportManager.Views
{
    using System.Collections.Generic;
    using System.Windows.Controls;
    using SCaddins.ExportManager.ViewModels;
    using Wpf.Ui.Appearance;

    public partial class SCexportView
    {
        public SCexportView()
        {
            InitializeComponent();
            this.Loaded += SCexportView_Loaded;
        }

        public static void SelectRowByIndex(DataGrid dataGrid, List<Autodesk.Revit.DB.ViewSheet> preSelectedViews)
        {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
            {
                return;
            }

            foreach (var datagridRow in dataGrid.Items)
            {
                var row = (ExportSheet)datagridRow;
                foreach (var viewSheet in preSelectedViews)
                {
                    if (viewSheet.SheetNumber == row.SheetNumber)
                    {
                        dataGrid.SelectedItems.Add(datagridRow);
                    }
                }
            }

            dataGrid.Focus();
        }

        private void SCexportView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SCexportViewModel vm = DataContext as SCexportViewModel;
            if (vm != null)
            {
                SelectRowByIndex(Sheets, vm.PreSelectedViews);
            }
        }

        private void SelectAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Sheets.SelectAll();
        }

        private void Sheets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sheets.SelectedItems.Count == 1)
            {
                Sheets.ScrollIntoView(Sheets.SelectedItem);
            }
        }

        private void Sheets_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            Sheets.Focus();
        }

        private void Sheets_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Sheets.Focus();
        }
    }
}