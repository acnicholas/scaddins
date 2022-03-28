import ('RevitAPI', 'Autodesk.Revit.DB')
import ('C:\\Code\\cs\\scaddins\\src\\bin\\Debug\\SCaddins.dll', 'SCaddins.RoomConverter')

doc = commandData.Application.ActiveUIDocument.Document
manager = RoomConversionManager(doc)
manager:CreateRoomMasses(manager.Candidates)

return "OK"


   