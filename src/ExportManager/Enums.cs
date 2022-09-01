// (C) Copyright 2015-2020 by Andrew Nicholas
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

    [Flags]
    public enum ExportOptions
    {
        /// <summary>Export Nothing.</summary>
        None = 0,

        /// <summary>Export files using Adobe Acrobat.</summary>
        PDF = 1,

        /// <summary>Export a AutoCAD file.</summary>
        DWG = 2,

        /// <summary>Export A Microstation file.</summary>
        DGN = 4,

        /// <summary>Export a Autodesk dwf file.</summary>
        DWF = 8,

        /// <summary>
        /// Export files using built in Revit exporter to create pdf's.
        /// </summary>
        DirectPDF = 16,

        /// <summary>Remove titleblock from sheet before exporting.
        /// </summary>
        NoTitle = 32,

        /// <summary>
        /// Export files using PDF24 create pdf's.
        /// </summary>
        PDF24 = 64,
    }

    public enum LogType
    {
        Error,
        Warning,
        Normal
    }
}
