import ('RevitAPI', 'Autodesk.Revit.DB')

local doc = commandData.Application.ActiveUIDocument.Document
local fec = FilteredElementCollector(doc):OfCategory(BuiltInCategory.OST_Rooms):ToElements()

pad = 5

t     = Transaction(doc)
t:Start('Pad room numbers')
for i = 0, fec.Count-1 do
        room  = fec[i]
        for j = 0, (pad - string.len(room.Number) - 1) do
            room.Number = '0' .. room.Number
        end
end
t:Commit()
t:Dispose()

return 'success'

   

   
