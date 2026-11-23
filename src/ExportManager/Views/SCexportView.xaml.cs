namespace SCaddins.ExportManager.Views
{
    using SCaddins.ExportManager.ViewModels;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
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

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            // Allow focus to move to a ContextMenu or MenuItem popup
            if (e.NewFocus is DependencyObject newFocus && IsInsideContextMenu(newFocus))
            {
                return;
            }

            base.OnPreviewLostKeyboardFocus(e);
        }

        private static bool IsInsideContextMenu(DependencyObject element)
        {
            while (element != null)
            {
                if (element is ContextMenu || element is MenuItem)
                {
                    return true;
                }
                element = VisualTreeHelper.GetParent(element) ?? LogicalTreeHelper.GetParent(element);
            }
            return false;
        }

        private void Sheets_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            Sheets.Focus();
        }

        private void Sheets_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Sheets.Focus();
        }

        private void ContentDialog_PreviewLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {

        }
    }
}