-- 'commandData' - Autodesk.Revit.UI.ExternalCommandData as passed to the host addin
-- 'fec' - FilteredElementCOllector on active doc
-- 'fecv' - FilteredElementCOllector on active view

import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.ExportManager')

uidoc = commandData.Application.ActiveUIDocument
currentSheet = commandData.Application.ActiveUIDocument.Document.ActiveView
Manager.OpenNextSheet(uidoc, currentSheet)

return "Done"