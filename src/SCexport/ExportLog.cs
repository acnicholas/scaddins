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
    using SCaddins.SCexport;
    
    public class ExportLog
    {    
        private const string ErrPrefix = "[ERROR]";
        private const string WarningPrefix = "[WARNING]";
        private StringBuilder errorLog;
        private StringBuilder warningLog;
        private StringBuilder successLog;
        private int warnings;
        private int errors;
        private int successes;
        private int totalExports;
        private System.DateTime startTime;
        private System.TimeSpan exportTime;
              
        public ExportLog(System.DateTime startTime, int totalExports)
        {
            this.errors = 0;
            this.warnings = 0;
            this.successes = 0;
            this.totalExports = totalExports;
            this.errorLog = new StringBuilder();
            this.warningLog = new StringBuilder();
            this.successLog = new StringBuilder();
            this.startTime = startTime;
            this.exportTime = System.TimeSpan.MinValue;
        }
        
        public StringBuilder ErrorLog
        {
            get { return errorLog; }
        }
               
        public void FinishLogging()
        {
            this.exportTime = System.DateTime.Now - this.startTime;
        }
        
        public void AddSuccess(string fileName, string msg)
        {
            this.successes++;
            this.successLog.AppendLine(fileName + " - " + msg);
        }
                      
        public void AddError(string fileName, string msg)
        {
            this.errors++;
            this.errorLog.AppendLine(fileName + " - " + msg);
        }
        
        public void AddWarning(string fileName, string msg)
        {   
            this.warnings++;
            this.warningLog.AppendLine(fileName + " - " + msg);
        }
        
        public void ShowSummaryDialog()
        {
            ExportLogDialog logDialog = new ExportLogDialog(this);
            logDialog.ShowDialog();
        }
    }
 }