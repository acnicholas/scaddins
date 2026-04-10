package.path = "C:\\Home\\Andrew\\Code\\cs\\scaddins\\share\\RunScript\\?.lua;" .. package.path

import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.ViewUtilities')

-- some funtions to draw lines and circles
require('2dGraphicsLib')

uidoc = commandData.Application.ActiveUIDocument
doc = uidoc.Document
view = doc.ActiveView

t = Transaction(uidoc.Document, 'Draw Some Cirles')
t:Start()

for x = 0, 500, 5 do
for y = 0, 500, 5 do
    drawLine(doc, view, x, y ,500-x, 500-y)
end 
end



t:Commit()
t:Dispose()

return arc

   
