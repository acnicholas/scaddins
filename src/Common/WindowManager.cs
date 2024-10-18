using System.Threading.Tasks;

namespace SCaddins.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Properties;
    using SCaddins.Common.ViewModels;
    using Wpf.Ui.Appearance;

#pragma warning disable CA1060 // Move pinvokes to native methods class
    public class WindowManager : Caliburn.Micro.WindowManager
#pragma warning restore CA1060 // Move pinvokes to native methods class
    {
        // ReSharper disable once InconsistentNaming
        private const int GWLSTYLE = -16;
        private readonly IDialogService dialogService;

        public WindowManager(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        public System.Windows.Window LastWindow
        {
            get; set;
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

        public override async Task<bool?> ShowDialogAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            System.Windows.Window window = await CreateWindowAsync(rootModel, true, context, settings);
            if (window == null)
            {
                return false;
            }
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(window);
            ApplicationThemeManager.Apply(window);
            helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
            window.SourceInitialized += Window_SourceInitialized;
            window.SizeChanged += Window_SizeChanged;
            window.LocationChanged += Window_LocationChanged;
            LastWindow = window;

            //if (Scaddins.Default.UseDarkMode)
            //{
            //    Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
            //        Wpf.Ui.Appearance.ApplicationTheme.Dark, // Theme type
            //        Wpf.Ui.Controls.WindowBackdropType.Mica,  // Background type
            //        true                                      // Whether to change accents automatically
            //    );
            //}
            //else
            //{
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                Wpf.Ui.Appearance.ApplicationTheme.Light, // Theme type
                Wpf.Ui.Controls.WindowBackdropType.Mica,  // Background type
                true                                      // Whether to change accents automatically
                );
            //}

            return window.ShowDialog();
        }

        public bool? ShowColourDialog()
        {
            return dialogService.ShowColourChooser();
        }

        public bool? ShowDirectorySelectionDialog(string defaultDir, out string dirPath)
        {
            return dialogService.ShowDirectorySelectionDialog(defaultDir, out dirPath);
        }

        public bool? ShowFileSelectionDialog(string defaultFile, out string filePath)
        {
            return dialogService.ShowFileSelectionDialog(defaultFile, out filePath);
        }

        public bool? ShowFileSelectionDialog(string defaultFile, out string filePath, string defaultExtension)
        {
            return dialogService.ShowFileSelectionDialog(defaultFile, out filePath, defaultExtension);
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task ShowPopupAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

        public override async Task ShowWindowAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            System.Windows.Window window = await CreateWindowAsync(rootModel, false, context, settings);
            if (window != null)
            {
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(window);
                helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
                ApplicationThemeManager.Apply(window);
                window.SourceInitialized += Window_SourceInitialized;
                window.SizeChanged += Window_SizeChanged;
                window.LocationChanged += Window_LocationChanged;
                LastWindow = window;
            }
            window.Show();
        }

        public async Task ShowModalWindowAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            System.Windows.Window window = await CreateWindowAsync(rootModel, true, context, settings);
            if (window != null)
            {
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(window);
                helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
                ApplicationThemeManager.Apply(window);
                window.SourceInitialized += Window_SourceInitialized;
                window.SizeChanged += Window_SizeChanged;
                window.LocationChanged += Window_LocationChanged;
                LastWindow = window;
            }
            window.Show();
        }

        public bool ShowYesNoDialog(string title, string message, bool defaultValue)
        {
            return dialogService.ShowYesNoDialog(title, message, defaultValue);
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr handle, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr handle, int index, int newLong);

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            var window = sender as System.Windows.Window;
            if (window != null)
            {
                Left = window.Left;
                Top = window.Top;
            }
            else
            {
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
            if (ret == 0)
            {
                Console.WriteLine(Resources.WindowManager_Window_SourceInitialized_WARNING__could_not_set_window_style);
            }
        }
    }
}
