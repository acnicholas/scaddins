-- ListAllWorksets.lua

import ('RevitAPI', 'Autodesk.Revit.DB')
import ('RevitAPIUI', 'Autodesk.Revit.UI')

-- get all worksets links in current model
doc = commandData.Application.ActiveUIDocument.Document
ws = FilteredWorksetCollector(doc):OfKind(WorksetKind.UserWorkset):ToWorksets()

-- open a file to write to
file = io.open("c:/Temp/worksets.txt", "a")
io.output(file)

--list all worksets
result = ""
for i = 0, ws.Count -1 do
    io.write(ws[i].Name .. "\n")
	result = result .. ws[i].Name .. "\n"
end

io.close(file)

return result
