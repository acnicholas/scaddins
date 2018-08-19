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

namespace SCaddins.SolarUtilities.ViewModels
{
    using System;
    using Autodesk.Revit.UI;
    using Caliburn.Micro;

    internal class SolarViewsViewModel : Screen
    {
        private DateTime creationDate;
        private DateTime endTime;
        private TimeSpan interval;
        private SolarViews model;
        private DateTime startTime;

        public SolarViewsViewModel(UIDocument uidoc)
        {
            model = new SolarViews(uidoc);
            creationDate = new DateTime(2018, 06, 21);
            startTime = new DateTime(2018, 06, 21, 9, 0, 0, DateTimeKind.Local);
            endTime = new DateTime(2018, 06, 21, 15, 0, 0);
            interval = new TimeSpan(1, 00, 00);
            RotateCurrentView = CanRotateCurrentView;
            if (!CanRotateCurrentView)
            {
                Create3dViews = true;
            }
        }

        public bool CanRotateCurrentView
        {
            get
            {
                return model.CanRotateActiveView;
            }
        }

        public bool Create3dViews
        {
            get
            {
                return model.Create3dViews;
            }

            set
            {
                if (model.Create3dViews != value)
                {
                    model.Create3dViews = value;
                    NotifyOfPropertyChange(() => CurrentModeSummary);
                }
            }
        }

        public bool CreateShadowPlans
        {
            get
            {
                return model.CreateShadowPlans;
            }

            set
            {
                if (model.CreateShadowPlans != value)
                {
                    model.CreateShadowPlans = value;
                    NotifyOfPropertyChange(() => CurrentModeSummary);
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
                if (value != creationDate)
                {
                    var oldStartIndex = StartTimes.IndexOf(SelectedStartTime);
                    var oldEndIndex = EndTimes.IndexOf(SelectedEndTime);
                    creationDate = value;
                    NotifyOfPropertyChange(() => StartTimes);
                    NotifyOfPropertyChange(() => EndTimes);
                    SelectedStartTime = StartTimes[oldStartIndex];
                    SelectedEndTime = EndTimes[oldEndIndex];
                }
            }
        }

        public string CurrentModeSummary
        {
            get
            {
                if (RotateCurrentView) {
                    return "Rotate Current View";
                }
                if (Create3dViews) {
                    return "Create View[s]";
                }
                if (CreateShadowPlans) {
                    return "Create Plans";
                }
                return "OK";
            }
        }

        public bool EnableRotateCurrentView
        {
            get
            {
                return CanRotateCurrentView;
            }
        }

        public BindableCollection<DateTime> EndTimes
        {
            get
            {
                var times = new BindableCollection<DateTime>();
                for (int hour = 9; hour < 18; hour++)
                {
                    times.Add(new DateTime(creationDate.Year, creationDate.Month, creationDate.Day, hour, 0, 0, DateTimeKind.Local));
                }
                return times;
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

        public bool RotateCurrentView
        {
            get {
                return model.RotateCurrentView;
            }

            set
            {
                if (model.RotateCurrentView != value)
                {
                    model.RotateCurrentView = value;
                    NotifyOfPropertyChange(() => CurrentModeSummary);
                }
            }
        }

        public DateTime SelectedEndTime
        {
            get
            {
                return endTime;
            }

            set
            {
                if (value != endTime)
                {
                    endTime = value;
                    NotifyOfPropertyChange(() => SelectedEndTime);
                }
            }
        }

        public TimeSpan SelectedInterval
        {
            get
            {
                return interval;
            }

            set
            {
                if (value != interval)
                {
                    interval = value;
                }
            }
        }

        public DateTime SelectedStartTime
        {
            get
            {
                return startTime;
            }

            set
            {
                if (value != startTime)
                {
                    startTime = value;
                    NotifyOfPropertyChange(() => SelectedStartTime);
                }
            }
        }

        public BindableCollection<DateTime> StartTimes
        {
            get
            {
                var times = new BindableCollection<DateTime>();
                for (int hour = 8; hour < 17; hour++)
                {
                    times.Add(new DateTime(creationDate.Year, creationDate.Month, creationDate.Day, hour, 0, 0, DateTimeKind.Local));
                }
                return times;
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