// (C) Copyright 2014 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.UI;

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SCaddinsApp : Autodesk.Revit.UI.IExternalApplication
    {
        public Autodesk.Revit.UI.Result OnStartup(
            UIControlledApplication application)
        {
            string scdll =
                new Uri(Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;
            var ribbonPanel = this.TryGetPanel(application, "Scott Carver");
            LoadScexport(scdll, ribbonPanel);
            LoadSCoord(scdll, ribbonPanel);
            LoadSCulcase(scdll, ribbonPanel);
            LoadSCwash(scdll, ribbonPanel);
            LoadSCaos(scdll, ribbonPanel);
            LoadSCopy(scdll, ribbonPanel);
            LoadSCloudShed(scdll, ribbonPanel);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private void LoadScexport(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                          "SCexport", "SCexport", dll, "SCaddins.SCexport.Command");
            var pushButton = rp.AddItem(pbd) as PushButton;
            this.AssignPushButtonImage(pushButton, @"scexport-rvt.png", 32);
            pushButton.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url, Constants.HelpLink));
            pushButton.ToolTip =
                "Export PDF/DWG file[s] with pre defined naming standards";
            pushButton.LongDescription =
                "SCexport will export file[s] using the internal Revit " +
                "revision for each sheet, and a predefined naming scheme.";
        }

        private void LoadSCoord(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                           "Scoord", "Scoord", dll, "SCaddins.SCoord.Command");
            var pushButton = rp.AddItem(pbd) as PushButton;
            this.AssignPushButtonImage(pushButton, @"scoord-rvt-16.png", 16);
        }

        private void LoadNextSheet()
        {
        }

        private void LoadPreviousSheet()
        {
        }

        private void LoadOpenSheet()
        {
        }

        private void LoadSCulcase(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                           "SCulcase", "SCulcase", dll, "SCaddins.SCulcase.Command");
            var pushButton = rp.AddItem(pbd) as PushButton;
            this.AssignPushButtonImage(pushButton, @"sculcase-rvt-16.png", 16);
        }

        private void LoadSCwash(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                              "SCwash", "SCwash", dll, "SCaddins.SCwash.Command");
            var pushButton = rp.AddItem(pbd) as PushButton;
            this.AssignPushButtonImage(pushButton, "scwash-rvt.png", 32);
        }

        private void LoadSCaos(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                              "SCaos", "SCwash", dll, "SCaddins.SCaos.Command");
            var pushButton = rp.AddItem(pbd) as PushButton;
            this.AssignPushButtonImage(pushButton, "scaos-rvt.png", 32);
        }

        private void LoadSCightlines(string dll, RibbonPanel rp)
        { 
            var pbd = new PushButtonData(
                              "SCightLines", "SCightLines", dll, "SCaddins.SCightLines.Command");
            var pushButton = rp.AddItem(pbd) as PushButton;
            this.AssignPushButtonImage(pushButton, "scightlines32.png", 32);
        }

        private void LoadSCincrement()
        {
        }

        private void LoadSCloudShed(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                              "SCloudSChed", "SCloudSChed", dll, "SCaddins.SCloudSChed.Command");
            var pushButton = rp.AddItem(pbd) as PushButton;
            this.AssignPushButtonImage(pushButton, "scloudsched-rvt.png", 32);
        }

        private void LoadSCopy(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                              "SCopy", "SCopy", dll, "SCaddins.SCopy.Command");
            var pushButton = rp.AddItem(pbd) as PushButton;
            this.AssignPushButtonImage(pushButton, "scopy-rvt.png", 32);
            pushButton.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url,
                    " https://bitbucket.org/anicholas/scaddins/wiki"));
            pushButton.ToolTip =
            "Copy a view sheet and all its content";
            pushButton.LongDescription =
            "SCopy will try to create a copy of the active(focused)sheet " +
            System.Environment.NewLine + System.Environment.NewLine +
            "NOTE: After the new sheet is created, view names need to be munaually edit.";
        }

        private void AssignPushButtonImage(PushButton pb, string iconName, int size)
        {
            BitmapSource image = LoadBitmapImage(SCaddins.Constants.IconDir + iconName, size);
            if (image != null && pb != null) {
                if(size == 32){
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
