import ('RevitAPI', 'Autodesk.Revit.DB')

doc = commandData.Application.ActiveUIDocument.Document
view = commandData.Application.ActiveUIDocument.ActiveView
grids = fecv:OfCategory(BuiltInCategory.OST_Grids):ToElements()
result = ''

t     = Transaction(doc)
t:Start("bubbles")
for i = 0, grids.Count - 1 do
	local grid = grids[i]
	grid:ShowBubbleInView(DatumEnds.End0, view)
	grid:ShowBubbleInView(DatumEnds.End1, view)
	result = result .. "Bubbles turned on for grid: " .. grid.Name ..'\n'
end
t:Commit()
t:Dispose()
fecv:Dispose()

return result
