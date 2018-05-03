using System;
using System.Windows.Controls;

namespace SCaddins.ExportManager.Views
{
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

        private void Grid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.A) Sheets.SelectAll();
            if (e.Key == System.Windows.Input.Key.N) Sheets.UnselectAll();
        }
    }
}