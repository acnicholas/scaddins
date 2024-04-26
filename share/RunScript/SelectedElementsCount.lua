import ('RevitAPI', 'Autodesk.Revit.DB')

local doc = commandData.Application.ActiveUIDocument.Document
local uidoc = commandData.Application.ActiveUIDocument
local sel = uidoc.Selection:GetElementIds()

return sel.Count