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
            bool? result = SCaddinsApp.WindowManager.ShowDialog(vm, null, ViewModels.RunScriptViewModel.DefaultViewSettings);

            //if (result.HasValue && result.Value == true) {
            RunScript(commandData.Application.ActiveUIDocument.Document, vm.Script);
            //}

            return Result.Succeeded;
        }

        public static void RunScript(Document doc, string script)
        {
            #if REVIT2018
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit 2018\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit 2018\RevitAPIUI.dll" };
            #elif REVIT2019
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit 2019\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit 2019\RevitAPIUI.dll" };
            #elif REVIT2020
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit 2019\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit 2019\RevitAPIUI.dll" };
            #else
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit 2016\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit 2016\RevitAPIUI.dll" };
            #endif
            var RunScript = CSScript.LoadMethod(script, assemblies).GetStaticMethod("*.Main", doc);
            RunScript(doc);
        }
    }
}
