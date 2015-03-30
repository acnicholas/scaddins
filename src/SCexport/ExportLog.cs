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
    using System.Text;
    
    public class ExportLog
    {       
        public enum LogType {Error, Warning, Normal};
        
        private StringBuilder errorLog;
        private StringBuilder warningLog;
        private const string errPrefix = "[ERROR]";
        private const string warningPrefix = "[WARNING]";
        
        public ExportLog()
        {
            this.errorLog = new StringBuilder();
            this.warningLog = new StringBuilder();
        }
               
        public void AddError(string filename, string msg)
        {
            errorLog.AppendLine()
        }
        
        public void AddWarning(string filename, string msg)
        {  
            warningLog.AppendLine()               
        }  

    }
}