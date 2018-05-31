// (C) Copyright 2013-2018 by Andrew Nicholas
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
    using Autodesk.Revit.UI;
        
    public class OpenableView
    {
        private Autodesk.Revit.DB.View view;

        public OpenableView(string name, string number, Autodesk.Revit.DB.View view)
        {
            this.view = view;
            Name = name;
            SheetNumber = number;
        }
        
        public string Name
        {
            get; set;
        }
        
        public string SheetNumber
        {
            get; set;
        }
        
        public string ViewType {
            get
            {
                return view.ViewType.ToString();
            }
        }

        internal bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) {
                return false;
            } else {
                return Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) > -1 || SheetNumber.IndexOf(searchString) > -1;
            }
        }
               
        public void Open()
        {
            if (view != null) {
                UIApplication uiapp = new UIApplication(view.Document.Application);
                uiapp.ActiveUIDocument.ActiveView = view;
            }
        }
    }
}
