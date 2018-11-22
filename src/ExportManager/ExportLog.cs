// (C) Copyright 2015-2018 by Andrew Nicholas
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

    public class ExportLog
    {
        private const string ErrPrefix = "[EE]";
        private const string ItemEndBanner = "## End Individual Sheet Export ##";
        private const string ItemStartBanner = "## Start Individual Sheet Export ##";
        private const string WarningPrefix = "[WW]";
        private DateTime endTime;
        private List<ExportLogItem> errorLog;
        private int errors;
        private StringBuilder fullLog;
        private DateTime startTime;
        private List<ExportLogItem> warningLog;
        private int warnings;

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

        public List<ExportLogItem> ErrorLog
        {
            get { return this.errorLog; }
        }

        public int Errors
        {
            get { return this.errors; }
        }

        public string FullOutputLog
        {
            get { return this.fullLog.ToString(); }
        }

        public TimeSpan LastItemElapsedTime
        {
            get; private set;
        }

        public string StartBanner
        {
            get; private set;
        }

        public string SummaryBanner
        {
            get; private set;
        }

        public TimeSpan TimeSinceStart
        {
            get { return DateTime.Now - this.startTime; }
        }

        public int TotalExports
        {
            get; set;
        }

        public TimeSpan TotalExportTime
        {
            get { return this.endTime - this.startTime; }
        }

        public List<ExportLogItem> WarningLog
        {
            get { return this.warningLog; }
        }

        public int Warnings
        {
            get { return this.warnings; }
        }

        public void AddError(string fileName, string message)
        {
            this.AddLogItem(ErrPrefix + message);
            this.errors++;
            this.errorLog.Add(new ExportLogItem(message, fileName));
        }

        public void AddMessage(string message)
        {
            this.AddLogItem(message);
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

        public void EndLoggingIndividualItem(DateTime itemStartTime, string message)
        {
            LastItemElapsedTime = DateTime.Now - itemStartTime;
            this.AddLogItem("Export Time: " + LastItemElapsedTime.ToString());
            this.AddLogItem(ItemEndBanner);
        }

        public void Start(string message)
        {
            this.Clear();
            this.AddLogItem(message);
            this.startTime = DateTime.Now;
            StartBanner = "Start Time: " + this.startTime.ToLongTimeString();
            AddLogItem("Start Time: " + this.startTime.ToLongTimeString());
        }

        public DateTime StartLoggingIndividualItem(string message)
        {
            TotalExports++;
            this.AddLogItem(ItemStartBanner);
            this.AddLogItem(message);
            return DateTime.Now;
        }

        public void Stop(string message)
        {
            this.AddLogItem(message);
            this.endTime = DateTime.Now;
            SummaryBanner = TotalExports + " exports completed with " + Errors + " errors and " + Warnings + " warnings";
            this.AddLogItem("End Time: " + this.endTime.ToLongTimeString());
            this.AddLogItem("Total Export Time: " + this.TotalExportTime.ToString());
        }
        
        private void AddLogItem(string msg)
        {
            this.fullLog.Append(msg).AppendLine();
        }
    }
 }
