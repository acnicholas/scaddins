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

using System;
using System.Security;
using System.Diagnostics;

namespace SCaddins.Common
{
    public static class SystemUtilities
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
