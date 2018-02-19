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
    using System.Runtime.CompilerServices;
    using System.Collections.ObjectModel;

    public class SheetCopierSheet : INotifyPropertyChanged
    {  
        private SheetCopierManager scopy;
        private string title;
        private string number;
        private string sheetCategory;            
        private ObservableCollection<SheetCopierViewOnSheet> viewsOnSheet;
        
        public SheetCopierSheet(string number, string title,  SheetCopierManager scopy, ViewSheet sourceSheet)
        {
            if (scopy == null) {
                throw new ArgumentNullException("scopy");
            }
            this.scopy = scopy;
            if (sourceSheet == null) {
                throw new ArgumentNullException("sourceSheet");
            }
            this.number = number;
            this.title = title; 
            this.SourceSheet = sourceSheet;
            this.sheetCategory = this.GetSheetCategory(SheetCopierConstants.SheetCategory);
            this.DestinationSheet = null;
            this.viewsOnSheet = new ObservableCollection<SheetCopierViewOnSheet>();
            foreach (ElementId id in sourceSheet.GetAllPlacedViews()) {              
                Element element = sourceSheet.Document.GetElement(id);
                if (element != null) {
                    var v = element as View;
                    this.viewsOnSheet.Add(new SheetCopierViewOnSheet(v.Name, v, scopy));
                }
            }
        }
               
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewSheet DestinationSheet {
            get; set;    
        }
                
        public ViewSheet SourceSheet {
            get; set;    
        }

        public Collection<string> SheetCategories
        {
            get
            {
                return scopy.SheetCategories;
            }
        }

        public String SelectedSheetCategories
        {
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
                NotifyPropertyChanged("Title");
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

        public ObservableCollection<SheetCopierViewOnSheet> ViewsOnSheet {
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
            var viewCategoryParamList = this.SourceSheet.GetParameters(parameterName);
            if (viewCategoryParamList != null && viewCategoryParamList.Count > 0) {
                Parameter viewCategoryParam = viewCategoryParamList.First();
                string s = viewCategoryParam.AsString();
                return s;
            } 
            return @"n/a";
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
