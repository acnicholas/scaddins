import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.ExportManager')

local doc   = commandData.Application.ActiveUIDocument.Document
local uidoc = commandData.Application.ActiveUIDocument
local log = ExportLog()

manager = Manager(uidoc)
--manager:AddExportOption(ExportOptions.DirectPDF)
manager:ExportSheet(manager.AllSheets[1], log);

return log.FullOutputLog



   
