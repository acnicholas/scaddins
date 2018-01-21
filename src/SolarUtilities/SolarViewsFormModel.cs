namespace SCaddins.SolarUtilities
{
    using Autodesk.Revit.UI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    class ViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private SolarViews model;

        public ViewModel(UIDocument uidoc)
        {
            model = new SolarViews(uidoc);
            var view = new SolarViewsForm();
            view.DataContext = this;
            RotateCurrentView = true;
            //EndTimeList = new List<DateTime>();
            //StartTimeList = new List<DateTime>();
            //EndTimeList.Add(new DateTime(2018,6,21,15,0,0));
            //EndTimeList.Add(new DateTime(2018,6,21,16,0,0));
//            _StartTimeList.Add(new DateTime(2018,6,21,8,0,0));
//            _StartTimeList.Add(new DateTime(2018,6,21,9,0,0));
//            StartTimeList = _StartTimeList;
//            StartTime = new DateTime(2018,6,21,9,0,0);
//            EndTime = new DateTime(2018,6,21,15,0,0);
            view.ShowDialog();
        }
       
        public bool RotateCurrentView
        {
            get { return model.RotateCurrentView; }
            set
            {
                if (model.RotateCurrentView != value) {
                    model.RotateCurrentView = value;
                    InvokePropertyChanged("RotateCurrentView");
                }
            }
        }
        
        public bool Create3dViews {
            get { return model.Create3dViews; }
            set
            {
                if (model.Create3dViews != value) {
                    model.Create3dViews = value;
                    InvokePropertyChanged("Create3dViews;");
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
                    InvokePropertyChanged("CreateShadowPlans");
                }
            }
        }
               
//        public DateTime StartTime {
//            get { return model.StartTime; }
//            set {
//
//                if (model.StartTime != value) {
//                    model.StartTime = value;
//                }
//            }
//        }
//        
//        public DateTime EndTime {
//            get { return model.EndTime; }
//            set {
//
//                if (model.EndTime != value) {
//                    model.EndTime = value;
//                }
//            }
//        }
        
//        public List<DateTime> EndTimeList
//        {
//            get {
//                return EndTimeList;
//            }
//            set {
//                EndTimeList = value;
//                //InvokePropertyChanged("EndTime");
//            }
//        }
//        
//        public List<DateTime> StartTimeList
//        {
//            get {
//                return StartTimeList;
//            }
//            set {
//                StartTimeList = value;
//                //InvokePropertyChanged("StartTime");
//            }
//        }
                
        public TimeSpan ExportTimeInterval {
            get;
            set;
        }
 
        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);

            PropertyChangedEventHandler changed = PropertyChanged;

            if (changed != null)
                changed(this, e);

        }
    }
}