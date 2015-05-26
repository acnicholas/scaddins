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
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    
    /// <summary>
    /// Description of SCopySheet.
    /// </summary>
    public class SCopySheet : INotifyPropertyChanged
    {  
        private SCopy scopy;
        private ViewSheet sourceSheet;
        private string title;
        private string number;
        private string sheetCategory;
        private ViewSheet destinationSheet;              
        private BindingList<SCopyViewOnSheet> viewsOnSheet;
        
        public SCopySheet(string number, string title, SCopy scopy, ViewSheet sourceSheet)
        {
            this.scopy = scopy;
            this.number = number;
            this.title = title;
            //FIXME add "SC_View-Category" var somewhere?
            this.sourceSheet = sourceSheet;
            this.sheetCategory = GetSheetCategory(SCopyConstants.SheetCategory);
            this.destinationSheet = null;
            this.viewsOnSheet = new BindingList<SCopyViewOnSheet>();
            #if REVIT2014
            foreach (View v in sourceSheet.Views) {
                this.viewsOnSheet.Add(new SCopyViewOnSheet(v.Name, v, scopy));
            }
            #else
            foreach (ElementId id in sourceSheet.GetAllPlacedViews()) {              
                Element element = sourceSheet.Document.GetElement(id);
                if (element != null) {
                    View v = element as View;
                    this.viewsOnSheet.Add(new SCopyViewOnSheet(v.Name, v, scopy));
                }
            }
            #endif
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewSheet DestinationSheet {
            get {
                return this.destinationSheet;
            }
            
            set {
                this.destinationSheet = value;
            }
        }
                
        public ViewSheet SourceSheet {
            get {
                return this.sourceSheet;
            }
            
            set {
                this.sourceSheet = value;
            }
        }
        
        private string GetSheetCategory(string parameterName)
        {
            #if REVIT2015
            var viewCategoryParamList = this.SourceSheet.GetParameters(parameterName);
            if (viewCategoryParamList.Count > 0) {
                Parameter viewCategoryParam = viewCategoryParamList.First();
                string s = viewCategoryParam.AsString();
                return s;
            }
            #else
            var viewCategoryParam = this.SourceSheet.get_Parameter(SCopyConstants.SheetCategory);
            if(viewCategoryParam != null){
                return viewCategoryParam.AsString();
            }
            #endif
            return "todo";
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
        
        public string SheetCategory {
            get {
                return this.sheetCategory;
            }
            
            set {
                if (value == "<CREATE NEW>"){
                    TaskDialog.Show("test","new clicked");
                }
                this.sheetCategory = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("SheetCategory"));
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
