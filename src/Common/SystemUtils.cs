using System;
using System.Security;
using System.Diagnostics;

namespace SCaddins.Common
{
    public static class SystemUtils
    {
        [SecurityCritical]
        public static void KillallProcesses(string processName)
        {
            try {
                foreach (Process proc in Process.GetProcessesByName(processName)) {
                    proc.Kill();
                }
            } catch (Exception ex) {
                Autodesk.Revit.UI.TaskDialog.Show("Error", ex.Message);
            }
        }
    }
}
