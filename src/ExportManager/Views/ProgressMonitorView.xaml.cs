using System.Windows.Controls;

namespace SCaddins.ExportManager.Views
{
    public partial class ProgressMonitorView : UserControl
    {
        public ProgressMonitorView()
        {
            InitializeComponent();
        }

        private void ProgressSummary_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            ScrollView.ScrollToBottom();
        }
    }
}
