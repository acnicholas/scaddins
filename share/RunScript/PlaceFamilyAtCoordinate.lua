import ('RevitAPI', 'Autodesk.Revit.DB')
import ('System', 'System.Collections.Generic')
import ('SCaddins', 'SCaddins.PlaceCoordinate')

doc = commandData.Application.ActiveUIDocument.Document

families = Command.GetAllFamilySymbols(doc)
family = Command.TryGetDefaultSpotCoordFamily(families)
location = XYZ(0,0,0)
Command.PlaceFamilyAtCoordinate(doc, family, location, false)


