import ('RevitAPI', 'Autodesk.Revit.DB')

local doc = commandData.Application.ActiveUIDocument.Document
local activeViewID = commandData.Application.ActiveUIDocument.ActiveView.Id
local fec = FilteredElementCollector(doc, activeViewID):OfCategory(BuiltInCategory.OST_Levels) --:ToElements()


return activeViewID

   
