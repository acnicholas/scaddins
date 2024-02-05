// (C) Copyright 2015-2020 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using SCaddins;

    public class PostExportHookCommand
    {
        private string cmd;
        private string args;
        private string name;
        private Collection<string> supportedFilenameExtensions;

        public PostExportHookCommand()
        {
            cmd = string.Empty;
            args = string.Empty;
            name = string.Empty;
            supportedFilenameExtensions = new Collection<string>();
        }

        public string Name
        {
            get { return name; }
        }

        public static string FormatConfigurationString(ExportSheet sheet, string value, string extension)
        {
            string result = value;
            result = result.Replace(@"$height", sheet.Height.ToString(CultureInfo.InvariantCulture));
            result = result.Replace(@"$width", sheet.Width.ToString(CultureInfo.InvariantCulture));
            result = result.Replace(@"$fullExportName", sheet.FullExportName);
            result = result.Replace(@"$fullExportPath", sheet.FullExportPath(extension));
            result = result.Replace(@"$exportDir", sheet.ExportDirectory);
            result = result.Replace(@"$pageSize", sheet.PageSize);
            result = result.Replace(@"$projectNumber", sheet.ProjectNumber);
            result = result.Replace(@"$sheetDescription", sheet.SheetDescription);
            result = result.Replace(@"$sheetNumber", sheet.SheetNumber);
            result = result.Replace(@"$sheetRevisionDate", sheet.SheetRevisionDate);
            result = result.Replace(@"$sheetRevision", sheet.SheetRevision);
            result = result.Replace(@"$sheetRevisionDescription", sheet.SheetRevisionDescription);
            result = result.Replace(@"$fileExtension", extension);

            // search for, and replace Custom Paramters
            string pattern = @"(__)(.*?)(__)";
            result = Regex.Replace(
                result,
                pattern,
                m => RoomConverter.RoomConversionCandidate.GetParamValueAsString(sheet.ParamFromString(m.Groups[2].Value)));
            return result;
        }

        public void SetCommand(string command)
        {
            cmd = command;
        }

        public string GetCommand()
        {
            return cmd;
        }

        public void SetArguments(string arguments)
        {
            args = arguments;
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        public void AddSupportedFilenameExtension(string extension)
        {
            supportedFilenameExtensions.Add(extension);
        }

        public bool HasExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }
            if (supportedFilenameExtensions.Count < 1)
            {
                return false;
            }
            return supportedFilenameExtensions.Contains(extension);
        }

        public string ListExtensions()
        {
            string s = string.Empty;
            foreach (string fne in supportedFilenameExtensions)
            {
                s += fne + System.Environment.NewLine;
            }
            return s;
        }

        public void Run(ExportSheet sheet, string extension, ExportLog log)
        {
            log.AddMessage("Running hook " + this.Name + System.Environment.NewLine +
                this.args + System.Environment.NewLine +
                this.cmd);
            string a = FormatConfigurationString(sheet, args, extension);
            log.AddMessage(a);
            if (!string.IsNullOrEmpty(a))
            {
                Common.ConsoleUtilities.StartHiddenConsoleProg(cmd, a);
            } else
            {
                Common.ConsoleUtilities.StartHiddenConsoleProg(cmd, null);
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
