import ('RevitAPI', 'Autodesk.Revit.DB')
import ('C:\\Code\\cs\\scaddins\\src\\bin\\Debug\\SCaddins.dll', 'SCaddins.LineOfSight')

doc = commandData.Application.ActiveUIDocument.Document

sst = StadiumSeatingTier(doc)
sst.TreadSize = 1200
sst.NumberOfRows = 6
sst:Draw()

return sst.InfoString


   