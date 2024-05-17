import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.ExportManager')
--import ('System.Collections.ObjectModel','System.Collections.ObjectModel')

uidoc = commandData.Application.ActiveUIDocument
log = ExportLog()
manager = Manager(uidoc)

-- ---------------------------------------------
-- ---------------------------------------------
--EDIT BELOW TO CHANGE EXPORT DIRECTORY AND VIEWS
manager.ExportDirectory = "C:\\Temp"
viewSetName = "an"
-- ---------------------------------------------
-- ---------------------------------------------

allViewSets = manager.AllViewSheetSets
allSheets = manager.AllSheets
viewIds = {}

for i = 0, allViewSets.Count - 1 do
    if allViewSets[i].Name == viewSetName then
        viewSetIds = allViewSets[i].ViewIds 
        for n = 0, allSheets.Count - 1 do
    		for m = 0, viewSetIds.Count - 1 do
        		if allSheets[n].Id.IntegerValue == viewSetIds[m] then
           			table.insert(viewIds, n)
           		end
        	end
    	end
    end
end

log:Clear()
log:Start("Started.")
       
for i=0, #(viewIds) -1 do
    manager:ExportSheet(allSheets[i], log);
end

log:Stop("Finished")

return log.FullOutputLog
