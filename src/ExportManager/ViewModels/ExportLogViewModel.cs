// (C) Copyright 2018 by Andrew Nicholas
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

using System.Dynamic;
using System.Collections.Generic;
using Caliburn.Micro;

namespace SCaddins.ExportManager.ViewModels
{
    class ExportLogViewModel : Screen
    {
        private ExportLog log;

        public ExportLogViewModel(ExportLog log)
        {
            this.log = log;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Width = 640;
                settings.MaxHeight = 480;
                settings.Title = "Export Log";
                settings.ShowInTaskbar = false;
                return settings;
            }
        }

        public string Output
        {
            get { return log.FullOutputLog; }
        }

        public List<ExportLogItem> Warnings
        {
            get { return log.WarningLog; }
        }

        public int WarningCount
        {
            get { return log.Warnings; }
        }

        public string WarningsHeader
        {
            get { return @"Warnings(" + WarningCount + @")"; }
        }

        public List<ExportLogItem> Errors
        {
            get { return log.ErrorLog; }
        }

        public int ErrorCount
        {
            get { return log.Errors; }
        }

        public string ErrorsHeader
        {
            get { return @"Errors(" + ErrorCount + @")"; }
        }
    }
}
