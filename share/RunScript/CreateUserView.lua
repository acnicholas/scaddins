import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.ViewUtilities')

uidoc = commandData.Application.ActiveUIDocument
activeView = uidoc.Document.ActiveView
t = Transaction(uidoc.Document, 'Create User View')
t:Start()
newViews = UserView.Create(activeView, uidoc)
t:Commit()
t:Dispose()

return newViews


   
