import ('RevitAPI', 'Autodesk.Revit.DB')
import ('SCaddins', 'SCaddins.SolarAnalysis')
import ('SCaddins', 'SCaddins.ModelSetupWizard')

uidoc = commandData.Application.ActiveUIDocument
manager = SolarAnalysisManager(uidoc)
manager.RotateCurrentView = false
manager.Create3dViews = true
log = TransactionLog('log')

-- The Go method in Solaranalysis creates its own transcaction.
manager:Go(log)

return log:Summary()

   
