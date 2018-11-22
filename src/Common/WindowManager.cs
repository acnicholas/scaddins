namespace SCaddins.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class WindowManager : Caliburn.Micro.WindowManager
    {
        private const int GWLSTYLE = -16;
        private IDialogService dialogService;

        public WindowManager(IDialogService dialogService) : base()
        { 
            this.dialogService = dialogService;
        }

        public double Left
        {
            get; private set;
        }

        public System.Windows.Size Size
        {
            get; private set;
        }

        public double Top
        {
            get; private set;
        }

        public bool? ShowConfirmationDialog(string message, bool? defaultCheckboxValue, out bool checkboxResult)
        {
            return dialogService.ShowConfirmationDialog(message, defaultCheckboxValue, out checkboxResult);
        }

        public override bool? ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            System.Windows.Window window = CreateWindow(rootModel, true, context, settings);
            if (window != null) {
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(window);
                helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
                window.SourceInitialized += new EventHandler(Window_SourceInitialized);
                window.SizeChanged += Window_SizeChanged;
                window.LocationChanged += Window_LocationChanged;
            }
            return window.ShowDialog();
        }

        public bool? ShowDirectorySelectionDialog(string defaultDir, out string dirPath)
        {
            return dialogService.ShowDirectorySelectionDialog(defaultDir, out dirPath);
        }

        public bool? ShowMessageBox(string message)
        {
            return dialogService.ShowMessageBox(message);
        }

        public override void ShowPopup(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            throw new NotImplementedException();
        }

        public bool? ShowSaveFileDialog(string defaultFileName, string defaultExtension, string filter, out string savePath)
        {
            return dialogService.ShowSaveAsDialog(defaultFileName, defaultExtension, filter, out savePath);
        }

        public override void ShowWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            System.Windows.Window window = CreateWindow(rootModel, true, context, settings);
            if (window != null) {
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(window);
                helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
                window.SourceInitialized += new EventHandler(Window_SourceInitialized);
                window.SizeChanged += Window_SizeChanged;
                window.LocationChanged += Window_LocationChanged;
            }
            window.Show();
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr handle, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr handle, int index, int newLong);

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            var window = sender as System.Windows.Window;
            if (window != null) {
                Left = window.Left;
                Top = window.Top;
            } else {
                ShowMessageBox("LocationChanged event handler error...");
                Left = 20;
                Top = 20;
            }
        }

        private void Window_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Size = e.NewSize;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper((System.Windows.Window)sender).Handle;
            var value = GetWindowLong(handle, GWLSTYLE);
            SetWindowLong(handle, GWLSTYLE, (int)(value & -131073 & -65537));
        }
    }
}
