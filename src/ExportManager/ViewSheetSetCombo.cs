// (C) Copyright 2012-2014 by Andrew Nicholas
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
        using Autodesk.Revit.DB;

        public class ViewSheetSetCombo
        {
                private string customName;

                public ViewSheetSetCombo(ViewSheetSet viewSheetSet)
                {
                        this.ViewSheetSet = viewSheetSet;
                        this.customName = this.ViewSheetSet.Name;
                }

                public ViewSheetSetCombo(string customName)
                {
                        this.ViewSheetSet = null;
                        this.customName = customName;
                }

                public ViewSheetSet ViewSheetSet
                {
                        get; set;
                }

                public string CustomName
                {
                        get { return this.customName; }
                }

                public override string ToString()
                {
                        return this.customName;
                }
        }
}
/* vim: set ts=8 sw=8 nu expandtab: */
