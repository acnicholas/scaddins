using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Newtonsoft.Json;
using System;

namespace SCaddins.Common.ViewModels
{
    class UpgradeViewModel : Screen
    {
        public UpgradeViewModel(Version installed, Version remote, string body, string downloadLink)
        {

            if (installed == null) {
                installed = new Version(0, 0, 0);
                VerboseInformationText = "Installed version could not be retrieved";
            }
            if (remote == null) {
                remote = new Version(0, 0, 0);
                VerboseInformationText = "Remote version information could not be retrieved";
            }

            InstalledVersion = Properties.Resources.InstalledVersion + @": " + installed.ToString();
            LatestVersion = Properties.Resources.LatestVersion + @": " + remote.ToString();
            VerboseInformationText = body;
            if (installed < remote) {
                InformationText = Properties.Resources.UpgrateNewVersionAvailableMessage;
            } else {
                InformationText = Properties.Resources.UpgradeUpToDateMessage;
            }
        }

        public string VerboseInformationText
        {
            get; set;
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

    }
}
