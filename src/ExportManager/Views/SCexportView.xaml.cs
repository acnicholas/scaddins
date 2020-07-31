namespace SCaddins.ExportManager.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class SCexportView
    {
        public SCexportView()
        {
            InitializeComponent();
            Sheets.Focus();
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