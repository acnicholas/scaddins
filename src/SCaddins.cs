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

    /// <summary>
    /// The Revit external Application.
    /// </summary>
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
            return Result.Succeeded;
        }
        
        private void LoadScexport(string dll, RibbonPanel rp)
        {
            var pbd = new PushButtonData(
                          "SCexport", "SCexport", dll, "SCaddins.SCexport.Command");
            var pushButton = rp.AddItem(pbd) as PushButton;
            pushButton.LargeImage = this.LoadBitmapImage(
                @"C:\Program Files\SCaddins\SCexport\Data\scexport.png", 32);
            pushButton.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url, Constants.HelpLink));
            pushButton.ToolTip =
                "Export PDF/DWG file[s] with pre defined naming standards";
            pushButton.LongDescription =
                "SCexport will export file[s] using the internal Revit " +
            "revision for each sheet, and a predefined naming scheme.";            
        }
        
        private void LoadScoord(string dll, RibbonPanel rp)
        {
             PushButton pushButton = rp.AddItem(
                    new PushButtonData("Coordinate Tool",
                    "Coordinate" + System.Environment.NewLine + "Tool",
                    @"C:\Program Files\SCaddins\SCoord\2014\SCoord14.dll",
                    "SCoord.Command")) as PushButton;
            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCoord\Data\scoord.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
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
        
        private void LoadSCoord(string dll, RibbonPanel rp)
        {
            PushButton pushButton = rp.AddItem(
                new PushButtonData(
                    "Coordinate Tool",
                    "Coordinate" + System.Environment.NewLine + "Tool",
                    dll,
                    "SCoord.Command")) as PushButton;
            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCoord\Data\scoord.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
        }
        
        private void LoadSCulcase(string dll, RibbonPanel rp)
        {
            PushButton pushButton = rp.AddItem(new PushButtonData("SCulcase",
                                        "Uppercase" + System.Environment.NewLine + "Text", @"C:\Program Files\SCaddins\SCulcase\2014\SCulcase14.dll", "SCulcase.Command")) as PushButton;
            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCulcase\Data\sculcase.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
        }
        
        private void LoadSCwash(string dll, RibbonPanel rp)
        {    
            PushButton pushButton = rp.AddItem(new PushButtonData("SCwash",
                                        "SCwash", @"C:\Program Files\SCaddins\SCwash\2014\SCwash14.dll", "SCwash.Command")) as PushButton;

            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCwash\Data\scwash.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
        }
        
        private void LoadSCaos(string dll, RibbonPanel rp)
        {     
            PushButton pushButton = rp.AddItem(new PushButtonData("Sun View",
                                        "Sun" + System.Environment.NewLine + "View", dll, "SCaos.Command")) as PushButton;
            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCaos\Data\scaos.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            if (largeImage != null) {
                pushButton.LargeImage = largeImage;
            }     
        }

        private void LoadSCightlines(string dll, RibbonPanel rp)
        {     
            PushButton pushButton = rp.AddItem(new PushButtonData("SCightLines",
                                        "Line" + System.Environment.NewLine + "of Sight", dll, "SCightLines.Command")) as PushButton;
            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCightLines\Data\scightlines32.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
        }

        private void LoadSCincrement()
        {           
        }
        
        public void LoadSCloudShed(string dll, RibbonPanel rp)
        {
            PushButton pushButton = rp.AddItem(new PushButtonData("Cloud Scheduler",
                                         "Cloud" + System.Environment.NewLine + "Scheduler", @"C:\Program Files\SCaddins\SCloudSChed\2014\SCloudSChed14.dll", "SCloudSChed.Command")) as PushButton;
            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCloudSChed\Data\scloudsched.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
        }
        
        private void LoadSCopy(string dll, RibbonPanel rp)
        {
            PushButtonData pbd = new PushButtonData(
                                 "SCopy", "SCopy", dll, "SCopy.Command");
            PushButton pushButton = rp.AddItem(pbd) as PushButton;
            Uri uriImage = new Uri(
                           @"C:\Program Files\SCaddins\SCopy\Data\scopy.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
            pushButton.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url,
                    " https://bitbucket.org/anicholas/scopy/wiki"));
            pushButton.ToolTip =
            "Copy a view sheet and all its content";
            pushButton.LongDescription =
            "SCopy will try to create a copy of the active(focused)sheet " +
            System.Environment.NewLine + System.Environment.NewLine +
            "NOTE: After the new sheet is created, view names need to be munaually edit.";
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
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
                BitmapPalette myPalette = new BitmapPalette(colors);
                var pixArray = System.Array.CreateInstance(typeof(byte), 32 * 32);
                return BitmapImage.Create(size, size, 96, 96, System.Windows.Media.PixelFormats.Indexed8, myPalette, pixArray, 1); 
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
