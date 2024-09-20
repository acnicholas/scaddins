import ('RevitAPI', 'Autodesk.Revit.DB')
import ('RevitAPIUI', 'Autodesk.Revit.UI')

-- get all warnings in current model
doc = commandData.Application.ActiveUIDocument.Document
warnings = doc:GetWarnings()

--list all warnings and ID's
result = ""
for i = 0, warnings.Count -1 do
	result = result .. warnings[i]:GetDescriptionText() .. " IDs:"
    ids = warnings[i]:GetFailingElements()
    for j = 0, ids.Count -1 do
        result = result .. ids[j]:ToString() ..";"
    end
    result = result .. "\n"
end

return result
