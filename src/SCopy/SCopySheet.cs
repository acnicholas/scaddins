using System;
using System.ComponentModel;
using Autodesk.Revit.DB;

namespace SCaddins.SCopy
{
    /// <summary>
    /// Description of SCopySheet.
    /// </summary>
    public class SCopySheet : INotifyPropertyChanged
    {
        private SCopy scopy;
        private string title;
        private string number;
        private ViewSheet destSheet;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        private BindingList<SCopyViewOnSheet> viewsOnSheet;

        public ViewSheet DestSheet {
            get {
                return this.destSheet;
            }
            
            set {
                this.destSheet = value;
            }
        }

        public string Number {
            get {
                return this.number;
            }
            
            set {
                if (value != this.number && this.scopy.CheckSheetNumberAvailability(value)) {
                    this.number = value;
                    if (this.PropertyChanged != null) {
                        this.PropertyChanged(
                            this, new PropertyChangedEventArgs("Number"));
                    }
                } else {
                    Autodesk.Revit.UI.TaskDialog.Show(
                        "SCopy - WARNING", value + " exists, you can't use it!.");
                }
            }
        }

        public string Title {
            get {
                return this.title;
            }
            
            set {
                this.title = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                }
            }
        }

        public string GetNewViewName(ElementId id)
        {
            foreach (SCopyViewOnSheet v in this.viewsOnSheet) {
                if (id == v.OldId) {
                    return v.Title;
                }
            }
            return null;
        }

        public BindingList<SCopyViewOnSheet> ViewsOnSheet {
            get {
                return this.viewsOnSheet;
            }
        }

        public SCopySheet(string number, string title, SCopy scopy)
        {
            this.scopy = scopy;
            this.number = number;
            this.title = title;
            this.destSheet = null;
            this.viewsOnSheet = new BindingList<SCopyViewOnSheet>();
            foreach (View v in scopy.SourceSheet.Views) {
                this.viewsOnSheet.Add(new SCopyViewOnSheet(v.Name, v, scopy));
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
