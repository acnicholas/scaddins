namespace SCaddins.ExportManager.Views
{
    using System.Windows.Controls;

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
    }
}