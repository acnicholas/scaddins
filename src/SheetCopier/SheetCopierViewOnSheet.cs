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

namespace SCaddins.SheetCopier
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using Autodesk.Revit.DB;

    public class SheetCopierViewOnSheet : INotifyPropertyChanged
    {
        private string associatedLevelName;
        private ViewPortPlacementMode creationMode;
        private bool duplicateWithDetailing;
        private string newTitle;
        private ElementId oldId;
        private View oldView;
        private string originalTitle;
        private SheetCopierManager scopy;
        private string viewTemplateName;

        public SheetCopierViewOnSheet(string title, View view, SheetCopierManager scopy)
        {
            if (scopy == null)
            {
                throw new ArgumentNullException(nameof(scopy));
            }
            this.scopy = scopy;
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }
            this.oldView = view;
            this.oldId = view.Id;
            this.originalTitle = title;
            this.SetDefualtCreationMode();
            this.newTitle =
                title + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) + @")";

            ////remove invalid chars before continuing
            this.newTitle = string.Join("_", this.newTitle.Split(System.IO.Path.GetInvalidFileNameChars()));
            this.associatedLevelName = SheetCopierConstants.MenuItemCopy;
            this.viewTemplateName = SheetCopierConstants.MenuItemCopy;
            this.duplicateWithDetailing = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string AssociatedLevelName
        {
            get
            {
                return this.associatedLevelName;
            }

            set
            {
                this.associatedLevelName = value;
                if (value != SheetCopierConstants.MenuItemCopy)
                {
                    this.DuplicateWithDetailing = false;
                    this.creationMode = ViewPortPlacementMode.New;
                }
                else
                {
                    this.creationMode = ViewPortPlacementMode.Copy;
                }
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(
                        this, new PropertyChangedEventArgs(nameof(AssociatedLevelName)));
                }
            }
        }

        public System.Collections.Generic.List<string> AvailableViewTemplates
        {
            get
            {
                List<string> list = new List<string>();
                list.Add(SheetCopierConstants.MenuItemCopy);
                list.AddRange(this.scopy.ViewTemplates.Select(k => k.Key).ToList<string>());
                return list;
            }
        }

        public ViewPortPlacementMode CreationMode
        {
            get
            {
                return this.creationMode;
            }
        }

        public bool DuplicateWithDetailing
        {
            get
            {
                return this.duplicateWithDetailing;
            }

            set
            {
                this.duplicateWithDetailing = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(
                        this, new PropertyChangedEventArgs(nameof(DuplicateWithDetailing)));
                }
            }
        }

        public System.Collections.Generic.List<string> LevelsInModel
        {
            get
            {
                List<string> list = new List<string>();
                list.Add(SheetCopierConstants.MenuItemCopy);
                list.AddRange(this.scopy.Levels.Select(k => k.Key).ToList<string>());
                return list;
            }
        }

        public ElementId OldId
        {
            get
            {
                return this.oldId;
            }
        }

        public View OldView
        {
            get
            {
                return this.oldView;
            }
        }

        public string OriginalTitle
        {
            get
            {
                return this.originalTitle;
            }
        }

        public ViewType RevitViewType
        {
            get
            {
                return this.oldView.ViewType;
            }
        }

        public string Title
        {
            get
            {
                return this.newTitle;
            }

            set
            {
                if (value != this.newTitle && this.scopy.ViewNameAvailable(value))
                {
                    this.newTitle = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(
                            this, new PropertyChangedEventArgs(nameof(Title)));
                    }
                }
                else
                {
                    Autodesk.Revit.UI.TaskDialog.Show(
                        "SCopy - WARNING", value + " exists, you can't use it!.");
                }
            }
        }

        public string ViewTemplateName
        {
            get
            {
                return this.viewTemplateName;
            }

            set
            {
                this.viewTemplateName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(
                        this, new PropertyChangedEventArgs(nameof(ViewTemplateName)));
                }
            }
        }

        public static bool PlanEnough(ViewType viewType)
        {
            switch (viewType)
            {
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
            this.creationMode = this.oldView.ViewType == ViewType.Legend ? ViewPortPlacementMode.Legend : ViewPortPlacementMode.Copy;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */