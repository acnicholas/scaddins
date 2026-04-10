import ('RevitAPI', 'Autodesk.Revit.DB')

local doc = commandData.Application.ActiveUIDocument.Document
local view = doc.ActiveView
local uidoc = commandData.Application.ActiveUIDocument
local sel = uidoc.Selection:GetElementIds()

collector = fec:OfCategory(BuiltInCategory.OST_KeynoteTags):ToElements()

keynoteTagSymbol = collector[0]

if not keynoteTagSymbol.IsActive then
    keynoteTagSymbol:Activate()
end

t = Transaction(doc)
t:Start('Keynote Selection')

for i = 0, sel.Count -1 do
    id = sel[i]
    element = doc:GetElement(id)
    elementRef = Reference(element)
    min = element:get_BoundingBox(view).Min
    max = element:get_BoundingBox(view).Max
    tagLocation = (min + max) / 2
    -- tagLocation = XYZ(10, 10, 0)
    --tag = IndependentTag.Create(doc, keynoteTagSymbol.Id, view.Id, elementRef,false, TagOrientation.Horizontal, tagLocation);
    tag = IndependentTag.Create(doc,view.Id,elementRef,false,TagMode.TM_ADDBY_KEYNOTE,TagOrientation.Horizontal,tagLocation)
    --tag:ChangeTypeId(keynoteTagSymbol.Id);
end

t:Commit()
t:Dispose()

return "success"

