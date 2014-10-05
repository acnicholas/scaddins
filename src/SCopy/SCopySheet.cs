// (C) Copyright 2014 by Andrew Nicholas
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

namespace SCaddins.SCopy
{
    using System;
    using System.ComponentModel;
    using Autodesk.Revit.DB;
    
    /// <summary>
    /// Description of SCopySheet.
    /// </summary>
    public class SCopySheet : INotifyPropertyChanged
    {  
        private SCopy scopy;
        private string title;
        private string number;
        private ViewSheet destSheet;              
        private BindingList<SCopyViewOnSheet> viewsOnSheet;
        
        public SCopySheet(string number, string title, SCopy scopy)
        {
            this.scopy = scopy;
            this.number = number;
            this.title = title;
            this.destSheet = null;
            this.viewsOnSheet = new BindingList<SCopyViewOnSheet>();
            #if REVIT2014
            foreach (View v in scopy.SourceSheet.Views) {
                this.viewsOnSheet.Add(new SCopyViewOnSheet(v.Name, v, scopy));
            }
            #else
            foreach (ElementId id in scopy.SourceSheet.GetAllPlacedViews()) {
                View v = this.DestSheet.Document.GetElement(id) as View;
                this.viewsOnSheet.Add(new SCopyViewOnSheet(v.Name, v, scopy));
            }
            #endif
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

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

        public BindingList<SCopyViewOnSheet> ViewsOnSheet {
            get {
                return this.viewsOnSheet;
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
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
