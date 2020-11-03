// (C) Copyright 2014-2020 by Andrew Nicholas
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
    using System.Collections.ObjectModel;
    using System.Linq;
    using Autodesk.Revit.DB;

    /// <summary>
    /// This could be a sheet or a model.
    /// </summary>
    public class SheetCopierViewHost : Caliburn.Micro.PropertyChangedBase
    {
        private string number;
        private SheetCopierManager scopy;
        private string sheetCategory;
        private string userCreatedSheetCategory;
        private string title;
        private ObservableCollection<SheetCopierView> childViews;

        public SheetCopierViewHost(string number, string title, SheetCopierManager scopy, ViewSheet sourceSheet)
        {
            this.scopy = scopy ?? throw new ArgumentNullException(nameof(scopy));
            this.number = number;
            this.title = title;
            SourceSheet = sourceSheet ?? throw new ArgumentNullException(nameof(sourceSheet));
            sheetCategory = GetSheetCategory(SheetCopierConstants.SheetCategory);
            userCreatedSheetCategory = sheetCategory;
            DestinationSheet = null;
            Type = ViewHostType.Sheet;
            childViews = new ObservableCollection<SheetCopierView>();
            foreach (var id in sourceSheet.GetAllPlacedViews())
            {
                Element element = sourceSheet.Document.GetElement(id);
                var v = element as View;
                if (v == null)
                {
                    continue;
                }
                childViews.Add(new SheetCopierView(v.Name, v, scopy));
            }
            SheetCategories = new ObservableCollection<string>(scopy.SheetCategories.ToList());
        }

        public SheetCopierViewHost(SheetCopierManager scopy)
        {
            this.scopy = scopy ?? throw new ArgumentNullException(nameof(scopy));
            this.number = @"<N/A>";
            this.title = "<Independent Views(no sheet) are itemized here>";
            SourceSheet = null;
            Type = ViewHostType.Model;
            sheetCategory = null;
            userCreatedSheetCategory = null;
            DestinationSheet = null;
            childViews = new ObservableCollection<SheetCopierView>();
            SheetCategories = new ObservableCollection<string>(scopy.SheetCategories.ToList());
        }

        public ViewHostType Type
        {
            get; set;
        }

        public ViewSheet DestinationSheet
        {
            get; set;
        }

        public string Number
        {
            get
            {
                return number;
            }

            set
            {
                if (value != number && scopy.SheetNumberAvailable(value))
                {
                    number = value;
                    NotifyOfPropertyChange(() => Number);
                }
                else
                {
                    SCaddinsApp.WindowManager.ShowMessageBox(
                        "SCopy - WARNING", value + " exists, you can't use it!.");
                }
            }
        }

        public ObservableCollection<string> SheetCategories
        {
            get; 
        }

        public string UserCreatedSheetCategory {
            get => userCreatedSheetCategory;

            set
            {
                userCreatedSheetCategory = value;

                foreach (var s in scopy.ViewHosts) {
                    if (!s.SheetCategories.Contains(userCreatedSheetCategory)) {
                        s.SheetCategories.Add(userCreatedSheetCategory);
                        s.RefreshSheetCategories();
                    }
                }
                SheetCategory = userCreatedSheetCategory;
                NotifyOfPropertyChange(() => UserCreatedSheetCategory);
            }
        }

        public string SheetCategory {
            get => sheetCategory;

            set
            {
                if (sheetCategory != value) {
                    sheetCategory = value;
                    NotifyOfPropertyChange(() => SheetCategory);
                }
            }
        }

public ViewSheet SourceSheet
        {
            get; set;
        }

        public string Title
        {
            get => title;

            set
            {
                title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public ObservableCollection<SheetCopierView> ChildViews => childViews;

        public string GetNewViewName(ElementId id)
        {
            return (from v in childViews where id == v.OldId select v.Title).FirstOrDefault();
        }

        public void RefreshSheetCategories()
        {
            NotifyOfPropertyChange(() => SheetCategories);
        }

        private string GetSheetCategory(string parameterName)
        {
            var viewCategoryParamList = SourceSheet.GetParameters(parameterName);
            if (viewCategoryParamList != null && viewCategoryParamList.Count > 0)
            {
                Parameter viewCategoryParam = viewCategoryParamList.First();
                string s = viewCategoryParam.AsString();
                if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                {
                    return @"<None>";
                }
                return s;
            }
            return @"<None>";
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */