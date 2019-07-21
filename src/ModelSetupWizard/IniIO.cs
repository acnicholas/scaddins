using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;

namespace SCaddins.ModelSetupWizard
{
    using IniParser;
    using IniParser.Model;

    class IniIO
    {
        public static List<System.Windows.Media.Color> ReadColours(string iniFilePath)
        {
            var result = new List<System.Windows.Media.Color>(16);
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(iniFilePath);
            result.Insert(0, ConvertStringToColor(data["Colors"]["CustomColor1"]));
            result.Insert(1, ConvertStringToColor(data["Colors"]["CustomColor2"]));
            result.Insert(2, ConvertStringToColor(data["Colors"]["CustomColor3"]));
            result.Insert(3, ConvertStringToColor(data["Colors"]["CustomColor4"]));
            result.Insert(4, ConvertStringToColor(data["Colors"]["CustomColor5"]));
            result.Insert(5, ConvertStringToColor(data["Colors"]["CustomColor6"]));
            result.Insert(6, ConvertStringToColor(data["Colors"]["CustomColor7"]));
            result.Insert(7, ConvertStringToColor(data["Colors"]["CustomColor8"]));
            result.Insert(8, ConvertStringToColor(data["Colors"]["CustomColor9"]));
            result.Insert(9, ConvertStringToColor(data["Colors"]["CustomColor10"]));
            result.Insert(10, ConvertStringToColor(data["Colors"]["CustomColor11"]));
            result.Insert(11, ConvertStringToColor(data["Colors"]["CustomColor12"]));
            result.Insert(12, ConvertStringToColor(data["Colors"]["CustomColor13"]));
            result.Insert(13, ConvertStringToColor(data["Colors"]["CustomColor14"]));
            result.Insert(14, ConvertStringToColor(data["Colors"]["CustomColor15"]));
            result.Insert(15, ConvertStringToColor(data["Colors"]["CustomColor16"]));
            return result;
        }

        public static void WriteColours(string iniFilePath, List<System.Windows.Media.Color> colours)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(iniFilePath);
            data["Colors"]["CustomColor1"] = ConvertColorToString(colours[0]);
            data["Colors"]["CustomColor2"] = ConvertColorToString(colours[1]);
            data["Colors"]["CustomColor3"] = ConvertColorToString(colours[2]);
            data["Colors"]["CustomColor4"] = ConvertColorToString(colours[3]);
            data["Colors"]["CustomColor5"] = ConvertColorToString(colours[4]);
            data["Colors"]["CustomColor6"] = ConvertColorToString(colours[5]);
            data["Colors"]["CustomColor7"] = ConvertColorToString(colours[6]);
            data["Colors"]["CustomColor8"] = ConvertColorToString(colours[7]);
            data["Colors"]["CustomColor9"] = ConvertColorToString(colours[8]);
            data["Colors"]["CustomColor10"] = ConvertColorToString(colours[9]);
            data["Colors"]["CustomColor11"] = ConvertColorToString(colours[10]);
            data["Colors"]["CustomColor12"] = ConvertColorToString(colours[11]);
            data["Colors"]["CustomColor13"] = ConvertColorToString(colours[12]);
            data["Colors"]["CustomColor14"] = ConvertColorToString(colours[13]);
            data["Colors"]["CustomColor15"] = ConvertColorToString(colours[14]);
            data["Colors"]["CustomColor16"] = ConvertColorToString(colours[15]);
            parser.WriteFile(iniFilePath, data, Encoding.Unicode);
        }

        public static string ConvertColorToString(System.Windows.Media.Color color)
        {
            return String.Format("{0}{1}{2}", color.B.ToString("X2"), color.G.ToString("X2"), color.R.ToString("X2"));
        }

        public static System.Windows.Media.Color ConvertStringToColor(String hex)
        {
            //remove the # at the front
            //hex = hex.Replace("#", "");

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8) {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            b = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            r = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }

        public static string GetIniFile(Document doc)
        {
            string version = doc.Application.VersionName;
            //// string version = doc.Application.VersionNumber;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fullPath = System.IO.Path.Combine(path, @"Autodesk\Revit", version, @"Revit.ini");
            //SCaddinsApp.WindowManager.ShowMessageBox(fullPath);
            return System.IO.File.Exists(fullPath) ? fullPath : string.Empty;
        }

    }
}
