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
            var pushButton = ribbonPanel.AddItem(this.LoadScexport(scdll, ribbonPanel)) as PushButton;
            ribbonPanel.AddStackedItems(
                    this.LoadSCoord(scdll, ribbonPanel),
                    this.LoadSCulcase(scdll, ribbonPanel),
                    this.LoadSCwash(scdll, ribbonPanel));
            ribbonPanel.AddStackedItems(
                    this.LoadSCaos(scdll, ribbonPanel),
                    this.LoadSCopy(scdll, ribbonPanel),
                    this.LoadSCloudShed(scdll, ribbonPanel));
            ribbonPanel.AddStackedItems(
                    this.LoadSCightlines(scdll, ribbonPanel),
                    this.LoadSCincrement(scdll, ribbonPanel),
                    this.LoadAbout(scdll, ribbonPanel));
            ribbonPanel.AddSlideOut();
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private PushButtonData LoadScexport(string dll, RibbonPanel rp)
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

        private PushButtonData LoadSCoord(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                           "Scoord", "Scoord", dll, "SCaddins.SCoord.Command");
            this.AssignPushButtonImage(pbd, @"scoord-rvt-16.png", 16);
            pbd.ToolTip =
                "Place a family at a specified shared coordinate.";
            return pbd;
        }

        private PushButtonData LoadSCulcase(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                           "SCulcase", "SCulcase", dll, "SCaddins.SCulcase.Command");
            this.AssignPushButtonImage(pbd, @"sculcase-rvt-16.png", 16);
            pbd.ToolTip =
                "Convert text from upper to lower case.";
            return pbd;
        }

        private PushButtonData LoadSCwash(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                              "SCwash", "SCwash", dll, "SCaddins.SCwash.Command");
            this.AssignPushButtonImage(pbd, "scwash-rvt-16.png", 16);
            pbd.ToolTip =
                "Clean up your model, in a more destructive way than a purge.";
            return pbd;
        }

        private PushButtonData LoadSCaos(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                              "SCaos", "SCaos", dll, "SCaddins.SCaos.Command");
            this.AssignPushButtonImage(pbd, "scaos-rvt-16.png", 16);
            pbd.ToolTip =
                "Rotate a 3d view to the location of the sun.";
            return pbd;
        }

        private PushButtonData LoadSCightlines(string dll, RibbonPanel rp)
        { 
            var pbd = new PushButtonData(
                              "SCightLines", "SCightLines", dll, "SCaddins.SCightLines.Command");
            this.AssignPushButtonImage(pbd, "scightlines-rvt-16.png", 16);
            pbd.ToolTip =
                "Create line of sight details for stadium seating.";
            return pbd;
        }

        private PushButtonData LoadSCloudShed(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                              "SCloudSChed", "SCloudSChed", dll, "SCaddins.SCloudSChed.Command");
            this.AssignPushButtonImage(pbd, "scloudsched-rvt-16.png", 16);
            pbd.ToolTip =
                "Schedule all revision clouds (in Excel).";
            return pbd;
        }
        
        private PushButtonData LoadSCincrement(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                              "SCincrement", "SCincrement", dll, "SCaddins.SCincrement.Command");
            this.AssignPushButtonImage(pbd, "scincrement-rvt-16.png", 16);
            pbd.ToolTip =
                "Increment room numbers and family marks.";
            return pbd;
        }
        
        private PushButtonData LoadAbout(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                              "SCaddinsAbout", "SCaddinsAbout", dll, "SCaddins.Common.About");
            this.AssignPushButtonImage(pbd, "help.png", 16);
            pbd.ToolTip =
                "About SCaddins.";
            return pbd;
        }

        private PushButtonData LoadSCopy(string dll, RibbonPanel rp)
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
            "NOTE: After the new sheet is created, view names need to be munaually edit.";
            return pbd;
        }

        private void AssignPushButtonImage(PushButtonData pb, string iconName, int size)
        {
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
