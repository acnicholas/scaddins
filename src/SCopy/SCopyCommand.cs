namespace SCaddins.SCopy
{
    using System;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIApplication application = commandData.Application;
            UIDocument document = application.ActiveUIDocument;
            View view = doc.ActiveView;
        
            Autodesk.Revit.DB.ViewSheet viewSheet = SCopy.ViewToViewSheet(doc.ActiveView);
            if (viewSheet == null) {
                TaskDialog.Show("SCopy", "Scopy need to be started in a sheet view...");
                return Autodesk.Revit.UI.Result.Failed;    
            }
        
            Transaction t = new Transaction(doc, "SCopy");
            t.Start();
            MainForm form = new MainForm(doc, viewSheet);
            form.ShowDialog();
            t.Commit();
            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
