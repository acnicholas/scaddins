// (C) Copyright 2012 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCexport.
//
// SCexport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCexport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCexport.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCexport
{
        using System;
        using Autodesk.Revit.DB;

        /// <summary>
        /// A Revit ViewSheetSet with ToString() overridden.
        /// </summary>
        public class ViewSheetSetCombo
        {
                private string customName;

                /// <summary>
                /// Initializes a new instance of the <see cref="ViewSheetSetCombo"/> class.
                /// </summary>
                /// <param name="vss">The view sheet set.</param>
                public ViewSheetSetCombo(ViewSheetSet vss)
                {
                        this.ViewSheetSet = vss;
                        this.customName = this.ViewSheetSet.Name;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="ViewSheetSetCombo"/> class.
                /// </summary>
                /// <param name="customName">Custom name.</param>
                public ViewSheetSetCombo(string customName)
                {
                        this.ViewSheetSet = null;
                        this.customName = customName;
                }

                /// <summary>
                /// Gets or sets the view sheet set.
                /// </summary>
                /// <value>The view sheet set.</value>
                public ViewSheetSet ViewSheetSet
                {
                        get; set;
                }

                /// <summary>
                /// Gets a custom name to display in the combo box.
                /// </summary>
                /// <value> Custom name to display.</value>
                /// <returns>
                /// The custom string to display in the combo box.
                /// </returns>
                public string CustomName
                {
                        get { return this.customName; }
                }

                /// <summary>
                /// Override ToString to display a useful title when used in
                /// a form.
                /// </summary>
                /// <returns>The view set name as stored in Revit.</returns>
                public override string ToString()
                {
                        return this.customName;
                }
        }
}

/* vim: set ts=8 sw=8 nu expandtab: */
