import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.RoomConverter')
-- uncomment below for debug mode 
-- import ('C:\\Code\\cs\\scaddins\\src\\bin\\Debug\\SCaddins.dll', 'SCaddins.RoomConverter')

doc = commandData.Application.ActiveUIDocument.Document
manager = RoomConversionManager(doc)
manager:CreateRoomMasses(manager.Candidates)

return "OK"


   
