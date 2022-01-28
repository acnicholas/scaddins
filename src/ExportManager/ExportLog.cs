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
            errors = 0;
            warnings = 0;
            fullLog = new StringBuilder();
            startTime = DateTime.Now;
            endTime = DateTime.Now;
            TotalExports = 0;
            errorLog = new List<ExportLogItem>();
            warningLog = new List<ExportLogItem>();
        }

        public List<ExportLogItem> ErrorLog => errorLog;

        public int Errors => errors;

        public string FullOutputLog => fullLog.ToString();

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

        public TimeSpan TimeSinceStart => DateTime.Now - startTime;

        public int TotalExports
        {
            get; set;
        }

        public TimeSpan TotalExportTime => endTime - startTime;

        public List<ExportLogItem> WarningLog => warningLog;

        public int Warnings => warnings;

        public void AddError(string fileName, string message)
        {
            AddLogItem(ErrPrefix + message);
            errors++;
            errorLog.Add(new ExportLogItem(message, fileName));
        }

        public void AddMessage(string message)
        {
            AddLogItem(message);
        }

        public void AddWarning(string fileName, string message)
        {
            AddLogItem(WarningPrefix + message);
            warnings++;
            warningLog.Add(new ExportLogItem(message, fileName));
        }

        public void Clear()
        {
            errorLog.Clear();
            warningLog.Clear();
            fullLog.Clear();
        }

        public void EndLoggingIndividualItem(DateTime itemStartTime, string message)
        {
            LastItemElapsedTime = DateTime.Now - itemStartTime;
            AddLogItem(message);
            AddLogItem("Export Time: " + LastItemElapsedTime.ToString());
            AddLogItem(ItemEndBanner);
        }

        public void Start(string message)
        {
            Clear();
            AddLogItem(message);
            startTime = DateTime.Now;
            StartBanner = "Start Time: " + startTime.ToLongTimeString();
            AddLogItem("Start Time: " + startTime.ToLongTimeString());
        }

        public DateTime StartLoggingIndividualItem(string message)
        {
            TotalExports++;
            AddLogItem(ItemStartBanner);
            AddLogItem(message);
            return DateTime.Now;
        }

        public void Stop(string message)
        {
            AddLogItem(message);
            endTime = DateTime.Now;
            SummaryBanner = TotalExports + " exports completed with " + Errors + " errors and " + Warnings + " warnings";
            AddLogItem("End Time: " + endTime.ToLongTimeString());
            AddLogItem("Total Export Time: " + TotalExportTime.ToString());
        }

        private void AddLogItem(string msg)
        {
            fullLog.Append(msg).AppendLine();
        }
    }
}
