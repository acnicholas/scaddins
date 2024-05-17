import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.RoomConverter')
doc = commandData.Application.ActiveUIDocument.Document
manager = RoomConversionManager(doc)
manager:CreateRoomMasses(manager.Candidates)
return "OK"


   
