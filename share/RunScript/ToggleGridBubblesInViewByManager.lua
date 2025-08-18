import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.GridManager')

-- local show = true
local doc = commandData.Application.ActiveUIDocument.Document
local uidoc = commandData.Application.ActiveUIDocument
local list = uidoc.Selection:GetElementIds()
local count = 0

count = count + GridManager.ShowTopGridBubblesByView(doc.ActiveView, true, list)
count = count + GridManager.ShowBottomGridBubblesByView(doc.ActiveView, false, list)
count = count + GridManager.ShowLeftGridBubblesByView(doc.ActiveView, true, list)
count = count + GridManager.ShowRightGridBubblesByView(doc.ActiveView, false, list)

return count