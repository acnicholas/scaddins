-- variables available:
--	commandData
--	fec  (empty filtered element collector)
--	fecv (empty filtered element collector for avtive view)

import ('RevitAPI', 'Autodesk.Revit.DB')
import ('RevitAPIUI', 'Autodesk.Revit.UI')

-- get all walls in current view
walls = fecv:OfCategory(BuiltInCategory.OST_Walls):ToElements()

--list all walls
result = ""
for i = 0, walls.Count -1 do
	result = result .. " - " .. walls[i].Name
end
return result
