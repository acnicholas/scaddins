namespace SCaddins.Common
{
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Settings : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {

            var vms = new ParameterUtilities.ViewModels.SCincrementViewModel();
            var vm = new ViewModels.SettingsViewModel(vms);
            SCaddinsApp.WindowManager.ShowDialog(vm, null, ViewModels.SettingsViewModel.DefaultWindowSettings);
            return Result.Succeeded;
        }
    }
}
