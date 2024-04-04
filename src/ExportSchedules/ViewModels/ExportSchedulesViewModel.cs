namespace SCaddins.ExportSchedules.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Caliburn.Micro;

    public class ExportSchedulesViewModel : Screen
    {
        private List<Schedule> selectedSchedules = new List<Schedule>();

        public ExportSchedulesViewModel(List<Schedule> schedules, string exportDir)
        {
            Schedules = new BindableCollection<Schedule>(schedules);
            ExportDir = exportDir;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 480;
                settings.Width = 768;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri("pack://application:,,,/SCaddins;component/Assets/table.png"));
                settings.Title = "Export Schedules";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public string ExportDir
        {
            get; set;
        }

        public bool ExportIsEnabled => selectedSchedules.Count > 0 && System.IO.Directory.Exists(ExportDir);

        public string ExportLabel
        {
            get
            {
                return ExportIsEnabled ? "Export " + selectedSchedules.Count + " Schedule(s)" : "Export";
            }
        }

        public BindableCollection<Schedule> Schedules
        {
            get; set;
        }

        public void Export()
        {
            var outputMsg = Utilities.Export(selectedSchedules, ExportDir);
            SCaddinsApp.WindowManager.ShowMessageBox(outputMsg);
        }

        public void Options()
        {
            var vm = new OptionsViewModel();
            SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, OptionsViewModel.DefaultWindowSettings);
        }

        public void RowSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs eventArgs)
        {
            selectedSchedules.AddRange(eventArgs.AddedItems.Cast<Schedule>());
            eventArgs.RemovedItems.Cast<Schedule>().ToList().ForEach(w => selectedSchedules.Remove(w));
            NotifyOfPropertyChange(() => ExportIsEnabled);
            NotifyOfPropertyChange(() => ExportLabel);
        }

        public void SelectExportDir()
        {
            var directory = string.Empty;
            var defaultDir = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var dialogResult = SCaddinsApp.WindowManager.ShowDirectorySelectionDialog(defaultDir, out directory);
            if (dialogResult.HasValue && dialogResult.Value == true)
            {
                if (System.IO.Directory.Exists(directory))
                {
                    ExportDir = directory;
                    NotifyOfPropertyChange(() => ExportDir);
                    NotifyOfPropertyChange(() => ExportIsEnabled);
                }
            }
        }
    }
}
