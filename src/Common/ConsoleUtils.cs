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
    /// <summary>
    /// Description of Utils.
    /// </summary>
    public static class ConsoleUtilities
    {
        /// <summary>
        /// Start a console program - Hidden.
        /// </summary>
        /// <param name="exePath">The program to start.</param>
        /// <param name="args">The args to send to the program.</param>
        [SecurityCritical]
        public static void StartHiddenConsoleProg(string exePath, string args)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.FileName = exePath;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.Arguments = args;
            var p = new Process();
            p = System.Diagnostics.Process.Start(startInfo);
            p.WaitForExit();
        }
    }
}
