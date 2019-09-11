using Autodesk.Revit.DB;

public static void Main(Document doc)
{
    using (var t = new Transaction(doc))
    {
        t.Start("Run Script");
        var fec = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms);
        foreach (var r in fec)
        {
            var room = r as Autodesk.Revit.DB.Architecture.Room;
            //convert sq feet to sqm
            var area = room.Area * 0.092903;
            if (area < 50) {
                room.Name = "Room";
                continue;
            }
            if (area < 75) {
                room.Name = "1B";
                continue;
            }
            if (area < 95) {
                room.Name = "2B";
                continue;
            }
            if (area >= 95) {
                room.Name = "3B";
                continue;
            }
        }
        t.Commit();
    }
}