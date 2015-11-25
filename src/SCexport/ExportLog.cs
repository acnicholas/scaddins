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
    using System.Text;
    using SCaddins.SCexport;

    public class ExportLog
    {
        private const string ErrPrefix = "[ERROR]";
        private const string WarningPrefix = "[WARNING]";
        private Collection<ExportLogItem> errorLog;
        private Collection<ExportLogItem> warningLog;
        private Collection<ExportLogItem> messageLog;
        private StringBuilder fullLog;
        private int warnings;
        private int errors;
        private int messages;
        private int totalExports;
        private DateTime startTime;

        public ExportLog(DateTime startTime, int totalExports)
        {
            this.errors = 0;
            this.warnings = 0;
            this.messages = 0;
            this.fullLog = new StringBuilder();
            this.startTime = startTime;
            this.totalExports = totalExports;
            this.errorLog = new Collection<ExportLogItem>();
            this.warningLog = new Collection<ExportLogItem>();
            this.messageLog = new Collection<ExportLogItem>();
        }

        public int Warnings
        {
            get { return this.warnings; }
        }

        public int Errors
        {
            get { return this.errors; }
        }

        public int Messages
        {
            get { return this.messages; }
        }

        public int TotalExports
        {
            get { return this.totalExports; }
        }

        public Collection<ExportLogItem> ErrorLog
        {
            get { return this.errorLog; }
        }

        public Collection<ExportLogItem> WarningLog
        {
            get { return this.warningLog; }
        }

        public Collection<ExportLogItem> MessageLog
        {
            get { return this.messageLog; }
        }
        
        public string FullOutputLog
        {
            get { return fullLog.ToString(); }
        }

        public TimeSpan TimeSinceStart
        {
            get { return DateTime.Now - this.startTime; }
        }

        public void AddMessage(string fileName, string msg)
        {
            this.AddLogItem(fileName, msg);
            this.messages++;
            this.messageLog.Add(new ExportLogItem(msg, fileName));
        }

        public void AddError(string fileName, string msg)
        {
            this.AddLogItem(fileName, "(EE)" + msg);
            this.errors++;
            this.errorLog.Add(new ExportLogItem(msg, fileName));
        }

        public void AddWarning(string fileName, string msg)
        {
            this.AddLogItem(fileName, "(WW)" + msg);
            this.warnings++;
            this.warningLog.Add(new ExportLogItem(msg, fileName));
        }

        public void ShowSummaryDialog()
        {
            var logDialog = new ExportLogDialog(this);
            logDialog.ShowDialog();
        }
        
        private void AddLogItem(string fileName, string msg)
        {
            this.fullLog.Append("[").Append(fileName).Append("] - ").Append(msg).AppendLine();
        }
    }
 }
