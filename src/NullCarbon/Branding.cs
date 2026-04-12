// (C) 2024-2026 nullCarbon. Licensed under LGPL-3.0-or-later. See COPYING.LESSER.
namespace SCaddins.NullCarbon
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Single source of truth for everything nullCarbon-branded.
    /// To bump the version, run:  scripts\release.ps1 -Version X.Y.Z
    /// (which rewrites src\SCaddins.csproj). Don't edit Version manually here —
    /// it's read from the assembly so it stays in sync automatically.
    /// </summary>
    internal static class Branding
    {
        public const string ProductName      = "nullCarbon";
        public const string AddInDisplayName = "nullCarbon-LCA-Export";
        public const string PanelName        = "nullCarbon";
        public const string ButtonText       = "nullCarbon Export";
        public const string ButtonTooltip    = "Export Schedules to nullCarbon";
        public const string VendorContact    = "nullCarbon, contact@nullcarbon.dk";

        public const string GitHubOwner      = "bhupas";
        public const string GitHubRepo       = "revit";
        public const string GitHubRepoApi    = "https://api.github.com/repos/" + GitHubOwner + "/" + GitHubRepo;
        public const string LatestReleaseApi = GitHubRepoApi + "/releases/latest";
        public const string ReleasesLink     = "https://github.com/" + GitHubOwner + "/" + GitHubRepo + "/releases/latest";
        public const string UserAgent        = "nullCarbon-RevitExport-Updater";

        // The updater downloads the first GitHub Release asset whose filename
        // ends with this suffix. Releases are produced by scripts\build-installer.ps1
        // and named nullCarbon-LCA-Export-win64-<version>.exe — matching the
        // upstream SCaddins installer naming convention.
        public const string PreferredAssetSuffix = ".exe";

        // ---- Version (auto-read from assembly) -----------------------------

        /// <summary>The full assembly version, e.g. 26.3.2.0.</summary>
        public static Version Version
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return v ?? new Version(0, 0, 0, 0);
            }
        }

        /// <summary>"vX.Y.Z" — short, user-facing form. Use this in UI.</summary>
        public static string VersionShort
        {
            get
            {
                var v = Version;
                return string.Format("v{0}.{1}.{2}", v.Major, v.Minor, v.Build);
            }
        }

        /// <summary>"nullCarbon Export vX.Y.Z" — for window titles and tooltips.</summary>
        public static string ProductWithVersion
        {
            get { return ButtonText + " " + VersionShort; }
        }
    }
}
