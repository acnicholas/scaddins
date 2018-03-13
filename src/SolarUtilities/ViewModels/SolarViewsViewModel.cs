namespace SCaddins.SolarUtilities.ViewModels
{
    using Autodesk.Revit.UI;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Caliburn.Micro;

    class SolarViewsViewModel : PropertyChangedBase
    {
        private SolarViews model;

        public SolarViewsViewModel(UIDocument uidoc)
        {
            model = new SolarViews(uidoc);
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

        //public DateTime Time
        //{
        //    get
        //}
        
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
                //return "test";
                return model.ActiveIewInformation;
            }
        }
                              
        public TimeSpan ExportTimeInterval {
            get;
            set;
        }

    }
}