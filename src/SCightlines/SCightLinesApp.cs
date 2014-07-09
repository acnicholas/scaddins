//
// (C) Copyright 2012-2013 by Andrew Nicholas
//
// This file is part of SCightlines.
//
// SCightlines is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCightlines is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCightlines.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace SCaddins.SCightLines
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class SCightLinesApp : Autodesk.Revit.UI.IExternalApplication
    {

        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel = TryGetPanel(application,"Scott Carver");
            #if REVIT2014
            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("SCightLines",
                "Line" + System.Environment.NewLine + "of Sight", @"C:\Program Files\SCaddins\SCightLines\2014\SCightLines.dll", "SCightLines.Command")) as PushButton;
            #elif REVIT2013
               PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("SCightLines",
                "Line" + System.Environment.NewLine + "of Sight", @"C:\Program Files\SCaddins\SCightLines\2013\SCightLines.dll", "SCightLines.Command")) as PushButton;
            #else
               PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("SCightLines",
                "Line" + System.Environment.NewLine + "of Sight", @"C:\Program Files\SCaddins\SCightLines\2012\SCightLines.dll", "SCightLines.Command")) as PushButton;
            #endif
            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCightLines\Data\scightlines32.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
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
