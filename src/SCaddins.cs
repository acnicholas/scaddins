// (C) Copyright 2014-2020 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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

// [assembly: System.CLSCompliant(true)]
namespace SCaddins
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.UI;
    using Newtonsoft.Json;
    using Properties;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SCaddinsApp : IExternalApplication
    {
        // ReSharper disable once InconsistentNaming
        private static Common.Bootstrapper bootstrapper;
        // ReSharper disable once InconsistentNaming
        private static Common.WindowManager windowManager;
        private RibbonPanel ribbonPanel;
        private PushButton modelWizardPushButton;
        private PushButton scaosPushButton;
        private PushButton scasfarPushButton;
        private PushButton sccopyPushButton;
        private PushButton scheduleExporterPushButton;
        private PushButton scexportPushButton;
        private PushButton scightlinesPushButton;
        private PushButton scloudschedPushButton;
        private PushButton scoordPushButton;
        private PushButton sculcasePushButton;
        private PushButton scincrementPushButton;
        private PushButton scwashPushButton;
        private PushButton spellingChecker;
        private PushButton gridManagerPushButton;
        private PushButton openSheetPushButton;

        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public static Common.WindowManager WindowManager
        {
            get
            {
                if (bootstrapper == null)
                {
                    bootstrapper = new Common.Bootstrapper();
                }
                if (windowManager != null)
                {
                    return windowManager;
                }
                else
                {
                    windowManager = new Common.WindowManager(new Common.BasicDialogService());
                    return windowManager;
                }
            }

            set => windowManager = value;
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void CheckForUpdates(bool newOnly)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = new Uri("https://api.github.com/repos/acnicholas/scaddins/releases/latest");
            var latestVersion = new LatestVersion();
#if NET48
            var webRequest = WebRequest.Create(uri) as HttpWebRequest;
            if (webRequest == null)
            {
                return;
            }

            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Nothing";
            var latestAsJson = "nothing to see here";
            if (latestAsJson == null) {
                throw new ArgumentNullException(nameof(latestAsJson));
            }

            using (var s = webRequest.GetResponse().GetResponseStream())
            using (var sr = new StreamReader(s))
            {
                latestAsJson = sr.ReadToEnd();
            }
            latestVersion = JsonConvert.DeserializeObject<LatestVersion>(latestAsJson);
#else
            var latestAsJson = string.Empty;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/acnicholas/scaddins/releases/latest");
                var response = httpClient.Send(request);
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    latestAsJson = reader.ReadToEnd();
                }
                try
                {
                    latestVersion = System.Text.Json.JsonSerializer.Deserialize<LatestVersion>(latestAsJson);
                }
                catch (Exception ex)
                {
                    SCaddinsApp.WindowManager.ShowErrorMessageBox("Json Error", ex.Message);
                    return;
                }
            }
#endif
            var installedVersion = Version;
            var latestAvailableVersion = new Version(latestVersion.tag_name.Replace("v", string.Empty).Trim());
            var info = latestVersion.body;

            var downloadLink = latestVersion.assets.FirstOrDefault().browser_download_url;
            if (string.IsNullOrEmpty(downloadLink))
            {
                downloadLink = Constants.DownloadLink;
            }

            if (latestAvailableVersion <= installedVersion && newOnly)
            {
                return;
            }
            dynamic settings = new ExpandoObject();
            settings.Height = 640;
            settings.Width = 480;
            settings.Title = "SCaddins Version Information";
            settings.ShowInTaskbar = false;
            settings.ResizeMode = System.Windows.ResizeMode.NoResize;
            settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            var upgradeViewModel = new Common.ViewModels.UpgradeViewModel(installedVersion, latestAvailableVersion, info, downloadLink);
            WindowManager.ShowDialogAsync(upgradeViewModel, null, settings);
        }

        public static PushButtonData LoadInfo(string dll)
        {
            var pbd = new PushButtonData(
                              "Element Information", "Element Information", dll, "SCaddins.WorksharingUtilities.WorksharingUtilitiesCommand");
            pbd.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url, Constants.HelpLink));
            AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.info-rvt-16.png", 16, dll);
            pbd.ToolTip = "Display Additional Element Information";
            pbd.LongDescription = "Display additional element information. Can be used to see the last user who edited an element";
            return pbd;
        }

        public static PushButtonData LoadSCaos(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaos", Resources.AngleOfSun, dll, "SCaddins.SolarAnalysis.Command");
            pbd.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url, Constants.HelpLink));
            //AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.scaos-rvt-16.png", 16, dll);
            pbd.ToolTip = Resources.AngleOfSunToolTip;
            pbd.LongDescription = Resources.AngleOfSunLongDescription;
            return pbd;
        }

        public static PushButtonData LoadSCopy(string dll, int iconSize)
        {
            var pbd = new PushButtonData(
                              "SCopy", Resources.CopySheets, dll, "SCaddins.SheetCopier.Command");

            //AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.scopy-rvt-16.png", 16, dll);
            //AssignPushButtonImage(pbd, "SheetCopier.Assets.scopy-rvt.png", 32, dll);
            pbd.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, Constants.HelpLink));
            pbd.ToolTip = Resources.CopySheetsToolTip;
            return pbd;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

#if REVIT2024 || REVIT2025
        public void ChangeTheme()
        {

#if NET48
            var dll = new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;
#else
        var dll = new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).Location).LocalPath;
#endif


            UITheme theme = UIThemeManager.CurrentTheme;
            //SCaddinsApp.WindowManager.ShowMessageBox(theme.ToString());
            switch (theme)
            {
                case UITheme.Dark:
                    //SCaddinsApp.WindowManager.ShowMessageBox("setting dark theme");
                    AssignPushButtonImage(scexportPushButton, @"SCaddins.Assets.Ribbon.scexport-rvt-dark.png", 32, dll);
                    AssignPushButtonImage(scexportPushButton, @"SCaddins.Assets.Ribbon.scexport-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(scincrementPushButton, @"SCaddins.Assets.Ribbon.scincrement-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(sculcasePushButton, @"SCaddins.Assets.Ribbon.sculcase-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(scwashPushButton,   @"SCaddins.Assets.Ribbon.scwash-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(gridManagerPushButton, @"SCaddins.Assets.Ribbon.gridman-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(sccopyPushButton, @"SCaddins.Assets.Ribbon.scopy-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(sccopyPushButton, @"SheetCopier.Assets.scopy-rvt-dark.png", 32, dll);
                    AssignPushButtonImage(scaosPushButton, @"SCaddins.Assets.Ribbon.scaos-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(scightlinesPushButton, @"SCaddins.Assets.Ribbon.scightlines-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(scasfarPushButton, "SCaddins.Assets.Ribbon.scasfar-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(scloudschedPushButton, "SCaddins.Assets.Ribbon.scloudsched-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(spellingChecker, "SCaddins.Assets.Ribbon.spelling-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(scheduleExporterPushButton, @"SCaddins.Assets.Ribbon.table-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(scoordPushButton, @"SCaddins.Assets.Ribbon.scoord-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(modelWizardPushButton, "SCaddins.Assets.Ribbon.checkdoc-rvt-16-dark.png", 16, dll);
                    AssignPushButtonImage(openSheetPushButton, "SCaddins.Assets.Ribbon.find-rvt-16-dark.png", 16, dll);
                    break;
                case UITheme.Light:
                    //SCaddinsApp.WindowManager.ShowMessageBox("setting light theme");
                    AssignPushButtonImage(scexportPushButton, @"SCaddins.Assets.Ribbon.scexport-rvt.png", 32, dll);
                    AssignPushButtonImage(scexportPushButton, @"SCaddins.Assets.Ribbon.scexport-rvt-16.png", 16, dll);
                    AssignPushButtonImage(scincrementPushButton, @"SCaddins.Assets.Ribbon.scincrement-rvt-16.png", 16, dll);
                    AssignPushButtonImage(sculcasePushButton, @"SCaddins.Assets.Ribbon.sculcase-rvt-16.png", 16, dll);
                    AssignPushButtonImage(scwashPushButton,   @"SCaddins.Assets.Ribbon.scwash-rvt-16.png", 16, dll);
                    AssignPushButtonImage(gridManagerPushButton, @"SCaddins.Assets.Ribbon.gridman-rvt-16.png", 16, dll);
                    AssignPushButtonImage(sccopyPushButton, @"SCaddins.Assets.Ribbon.scopy-rvt-16.png", 16, dll);
                    AssignPushButtonImage(sccopyPushButton, @"SheetCopier.Assets.scopy-rvt.png", 32, dll);
                    AssignPushButtonImage(scaosPushButton, @"SCaddins.Assets.Ribbon.scaos-rvt-16.png", 16, dll);
                    AssignPushButtonImage(scightlinesPushButton, @"SCaddins.Assets.Ribbon.scightlines-rvt-16.png", 16, dll);
                    AssignPushButtonImage(scasfarPushButton, "SCaddins.Assets.Ribbon.scasfar-rvt-16.png", 16, dll);
                    AssignPushButtonImage(scloudschedPushButton, "SCaddins.Assets.Ribbon.scloudsched-rvt-16.png", 16, dll);
                    AssignPushButtonImage(spellingChecker, "SCaddins.Assets.Ribbon.spelling-rvt-16.png", 16, dll);
                    AssignPushButtonImage(scheduleExporterPushButton, @"SCaddins.Assets.Ribbon.table-rvt-16.png", 16, dll);
                    AssignPushButtonImage(scoordPushButton, @"SCaddins.Assets.Ribbon.scoord-rvt-16.png", 16, dll);
                    AssignPushButtonImage(modelWizardPushButton, "SCaddins.Assets.Ribbon.checkdoc-rvt-16.png", 16, dll);
                    AssignPushButtonImage(openSheetPushButton, "SCaddins.Assets.Ribbon.find-rvt-16.png", 16, dll);
                    break;
            }
            ribbonPanel.Visible = false;
            ribbonPanel.Visible = true;
        }
#endif

        public Result OnStartup(UIControlledApplication application)
        {
#if REVIT2024 || REVIT2025
            application.ThemeChanged += Application_ThemeChanged;
#endif

            ribbonPanel = TryGetPanel(application, "Studio.SC");

            if (ribbonPanel == null)
            {
                return Result.Failed;
            }

#if NET48
            var scdll = new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;
#else
            var scdll = new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).Location).LocalPath;
#endif

            var scx = LoadScexport(scdll);
            scexportPushButton = ribbonPanel.AddItem(scx) as PushButton;

            var stackedItemZero = ribbonPanel.AddStackedItems(
                LoadSCopy(scdll, 16),
                LoadSCuv(scdll),
                LoadHatchEditor(scdll));

            sccopyPushButton = stackedItemZero[0] as PushButton;

            var stackedItemOne = ribbonPanel.AddStackedItems(
                LoadSCaos(scdll),
                LoadSCightlines(scdll),
                LoadSCasfar(scdll));

            scaosPushButton = stackedItemOne[0] as PushButton;
            scightlinesPushButton = stackedItemOne[1] as PushButton;
            scasfarPushButton = stackedItemOne[2] as PushButton;

            var stackedItemTwo = ribbonPanel.AddStackedItems(
                LoadScheduleExporter(scdll),
                LoadSCloudShed(scdll),
                LoadSCoord(scdll));

            scheduleExporterPushButton = stackedItemTwo[0] as PushButton;
            scloudschedPushButton = stackedItemTwo[1] as PushButton;
            scoordPushButton = stackedItemTwo[2] as PushButton;


            var stackedItemThree = ribbonPanel.AddStackedItems(
                LoadSCulcase(scdll),
                LoadSpellingChecker(scdll),
                LoadSCincrement(scdll));

            sculcasePushButton = stackedItemThree[0] as PushButton;
            spellingChecker = stackedItemThree[1] as PushButton;
            scincrementPushButton = stackedItemThree[2] as PushButton;

            var stackedItemFour = ribbonPanel.AddStackedItems(
                LoadGridManager(scdll),
                LoadInfo(scdll),
                LoadOpenSheet(scdll));

            gridManagerPushButton = stackedItemFour[0] as PushButton;
            openSheetPushButton = stackedItemFour[2] as PushButton;


            var stackedItemFive = ribbonPanel.AddStackedItems(
                LoadSCwash(scdll),
                LoadModelWizard(scdll),
                LoadAbout(scdll));

            scwashPushButton = stackedItemFive[0] as PushButton;
            modelWizardPushButton = stackedItemFive[1] as PushButton;

            ribbonPanel.AddSlideOut();
            ribbonPanel.AddStackedItems(
                LoadGlobalSettings(scdll),
                LoadRunScript(scdll));
#if REVIT2024 || REVIT2025
            ChangeTheme(); //FIXME, this doesn't need to run everytime, load the correct theme once.
#endif

            return Result.Succeeded;
        }

#if REVIT2024 || REVIT2025
        private void Application_ThemeChanged(object sender, Autodesk.Revit.UI.Events.ThemeChangedEventArgs e)
        {
            //SCaddinsApp.WindowManager.ShowMessageBox("theme changed");
            ChangeTheme();
        }
#endif

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
            if (size == -1)
            {
                size = 32;
            }
            ImageSource image = LoadPNGImageSource(iconName, dll);
            if (image != null && pushButton != null)
            {
                if (size == 32)
                {
                    pushButton.LargeImage = image;
                }
                else
                {
                    pushButton.Image = image;
                }
            }
        }

        private static void AssignPushButtonImage(ButtonData pushButtonData, string iconName, int size, string dll)
        {
            if (size == -1)
            {
                size = 32;
            }
            ImageSource image = LoadPNGImageSource(iconName, dll);
            if (image != null && pushButtonData != null)
            {
                if (size == 32)
                {
                    pushButtonData.LargeImage = image;
                }
                else
                {
                    pushButtonData.Image = image;
                }
            }
        }

        //private static void AssignThemedPushButtonImage(ButtonData pushButtonData, string iconName, int size, string dll)
        //{

        //}

        private static PushButtonData LoadAbout(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaddinsAbout", Resources.About, dll, "SCaddins.Common.About");
            AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.help-rvt-16.png", 16, dll);
            pbd.ToolTip = "About SCaddins.";
            return pbd;
        }

        // from https://github.com/WeConnect/issue-tracker/blob/master/Case.IssueTracker.Revit/Entry/AppMain.cs
        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom")]
        private static ImageSource LoadPNGImageSource(string sourceName, string path)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(Path.Combine(path));
                var icon = assembly.GetManifestResourceStream(sourceName);
                var decoder = new PngBitmapDecoder(icon, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                ImageSource source = decoder.Frames[0];
                return source;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private static PushButtonData LoadSCasfar(string dll)
        {
            var pbd = new PushButtonData(
                              "SCasfar", Resources.RoomTools, dll, "SCaddins.RoomConverter.RoomConverterCommand");
            pbd.ToolTip = Resources.RoomToolsToolTip;
            return pbd;
        }

        private static PushButtonData LoadScexport(string dll)
        {
            var pbd = new PushButtonData(
                          "SCexport", Resources.SCexport, dll, "SCaddins.ExportManager.Command");
            pbd.SetContextualHelp(
                new ContextualHelp(ContextualHelpType.Url, Constants.HelpLink));
            pbd.ToolTip = Resources.SCexportToolTip;
            pbd.LongDescription = Resources.SCexportLongDescription;
            return pbd;
        }

        private static PushButtonData LoadSCightlines(string dll)
        {
            var pbd = new PushButtonData(
                              "SCightLines", Resources.LineofSight, dll, "SCaddins.LineOfSight.Command");
            pbd.ToolTip = Resources.LineofSightToolTip;
            return pbd;
        }

        private static PushButtonData LoadGridManager(string dll)
        {
            var pbd = new PushButtonData(
                              "GridManager", Resources.GridManager, dll, "SCaddins.GridManager.GridManager");
            pbd.ToolTip = Resources.GridManagerToolTip;
            return pbd;
        }

        private static PushButtonData LoadHatchEditor(string dll)
        {
            var pbd = new PushButtonData(
                              "HatchEditor", Resources.HatchEditor, dll, "SCaddins.HatchEditor.Command");
            AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.hatch-rvt-16.png", 16, dll);
            pbd.ToolTip = Resources.HatchEditorToolTip;
            return pbd;
        }

        private static PushButtonData LoadNextSheet(string dll)
        {
            var pbd = new PushButtonData(
                              "Next Sheet", @"Next Sheet", dll, "SCaddins.ExportManager.NextSheet");
            AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.forward-rvt-16.png", 16, dll);
            pbd.ToolTip = "Attempt to open the next sheet";
            return pbd;
        }

        private static PushButtonData LoadOpenSheet(string dll)
        {
            var pbd = new PushButtonData(
                              "Quick Open", @"Quick Open", dll, "SCaddins.ExportManager.OpenSheet");
            pbd.ToolTip = "Find and open a view/sheet";
            return pbd;
        }

        private static PushButtonData LoadPreviousSheet(string dll)
        {
            var pbd = new PushButtonData(
                              "Previous Sheet", @"Previous Sheet", dll, "SCaddins.ExportManager.PreviousSheet");
            AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.back-rvt-16.png", 16, dll);
            pbd.ToolTip = "Attempt to open the previous sheet";
            return pbd;
        }

        private static PushButtonData LoadRunScript(string dll)
        {
            var pbd = new PushButtonData(
                              "RunScript", @"Run Script (lua)", dll, "SCaddins.RunScript.RunScriptCommand");
            AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.lua-rvt-16.png", 16, dll);
            pbd.ToolTip = "Run a lua script";
            return pbd;
        }

        private static PushButtonData LoadSCincrement(string dll)
        {
            var pbd = new PushButtonData(
                              "SCincrement", Resources.IncrementTool, dll, "SCaddins.ParameterUtilities.Command");
            //AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.scincrement-rvt-16.png", 16, dll);
            pbd.ToolTip = Resources.IncrementToolToolTip;
            return pbd;
        }

        private static PushButtonData LoadGlobalSettings(string dll)
        {
            var pbd = new PushButtonData(
                              "Glabal Settings", "Global Settings", dll, "SCaddins.Common.Settings");
            AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.gear-rvt-16.png", 16, dll);
            pbd.ToolTip = Resources.IncrementToolSettings;
            return pbd;
        }

        private static PushButtonData LoadSCloudShed(string dll)
        {
            var pbd = new PushButtonData(
                              "SCloudSChed", Resources.ScheduleClouds, dll, "SCaddins.RevisionUtilities.Command");
            //AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.scloudsched-rvt-16.png", 16, dll);
            pbd.ToolTip = Resources.ScheduleCloudsToolTip;
            return pbd;
        }

        private static PushButtonData LoadScheduleExporter(string dll)
        {
            var pbd = new PushButtonData(
                              "Export Schedules", "Export Schedules", dll, "SCaddins.ExportSchedules.Command");
            pbd.ToolTip = Resources.ScheduleCloudsToolTip;
            return pbd;
        }

        private static PushButtonData LoadSCoord(string dll)
        {
            var pbd = new PushButtonData(
                           "Scoord", Resources.PlaceCoordinate, dll, "SCaddins.PlaceCoordinate.Command");
            pbd.ToolTip = Resources.PlaceCoordinateToolTip;
            return pbd;
        }

        private static PushButtonData LoadSCulcase(string dll)
        {
            var pbd = new PushButtonData(
                           "Rename", Resources.Rename, dll, "SCaddins.RenameUtilities.RenameUtilitiesCommand");
            pbd.ToolTip = Resources.RenameToolTip;
            pbd.LongDescription = Resources.RenameLongDescription;
            return pbd;
        }

        private static PushButtonData LoadSCuv(string dll)
        {
            var pbd = new PushButtonData(
                              "SCuv", Resources.UserView, dll, "SCaddins.ViewUtilities.CreateUserViewCommand");
            AssignPushButtonImage(pbd, "SCaddins.Assets.Ribbon.user-rvt-16.png", 16, dll);
            pbd.ToolTip = Resources.UserViewToolTip;
            return pbd;
        }

        private static PushButtonData LoadSCwash(string dll)
        {
            var pbd = new PushButtonData(
                              "SCwash", Resources.DestructivePurge, dll, "SCaddins.DestructivePurge.Command");
            pbd.ToolTip = Resources.DestructivePurgeToolTip;
            return pbd;
        }

        private static PushButtonData LoadSpellingChecker(string dll)
        {
            var pbd = new PushButtonData("Spelling Checker", "Check Spelling", dll, "SCaddins.SpellChecker.Command"); 
            return pbd;
        }

        private static PushButtonData LoadModelWizard(string dll)
        {
            var pbd = new PushButtonData(
                              "Model Setup Wizard", "Model Setup", dll, "SCaddins.ModelSetupWizard.Command");
            pbd.ToolTip = "Setup up model work sets and parameters";
            return pbd;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
