import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.SheetCopier')

uidoc = commandData.Application.ActiveUIDocument
manager = SheetCopierManager(uidoc)
manager:AddCurrentView()
manager:CreateSheets()
return "done"


   
