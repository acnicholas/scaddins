// (C) Copyright 2015 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.Common
{
    using System;
    using System.Diagnostics;
    using System.Security;
    using System.Security.Permissions;

    public static class SystemUtilities
    {
        [SecurityCritical]
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        internal static void KillAllProcesses(string processName)
        {
            try {
                foreach (Process proc in Process.GetProcessesByName(processName)) {
                    proc.Kill();
                }
            } catch (InvalidOperationException ex) {
                Autodesk.Revit.UI.TaskDialog.Show("InvalidOperationException", ex.Message);
            } catch (NotSupportedException ex) {
                Autodesk.Revit.UI.TaskDialog.Show("NotSupportedException", ex.Message);
            } catch (System.ComponentModel.Win32Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
