// (C) Copyright 2014-2015 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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
    using System.Text.RegularExpressions;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.UI;
        
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
            const string DownloadURL = SCaddins.Constants.DownloadLink;
            var request = (HttpWebRequest)WebRequest.Create(new Uri(DownloadURL));
            HttpWebResponse response;
            try {
                response = (HttpWebResponse)request.GetResponse();
            } catch (WebException e) {
                System.Diagnostics.Debug.WriteLine("Error: Check For Updates WebException: " + e.Message);
                return;
            } catch (InvalidOperationException e) {
                System.Diagnostics.Debug.WriteLine("Error: Check For Updates InvalidOperationException: " + e.Message);
                return;                             
            } catch (NotSupportedException e) {
                System.Diagnostics.Debug.WriteLine("Error: Check For Updates NotSupportedException: " + e.Message);
                return;                
            }
            
            if (response.StatusCode == HttpStatusCode.NotFound) {
                System.Diagnostics.Debug.WriteLine("Error: Check For Updates" + DownloadURL + " not found");
                return;
            }
            
            string html = string.Empty;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                try {
                    html = reader.ReadToEnd();
                } catch (IOException e) {
                    System.Diagnostics.Debug.WriteLine("Error: Check For Updates IOException: " + e.Message);                    
                } catch (OutOfMemoryException e) {
                    System.Diagnostics.Debug.WriteLine("Error: Check For Updates OutOfMemoryException: " + e.Message);                    
                }
            }

            var r = new Regex("href=\"(.*)\">.*SCaddins-win64-(.*).msi</a>");
            Match m = r.Match(html);
            var latestVersion = new Version(0, 0, 0, 0);
            while (m.Success)
            {
                var v = new Version(m.Groups[2].Value);
                if (v > latestVersion) {
                    latestVersion = v;
                }
                m = m.NextMatch();
            }
            
            var installedVersion = SCaddinsApp.Version;

            if (latestVersion > installedVersion) {
                var upgradeForm = new SCaddins.Common.UpgradeForm(installedVersion, latestVersion);
                upgradeForm.ShowDialog();
            } else if (latestVersion < SCaddinsApp.Version) {
                if (!newOnly) {
                    var upgradeForm = new SCaddins.Common.UpgradeForm(installedVersion, latestVersion);
                    upgradeForm.ShowDialog();  
                }
            }
        }
        
        public Autodesk.Revit.UI.Result OnStartup(
            UIControlledApplication application)
        {            
            var ribbonPanel = TryGetPanel(application, "Scott Carver");
                               
            

            string scdll =
                new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;
            
            ribbonPanel.AddItem(LoadScexport(scdll));
            ribbonPanel.AddStackedItems(
                LoadSCoord(scdll),
                LoadSCulcase(scdll),
                LoadSCwash(scdll)
            );
            ribbonPanel.AddStackedItems(
                LoadSCaos(scdll),
                LoadSCopy(scdll),
                LoadSCloudShed(scdll)
            );
            ribbonPanel.AddStackedItems(
                LoadSCightlines(scdll),
                LoadSCincrement(scdll),
                LoadSCuv(scdll)
            );
            //ribbonPanel.AddItem(LoadSCasfar(scdll));
            //ribbonPanel.AddItem(LoadSCam(scdll));
            
            ribbonPanel.AddSlideOut();

            ribbonPanel.AddStackedItems(
                LoadAbout(scdll),
                LoadSCincrementSettings(scdll),
                LoadSCaddinSettings(scdll)
            );
                        
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
                          "SCexport", "SCexport", dll, "SCaddins.SCexport.Command");
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
                           "SCulcase", "Change Case", dll, "SCaddins.SCulcase.Command");
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

        private static PushButtonData LoadSCaos(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaos", "Angle Of Sun", dll, "SCaddins.SCaos.Command");
            pbd.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url, Constants.HelpLink));
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scaos-rvt-16.png", 16, dll);
            pbd.ToolTip =
                "Rotate a 3d view to the location of the sun.";
            pbd.LongDescription =
                "...Or create multiple views for winter(June 21) in one go.";
            return pbd;
        }

        private static PushButtonData LoadSCightlines(string dll)
        { 
            var pbd = new PushButtonData(
                              "SCightLines", "Line of Sight", dll, "SCaddins.SCightLines.Command");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scightlines-rvt-16.png", 16, dll);
            pbd.ToolTip =
                "Create line of sight details for stadium seating.";
            return pbd;
        }

        private static PushButtonData LoadSCloudShed(string dll)
        {
            var pbd = new PushButtonData(
                              "SCloudSChed", "Schedule Clouds", dll, "SCaddins.SCloudSChed.Command");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scloudsched-rvt-16.png", 16, dll);
            pbd.ToolTip = "Schedule all revision clouds (in Excel).";
            return pbd;
        }
        
        private static PushButtonData LoadSCincrement(string dll)
        {
            var pbd = new PushButtonData(
                              "SCincrement", "Increment Tool", dll, "SCaddins.SCincrement.Command");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scincrement-rvt-16.png", 16, dll);
            pbd.ToolTip = "Increment room numbers and family marks.";
            return pbd;
        }
        
        private static PushButtonData LoadSCincrementSettings(string dll)
        {
            var pbd = new PushButtonData(
                              "SCincrementSettings", "Increment Tool Settings", dll, "SCaddins.SCincrement.SCincrementSettingsCommand");
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
                              "SCuv", "User View", dll, "SCaddins.SCuv.Command");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.user.png", 16, dll);
            pbd.ToolTip = "Create a user view.";
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

        private static PushButtonData LoadSCopy(string dll)
        {
            var pbd = new PushButtonData(
                              "SCopy", "Copy Sheets", dll, "SCaddins.SCopy.Command");
            AssignPushButtonImage(pbd, "SCaddins.src.Assets.scopy-rvt-16.png", 16, dll);
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
              
        private static void AssignPushButtonImage(PushButtonData pb, string iconName, int size, string dll)
        {
            if (size == -1) {
                size = 32;
            }
            ImageSource image = LoadPngImgSource(iconName, dll);
            if (image != null && pb != null) {
                if (size == 32) {
                    pb.LargeImage = image;
                } else {
                    pb.Image = image;
                }
            }
        }

        //from https://github.com/WeConnect/issue-tracker/blob/master/Case.IssueTracker.Revit/Entry/AppMain.cs
        private static ImageSource LoadPngImgSource(string sourceName, string path)
        {
            try {
                Assembly m_assembly = Assembly.LoadFrom(Path.Combine(path));
                Stream m_icon = m_assembly.GetManifestResourceStream(sourceName);
                PngBitmapDecoder m_decoder = new PngBitmapDecoder(m_icon, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                ImageSource m_source = m_decoder.Frames[0];
                return (m_source);
            } catch {
            }
            return null;

        }
        
        private static RibbonPanel TryGetPanel(UIControlledApplication application, string name)
        {
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
