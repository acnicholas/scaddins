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
        private string primaryCustomSheetParameter;
        private string secondaryCustomSheetParameter;
        private string tertiaryCustomSheetParameter;
        private ObservableCollection<SheetCopierView> childViews;
        private string number;
        private SheetCopierManager scopy;
        private string sheetCategory;
        private string title;

        public SheetCopierViewHost(string number, string title, SheetCopierManager scopy, ViewSheet sourceSheet)
        {
            this.scopy = scopy ?? throw new ArgumentNullException(nameof(scopy));
            this.number = number;
            this.title = title;
            SourceSheet = sourceSheet ?? throw new ArgumentNullException(nameof(sourceSheet));
            sheetCategory = GetSheetCategory(SheetCopierConstants.SheetCategory);
            PrimaryCustomSheetParameter = GetSheetCategory(Settings.Default.CustomSheetParameterOne);
            SecondaryCustomSheetParameter = GetSheetCategory(Settings.Default.CustomSheetParameterTwo);
            TertiaryCustomSheetParameter = GetSheetCategory(Settings.Default.CustomSheetParameterThree);
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
        }

        public SheetCopierViewHost(SheetCopierManager scopy)
        {
            this.scopy = scopy ?? throw new ArgumentNullException(nameof(scopy));
            this.number = @"<N/A>";
            this.title = "<Independent Views(no sheet) are itemized here>";
            SourceSheet = null;
            Type = ViewHostType.Model;
            PrimaryCustomSheetParameter = null;
            /////userCreatedSheetCategory = null;
            DestinationSheet = null;
            childViews = new ObservableCollection<SheetCopierView>();
        }

        public ObservableCollection<SheetCopierView> ChildViews => childViews;

        public ViewSheet DestinationSheet
        {
            get; set;
        }

        public bool IsSheet => Type == ViewHostType.Sheet;

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

        public string PrimaryCustomSheetParameter
        {
            get
            {
                return primaryCustomSheetParameter;
            }

            set
            {
                primaryCustomSheetParameter = value;
                if (!scopy.CustomSheetParametersOne.Contains(primaryCustomSheetParameter))
                {
                    scopy.CustomSheetParametersOne.Add(primaryCustomSheetParameter);
                }
            }
        }

        public ObservableCollection<string> PrimaryCustomSheetParameters
        {
            get
            {
                return scopy.CustomSheetParametersOne;
            }

            set
            {
                scopy.CustomSheetParametersOne = value;
            }
        }

        public string SecondaryCustomSheetParameter
        {
            get
            {
                return secondaryCustomSheetParameter;
            }

            set
            {
                secondaryCustomSheetParameter = value;
                if (!scopy.CustomSheetParametersTwo.Contains(secondaryCustomSheetParameter))
                {
                    scopy.CustomSheetParametersTwo.Add(secondaryCustomSheetParameter);
                }
            }
        }

        public ObservableCollection<string> SecondaryCustomSheetParameters
        {
            get
            {
                return scopy.CustomSheetParametersTwo;
            }

            set
            {
                scopy.CustomSheetParametersTwo = value;
            }
        }

        public string TertiaryCustomSheetParameter
        {
            get
            {
                return tertiaryCustomSheetParameter;
            }

            set
            {
                tertiaryCustomSheetParameter = value;
                if (!scopy.CustomSheetParametersThree.Contains(tertiaryCustomSheetParameter))
                {
                    scopy.CustomSheetParametersThree.Add(tertiaryCustomSheetParameter);
                }
            }
        }

        public ObservableCollection<string> TertiaryCustomSheetParameters
        {
            get
            {
                return scopy.CustomSheetParametersThree;
            }

            set
            {
                scopy.CustomSheetParametersThree = value;
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

        public ViewHostType Type
        {
            get; set;
        }

        public string GetNewViewName(ElementId id)
        {
            return (from v in childViews where id == v.OldId select v.Title).FirstOrDefault();
        }

        public void RefreshPrimaryCustomSheetParameters()
        {
            NotifyOfPropertyChange(() => PrimaryCustomSheetParameters);
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
