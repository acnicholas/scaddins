// (C) Copyright 2014-2016 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.UI;
    using Newtonsoft.Json;

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SCaddinsApp : Autodesk.Revit.UI.IExternalApplication
    {
        public static Version Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
        }
        
        public static void CheckForUpdates(bool newOnly)
        {
            var uri = new Uri("https://api.github.com/repos/acnicholas/scaddins/releases/latest");
            var webRequest = WebRequest.Create(uri) as HttpWebRequest;
            if (webRequest == null) {
                return;
            }

            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Nothing";
            string latestAsJson = "nothing to see here";

            using (var s = webRequest.GetResponse().GetResponseStream()) {
                using (var sr = new StreamReader(s)) {
                    latestAsJson = sr.ReadToEnd();
                }
            }

            LatestVersion latestVersion = JsonConvert.DeserializeObject<LatestVersion>(latestAsJson);
            
            var installedVersion = SCaddinsApp.Version;
            Version latestAvailableVersion = new Version(latestVersion.tag_name.Replace("v","").Trim());
            string info = latestVersion.body;
            
            if (latestAvailableVersion > installedVersion || !newOnly) {
                var upgradeForm = new SCaddins.Common.UpgradeForm(installedVersion, latestAvailableVersion, info);
                upgradeForm.ShowDialog();
                upgradeForm.Dispose();
            } 
        }
        
        public Autodesk.Revit.UI.Result OnStartup(
            UIControlledApplication application)
        {
            var ribbonPanel = TryGetPanel(application, "Scott Carver");

            if (ribbonPanel == null) {
                return Result.Failed;
            }

            string scdll =
                new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;

            ribbonPanel.AddItem(LoadScexport(scdll));
            ribbonPanel.AddStackedItems(
                LoadSCoord(scdll),
                LoadSCulcase(scdll),
                LoadSCwash(scdll));
            ribbonPanel.AddStackedItems(
                LoadSCaos(scdll, 16),
                LoadSCopy(scdll, 16),
                LoadSCloudShed(scdll));
            ribbonPanel.AddStackedItems(
                LoadSCightlines(scdll),
                LoadSCincrement(scdll),
                LoadSCuv(scdll));
            ribbonPanel.AddStackedItems(
                LoadSCasfar(scdll),
                LoadSCam(scdll));

            ribbonPanel.AddSlideOut();

            ribbonPanel.AddStackedItems(
                LoadAbout(scdll),
                LoadSCincrementSettings(scdll),
                LoadSCaddinSettings(scdll));

            if (SCaddins.Scaddins.Default.UpgradeCheckOnStartUp) {    
                CheckForUpdates(true);
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private static PushButtonData LoadScexport(string dll)
        {
            var pbd = new PushButtonData(
                          "SCexport", "SCexport", dll, "SCaddins.ExportManager.Command");
            AssignPushButtonImage(pbd, @"SCaddins.src.Assets.scexport-rvt.png", 32, dll);
            pbd.SetContextualHelp(
                new ContextualHelp(ContextualHelpType.Url, Constants.HelpLink));
            pbd.ToolTip =
                "Export PDF/DWG file[s] with pre defined naming standards";
            pbd.LongDescription =
                "SCexport will export file[s] using the internal Revit " +
                "revision for each sheet, and a predefined naming scheme.";
            return pbd;
        }

        private static PushButtonData LoadSCoord(string dll)
        {
            var pbd = new PushButtonData(
                           "Scoord", "Place Coordinate", dll, "SCaddins.SCoord.Command");
            AssignPushButtonImage(pbd, @"SCaddins.src.Assets.scoord-rvt-16.png", 16, dll);
            pbd.ToolTip =
                "Place a family at a specified shared coordinate.";
            return pbd;
        }

        private static PushButtonData LoadSCulcase(string dll)
        {
            var pbd = new PushButtonData(
                           "SCulcase", "Change Case", dll, "SCaddins.ParameterUtils.EditTextParameters");
            AssignPushButtonImage(pbd, @"SCaddins.src.Assets.sculcase-rvt-16.png", 16, dll);
            pbd.ToolTip =
                "Convert text from upper to lower case, or vise-versa";
            pbd.LongDescription =
                "Pre-select text/tags to change a selection. " +
                "Run with no selection to change the entire project.";
            return pbd;
        }

        private static PushButtonData LoadSCwash(string dll)
        {
            var pbd = new PushButtonData(
                              "SCwash", "Destructive Purge", dll, "SCaddins.SCwash.Command");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scwash-rvt-16.png", 16, dll);
            pbd.ToolTip =
                "Clean up your model, in a more destructive way than a purge.";
            return pbd;
        }

        public static PushButtonData LoadSCaos(string dll, int iconSize)
        {
            var pbd = new PushButtonData(
                              "SCaos", "Angle Of Sun", dll, "SCaddins.SolarUtilities.Command");
            pbd.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url, Constants.HelpLink));
            if(iconSize == 16) {
                AssignPushButtonImage(pbd, "SCaddins.src.Assets.scaos-rvt-16.png", 16, dll);
            } else {
                AssignPushButtonImage(pbd, "AngleOfSun.Assets.scaos-rvt.png", 32, dll);
            }
            pbd.ToolTip =
                "Rotate a 3d view to the location of the sun.";
            pbd.LongDescription =
                "...Or create multiple views for winter(June 21) in one go.";
            return pbd;
        }

        private static PushButtonData LoadSCightlines(string dll)
        { 
            var pbd = new PushButtonData(
                              "SCightLines", "Line of Sight", dll, "SCaddins.LineOfSight.Command");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scightlines-rvt-16.png", 16, dll);
            pbd.ToolTip =
                "Create line of sight details for stadium seating.";
            return pbd;
        }

        private static PushButtonData LoadSCloudShed(string dll)
        {
            var pbd = new PushButtonData(
                              "SCloudSChed", "Schedule Clouds", dll, "SCaddins.RevisionUtilities.Command");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scloudsched-rvt-16.png", 16, dll);
            pbd.ToolTip = "Schedule revision clouds and/or re-assign revisions to them.";
            return pbd;
        }

        private static PushButtonData LoadSCincrement(string dll)
        {
            var pbd = new PushButtonData(
                              "SCincrement", "Increment Tool", dll, "SCaddins.ParameterUtils.Command");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scincrement-rvt-16.png", 16, dll);
            pbd.ToolTip = "Increment room numbers and family marks.";
            return pbd;
        }

        private static PushButtonData LoadSCincrementSettings(string dll)
        {
            var pbd = new PushButtonData(
                              "SCincrementSettings", "Increment Tool Settings", dll, "SCaddins.ParameterUtils.SCincrementSettingsCommand");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scincrement-rvt-16.png", 16, dll);
            pbd.ToolTip = "Increment settings.";
            return pbd;
        }

        private static PushButtonData LoadSCaddinSettings(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaddinsOptions", "Options", dll, "SCaddins.SCaddinsOptions");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.gear.png", 16, dll);
            pbd.ToolTip = "SCaddins settings.";
            return pbd;
        }

        private static PushButtonData LoadSCuv(string dll)
        {
            var pbd = new PushButtonData(
                              "SCuv", "User View", dll, "SCaddins.ViewUtilities.CreateUserViewCommand");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.user.png", 16, dll);
            pbd.ToolTip = "Create a user view.";
            return pbd;
        }
        
        private static PushButtonData LoadSCam(string dll)
        {
            var pbd = new PushButtonData(
                              "SCam", "Create Perspective", dll, "SCaddins.ViewUtilities.CameraFromViewCommand");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scam-rvt-16.png", 16, dll);
            pbd.ToolTip = "Create a perspective view from the current view (3d or plan).";
            return pbd;
        }

        private static PushButtonData LoadSCasfar(string dll)
        {
            var pbd = new PushButtonData(
                              "SCasfar", "Room Tools", dll, "SCaddins.RoomConvertor.RoomConvertorCommand");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scasfar-rvt-16.png", 16, dll);
            pbd.ToolTip = "Creates sheets and/or solids(masses) from a selection of rooms.";
            return pbd;
        }

        private static PushButtonData LoadAbout(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaddinsAbout", "About", dll, "SCaddins.Common.About");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.help.png", 16, dll);
            pbd.ToolTip = "About SCaddins.";
            return pbd;
        }

        public static PushButtonData LoadSCopy(string dll, int iconSize)
        {
            var pbd = new PushButtonData(
                              "SCopy", "Copy Sheets", dll, "SCaddins.SheetCopier.Command");
            if(iconSize == 16) {
                AssignPushButtonImage(pbd, "SCaddins.src.Assets.scopy-rvt-16.png", 16, dll);
            } else {
                AssignPushButtonImage(pbd, "SheetCopier.Assets.scopy-rvt.png", 32, dll);    
            }
            pbd.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url,
                    " https://bitbucket.org/anicholas/scaddins/wiki"));
            pbd.ToolTip =
            "Copy a view sheet and all its content";
            pbd.LongDescription =
            "SCopy will try to create a copy of the active(focused)sheet " +
            System.Environment.NewLine + System.Environment.NewLine +
            "NOTE: After the new sheet is created, view names may need to be munaually edit.";
            return pbd;
        }

        public static void AssignPushButtonImage(ButtonData pushButtonData, string iconName, int size, string dll)
        {
            if (size == -1) {
                size = 32;
            }
            ImageSource image = LoadPNGImageSource(iconName, dll);
            if (image != null && pushButtonData != null) {
                if (size == 32) {
                    pushButtonData.LargeImage = image;
                } else {
                    pushButtonData.Image = image;
                }
            }
        }

        //from https://github.com/WeConnect/issue-tracker/blob/master/Case.IssueTracker.Revit/Entry/AppMain.cs
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom")]
        public static ImageSource LoadPNGImageSource(string sourceName, string path)
        {
            try {
                Assembly m_assembly = Assembly.LoadFrom(Path.Combine(path));
                Stream m_icon = m_assembly.GetManifestResourceStream(sourceName);
                PngBitmapDecoder m_decoder = new PngBitmapDecoder(m_icon, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                ImageSource m_source = m_decoder.Frames[0];
                return (m_source);
            } catch (Exception ex){
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static RibbonPanel TryGetPanel(UIControlledApplication application, string name)
        {
            if (application == null || string.IsNullOrEmpty(name)) {
                return null;
            }
            List<RibbonPanel> loadedPanels = application.GetRibbonPanels();
            foreach (RibbonPanel p in loadedPanels) {
                if (p.Name.Equals(name)) {
                    return p;
                }
            }
            return application.CreateRibbonPanel(name);
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
