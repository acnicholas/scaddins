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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    
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
        
        public void Run()
        {
            Autodesk.Revit.UI.TaskDialog.Show("DEBUG", this.cmd + " --- " + this.args);
            
            // Common.ConsoleUtilities.StartHiddenConsoleProg(this.cmd, this.args);
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
            return this.supportedFilenameExtensions.Contains(extension);
        }
    }
}
