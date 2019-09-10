namespace SCaddins.RunScript
{
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using CSScriptLibrary;

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RunScriptCommand : IExternalCommand
    {
        public static void RunScript(Document doc, string script)
        {
            string scdll = new System.Uri(System.Reflection.Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;
            ////SCaddinsApp.WindowManager.ShowMessageBox(scdll);

            #if REVIT2018
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit 2018\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit 2018\RevitAPIUI.dll, scdll };
            #elif REVIT2019
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit 2019\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit 2019\RevitAPIUI.dll", scdll};
            #elif REVIT2020
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit 2020\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit 2020\RevitAPIUI.dll", scdll };
            #else
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit 2016\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit 2016\RevitAPIUI.dll", scdll };
            #endif
            var runScript = CSScript.LoadMethod(script, assemblies).GetStaticMethod("*.Main", doc);
            runScript(doc);
        }

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

            ////if (result.HasValue && result.Value == true) {
            RunScript(commandData.Application.ActiveUIDocument.Document, vm.Script);
            ////}

            return Result.Succeeded;
        }
    }
}
