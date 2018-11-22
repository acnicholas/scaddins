namespace SCaddins.ExportManager.Views
{
    using System.Windows.Controls;

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
