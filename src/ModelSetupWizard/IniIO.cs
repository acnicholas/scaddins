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
    using System.Collections.Generic;
    using System.Text;
    using Autodesk.Revit.DB;
    using IniParser;
    using IniParser.Model;

    public static class IniIO
    {
        public static List<System.Windows.Media.Color> ReadColours(string iniFilePath)
        {
            var result = new List<System.Windows.Media.Color>(16);
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(iniFilePath);
            TryReadColorAndAddToList(result, data, "CustomColor1", 0);
            TryReadColorAndAddToList(result, data, "CustomColor2", 1);
            TryReadColorAndAddToList(result, data, "CustomColor3", 2);
            TryReadColorAndAddToList(result, data, "CustomColor4", 3);
            TryReadColorAndAddToList(result, data, "CustomColor5", 4);
            TryReadColorAndAddToList(result, data, "CustomColor6", 5);
            TryReadColorAndAddToList(result, data, "CustomColor7", 6);
            TryReadColorAndAddToList(result, data, "CustomColor8", 7);
            TryReadColorAndAddToList(result, data, "CustomColor9", 8);
            TryReadColorAndAddToList(result, data, "CustomColor10", 9);
            TryReadColorAndAddToList(result, data, "CustomColor11", 10);
            TryReadColorAndAddToList(result, data, "CustomColor12", 11);
            TryReadColorAndAddToList(result, data, "CustomColor13", 12);
            TryReadColorAndAddToList(result, data, "CustomColor14", 13);
            TryReadColorAndAddToList(result, data, "CustomColor15", 14);
            TryReadColorAndAddToList(result, data, "CustomColor16", 15);
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
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1}{2}", color.B.ToString("X2", System.Globalization.CultureInfo.InvariantCulture), color.G.ToString("X2", System.Globalization.CultureInfo.InvariantCulture), color.R.ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
        }

        public static System.Windows.Media.Color ConvertStringToColor(string hex)
        {
            byte a = 255;
            var start = 0;

            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                start = 2;
            }

            var b = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
            var g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
            var r = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);

            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }

        public static string GetIniFile(Document doc)
        {
            var version = doc.Application.VersionName;
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var fullPath = System.IO.Path.Combine(path, @"Autodesk\Revit", version, @"Revit.ini");
            return System.IO.File.Exists(fullPath) ? fullPath : string.Empty;
        }

        private static void TryReadColorAndAddToList(List<System.Windows.Media.Color> colors, IniData data, string key, int index)
        {
            string s = string.Empty;
            if (data.TryGetKey("Colors" + data.SectionKeySeparator + key, out s))
            {
                colors.Insert(index, ConvertStringToColor(s));
            }
            else
            {
                colors.Insert(index, System.Windows.Media.Color.FromRgb(255, 255, 255));
            }
        }
    }
}
