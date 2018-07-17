namespace SCaddins.SolarUtilities.ViewModels
{
    using Autodesk.Revit.UI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Caliburn.Micro;

    class SolarViewsViewModel : Screen
    {
        private SolarViews model;
        private DateTime creationDate;
        private DateTime startTime;
        private DateTime endTime;
        private TimeSpan interval;

        public SolarViewsViewModel(UIDocument uidoc)
        {
            var uiapp = uidoc.Application;
            var addInId = uiapp.ActiveAddInId;
            var addIn = addInId.GetAddInName();
            SCaddinsApp.WindowManager.ShowMessageBox(addIn);

            //TEST MSG BOX
            SCaddinsApp.WindowManager.ShowMessageBox("TEST MESSAGE");
            string outString = string.Empty;
            SCaddinsApp.WindowManager.ShowSaveFileDialog("c:/temp/testit.txt", "*.txt", "Text documents (.txt)|*.txt", out outString);
            SCaddinsApp.WindowManager.ShowMessageBox(outString);

            model = new SolarViews(uidoc);
            creationDate = new DateTime(2018, 06, 21);
            startTime = new DateTime(2018, 06, 21, 9,0,0,DateTimeKind.Local);
            endTime = new DateTime(2018, 06, 21,15,0,0);
            interval = new TimeSpan(1,00,00);
        }
       
        public bool RotateCurrentView
        {
            get { return model.RotateCurrentView; }
            set
            {
                if (model.RotateCurrentView != value) {
                    model.RotateCurrentView = value;
                }
            }
        }

        public bool CanRotateCurrentView
        {
            get
            {
                return model.CanRotateActiveView;
            }
        }
        
        public bool Create3dViews {
            get { return model.Create3dViews; }
            set
            {
                if (model.Create3dViews != value) {
                    model.Create3dViews = value;
                }
            }
        }

        public DateTime CreationDate
        {
            get
            {
                return creationDate;
            }
            set
            {
                if (value != creationDate) {
                    creationDate = value;
                    NotifyOfPropertyChange(() => StartTimes);
                    NotifyOfPropertyChange(() => EndTimes);
                }
            }
        }

        public BindableCollection<DateTime> StartTimes
        {
            get
            {
                var times = new BindableCollection<DateTime>();
                for (int hour = 8; hour < 17; hour++) {
                    times.Add(new DateTime(creationDate.Year, creationDate.Month, creationDate.Day, hour, 0, 0));
                }
                return times;
            }
        }

        public DateTime SelectedStartTime
        {
            get { return startTime; }
            set
            {
                if (value != startTime) {
                    startTime = value;
                    NotifyOfPropertyChange(() => SelectedStartTime);
                }
            }
        }

        public BindableCollection<DateTime> EndTimes
        {
            get
            {
                var times = new BindableCollection<DateTime>();
                for (int hour = 9; hour < 18; hour++) {
                    times.Add(new DateTime(creationDate.Year, creationDate.Month, creationDate.Day, hour, 0, 0));
                }
                return times;
            }
        }

        public DateTime SelectedEndTime
        {
            get { return endTime; }
            set
            {
                if (value != endTime) {
                    endTime = value;
                    NotifyOfPropertyChange(() => SelectedEndTime);
                }
            }
        }

        public BindableCollection<TimeSpan> Intervals
        {
            get
            {
                var times = new BindableCollection<TimeSpan>();
                times.Add(new TimeSpan(00, 15, 00));
                times.Add(new TimeSpan(00, 30, 00));
                times.Add(new TimeSpan(1, 00, 00));
                return times;
            }
        }

        public TimeSpan SelectedInterval
        {
            get { return interval; }
            set
            {
                if (value != interval) {
                    interval = value;
                }
            }
        }

        public bool CreateShadowPlans
        {
            get { return model.CreateShadowPlans; }
            set
            {
                if (model.CreateShadowPlans != value) {
                    model.CreateShadowPlans = value;
                }
            }
        }

        public string ViewInformation
        {
            get
            {
                return model.ActiveIewInformation;
            }
        }

        public void OK()
        {
            model.StartTime = startTime;
            model.EndTime = endTime;
            model.ExportTimeInterval = interval;
            model.Go();
        }
                             
    }
}