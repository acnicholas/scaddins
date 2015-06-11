// (C) Copyright 2014-2015 by Andrew Nicholas
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
    using System.Globalization;
    using Autodesk.Revit.DB;

    /// <summary>
    /// Description of SCopySheetView.
    /// </summary>
    public class SCopyViewOnSheet : INotifyPropertyChanged
    {
        private SCopy scopy;
        private string originalTitle;
        private string newTitle;
        private ElementId oldId;
        private string associatedLevelName;
        private View oldView;
        private string viewTemplateName;
              
        private bool duplicateWithDetailing;
        private SCopy.ViewCreationMode creationMode;
   
        public SCopyViewOnSheet(string title, View v, SCopy scopy)
        {
            this.scopy = scopy;
            this.oldView = v;
            this.oldId = v.Id;
            this.originalTitle = title;
            this.SetDefualtCreationMode();
            this.newTitle =
                title + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) + @")";
            this.associatedLevelName = SCopyConstants.MenuItemCopy;
            this.viewTemplateName = SCopyConstants.MenuItemCopy;
            this.duplicateWithDetailing = true;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    
        public SCopy.ViewCreationMode CreationMode {
            get {
                return this.creationMode;
            }
        }

        public ElementId OldId {
            get {
                return this.oldId;
            }
        }

        public View OldView {
            get {
                return this.oldView;
            }
        }

        public ViewType RevitViewType {
            get {
                return this.oldView.ViewType;
            }
        }

        public string ViewTemplateName {
            get {
                return this.viewTemplateName;
            }
            
            set {
                this.viewTemplateName = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(
                        this, new PropertyChangedEventArgs("ViewTemplateName"));
                }
            }
        }

        public string AssociatedLevelName {
            get {
                return this.associatedLevelName;
            }
            
            set {
                this.associatedLevelName = value;
                if (value != SCopyConstants.MenuItemCopy) {
                    this.DuplicateWithDetailing = false;
                    this.creationMode = SCopy.ViewCreationMode.CopyAndModify;
                } else {
                    this.creationMode = SCopy.ViewCreationMode.Copy;   
                }
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(
                        this, new PropertyChangedEventArgs("AssociatedLevelName"));
                }
            }
        }

        public string Title {
            get {
                return this.newTitle;
            }
            
            set {
                if (value != this.newTitle && this.scopy.CheckViewNameAvailability(value)) {
                    this.newTitle = value;
                    if (this.PropertyChanged != null) {
                        this.PropertyChanged(
                            this, new PropertyChangedEventArgs("Title"));
                    }
                } else {
                    Autodesk.Revit.UI.TaskDialog.Show(
                        "SCopy - WARNING", value + " exists, you can't use it!.");
                }
            }
        }

        public string OriginalTitle {
            get {
                return this.originalTitle;
            }
        }
        
        public bool DuplicateWithDetailing {
            get {
                return this.duplicateWithDetailing;
            }
            
            set {
                this.duplicateWithDetailing = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(
                        this, new PropertyChangedEventArgs("DuplicateWithDetailing"));
                }
            }
        }
        
        public static bool PlanEnough(ViewType vt)
        {
            switch (vt) {
                case ViewType.FloorPlan:
                case ViewType.CeilingPlan:
                case ViewType.AreaPlan:
                    return true;
                default:
                    return false;
            }
        }
        
        public bool PlanEnough()
        {
            return PlanEnough(this.RevitViewType);
        }
        
        private void SetDefualtCreationMode()
        {
            if (this.oldView.ViewType == ViewType.Legend) {
                this.creationMode = SCopy.ViewCreationMode.Place;   
            } else {
                this.creationMode = SCopy.ViewCreationMode.Copy;
            }
        }  
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
