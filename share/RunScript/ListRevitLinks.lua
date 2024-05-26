-- ListRevitLinks.lua
--
-- This will export a file with the names of all revit links in the current model

import ('RevitAPI', 'Autodesk.Revit.DB')
import ('RevitAPIUI', 'Autodesk.Revit.UI')

-- get all revit links in current model
links = fec:OfCategory(BuiltInCategory.OST_RvtLinks):ToElements()

-- open a file to write to
file = io.open("c:/Temp/out.txt", "a")
io.output(file)

--list all revit links
result = ""
for i = 0, links.Count -1 do
    io.write(links[i].Name .. "\n")
	result = result .. " - " .. links[i].Name .. "\n"
end

io.close(file)

return result
