import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.ExportManager')

--doc   = commandData.Application.ActiveUIDocument.Document
uidoc = commandData.Application.ActiveUIDocument
log = ExportLog()

manager = Manager(uidoc)
manager.ExportDirectory = "C:/Temp"
allViewSets = manager.AllViewSheetSets

for i = 0, allViewSets.Count - 1 do
    if allViewSets[i].Name == 'DA BIM Set' then
        viewIds = allViewSets[i].ViewIds
    end
end

sheets = {}
for n = 0, manager.AllSheets.Count - 1 do
    for m = 0, viewIds.Count - 1 do
        if manager.AllSheets[n].Id == viewIds[m] then
           insert(sheets, manager.AllSheets[n])
        end
    end
end


for j = 0, #(sheets) - 1 do
    manager:ExportSheet(sheets[j], log);
end


return log.FullOutputLog



   
