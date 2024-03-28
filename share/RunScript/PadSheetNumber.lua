import ('RevitAPI', 'Autodesk.Revit.DB')

local doc = commandData.Application.ActiveUIDocument.Document
local fec = FilteredElementCollector(doc):OfCategory(BuiltInCategory.OST_Sheets):ToElements()

pad = 10

t     = Transaction(doc)
t:Start('Pad sheet numbers')
for i = 0, fec.Count-1 do
    sheet = fec[i]
    for j = 0, (pad - string.len(sheet.SheetNumber) - 1) do
        sheet.SheetNumber = '0' .. sheet.SheetNumber
        end
end
t:Commit()
t:Dispose()

return 'success'

   

   
