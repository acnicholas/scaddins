using System.Windows.Controls;

namespace SCaddins.ExportManager.Views
{
    public partial class ProgressMonitorView : UserControl
    {
        public ProgressMonitorView()
        {
            InitializeComponent();
        }

        private void ProgressSummary_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            ////ScrollView.ScrollToBottom();
        }

        private void ProgressSummary_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            //// ScrollView.ScrollToBottom();
            ScrollView.ScrollToEnd();
        }
    }
}
