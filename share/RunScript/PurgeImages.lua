import ('RevitAPI', 'Autodesk.Revit.DB')
import ('C:\\Code\\cs\\scaddins\\src\\bin\\Debug\\SCaddins.dll', 'SCaddins.DestructivePurge')

doc = commandData.Application.ActiveUIDocument.Document
images = DestructivePurgeUtilitiles.Images(doc);
DestructivePurgeUtilitiles.RemoveElements(doc, images);    