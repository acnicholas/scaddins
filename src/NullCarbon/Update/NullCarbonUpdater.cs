// (C) 2024-2026 nullCarbon. Licensed under LGPL-3.0-or-later. See COPYING.LESSER.
namespace SCaddins.NullCarbon.Update
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using Autodesk.Revit.UI;
    using Newtonsoft.Json;

    /// <summary>
    /// One-click updater for the nullCarbon Revit Export.
    /// Checks the fork's GitHub Releases for a newer version, prompts the user
    /// with a Revit TaskDialog, downloads the Inno Setup installer, and runs it.
    /// </summary>
    internal static class NullCarbonUpdater
    {
        public static void CheckForUpdates(bool quietIfNotNewer)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            LatestVersion latest;
            try
            {
                latest = FetchLatestVersion();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("nullCarbon updater: " + ex.Message);
                if (!quietIfNotNewer)
                {
                    ShowError("Could not reach GitHub to check for updates.\n\n" + ex.Message);
                }
                return;
            }

            if (latest == null || string.IsNullOrEmpty(latest.tag_name))
            {
                if (!quietIfNotNewer) ShowError("No releases found.");
                return;
            }

            Version installed = SCaddinsApp.Version;
            Version available;
            try
            {
                available = new Version(latest.tag_name.Replace("v", string.Empty).Trim());
            }
            catch (Exception)
            {
                if (!quietIfNotNewer) ShowError("Could not parse release tag '" + latest.tag_name + "'.");
                return;
            }

            if (available <= installed)
            {
                if (!quietIfNotNewer)
                {
                    ShowInfo("You're up to date.\n\nInstalled version: " + installed);
                }
                return;
            }

            string downloadUrl = SelectPreferredAsset(latest);
            string fileName    = SelectPreferredAssetName(latest);
            if (string.IsNullOrEmpty(downloadUrl))
            {
                ShowError("A new version (" + available + ") is available, but no installer asset was found in the release.\n\nOpen the releases page manually:\n" + Branding.ReleasesLink);
                return;
            }

            if (!ConfirmInstall(installed, available, latest.body))
            {
                return;
            }

            string installerPath;
            try
            {
                installerPath = DownloadInstaller(downloadUrl, fileName);
            }
            catch (Exception ex)
            {
                ShowError("Download failed.\n\n" + ex.Message + "\n\nYou can download manually:\n" + Branding.ReleasesLink);
                return;
            }

            LaunchInstaller(installerPath);
        }

        // ---- HTTP fetch -----------------------------------------------------

        private static LatestVersion FetchLatestVersion()
        {
            string json;
#if NET48
            var req = (HttpWebRequest)WebRequest.Create(Branding.LatestReleaseApi);
            req.ContentType = "application/json";
            req.UserAgent   = Branding.UserAgent;
            using (var s  = req.GetResponse().GetResponseStream())
            using (var sr = new StreamReader(s))
            {
                json = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<LatestVersion>(json);
#else
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("User-Agent", Branding.UserAgent);
                var resp = http.Send(new HttpRequestMessage(HttpMethod.Get, Branding.LatestReleaseApi));
                using (var reader = new StreamReader(resp.Content.ReadAsStream()))
                {
                    json = reader.ReadToEnd();
                }
                return System.Text.Json.JsonSerializer.Deserialize<LatestVersion>(json);
            }
#endif
        }

        // ---- Asset selection ------------------------------------------------

        private static Asset PickAsset(LatestVersion latest)
        {
            if (latest == null || latest.assets == null || latest.assets.Count == 0) return null;

            // Prefer the Inno Setup .exe matching Branding.PreferredAssetSuffix.
            // Fall back to anything ending in .exe, then any first asset.
            return
                   latest.assets.FirstOrDefault(a => a.name != null && a.name.EndsWith(Branding.PreferredAssetSuffix, StringComparison.OrdinalIgnoreCase))
                ?? latest.assets.FirstOrDefault(a => a.name != null && a.name.EndsWith(".exe",  StringComparison.OrdinalIgnoreCase))
                ?? latest.assets.FirstOrDefault();
        }

        private static string SelectPreferredAsset(LatestVersion latest)
        {
            var a = PickAsset(latest);
            return a == null ? null : a.browser_download_url;
        }

        private static string SelectPreferredAssetName(LatestVersion latest)
        {
            var a = PickAsset(latest);
            return (a != null && !string.IsNullOrEmpty(a.name)) ? a.name : "nullCarbon-LCA-Export.exe";
        }

        // ---- Download -------------------------------------------------------

        private static string DownloadInstaller(string url, string fileName)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "nullCarbon-RevitExport-Update");
            Directory.CreateDirectory(tempDir);
            string outPath = Path.Combine(tempDir, fileName);

            // Best-effort delete of any prior partial download.
            try { if (File.Exists(outPath)) File.Delete(outPath); } catch { }

#if NET48
            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent, Branding.UserAgent);
                wc.DownloadFile(url, outPath);
            }
#else
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("User-Agent", Branding.UserAgent);
                http.Timeout = TimeSpan.FromMinutes(10);
                var bytes = http.GetByteArrayAsync(url).GetAwaiter().GetResult();
                File.WriteAllBytes(outPath, bytes);
            }
#endif
            return outPath;
        }

        // ---- Install --------------------------------------------------------

        private static void LaunchInstaller(string installerPath)
        {
            // Inno Setup flags:
            //   /SILENT             - no wizard, just a progress window
            //   /CLOSEAPPLICATIONS  - close Revit before file replacement
            //                         (the .iss already sets CloseApplications=yes)
            //   /NORESTART          - don't auto-restart Windows after install
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName        = installerPath,
                    Arguments       = "/SILENT /CLOSEAPPLICATIONS /NORESTART",
                    UseShellExecute = true,
                });

                ShowInfo(
                    "The installer has been launched.\n\n" +
                    "Revit will close automatically so the update can apply. Re-open Revit when the installer finishes.");
            }
            catch (Exception ex)
            {
                ShowError("Could not launch the installer:\n" + installerPath + "\n\n" + ex.Message);
            }
        }

        // ---- TaskDialog wrappers --------------------------------------------

        private static bool ConfirmInstall(Version installed, Version available, string releaseNotes)
        {
            var dlg = new TaskDialog(Branding.ProductName + " update");
            dlg.MainInstruction = "An update is available";
            dlg.MainContent =
                "Installed: " + installed + "\n" +
                "Available: " + available + "\n\n" +
                (string.IsNullOrEmpty(releaseNotes) ? string.Empty : ("What's new:\n" + Truncate(releaseNotes, 600)));
            dlg.CommonButtons = TaskDialogCommonButtons.None;
            dlg.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Update now", "Download and install. Revit will close automatically.");
            dlg.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Later",      "Skip this update for now.");
            dlg.DefaultButton = TaskDialogResult.CommandLink1;
            return dlg.Show() == TaskDialogResult.CommandLink1;
        }

        private static void ShowInfo(string message)
        {
            var dlg = new TaskDialog(Branding.ProductName + " update");
            dlg.MainContent = message;
            dlg.CommonButtons = TaskDialogCommonButtons.Ok;
            dlg.Show();
        }

        private static void ShowError(string message)
        {
            var dlg = new TaskDialog(Branding.ProductName + " update");
            dlg.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            dlg.MainContent = message;
            dlg.CommonButtons = TaskDialogCommonButtons.Ok;
            dlg.Show();
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s) || s.Length <= max) return s ?? string.Empty;
            return s.Substring(0, max) + "…";
        }
    }
}
