namespace SCaddins.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

#pragma warning disable CA1060 // Move pinvokes to native methods class
    public class WindowManager : Caliburn.Micro.WindowManager
#pragma warning restore CA1060 // Move pinvokes to native methods class
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

        public void ShowMessageBox(string message)
        {
            dialogService.ShowMessageBox(message);
        }

        public void ShowMessageBox(string title, string message)
        {
            dialogService.ShowMessageBox(title, message);
        }

        public void ShowErrorMessageBox(string title, string message)
        {
            dialogService.ShowErrorMessageBox(title, message);
        }

        public void ShowWarningMessageBox(string title, string message)
        {
            dialogService.ShowWarningMessageBox(title, message);
        }

        public override void ShowPopup(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            throw new NotImplementedException();
        }

        public bool? ShowSaveFileDialog(string defaultFileName, string defaultExtension, string filter, out string savePath)
        {
            return dialogService.ShowSaveAsDialog(defaultFileName, defaultExtension, filter, out savePath);
        }

        public bool? ShowOpenFileDialog(string defaultFileName, out string fileName)
        {
            return dialogService.ShowOpenFileDialog(defaultFileName, out fileName);
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
            var ret = SetWindowLong(handle, GWLSTYLE, value & -131073 & -65537);
            if (ret == 0) {
                System.Console.WriteLine("WARNING: could not set window style");
            }
        }
    }
}
