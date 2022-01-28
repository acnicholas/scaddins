// (C) Copyright 2013-2020 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    public class OpenableView
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "view")]
        private View view;

        public OpenableView(string name, string number, View view)
        {
            this.view = view;
            Name = name;
            SheetNumber = number;
            Description = Name + "[" + SheetNumber + "]" + "<" + ViewType + ">";
            ShortDescription = Name + "[" + SheetNumber + "]";
        }

        public string Name
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public ViewType RevitViewType
        {
            get
            {
                return view.ViewType;
            }
        }

        public string SheetNumber
        {
            get; set;
        }

        public string ShortDescription
        {
            get; set;
        }

        public string ViewType => view.ViewType.ToString();

        public void Open()
        {
            if (view != null)
            {
                UIApplication uiApplication = new UIApplication(view.Document.Application);
                uiApplication.ActiveUIDocument.ActiveView = view;
            }
        }

        public bool IsMatch(string searchString, ViewType viewType)
        {
            if (searchString == null)
            {
                return false;
            }
            else
            {
                if (viewType != Autodesk.Revit.DB.ViewType.Undefined)
                {
                    return ShortDescription.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) > -1 && RevitViewType == viewType;
                }
                else
                {
                    return ShortDescription.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) > -1;
                }
            }
        }
    }
}
