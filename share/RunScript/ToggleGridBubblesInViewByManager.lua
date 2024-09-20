import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.GridManager')

-- local show = true
local show = false
local doc = commandData.Application.ActiveUIDocument.Document
local uidoc = commandData.Application.ActiveUIDocument
local list = uidoc.Selection:GetElementIds()
local count = 0

count = count + GridManager.ShowTopGridBubblesByView(doc.ActiveView, show, list)
count = count + GridManager.ShowBottomGridBubblesByView(doc.ActiveView, show, list)

return count