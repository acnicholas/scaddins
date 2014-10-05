// (C) Copyright 2013-2014 by Andrew Nicholas
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

namespace SCaddins.SCexport
{
    using System;
    
    /// <summary>
    /// Description of TipOfDay.
    /// </summary>
    public static class TipOfDay
    {
        private static readonly string[] Tips =
        {
            "Press '?' to get a list of keyboard shorcuts",
            "Press '/' to filter the main view via a search queery",
            "You can press 'i' for more tips",
            "Pressing 'L' will filter the main view to display only the latest revision",
            "You can export AutoCAD files without the titleblock (it's under options)",
            "SCexport adds meta tags to pdf exports...",
            "You can add project specfic naming, goto Options-->Exported filenames config",
            "You can click on the export dir column to change the export dirictory",
            "You can click on the scale verify the scale and print settings",
            "Press 'P' to set the revision to todays date.",
            "If you press 'O' SCexport will open the current selection.",
            "Press 'A' to select all.",
            "Press 'N' to select none.",
            "If you press 'S' the main view will scroll to the currently " +
                "active view.",
            "Press 'T' for another Tip of The Day.",
            "The quickes way to export your latest revision is to type:" +
                System.Environment.NewLine +
                "    LDX - to export to Deleverables" +
                System.Environment.NewLine +
                @"    LX - to export to c:\Temp",
            @"Some advanced use of the search feature '/':" +
                System.Environment.NewLine +
                @"    type '12' to find files that contain '12'"  +
                System.Environment.NewLine +
                @"    type '^12' to find files that start with '12'"
        };

        /// <summary>
        /// Create a random tip-of-the-day.
        /// </summary>
        /// <returns> A tip of the day string.</returns>
        public static string Tip()
        {
            var random = new Random();
            int randomNumber = random.Next(0, Tips.Length);
            return Tips[randomNumber];
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
