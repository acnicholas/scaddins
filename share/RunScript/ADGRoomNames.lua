import ('RevitAPI', 'Autodesk.Revit.DB')

local doc = commandData.Application.ActiveUIDocument.Document
local fec = FilteredElementCollector(doc):OfCategory(BuiltInCategory.OST_Rooms):ToElements()

out = {}

t     = Transaction(doc)
t:Start('ADG rooms names')
for i = 0, fec.Count-1 do
        room = fec[i]
        area = room.Area * 0.092903              -- convert sq feet to sqm
        if area < 50 then
              room.Name = "Room"
        elseif area < 75 then
              room.Name = "1B"
        elseif area < 95 then
              room.Name = "2B"
        elseif area >= 95 then
              room.Name = "3B"
        end
end
t:Commit()
t:Dispose()

return 'success'

   
