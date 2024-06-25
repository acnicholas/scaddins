import ('RevitAPI', 'Autodesk.Revit.DB')

doc = commandData.Application.ActiveUIDocument.Document
startLevel = 10000		     -- start levels in mm
currentLevel = 10000/304.8   --converft to feet for revit API
out = ""
levels = {
	{'Ground', 0},
	{'Level 01', 4500},
	{'Level 02', 3100},
	{'Level 03', 3100},
	{'Level 04', 3100},
	{'Level 05', 3100},
	{'Level 06', 3100},
	{'Level 07', 3100},
	{'Level 08', 3100},
	{'Level 09', 3100},
	{'Level 10', 3100},
	{'Level 11', 3100},
	{'Level 12', 3100},
	{'Level 13', 3100},
	{'Level 14', 3100},
	{'Level 15', 3100}
}

t = Transaction(doc, 'Create Levels')
t:Start()
for i = 1, #(levels), 1 do
        currentLevel = currentLevel + levels[i][2]/304.8
        level = Level.Create(doc, currentLevel)
        level.Name = levels[i][1]
        out = out .. level.Name .. ' created at RL' .. level.Elevation * 304.8 .. '\n'
end
t:Commit()
t:Dispose()

return out

   

   
