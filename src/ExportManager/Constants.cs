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
    public static class Constants
    {
        /// <summary>
        /// The acrobat Printer Job Control Registry setting.
        /// </summary>
        public const string AcrobatPrinterJobControl =
            @"HKEY_CURRENT_USER\Software\Adobe\Acrobat Distiller\PrinterJobControl";

        /// <summary>
        /// The acrobat Printer Job Control Registry setting.
        /// </summary>
        public const string PDF24AutoSaveDir =
            @"HKEY_CURRENT_USER\Software\PDF24\Services\PDF";

        /// <summary>
        /// The hung app timeout.
        /// This is the amount of time that windows waits before exiting
        /// a non-responsive program.
        /// </summary>
        public const string HungAppTimeout =
            @"HKEY_CURRENT_USER\Control Panel\Desktop";

        /// <summary> Example project configuration file. </summary>
        public const string ExampleConfigFileName = "SCexport-example-conf.xml";

        /// <summary>The export directory.</summary>
        public const string DefaultExportDirectory = @"C:\Temp";

        /// <summary> Param name of scale bar visibility. </summary>
        public const string TitleScale = "Scale Bar 1 to";
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
