using Autodesk.Revit.DB;
using SCaddins.DestructivePurge;

public static void Main(Document doc)
{
    var images = DestructivePurgeUtilitiles.Images(doc);
    DestructivePurgeUtilitiles.RemoveElements(doc, images);    
}
