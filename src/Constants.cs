namespace SCaddins
{
    using System;

    /// <summary>
    /// Description of Conctants.
    /// </summary>
    public static class Constants
    {
        /// <summary> Example project configuration dir. </summary>
        public const string EtcDir = "etc";

        /// <summary> Data directory. </summary>
        public const string ShareDir = "share";

        /// <summary> Data directory. </summary>
        public const string OptDir = "opt";

        /// <summary> The install dir.</summary>
        public const string InstallDir = @"C:\Program Files\SCaddins\SCaddins\";

        /// <summary> Web [http] link to source code. </summary>
        public const string SourceLink =
            "https://bitbucket.org/anicholas/scaddins";

        /// <summary> Web [http] link to download binaries. </summary>
        public const string DownloadLink =
            "https://bitbucket.org/anicholas/scaddins/downloads";

        /// <summary> Web [http] link to forum. </summary>
        public const string HelpLink =
            "https://bitbucket.org/anicholas/scaddins/wiki/Home";

        public static string FamilyDir
        {
            get {
                return InstallDir + @"\" + ShareDir + @"\rfa\";
            }
        }

        public static string IconDir
        {
            get {
                return InstallDir + @"\" + ShareDir + @"\icons\";
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
