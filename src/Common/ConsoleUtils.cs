// (C) Copyright 2015-2020 by Andrew Nicholas
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
    using System.Diagnostics;
    using System.Security;
    using System.Security.Permissions;

    internal static class ConsoleUtilities
    {
        [SecurityCritical]
        //[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        internal static void StartHiddenConsoleProg(string exePath, string args)
        {
            // SCaddinsApp.WindowManager.ShowMessageBox("XXX " + exePath, args);
            if (args != null)
            {
                StartHiddenConsoleProg(exePath, args, 20000);
            }
            else
            {
                StartHiddenConsoleProg(exePath, null, 20000);
            }
        }

        [SecurityCritical]
        //[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        internal static void StartHiddenConsoleProg(string exePath, string args, int waitTime)
        {
            if (System.IO.File.Exists(exePath) == false)
            {
                return;
            }

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = exePath;

            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (!string.IsNullOrEmpty(args))
            {
                startInfo.Arguments = args;
            }
            Process p = Process.Start(startInfo);
            p.WaitForExit(waitTime);
            p.Dispose();
        }
    }
}