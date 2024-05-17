import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.ExportManager')

uidoc = commandData.Application.ActiveUIDocument
log = ExportLog()

manager = Manager(uidoc)
manager.ExportDirectory = "C:\\Temp"
manager:AddExportOption(ExportOptions.DirectPDF)
manager:ExportSheet(manager.AllSheets[0], log);

return log.FullOutputLog



   
