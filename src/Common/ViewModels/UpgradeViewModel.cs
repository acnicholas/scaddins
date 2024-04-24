// (C) Copyright 2018 by Andrew Nicholas
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

namespace SCaddins.Common.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Security.Policy;
    using Caliburn.Micro;
    using Microsoft.Office.Interop.Excel;

    public class UpgradeViewModel : Screen
    {
        private readonly string downloadLink;

        public UpgradeViewModel(Version installed, Version remote, string body, string downloadLink)
        {
            this.downloadLink = downloadLink;

            if (installed == null)
            {
                installed = new Version(0, 0, 0);
                VerboseInformationText = "Installed version could not be retrieved";
            }
            if (remote == null)
            {
                remote = new Version(0, 0, 0);
                VerboseInformationText = "Remote version information could not be retrieved";
            }

            InstalledVersion = Properties.Resources.InstalledVersion + @": " + installed;
            LatestVersion = Properties.Resources.LatestVersion + @": " + remote;
            VerboseInformationText = body;
            if (installed < remote)
            {
                InformationText = Properties.Resources.UpgrateNewVersionAvailableMessage;
            }
            else
            {
                InformationText = Properties.Resources.UpgradeUpToDateMessage;
            }
        }

        public string InformationText
        {
            get; set;
        }

        public string InstalledVersion
        {
            get; set;
        }

        public string LatestVersion
        {
            get; set;
        }

        public string VerboseInformationText
        {
            get; set;
        }

        public static void OpenChangeLog()
        {
            var ps = new ProcessStartInfo(SCaddins.Constants.ChangelogLink)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        public void Download()
        {
            var ps = new ProcessStartInfo(downloadLink)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }
    }
}
