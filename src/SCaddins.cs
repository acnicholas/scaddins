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
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
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
                
        // FIXME this is messy
        public static void CheckForUpdates(bool newOnly)
        {
            const string DownloadURL = SCaddins.Constants.DownloadLink;
            var request = (HttpWebRequest)WebRequest.Create(DownloadURL);
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
            var collection = Scaddins.Default.DisplayOrder;
            
            if (collection.Count < 1) {
                collection = SCaddinsOptionsForm.DefaultCollection;
            }
                                   
            var numberOfAddins = collection.Count;
            
            var ribbonPanel = TryGetPanel(application, "Scott Carver");
            
            if (numberOfAddins > 0) {
                ribbonPanel.AddItem(GetButtonByIndex(collection, 0));
            }
            
            if (numberOfAddins > 3) {
            ribbonPanel.AddStackedItems(
                    GetButtonByIndex(collection, 1),
                    GetButtonByIndex(collection, 2),
                    GetButtonByIndex(collection, 3));
            }
            
            if (numberOfAddins > 6) {
            ribbonPanel.AddStackedItems(
                    GetButtonByIndex(collection, 4),
                    GetButtonByIndex(collection, 5),
                    GetButtonByIndex(collection, 6));
            }
            
            if (numberOfAddins > 9) {
            ribbonPanel.AddStackedItems(
                    GetButtonByIndex(collection, 7),
                    GetButtonByIndex(collection, 8),
                    GetButtonByIndex(collection, 9));
            }
            
            ribbonPanel.AddSlideOut();

            // FIXME - dont do this again.
            string scdll =
                new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;

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
              
        private static PushButtonData GetButtonByName(string name)
        {
            string scdll =
                new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;

            switch (name.ToLower(CultureInfo.CurrentCulture)) {
                case "scexport":
                    return LoadScexport(scdll);
                case "scoord":
                    return LoadSCoord(scdll);
                case "sculcase":
                    return LoadSCulcase(scdll);
                case "scwash":
                    return LoadSCwash(scdll);
                case "scaos":
                    return LoadSCaos(scdll);
                case "scopy":
                    return LoadSCopy(scdll);
                case "scloudsched":
                    return LoadSCloudShed(scdll);
                case "scightlines":
                    return LoadSCightlines(scdll);
                case "scincrement":
                    return LoadSCincrement(scdll);
                case "scuv":
                    return LoadSCuv(scdll);
                default:
                    return null;
            }
        }
        
        private static PushButtonData LoadScexport(string dll)
        {
            var pbd = new PushButtonData(
                          "SCexport", "SCexport", dll, "SCaddins.SCexport.Command");
            AssignPushButtonImage(pbd, @"scexport-rvt.png", 32);
            pbd.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url, Constants.HelpLink));
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
                           "Scoord", "Scoord", dll, "SCaddins.SCoord.Command");
            AssignPushButtonImage(pbd, @"scoord-rvt-16.png", 16);
            pbd.ToolTip =
                "Place a family at a specified shared coordinate.";
            return pbd;
        }

        private static PushButtonData LoadSCulcase(string dll)
        {
            var pbd = new PushButtonData(
                           "SCulcase", "SCulcase", dll, "SCaddins.SCulcase.Command");
            AssignPushButtonImage(pbd, @"sculcase-rvt-16.png", 16);
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
                              "SCwash", "SCwash", dll, "SCaddins.SCwash.Command");
            AssignPushButtonImage(pbd, "scwash-rvt-16.png", 16);
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
            AssignPushButtonImage(pbd, "scaos-rvt-16.png", 16);
            pbd.ToolTip =
                "Rotate a 3d view to the location of the sun.";
            pbd.LongDescription =
                "...Or create multiple views for winter(June 21) in one go.";
            return pbd;
        }

        private static PushButtonData LoadSCightlines(string dll)
        { 
            var pbd = new PushButtonData(
                              "SCightLines", "SCightLines", dll, "SCaddins.SCightLines.Command");
            AssignPushButtonImage(pbd, "scightlines-rvt-16.png", 16);
            pbd.ToolTip =
                "Create line of sight details for stadium seating.";
            return pbd;
        }

        private static PushButtonData LoadSCloudShed(string dll)
        {
            var pbd = new PushButtonData(
                              "SCloudSChed", "SCloudSChed", dll, "SCaddins.SCloudSChed.Command");
            AssignPushButtonImage(pbd, "scloudsched-rvt-16.png", 16);
            pbd.ToolTip =
                "Schedule all revision clouds (in Excel).";
            return pbd;
        }
        
        private static PushButtonData LoadSCincrement(string dll)
        {
            var pbd = new PushButtonData(
                              "SCincrement", "SCincrement", dll, "SCaddins.SCincrement.Command");
            AssignPushButtonImage(pbd, "scincrement-rvt-16.png", 16);
            pbd.ToolTip =
                "Increment room numbers and family marks.";
            return pbd;
        }
        
        private static PushButtonData LoadSCincrementSettings(string dll)
        {
            var pbd = new PushButtonData(
                              "SCincrementSettings", "SCincrementSettings", dll, "SCaddins.SCincrement.SCincrementSettingsCommand");
            AssignPushButtonImage(pbd, "scincrement-rvt-16.png", 16);
            pbd.ToolTip =
                "Increment settings.";
            return pbd;
        }
        
        private static PushButtonData LoadSCaddinSettings(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaddinsOptions", "SCaddinsOptions", dll, "SCaddins.SCaddinsOptions");
            AssignPushButtonImage(pbd, "gear.png", 16);
            pbd.ToolTip =
                "SCaddins settings.";
            return pbd;
        }
        
        private static PushButtonData LoadSCuv(string dll)
        {
            var pbd = new PushButtonData(
                              "SCuv", "SCuv", dll, "SCaddins.SCuv.Command");
            AssignPushButtonImage(pbd, "user.png", 16);
            pbd.ToolTip =
                "Create a user view.";
            return pbd;
        }
        
        private static PushButtonData LoadAbout(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaddinsAbout", "SCaddinsAbout", dll, "SCaddins.Common.About");
            AssignPushButtonImage(pbd, "help.png", 16);
            pbd.ToolTip = "About SCaddins.";
            return pbd;
        }

        private static PushButtonData LoadSCopy(string dll)
        {
            var pbd = new PushButtonData(
                              "SCopy", "SCopy", dll, "SCaddins.SCopy.Command");
            AssignPushButtonImage(pbd, "scopy-rvt-16.png", 16);
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
              
        private static void AssignPushButtonImage(PushButtonData pb, string iconName, int size)
        {
            if (size == -1) {
                size = 32;
            }
            BitmapSource image = LoadBitmapImage(SCaddins.Constants.IconDir + iconName, size);
            if (image != null && pb != null) {
                if (size == 32) {
                    pb.LargeImage = image;
                } else {
                    pb.Image = image;
                }
            }
        }

        private static BitmapSource LoadBitmapImage(string imagePath, int size)
        {
            if (File.Exists(imagePath)) {
                var uriImage = new Uri(imagePath);
                return new BitmapImage(uriImage);   
            } else {
                List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
                var c = System.Windows.Media.Color.FromRgb(128, 128, 128);
                colors.Add(c);
                BitmapPalette colourPalette = new BitmapPalette(colors);
                var pixArray = System.Array.CreateInstance(typeof(byte), size * size);
                return BitmapImage.Create(size, size, 95, 96, System.Windows.Media.PixelFormats.Indexed8, colourPalette, pixArray, 1); 
            }
        }

        private static RibbonPanel TryGetPanel(
            UIControlledApplication application, string name)
        {
            List<RibbonPanel> loadedPanels = application.GetRibbonPanels();
            foreach (RibbonPanel p in loadedPanels) {
                if (p.Name.Equals(name)) {
                    return p;
                }
            }

            return application.CreateRibbonPanel(name);
        }
        
        private static PushButtonData GetButtonByIndex(StringCollection collection, int index)
        {
            return GetButtonByName(collection[index]);
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
