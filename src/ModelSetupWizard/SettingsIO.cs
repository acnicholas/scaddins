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
                //// SCaddinsApp.WindowManager.ShowMessageBox(appSettingsXmlName);

                // Open settings file as XML
                var import = XDocument.Load(settingsFilePath);
                var settings = import.Element("configuration").Element("userSettings").Element(appSettingsXmlName);

                config.GetSectionGroup("userSettings")
                    .Sections[appSettingsXmlName]
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
