// (C) Copyright 2014-2021 by Andrew Nicholas
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

    public class SheetCopierView : INotifyPropertyChanged
    {
        private string associatedLevelName;
        private ViewPortPlacementMode creationMode;
        private bool duplicateWithDetailing;
        private string newTitle;
        private ElementId oldId;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "oldView")]
        private View oldView;
        private string originalTitle;
        private SheetCopierManager scopy;
        private string viewTemplateName;

        public SheetCopierView(string title, View view, SheetCopierManager scopy)
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
            oldView = view;
            oldId = view.Id;
            originalTitle = title;
            SetDefualtCreationMode();
            newTitle =
                title + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) + @")";

            ////remove invalid chars before continuing
            newTitle = string.Join("_", newTitle.Split(System.IO.Path.GetInvalidFileNameChars()));
            associatedLevelName = SheetCopierConstants.MenuItemCopy;
            viewTemplateName = SheetCopierConstants.MenuItemCopy;
            duplicateWithDetailing = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string AssociatedLevelName
        {
            get => associatedLevelName;

            set
            {
                associatedLevelName = value;
                if (value != SheetCopierConstants.MenuItemCopy)
                {
                    DuplicateWithDetailing = false;
                    creationMode = ViewPortPlacementMode.New;
                }
                else
                {
                    creationMode = ViewPortPlacementMode.Copy;
                }

                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(AssociatedLevelName)));
            }
        }

        public bool AssociatedLevelNameIsEnabled
        {
            get => this.PlanEnough();
        }

        public List<string> AvailableViewTemplates
        {
            get
            {
                List<string> list = new List<string>();
                list.Add(SheetCopierConstants.MenuItemCopy);
                list.AddRange(scopy.ViewTemplates.Select(k => k.Key).ToList());
                return list;
            }
        }

        public ViewPortPlacementMode CreationMode => creationMode;

        public bool DuplicateWithDetailing
        {
            get => duplicateWithDetailing;

            set
            {
                duplicateWithDetailing = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(
                        this, new PropertyChangedEventArgs(nameof(DuplicateWithDetailing)));
                }
            }
        }

        public List<string> LevelsInModel
        {
            get
            {
                List<string> list = new List<string>();
                list.Add(SheetCopierConstants.MenuItemCopy);
                list.AddRange(scopy.Levels.Select(k => k.Key).ToList());
                return list;
            }
        }

        public ElementId OldId => oldId;

        public View OldView => oldView;

        public string OriginalTitle => originalTitle;

        public ViewType RevitViewType => oldView.ViewType;

        public string Title
        {
            get => newTitle;

            set
            {
                if (value != newTitle && scopy.ViewNameAvailable(value))
                {
                    newTitle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
                }
                else
                {
                    SCaddinsApp.WindowManager.ShowMessageBox(
                        "SCopy - WARNING", value + " exists, you can't use it!.");
                }
            }
        }

        public string ViewTemplateName
        {
            get => viewTemplateName;

            set
            {
                viewTemplateName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(
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
            return PlanEnough(RevitViewType);
        }

        private void SetDefualtCreationMode()
        {
            creationMode = oldView.ViewType == ViewType.Legend ? ViewPortPlacementMode.Legend : ViewPortPlacementMode.Copy;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
