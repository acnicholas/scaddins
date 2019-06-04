using System;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SCaddins.ModelSetupWizard
{
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

                //string appSettingsXmlName = ModelSetupWizardSettings.Default.Context["SCaddins.ModelSetupWizard.ModelSetupWizardSettings"].ToString();
                string appSettingsXmlName = ModelSetupWizardSettings.Default.Context["GroupName"].ToString();
                SCaddinsApp.WindowManager.ShowMessageBox(appSettingsXmlName);
                // returns "MyApplication.Properties.Settings";

                // Open settings file as XML
                var import = XDocument.Load(settingsFilePath);
                // Get the whole XML inside the settings node
                //var settings = import.XPathSelectElements("//" + appSettingsXmlName);
                //var settings = import.Element("configuration").Element("userSettings").Element(appSettingsXmlName);
                var settings = import.Element("configuration").Element("userSettings").Element(appSettingsXmlName);

                config.GetSectionGroup("userSettings")
                    .Sections[appSettingsXmlName]
                    .SectionInformation
                    .SetRawXml(settings.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("userSettings");
                appSettings.Reload();
            }
            catch (Exception) // Should make this more specific
            {
                // Could not import settings.
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






