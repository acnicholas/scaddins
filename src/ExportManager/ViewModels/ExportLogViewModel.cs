using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.UI;
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

        public List<ExportLogItem> Errors
        {
            get { return log.ErrorLog; }
        }

        public int ErrorCount
        {
            get { return log.Errors; }
        }
    }
}
