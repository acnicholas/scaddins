using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCaddins.ExportManager.Views
{
    /// <summary>
    /// Interaction logic for OpenSheetView.xaml
    /// </summary>
    public partial class OpenSheetView : UserControl
    {
        public OpenSheetView()
        {
            InitializeComponent();
            this.SearchInput.Focus();
            if (this.SearchResults.Items.Count > 0) {
                this.SearchResults.SelectedIndex = 0;
            }
        }

        private void ScrollDown()
        {
            int selectedRow = SearchResults.SelectedIndex;
            if (selectedRow < SearchResults.Items.Count - 1) {
                SearchResults.SelectedIndex++;
            } else {
                SearchResults.SelectedIndex = 0;
            }
        }

        private void ScrollUp()
        {
            int selectedRow = SearchResults.SelectedIndex;
            if (selectedRow > 0) {
                SearchResults.SelectedIndex--;
            } else {
                SearchResults.SelectedIndex = SearchResults.Items.Count - 1;
            }
        }

        private void SearchInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var resultCount = this.SearchResults.Items.Count;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.J)) {
                 ScrollDown();
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.K)) {
                ScrollUp();
            }
            return;
        }
    }
}
