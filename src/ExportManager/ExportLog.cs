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
    using System.Collections.Generic;
    using System.Text;
    using SCaddins.ExportManager;

    public class ExportLog
    {
        private const string ErrPrefix = "[EE]";
        private const string WarningPrefix = "[WW]";
        private List<ExportLogItem> errorLog;
        private List<ExportLogItem> warningLog;
        private StringBuilder fullLog;
        private int warnings;
        private int errors;
        private DateTime startTime;
        private DateTime endTime;

        public ExportLog()
        {
            this.errors = 0;
            this.warnings = 0;
            this.fullLog = new StringBuilder();
            this.startTime = DateTime.Now;
            this.endTime = DateTime.Now;
            this.TotalExports = 0;
            this.errorLog = new List<ExportLogItem>();
            this.warningLog = new List<ExportLogItem>();
        }
                
        public int Warnings
        {
            get { return this.warnings; }
        }

        public int Errors
        {
            get { return this.errors; }
        }

        public int TotalExports
        {
            get; set;
        }

        public List<ExportLogItem> ErrorLog
        {
            get { return this.errorLog; }
        }

        public List<ExportLogItem> WarningLog
        {
            get { return this.warningLog; }
        }
        
        public string FullOutputLog
        {
            get { return this.fullLog.ToString(); }
        }

        public TimeSpan TimeSinceStart
        {
            get { return DateTime.Now - this.startTime; }
        }
        
        public TimeSpan TotalExportTime
        {
            get { return this.endTime - this.startTime; }
        }
        
        public void AddMessage(string message)
        {
            this.AddLogItem(message);
        }

        public void AddError(string fileName, string message)
        {
            this.AddLogItem(ErrPrefix + message);
            this.errors++;
            this.errorLog.Add(new ExportLogItem(message, fileName));
        }

        public void AddWarning(string fileName, string message)
        {
            this.AddLogItem(WarningPrefix + message);
            this.warnings++;
            this.warningLog.Add(new ExportLogItem(message, fileName));
        }
        
        public void Clear()
        {
            this.errorLog.Clear();
            this.warningLog.Clear();
            this.fullLog.Clear();
        }
       
        public void Start(string message)
        {
            this.AddLogItem(message);
            this.startTime = DateTime.Now;
            this.AddLogItem("Start Time: " + this.startTime.ToLongTimeString());
        }
        
        public void Stop(string message)
        {
            this.AddLogItem(message);
            this.endTime = DateTime.Now;
            this.AddLogItem("End Time: " + this.endTime.ToLongTimeString());
            this.AddLogItem("Total Export Time: " + this.TotalExportTime.ToString());
        }
        
        private void AddLogItem(string msg)
        {
            this.fullLog.Append(msg).AppendLine();
        }
    }
 }
