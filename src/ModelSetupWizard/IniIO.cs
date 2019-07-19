using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace SCaddins.ModelSetupWizard
{
    using IniParser;
    using IniParser.Model;

    class IniIO
    {
        public static string[] ReadColours(string iniFile)
        {
            return null;
        }

        public static string GetIniFile(Document doc)
        {
            string version = doc.Application.VersionName;
            SCaddinsApp.WindowManager.ShowMessageBox(version);
            return string.Empty;
        }

    }
}
