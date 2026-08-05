using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using Autodesk.Revit.UI;

namespace ColouredTabs {
    // Replaces pyRevit's UIFramework.MainWindow.FindFirstChild / FindVisualChildren helpers
    // so no compile-time reference to Revit's internal UIFramework.dll is needed.
    public static class VisualTreeUtils {
        public static Visual GetWindowRoot(UIApplication uiapp) {
            try {
                IntPtr wndHandle = uiapp.MainWindowHandle;
                if (wndHandle != IntPtr.Zero)
                    return HwndSource.FromHwnd(wndHandle)?.RootVisual;
            }
            catch { } // interop into Revit's main-window HWND — can fail during startup/teardown
            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject parent)
            where T : DependencyObject {
            if (parent == null)
                yield break;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++) {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T match)
                    yield return match;
                foreach (T grandchild in FindVisualChildren<T>(child))
                    yield return grandchild;
            }
        }

        public static T FindFirstChild<T>(DependencyObject parent)
            where T : DependencyObject {
            return FindVisualChildren<T>(parent).FirstOrDefault();
        }
    }
}
