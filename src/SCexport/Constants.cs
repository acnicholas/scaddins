// (C) Copyright 2013 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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

    /// <summary>
    /// Description of Constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The acrobat Printer Job Control Registry setting.
        /// </summary>
        public const string AcrobatPJC =
            @"HKEY_CURRENT_USER\Software\Adobe\Acrobat Distiller\PrinterJobControl";

        /// <summary>
        /// The hung app timeout.
        /// This is the amount of time that windows waits before exiting
        /// a non-responsive program.
        /// </summary>
        public const string HungAppTimeout =
            @"HKEY_CURRENT_USER\Control Panel\Desktop";

        /// <summary> The install dir.</summary>
        public const string InstallDir = @"C:\Program Files\SCaddins\SCexport\";

        /// <summary> Example project configuration dir. </summary>
        public const string ExampleConfigDir = "Examples";

        /// <summary> Data directory. </summary>
        public const string DataDir = "Data";

        /// <summary> Example project configuration file. </summary>
        public const string ExampleConfig = "SCexport-example-conf.xml";

        /// <summary>The export directory.</summary>
        public const string DefaultExportDir = "C:\\Temp";
               
        /// <summary> Param name of scale bar visibily. </summary>
        public const string TitleScale = "Scale Bar 1 to";

        /// <summary> Web [http] link to source code. </summary>
        public const string SourceLink =
            "https://bitbucket.org/anicholas/scexport";

        /// <summary> Web [http] link to download binaries. </summary>
        public const string DownloadLink =
            "https://bitbucket.org/anicholas/scexport/downloads";

        /// <summary> Web [http] link to forum. </summary>
        public const string HelpLink =
            "https://bitbucket.org/anicholas/scexport/wiki/Home";

        /// <summary> 
        /// The defualt text to add to an exported pdf's "Author" tag.
        /// </summary>
        public const string PdfAuthor = "SCexport";

        /// <summary>
        /// The tag to add to a pdf if it is not for issue.
        /// This tag is also added when the sheet has no reivision.
        /// </summary>
        public const string PdfNonIssueTag = "***NOT FOR ISSUE***";

        /// <summary>
        /// SCexport registry path.
        /// </summary>
        public const string RegistryPath = 
            @"HKEY_CURRENT_USER\Software\SCaddins\SCexport";
        
        /// <summary> License to display in about box. </summary>
        public static readonly string License =
            "SCexport is free software: you can redistribute it and/or modify " +
            "it under the terms of the GNU Lesser General Public License as "  +
            "published by the Free Software Foundation, either version 3 of " +
            "the License, or (at your option) any later version." +
            System.Environment.NewLine +
            System.Environment.NewLine +
            "SCexport is distributed in the hope that it will be useful, "  +
            "but WITHOUT ANY WARRANTY; without even the implied warranty of " +
            "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the " +
            "GNU Lesser General Public License for more details." +
            System.Environment.NewLine +
            System.Environment.NewLine +
            "You should have received a copy of the GNU Lesser General " +
            "Public License along with SCexport.  " +
            "If not, see <http://www.gnu.org/licenses/>.";
        
        public static string UnionSquareWorkingFiles {
            get { return System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + 
                    @"\workingfiles\unionsquare.scottcarver.com.au";
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
