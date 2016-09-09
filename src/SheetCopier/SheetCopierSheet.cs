// (C) Copyright 2014-2016 by Andrew Nicholas
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

namespace SCaddins.SheetCopier
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Autodesk.Revit.DB;
    
    public class SheetCopierSheet : INotifyPropertyChanged
    {  
        private SheetCopierManager scopy;
        private string title;
        private string number;
        private string sheetCategory;            
        private BindingList<SheetCopierViewOnSheet> viewsOnSheet;
        
        public SheetCopierSheet(string number, string title,  SheetCopierManager scopy, ViewSheet sourceSheet)
        {
            this.scopy = scopy;
            this.number = number;
            this.title = title; 
            this.SourceSheet = sourceSheet;
            this.sheetCategory = this.GetSheetCategory(SheetCopierConstants.SheetCategory);
            this.DestinationSheet = null;
            this.viewsOnSheet = new BindingList<SheetCopierViewOnSheet>();
            #if REVIT2014
            foreach (View v in sourceSheet.Views) {
                this.viewsOnSheet.Add(new SheetCopierViewOnSheet(v.Name, v, scopy));
            }
            #else
            foreach (ElementId id in sourceSheet.GetAllPlacedViews()) {              
                Element element = sourceSheet.Document.GetElement(id);
                if (element != null) {
                    var v = element as View;
                    this.viewsOnSheet.Add(new SheetCopierViewOnSheet(v.Name, v, scopy));
                }
            }
            #endif
        }
               
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewSheet DestinationSheet {
            get; set;    
        }
                
        public ViewSheet SourceSheet {
            get; set;    
        }
        
        public string Number {
            get {
                return this.number;
            }
            
            set {
                if (value != this.number && this.scopy.SheetNumberAvailable(value)) {
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
                this.sheetCategory = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("SheetCategory"));
                }
            }
        }

        public BindingList<SheetCopierViewOnSheet> ViewsOnSheet {
            get {
                return this.viewsOnSheet;
            }
        }
        
        public string GetNewViewName(ElementId id)
        {
            foreach (SheetCopierViewOnSheet v in this.viewsOnSheet) {
                if (id == v.OldId) {
                    return v.Title;
                }
            }
            return null;
        }
        
        private string GetSheetCategory(string parameterName)
        {
            #if ( REVIT2015 || REVIT2016 || REVIT2017 )
            var viewCategoryParamList = this.SourceSheet.GetParameters(parameterName);
            if (viewCategoryParamList != null && viewCategoryParamList.Count > 0) {
                Parameter viewCategoryParam = viewCategoryParamList.First();
                string s = viewCategoryParam.AsString();
                return s;
            } 
            #else
            var viewCategoryParam = this.SourceSheet.get_Parameter(parameterName);
            if (viewCategoryParam != null) {
                return viewCategoryParam.AsString();
            }
            #endif
            return @"n/a";
        }               
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
