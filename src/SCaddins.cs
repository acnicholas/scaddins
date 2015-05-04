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

namespace SCaddins
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
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
            var url = SCaddins.Constants.DownloadLink;
            var request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response;
            try {
                response = (HttpWebResponse)request.GetResponse();
            } catch (WebException e) {
                TaskDialog.Show("Error: Check For Updates", e.Message); 
                return;
            } catch (Exception e) {
                TaskDialog.Show("Error: Check For Updates", e.Message);
                return;                
            }
            
            if (response.StatusCode == HttpStatusCode.NotFound) {
                TaskDialog.Show("Error: Check For Updates", url + " not found"); 
                return;
            }
            
            string html = string.Empty;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                try {
                    html = reader.ReadToEnd();
                } catch (IOException e) {
                    TaskDialog.Show("Error: Check For Updates", e.Message);    
                } catch (OutOfMemoryException e) {
                    TaskDialog.Show("Error: Check For Updates", e.Message);    
                }
            }

            var r = new Regex("href=\"(.*)\">.*SCaddins-win64-(.*).msi</a>");
            Match m = r.Match(html);
            Version latestVersion = new Version(0, 0, 0, 0);
            while (m.Success)
            {
                var v = new Version(m.Groups[2].Value);
                if (v > latestVersion) {
                    latestVersion = v;
                }
                m = m.NextMatch();
            }
            
            var installedVersion = SCaddins.SCaddinsApp.Version;

            if (latestVersion > installedVersion) {
                var upgradeForm = new SCaddins.Common.UpgradeForm(installedVersion, latestVersion);
                upgradeForm.ShowDialog();
            } else if (latestVersion < SCaddins.SCaddinsApp.Version) {
                if (!newOnly) {
                    var upgradeForm = new SCaddins.Common.UpgradeForm(installedVersion, latestVersion);
                    upgradeForm.ShowDialog();  
                }
            }
        }
        
        public Autodesk.Revit.UI.Result OnStartup(
            UIControlledApplication application)
        {
            var collection = SCaddins.Scaddins.Default.DisplayOrder;
            
            if (collection.Count < 1) {
                collection = ScaddinsOptionsForm.GetDefualtCollection();
            }
                                   
            var numberOfAddins = collection.Count;
            
            var ribbonPanel = this.TryGetPanel(application, "Scott Carver");
            
            if (numberOfAddins > 0) {
                ribbonPanel.AddItem(this.GetButtonByIndex(collection, 0));
            }
            
            if (numberOfAddins > 3) {
            ribbonPanel.AddStackedItems(
                    this.GetButtonByIndex(collection, 1),
                    this.GetButtonByIndex(collection, 2),
                    this.GetButtonByIndex(collection, 3));
            }
            
            if (numberOfAddins > 6) {
            ribbonPanel.AddStackedItems(
                    this.GetButtonByIndex(collection, 4),
                    this.GetButtonByIndex(collection, 5),
                    this.GetButtonByIndex(collection, 6));
            }
            
            if (numberOfAddins > 9) {
            ribbonPanel.AddStackedItems(
                    this.GetButtonByIndex(collection, 7),
                    this.GetButtonByIndex(collection, 8),
                    this.GetButtonByIndex(collection, 9));
            }
            
            ribbonPanel.AddSlideOut();

            // FIXME - dont do this again.
            string scdll =
                new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;

             ribbonPanel.AddStackedItems(
                    this.LoadAbout(scdll),
                    this.LoadSCincrementSettings(scdll),
                    this.LoadSCaddinSettings(scdll));
                        
            if (SCaddins.Scaddins.Default.UpgradeCheckOnStartUp) {    
                CheckForUpdates(true);
            }
            
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        
        private PushButtonData GetButtonByIndex(StringCollection collection, int index)
        {
            return this.GetButtonByName(collection[index]);
        }
        
        private PushButtonData GetButtonByName(string name)
        {
            string scdll =
                new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;

            switch (name.ToLower()) {
                case "scexport":
                    return this.LoadScexport(scdll);
                case "scoord":
                    return this.LoadSCoord(scdll);
                case "sculcase":
                    return this.LoadSCulcase(scdll);
                case "scwash":
                    return this.LoadSCwash(scdll);
                case "scaos":
                    return this.LoadSCaos(scdll);
                case "scopy":
                    return this.LoadSCopy(scdll);
                case "scloudsched":
                    return this.LoadSCloudShed(scdll);
                case "scightlines":
                    return this.LoadSCightlines(scdll);
                case "scincrement":
                    return this.LoadSCincrement(scdll);
                case "scuv":
                    return this.LoadSCuv(scdll);
                default:
                    return null;
            }
        }
        
        private PushButtonData LoadScexport(string dll)
        {
            var pbd = new PushButtonData(
                          "SCexport", "SCexport", dll, "SCaddins.SCexport.Command");
            this.AssignPushButtonImage(pbd, @"scexport-rvt.png", 32);
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

        private PushButtonData LoadSCoord(string dll)
        {
            var pbd = new PushButtonData(
                           "Scoord", "Scoord", dll, "SCaddins.SCoord.Command");
            this.AssignPushButtonImage(pbd, @"scoord-rvt-16.png", 16);
            pbd.ToolTip =
                "Place a family at a specified shared coordinate.";
            return pbd;
        }

        private PushButtonData LoadSCulcase(string dll)
        {
            var pbd = new PushButtonData(
                           "SCulcase", "SCulcase", dll, "SCaddins.SCulcase.Command");
            this.AssignPushButtonImage(pbd, @"sculcase-rvt-16.png", 16);
            pbd.ToolTip =
                "Convert text from upper to lower case.";
            return pbd;
        }

        private PushButtonData LoadSCwash(string dll)
        {
            var pbd = new PushButtonData(
                              "SCwash", "SCwash", dll, "SCaddins.SCwash.Command");
            this.AssignPushButtonImage(pbd, "scwash-rvt-16.png", 16);
            pbd.ToolTip =
                "Clean up your model, in a more destructive way than a purge.";
            return pbd;
        }

        private PushButtonData LoadSCaos(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaos", "Angle Of Sun", dll, "SCaddins.SCaos.Command");
            this.AssignPushButtonImage(pbd, "scaos-rvt-16.png", 16);
            pbd.ToolTip =
                "Rotate a 3d view to the location of the sun.";
            return pbd;
        }

        private PushButtonData LoadSCightlines(string dll)
        { 
            var pbd = new PushButtonData(
                              "SCightLines", "SCightLines", dll, "SCaddins.SCightLines.Command");
            this.AssignPushButtonImage(pbd, "scightlines-rvt-16.png", 16);
            pbd.ToolTip =
                "Create line of sight details for stadium seating.";
            return pbd;
        }

        private PushButtonData LoadSCloudShed(string dll)
        {
            var pbd = new PushButtonData(
                              "SCloudSChed", "SCloudSChed", dll, "SCaddins.SCloudSChed.Command");
            this.AssignPushButtonImage(pbd, "scloudsched-rvt-16.png", 16);
            pbd.ToolTip =
                "Schedule all revision clouds (in Excel).";
            return pbd;
        }
        
        private PushButtonData LoadSCincrement(string dll)
        {
            var pbd = new PushButtonData(
                              "SCincrement", "SCincrement", dll, "SCaddins.SCincrement.Command");
            this.AssignPushButtonImage(pbd, "scincrement-rvt-16.png", 16);

            pbd.ToolTip =
                "Increment room numbers and family marks.";
            return pbd;
        }
        
        private PushButtonData LoadSCincrementSettings(string dll)
        {
            var pbd = new PushButtonData(
                              "SCincrementSettings", "SCincrementSettings", dll, "SCaddins.SCincrement.SCincrementSettingsCommand");
            this.AssignPushButtonImage(pbd, "scincrement-rvt-16.png", 16);
            pbd.ToolTip =
                "Increment settings.";
            return pbd;
        }
        
        private PushButtonData LoadSCaddinSettings(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaddinsOptions", "SCaddinsOptions", dll, "SCaddins.SCaddinsOptions");
            this.AssignPushButtonImage(pbd, "gear.png", 16);
            pbd.ToolTip =
                "SCaddins settings.";
            return pbd;
        }
        
        private PushButtonData LoadSCuv(string dll)
        {
            var pbd = new PushButtonData(
                              "SCuv", "SCuv", dll, "SCaddins.SCuv.Command");
            this.AssignPushButtonImage(pbd, "user.png", 16);
            pbd.ToolTip =
                "Create a user view.";
            return pbd;
        }
        
        private PushButtonData LoadAbout(string dll)
        {
            var pbd = new PushButtonData(
                              "SCaddinsAbout", "SCaddinsAbout", dll, "SCaddins.Common.About");
            this.AssignPushButtonImage(pbd, "help.png", 16);
            pbd.ToolTip = "About SCaddins.";
            return pbd;
        }

        private PushButtonData LoadSCopy(string dll)
        {
            var pbd = new PushButtonData(
                              "SCopy", "SCopy", dll, "SCaddins.SCopy.Command");
            this.AssignPushButtonImage(pbd, "scopy-rvt-16.png", 16);
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
              
        private void AssignPushButtonImage(PushButtonData pb, string iconName, int size)
        {
            if (size == -1) {
                size = 32;
            }
            BitmapSource image = this.LoadBitmapImage(SCaddins.Constants.IconDir + iconName, size);
            if (image != null && pb != null) {
                if (size == 32) {
                    pb.LargeImage = image;
                } else {
                    pb.Image = image;
                }
            }
        }

        private BitmapSource LoadBitmapImage(string imagePath, int size)
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

        private RibbonPanel TryGetPanel(
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
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
