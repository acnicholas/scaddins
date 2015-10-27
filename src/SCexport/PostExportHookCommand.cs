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

namespace SCaddins.SCexport
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
        
        public void Run(SCaddins.SCexport.ExportSheet sheet, string extension)
        {
            string a = this.CreateArgs(sheet, extension);
            #if DEBUG
            Autodesk.Revit.UI.TaskDialog.Show("DEBUG", this.cmd + " " + a); 
            #endif            
            Common.ConsoleUtilities.StartHiddenConsoleProg(this.cmd, a);
        }
        
        public void SetCommand(string cmd)
        {
            this.cmd = cmd;
        }
        
        public void SetArguments(string args)
        {
            this.args = args;
        }
        
        public void SetName(string name)
        {
            this.name = name;
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
        
        /*
        $height;
        $width;
        $fullExportName;
        $fullExportPath;
        $exportDir;
        $pageSize;
        $projectNumber;
        $sheetDescription;
        $sheetNumber;
        $sheetRevision;
        $sheetRevisionDate;
        $sheetRevisionDescription;
        $fileExtension;
        */
        private string CreateArgs(SCaddins.SCexport.ExportSheet sheet, string extension)
        {
            string result = this.args;
            if (this.args.Contains(@"$height")) {
                result = result.Replace(@"$height", sheet.Height.ToString(CultureInfo.InvariantCulture));
            }
            if (this.args.Contains(@"$width")) {
                result = result.Replace(@"$width", sheet.Width.ToString(CultureInfo.InvariantCulture));
            }
            if (this.args.Contains(@"$fullExportName")) {
                result = result.Replace(@"$fullExportName", sheet.FullExportName);
            }
            if (this.args.Contains(@"$fullExportPath")) {
                result = result.Replace(@"$fullExportPath", sheet.FullExportPath(extension));
            }
            if (this.args.Contains(@"$exportDir")) {
                result = result.Replace(@"$exportDir", sheet.ExportDir);
            }
            if (this.args.Contains(@"$pageSize")) {
                result = result.Replace(@"$pageSize", sheet.PageSize);
            }
            if (this.args.Contains(@"$projectNumber")) {
                result = result.Replace(@"$projectNumber", sheet.ProjectNumber);
            }
            if (this.args.Contains(@"$sheetDescription")) {
                result = result.Replace(@"$sheetDescription", sheet.SheetDescription);
            }
            if (this.args.Contains(@"$sheetNumber")) {
                result = result.Replace(@"$sheetNumber", sheet.SheetNumber);
            }
            if (this.args.Contains(@"$sheetRevision")) {
                result = result.Replace(@"$sheetRevision", sheet.SheetRevision);
            }
            if (this.args.Contains(@"$sheetRevisionDate")) {
                result = result.Replace(@"$sheetRevisionDate", sheet.SheetRevisionDate);
            }
            if (this.args.Contains(@"$sheetRevisionDescription")) {
                result = result.Replace(@"$sheetRevisionDescription", sheet.SheetRevisionDescription);
            } 
            if (this.args.Contains(@"$fileExtension")) {
                result = result.Replace(@"$fileExtension", extension);
            } 
            return result;
        }
    }
}
