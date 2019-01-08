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
    /// Interaction logic for ViewSetSaveAsView.xaml
    /// </summary>
    public partial class ViewSetSaveAsView : UserControl
    {
        public ViewSetSaveAsView()
        {
            InitializeComponent();
            Name.Focus();
        }
    }
}
