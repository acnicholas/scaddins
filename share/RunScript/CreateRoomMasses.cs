using Autodesk.Revit.DB;
using SCaddins.RoomConverter;

public static void Main(Document doc)
{      
    var manager =  new RoomConversionManager(doc);
    manager.CreateRoomMasses(manager.Candidates);
}
