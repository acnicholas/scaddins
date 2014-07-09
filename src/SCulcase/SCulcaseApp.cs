using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace SCaddins.SCulcase
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class SCulcaseApp : Autodesk.Revit.UI.IExternalApplication
    {

        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel = TryGetPanel(application,"Good Stuff");
            #if REVIT2012
            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("SCulcase",
                "Uppercase" + System.Environment.NewLine + "Text", @"C:\Program Files\SCaddins\SCulcase\2012\SCulcase12.dll", "SCulcase.Command")) as PushButton;
            #elif REVIT2013
            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("SCulcase",
                "Uppercase" + System.Environment.NewLine + "Text", @"C:\Program Files\SCaddins\SCulcase\2013\SCulcase13.dll", "SCulcase.Command")) as PushButton;
            #else
            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("SCulcase",
                "Uppercase" + System.Environment.NewLine + "Text", @"C:\Program Files\SCaddins\SCulcase\2014\SCulcase14.dll", "SCulcase.Command")) as PushButton;
            #endif
            Uri uriImage = new Uri(@"C:\Program Files\SCaddins\SCulcase\Data\sculcase.png");
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
