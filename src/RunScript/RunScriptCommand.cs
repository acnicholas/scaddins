using System;
using System.Collections.Generic;
using System.Linq;


namespace SCaddins.RunScript
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.Attributes;
    using CSScriptLibrary;

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RunScriptCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (commandData == null)
            {
                return Result.Failed;
            }

            var vm = new ViewModels.RunScriptViewModel();
            SCaddinsApp.WindowManager.ShowDialog(vm, null, ViewModels.RunScriptViewModel.DefaultViewSettings);

            RunScript(commandData.Application.ActiveUIDocument.Document, vm.Script);

            return Result.Succeeded;
        }

        public static void RunScript(Document doc, string script)
        {
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit 2020\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit 2020\RevitAPIUI.dll" };
            var RunScript = CSScript.LoadMethod(script, assemblies).GetStaticMethod("*.Main", doc, script);
            RunScript(doc, script);
        }
    }
}
