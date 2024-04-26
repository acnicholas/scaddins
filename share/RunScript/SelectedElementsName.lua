import ('RevitAPI', 'Autodesk.Revit.DB')

local doc = commandData.Application.ActiveUIDocument.Document
local uidoc = commandData.Application.ActiveUIDocument
local sel = uidoc.Selection:GetElementIds()
outlist = "test"

for i = 0, sel.Count-1 do
    elem = doc:GetElement(sel[i])
    outlist = outlist .. ", " .. elem.Name
end

return outlist

   
