using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SCaddins.Common
{
    public class WindowManager : Caliburn.Micro.WindowManager
    {
        private IDialogService dialogService;

        public WindowManager(IDialogService dialogService) : base()
        {
            this.dialogService = dialogService;
        }

        public override bool? ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            System.Windows.Window window = CreateWindow(rootModel, true, context, settings);
            if (window != null) {
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(window);
                helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
                window.SourceInitialized += new EventHandler(Window_SourceInitialized);
            }
            return window.ShowDialog();
        }

        public bool? ShowMessageBox(string message)
        {
            return dialogService.ShowMessageBox(message);
        }

        public bool? ShowSaveFileDialog(ref string resultPath)
        {
            return dialogService.ShowSaveAsDialog();
        }

        public void ShowPopup(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            throw new NotImplementedException();
        }

        public void ShowWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            throw new NotImplementedException();
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper((System.Windows.Window)sender).Handle;
            var value = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, (int)(value & -131073 & -65537));
        }
    }
}
