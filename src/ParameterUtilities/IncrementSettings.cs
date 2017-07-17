namespace SCaddins.ParameterUtils
{
    using System;
    using Autodesk.Revit.UI;
  
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SCincrementSettingsCommand : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            using (var settingsForm = new SCincrementSettingsForm()) {
                settingsForm.ShowDialog();
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}
