//
// (C) Copyright 2013 by Andrew Nicholas
//
// This file is part of SCaos.
//
// SCaos is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaos is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaos.  If not, see <http://www.gnu.org/licenses/>.
//

namespace SCaddins.SCaos
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Autodesk.Revit.UI;
    using System.Windows.Media.Imaging;


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class SCaosApp : Autodesk.Revit.UI.IExternalApplication
    {

        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            string scdll =
                new Uri(Assembly.GetAssembly(typeof(SCaosApp)).CodeBase).LocalPath;
            RibbonPanel ribbonPanel = TryGetPanel(application, "Good Stuff");
            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("Sun View",
                                        "Sun" + System.Environment.NewLine + "View", scdll, "SCaos.Command")) as PushButton;
            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCaos\Data\scaos.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            if (largeImage != null) {
                pushButton.LargeImage = largeImage;
            }
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private RibbonPanel TryGetPanel(UIControlledApplication application, string name)
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
