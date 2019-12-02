namespace SCaddins.SpellChecker
{
    using System.IO;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using SCaddins;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var version = string.Empty;
            #if REVIT2016
            version = "Revit 2016";
            #elif REVIT2017
            version = "Revit 2017";
            #elif REVIT2018
            version = "Revit 2018";
            #elif REVIT2019
            version = "Revit 2019";
            #elif REVIT2020
            version = "Revit 2020";
            #endif

            if (!File.Exists(Path.Combine(@"C:\Program Files\Autodesk", version, "Hunspellx64.dll"))) {
                var bin = Path.Combine(Constants.InstallDirectory, "bin", "NHunspellInstaller.exe");
                Common.ConsoleUtilities.StartHiddenConsoleProg(bin, null);
            }

            var document = commandData.Application.ActiveUIDocument.Document;

            var spellChecker = new SpellChecker(document);
            var viewModel = new ViewModels.SpellCheckerViewModel(spellChecker);
            bool? result = SCaddinsApp.WindowManager.ShowDialog(viewModel, null, ViewModels.SpellCheckerViewModel.DefaultWindowSettings);

            return Result.Succeeded;
        }    
    }
}
