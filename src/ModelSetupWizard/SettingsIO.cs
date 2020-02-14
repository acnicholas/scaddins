// (C) Copyright 2019-2020 by Andrew Nicholas
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

namespace SCaddins.ModelSetupWizard
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Xml.Linq;

    public static class SettingsIO
    {
        internal static void Import(string settingsFilePath)
        {
            if (!File.Exists(settingsFilePath))
            {
                throw new FileNotFoundException();
            }

            var appSettings = ModelSetupWizardSettings.Default;
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                string appSettingsXmlName = ModelSetupWizardSettings.Default.Context["GroupName"].ToString();

                // Open settings file as XML
                var import = XDocument.Load(settingsFilePath);
                var settings = import.Element("configuration")?.Element("userSettings")?.Element(appSettingsXmlName);

                if (settings == null)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("Could not import settings");
                    return;
                }
                config.GetSectionGroup("userSettings")
                    ?.Sections[appSettingsXmlName]
                    .SectionInformation
                    .SetRawXml(settings.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("userSettings");
                appSettings.Reload();
            }
            catch (Exception)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Could not import settings");
                appSettings.Reload(); // from last set saved, not defaults
            }
        }

        internal static void Export(string settingsFilePath)
        {
            ModelSetupWizardSettings.Default.Save();
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            config.SaveAs(settingsFilePath);
        }
    }
}
