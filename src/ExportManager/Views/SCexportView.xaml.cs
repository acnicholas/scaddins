namespace SCaddins.ExportManager.Views
{
    using System.Windows.Controls;

    public partial class SCexportView : UserControl
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

        private void SelectNone_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Sheets.UnselectAll();
        }

        private void Sheets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sheets.ScrollIntoView(Sheets.SelectedItem);
        }
    }
}