// (C) Copyright 2014-2017 by Andrew Nicholas andrewnicholas@iinet.net.au
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

namespace SCaddins
{
    public static class Constants
    {
        /// <summary> Web [http] link to changelog. </summary>
        public const string ChangelogLink =
            "https://github.com/acnicholas/scaddins/blob/master/CHANGELOG.md";

        /// <summary> Web [http] link to download binaries. </summary>
        public const string DownloadLink =
            "https://github.com/acnicholas/scaddins/releases/latest";

        //// <summary> Example project configuration dir. </summary>
        //// public const string EtcDirectory = "etc";

        /// <summary> Web [http] link to forum. </summary>
        public const string HelpLink =
            "https://github.com/acnicholas/scaddins/wiki";

        /// <summary> The install dir.</summary>
        public const string InstallDirectory = @"C:\Program Files\Studio.SC\SCaddins\";

        /// <summary> Data directory. </summary>
        public const string ShareDirectory = "share";

        //// <summary> Web [http] link to source code. </summary>
        //// public const string SourceLink = "https://github.com/acnicholas/scaddins";

        /// <summary> License to display in about box. </summary>
        public static readonly string License =
            "SCaddins is free software: you can redistribute it and/or modify " +
            "it under the terms of the GNU Lesser General Public License as " +
            "published by the Free Software Foundation, either version 3 of " +
            "the License, or (at your option) any later version." +
            System.Environment.NewLine +
            System.Environment.NewLine +
            "SCaddins is distributed in the hope that it will be useful, " +
            "but WITHOUT ANY WARRANTY; without even the implied warranty of " +
            "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the " +
            "GNU Lesser General Public License for more details." +
            System.Environment.NewLine +
            System.Environment.NewLine +
            "You should have received a copy of the GNU Lesser General " +
            "Public License along with SCaddins.  " +
            "If not, see <http://www.gnu.org/licenses/>.";

        public static string FamilyDirectory => InstallDirectory + @"\" + ShareDirectory + @"\rfa\";

        //// public static string IconDirectory => InstallDirectory + @"\" + ShareDirectory + @"\icons\";
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
