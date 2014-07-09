using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace SCaddins.SCopy
{

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
[Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
[Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
class SCopyApp : Autodesk.Revit.UI.IExternalApplication
{

    public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
    {
        string scdll =
            new Uri(Assembly.GetAssembly(typeof(SCopy)).CodeBase).LocalPath;
        var ribbonPanel = this.TryGetPanel(application, "Scott Carver");
        PushButtonData pbd = new PushButtonData(
            "SCopy", "SCopy", scdll, "SCopy.Command");
        PushButton pushButton = ribbonPanel.AddItem(pbd) as PushButton;
        Uri uriImage = new Uri(
            @"C:\Program Files\SCaddins\SCopy\Data\scopy.png");
        BitmapImage largeImage = new BitmapImage(uriImage);
        pushButton.LargeImage = largeImage;
        #if (!REVIT2012)
        pushButton.SetContextualHelp(
            new ContextualHelp(
                ContextualHelpType.Url,
                " https://bitbucket.org/anicholas/scopy/wiki"));
        #endif
        pushButton.ToolTip =
            "Copy a view sheet and all its content";
        pushButton.LongDescription =
            "SCopy will try to create a copy of the active(focused)sheet " +
            System.Environment.NewLine + System.Environment.NewLine +
            "NOTE: After the new sheet is created, view names need to be munaually edit.";
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
/* vim: set ts=4 sw=4 nu expandtab: */
