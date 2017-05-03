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

namespace SCaddins.ExportManager
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;

    public class PostExportHookCommand
    {
        private string cmd;
        private string args;
        private string name;
        private Collection<string> supportedFilenameExtensions;

        public PostExportHookCommand()
        {
            this.cmd = string.Empty;
            this.args = string.Empty;
            this.name = string.Empty;
            this.supportedFilenameExtensions = new Collection<string>();
        }

        public string Name
        {
            get { return this.name; }
        }

        public static string FormatConfigurationString(SCaddins.ExportManager.ExportSheet sheet, string value, string extension)
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
            result = result.Replace(@"$sheetRevision", sheet.SheetRevision);
            result = result.Replace(@"$sheetRevisionDate", sheet.SheetRevisionDate);
            result = result.Replace(@"$sheetRevisionDescription", sheet.SheetRevisionDescription);
            result = result.Replace(@"$fileExtension", extension);
            return result;
        }

        public void Run(SCaddins.ExportManager.ExportSheet sheet, string extension)
        {
            string a = FormatConfigurationString(sheet, this.args, extension);
            #if DEBUG
            Autodesk.Revit.UI.TaskDialog.Show("DEBUG", this.args + " " + a);
            #endif
            Common.ConsoleUtilities.StartHiddenConsoleProg(this.cmd, a);
        }

        public void SetCommand(string command)
        {
            this.cmd = command;
        }

        public void SetArguments(string arguments)
        {
            this.args = arguments;
        }

        public void SetName(string newName)
        {
            this.name = newName;
        }

        public void AddSupportedFilenameExtension(string extension)
        {
            this.supportedFilenameExtensions.Add(extension);
        }

        public bool HasExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension)) {
                return false;
            }
            if (this.supportedFilenameExtensions == null || this.supportedFilenameExtensions.Count < 1) {
                return false;
            }
            return this.supportedFilenameExtensions.Contains(extension);
        }

        public string ListExtensions()
        {
            string s = string.Empty;
            foreach (string fne in this.supportedFilenameExtensions) {
                s += fne + System.Environment.NewLine;
            }
            return s;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
