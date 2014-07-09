// (C) Copyright 2012-2013 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCexport.
//
// SCexport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCexport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCexport.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCexport
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
    public class SCexportApp : Autodesk.Revit.UI.IExternalApplication
    {
        /// <summary>
        /// Setup and run the Add-in when Revit starts.
        /// </summary>
        /// <param name="application"> This Revit isntance. </param>
        /// <returns> The exit status. </returns>
        public Autodesk.Revit.UI.Result OnStartup(
                UIControlledApplication application)
        {
            string scdll =
                new Uri(Assembly.GetAssembly(typeof(SCexport)).CodeBase).LocalPath;
            var ribbonPanel = this.TryGetPanel(application, "Scott Carver");
            var pbd = new PushButtonData(
                    "SCexport", "SCexport", scdll, "SCexport.Command");
            var pushButton = ribbonPanel.AddItem(pbd) as PushButton;
            var uriImage = new Uri(
                    @"C:\Program Files\SCaddins\SCexport\Data\scexport.png");
            var largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
            #if (!REVIT2012)
            pushButton.SetContextualHelp(
                new ContextualHelp(
                    ContextualHelpType.Url, Constants.HelpLink));
            #endif
            pushButton.ToolTip =
                "Export PDF/DWG file[s] with pre defined naming standards";
            pushButton.LongDescription =
                "SCexport will export file[s] using the internal Revit " +
                "revision for each sheet, and a predefined naming scheme.";
            return Result.Succeeded;
        }

        /// <summary>
        /// What happens on shutdown?.
        /// </summary>
        /// <param name="application"> The Revit application that this add-in is runing in. </param>
        /// <returns> The exit status. </returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
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
