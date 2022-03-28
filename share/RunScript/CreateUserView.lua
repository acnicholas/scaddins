import ('RevitAPI', 'Autodesk.Revit.DB')
import ('C:\\Program Files\\SCaddins\\SCaddins\\2022\\SCaddins.dll', 'SCaddins.ViewUtilities')

uidoc = commandData.Application.ActiveUIDocument
activeView = uidoc.Document.ActiveView
t = Transaction(uidoc.Document, 'Create User View')
t:Start()
newViews = UserView.Create(activeView, uidoc)
t:Commit()
t:Dispose()

return newViews


   