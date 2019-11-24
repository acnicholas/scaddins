
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

            var viewModel = new ViewModels.SpellCheckerViewModel(new SpellChecker(document));
            var result = SCaddinsApp.WindowManager.ShowDialog(viewModel,
                null,
                ViewModels.SpellCheckerViewModel.DefaultWindowSettings);

            return Result.Succeeded;
        }

     
    }
}
