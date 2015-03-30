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
        private int warnings;
        private int errors;
        private const string errPrefix = "[ERROR]";
        private const string warningPrefix = "[WARNING]";
        private System.DateTime startTime;
        private System.DateTime endTime;
        
        public ExportLog(System.DateTime startTime)
        {
            this.errors = 0;
            this.warnings = 0;
            this.errorLog = new StringBuilder();
            this.warningLog = new StringBuilder();
            this.startTime = startTime;
            this.endTime = System.DateTime.MinValue;
        }
                      
        public void AddError(string filename, string msg)
        {
            errors++;
            errorLog.AppendLine();
        }
        
        public void AddWarning(string filename, string msg)
        {   
            warnings++;
            warningLog.AppendLine();
        }
        
        public void ShowSummaryDialog(LogType summaryType)
        {
            switch (summaryType) {
                case LogType.Error :
                    System.Windows.Forms.MessageBox.Show(errorLog.ToString(), errors + " Errors found");
                    break;
                case LogType.Warning :
                    System.Windows.Forms.MessageBox.Show(warningLog.ToString(), warnings + " Warnings found");
                    break;
            }
        }
    }
}