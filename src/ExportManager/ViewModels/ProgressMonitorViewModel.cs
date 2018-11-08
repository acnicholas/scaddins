namespace SCaddins.ExportManager.ViewModels
{
    using System.Dynamic;
    using Caliburn.Micro;

    class ProgressMonitorViewModel : PropertyChangedBase
    {

        private int currentValue;
        private int maximumValue;
        private string text;
        private string progressSummary;
        private bool processComplete;

        public ProgressMonitorViewModel()
        {
            CancelPressed = false;
            ProcessComplete = false;
        }

        public static dynamic DefaultWindowSettings {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 320;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri("pack://application:,,,/SCaddins;component/Assets/scexport.png"));
                settings.Width = 640;
                settings.Title = "SCexport - Export Progress";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                return settings;
            }
        }

        public int Value
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                NotifyOfPropertyChange(() => Value);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        public string ProgressSummary
        {
            get
            {
                return progressSummary;
            }

            set
            {
                progressSummary = value;
                NotifyOfPropertyChange(() => ProgressSummary);
                System.Windows.Forms.Application.DoEvents();
            }

        }

        public string ButtonLabel
        {
            get
            {
                return ProcessComplete ? "Done" : "Cancel";
            }
        }

        public int MaximumValue
        {
            get
            {
                return maximumValue;
            }

            set
            {
                maximumValue = value;
                NotifyOfPropertyChange(() => MaximumValue);
            }
        }

        public bool CancelPressed
        {
            get; set;
        }

        public bool ProcessComplete
        {
            get
            {
                return processComplete;
            }
            set
            {
                processComplete = value;
                NotifyOfPropertyChange(() => ProcessComplete);
                NotifyOfPropertyChange(() => ButtonLabel);
            }
        }

        public void Cancel()
        {
            CancelPressed = true;
        }
    }
}
