using Autodesk.Revit.DB;
using SCaddins.SolarAnalysis;

public static void Main(Document doc)
{      
    //// FIXME create new constructor with doc not uidoc.
    var manager =  new SolarAnalysisManager(doc);
    manager.Create3dViews = true;
    manager.Go(null);
}
