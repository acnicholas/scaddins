// (C) 2024-2026 nullCarbon. Licensed under LGPL-3.0-or-later. See COPYING.LESSER.
namespace SCaddins.NullCarbon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.UI;
    using SCaddins.NullCarbon.Update;

    internal static class NullCarbonModule
    {
        private const string IconResourcePrefix = "SCaddins.Assets.Ribbon.";
        private const string Icon16Light       = IconResourcePrefix + "nullcarbon-rvt-16.png";
        private const string Icon16Dark        = IconResourcePrefix + "nullcarbon-rvt-16-dark.png";
        private const string Icon32Light       = IconResourcePrefix + "nullcarbon-rvt.png";
        private const string Icon32Dark        = IconResourcePrefix + "nullcarbon-rvt-dark.png";

        private static RibbonPanel ribbonPanel;
        private static PushButton  exportButton;

        public static Result Initialize(UIControlledApplication application)
        {
#if REVIT2024 || REVIT2025 || REVIT2026 || REVIT2027
            application.ThemeChanged += OnThemeChanged;
#endif

            ribbonPanel = TryGetPanel(application, Branding.PanelName);
            if (ribbonPanel == null)
            {
                return Result.Failed;
            }

#if NET48
            var dll = new Uri(Assembly.GetAssembly(typeof(NullCarbonModule)).CodeBase).LocalPath;
#else
            var dll = new Uri(Assembly.GetAssembly(typeof(NullCarbonModule)).Location).LocalPath;
#endif

            var data = LoadExportButtonData(dll);
            exportButton = ribbonPanel.AddItem(data) as PushButton;

            AssignPushButtonImage(exportButton, Icon16Light, 16, dll);
            AssignPushButtonImage(exportButton, Icon32Light, 32, dll);

#if REVIT2024 || REVIT2025 || REVIT2026 || REVIT2027
            ApplyTheme(dll);
#endif

            // Background, fire-and-forget update check. Quiet if up to date so
            // we never block startup or annoy users on a stable build. Errors
            // are swallowed so a dead GitHub never breaks Revit launch.
            System.Threading.Tasks.Task.Run(() =>
            {
                try { NullCarbonUpdater.CheckForUpdates(quietIfNotNewer: true); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("nullCarbon updater (startup): " + ex.Message); }
            });

            return Result.Succeeded;
        }

        public static Result Shutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public static void CheckForUpdates(bool newOnlyIfNewer)
            => NullCarbonUpdater.CheckForUpdates(newOnlyIfNewer);

#if REVIT2024 || REVIT2025 || REVIT2026 || REVIT2027
        private static void OnThemeChanged(object sender, Autodesk.Revit.UI.Events.ThemeChangedEventArgs e)
        {
#if NET48
            var dll = new Uri(Assembly.GetAssembly(typeof(NullCarbonModule)).CodeBase).LocalPath;
#else
            var dll = new Uri(Assembly.GetAssembly(typeof(NullCarbonModule)).Location).LocalPath;
#endif
            ApplyTheme(dll);
        }

        private static void ApplyTheme(string dll)
        {
            var theme = UIThemeManager.CurrentTheme;
            switch (theme)
            {
                case UITheme.Dark:
                    AssignPushButtonImage(exportButton, Icon16Dark, 16, dll);
                    AssignPushButtonImage(exportButton, Icon32Dark, 32, dll);
                    break;
                case UITheme.Light:
                    AssignPushButtonImage(exportButton, Icon16Light, 16, dll);
                    AssignPushButtonImage(exportButton, Icon32Light, 32, dll);
                    break;
            }
            // Force a ribbon refresh
            ribbonPanel.Visible = false;
            ribbonPanel.Visible = true;
        }
#endif

        private static PushButtonData LoadExportButtonData(string dll)
        {
            var pbd = new PushButtonData(
                "ExportSchedules",
                Branding.ButtonText,
                dll,
                "SCaddins.ExportSchedules.Command");
            pbd.ToolTip          = Branding.ButtonTooltip;
            pbd.LongDescription  = Branding.ProductWithVersion;
            AssignPushButtonImage(pbd, Icon16Light, 16, dll);
            AssignPushButtonImage(pbd, Icon32Light, 32, dll);
            return pbd;
        }

        private static RibbonPanel TryGetPanel(UIControlledApplication application, string name)
        {
            if (application == null || string.IsNullOrEmpty(name))
            {
                return null;
            }
            List<RibbonPanel> loadedPanels = application.GetRibbonPanels();
            foreach (RibbonPanel p in loadedPanels)
            {
                if (p.Name.Equals(name, StringComparison.InvariantCulture))
                {
                    return p;
                }
            }
            return application.CreateRibbonPanel(name);
        }

        private static void AssignPushButtonImage(PushButton pushButton, string iconName, int size, string dll)
        {
            ImageSource image = LoadPNGImageSource(iconName, dll);
            if (image != null && pushButton != null)
            {
                if (size == 32) pushButton.LargeImage = image;
                else            pushButton.Image      = image;
            }
        }

        private static void AssignPushButtonImage(ButtonData pushButtonData, string iconName, int size, string dll)
        {
            ImageSource image = LoadPNGImageSource(iconName, dll);
            if (image != null && pushButtonData != null)
            {
                if (size == 32) pushButtonData.LargeImage = image;
                else            pushButtonData.Image      = image;
            }
        }

        private static ImageSource LoadPNGImageSource(string sourceName, string path)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(Path.Combine(path));
                var icon = assembly.GetManifestResourceStream(sourceName);
                if (icon == null) return null;
                var decoder = new PngBitmapDecoder(icon, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return decoder.Frames[0];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
