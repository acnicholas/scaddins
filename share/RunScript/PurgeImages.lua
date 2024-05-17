import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.DestructivePurge')

doc = commandData.Application.ActiveUIDocument.Document
images = DestructivePurgeUtilitiles.Images(doc);
DestructivePurgeUtilitiles.RemoveElements(doc, images);    
