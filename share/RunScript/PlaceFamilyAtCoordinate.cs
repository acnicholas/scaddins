using Autodesk.Revit.DB;
using SCaddins.PlaceCoordinate;
using System.Collections.Generic;

public static void Main(Document doc)
{      
        var families = Command.GetAllFamilySymbols(doc);
        var family = Command.TryGetDefaultSpotCoordFamily(families);
        var location = new XYZ(0,0,0);
        Command.PlaceFamilyAtCoordinate(doc, family, location, true);
}
