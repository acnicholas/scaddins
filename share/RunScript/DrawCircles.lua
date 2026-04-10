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

for a = 0, 360, 4 do
for i = 0, 50, 1 do
    x = i * 50 * math.sin(a * math.pi / 180)
    y = i * 50 * math.cos(a * math.pi / 180)
    r = (i + 10)/2
    drawCircle(doc, view, x, y , r)
end 
end

for b = 2, 360, 4 do
for j = 0, 50, 1 do
    x = j * 50 * math.sin(b * math.pi / 180)
    x = x + 25 * math.sin(b * math.pi / 180)
    y = j * 50 * math.cos(b * math.pi / 180)
    y = y + 25 * math.cos(b * math.pi / 180) 
    r = (j + 10)/2
    drawCircle(doc, view, x, y , r)
end 
end


t:Commit()
t:Dispose()

return arc

   
