// (C) Copyright 2015-2016 by Andrew Nicholas
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

    public static class ConsoleUtilities
    {
        [SecurityCritical]
        public static void StartHiddenConsoleProg(string exePath, string args)
        {
            StartHiddenConsoleProg(exePath, args, 20000);
        }
        
        [SecurityCritical]
        public static void StartHiddenConsoleProg(string exePath, string args, int waitTime)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = exePath;

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            if (!string.IsNullOrEmpty(args)) {
                startInfo.Arguments = args;
            }
            Process p = System.Diagnostics.Process.Start(startInfo);
            p.WaitForExit(waitTime);
            p.Dispose();
        }
    }
}
