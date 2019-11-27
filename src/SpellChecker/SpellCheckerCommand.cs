
namespace SCaddins.SpellChecker
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using NHunspell;
    using SCaddins;
    using SCaddins.RenameUtilities;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;

            var spellChecker = new SpellChecker(document);
            var viewModel = new ViewModels.SpellCheckerViewModel(spellChecker);
            bool? result = SCaddinsApp.WindowManager.ShowDialog(viewModel,
                null,
                ViewModels.SpellCheckerViewModel.DefaultWindowSettings);

            //if (result.HasValue && result.Value == true)
            //{
                //// SCaddinsApp.WindowManager.ShowMessageBox("About to rename");
            //    spellChecker.CommitSpellingChangesToModel();
            //}

            return Result.Succeeded;
        }

     
    }
}
