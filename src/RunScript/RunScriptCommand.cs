namespace SCaddins.RunScript
{
    using System;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using CSScriptLibrary;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class RunScriptCommand : IExternalCommand
    {
        public static String ClassifyScript(string s)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(s);
            sb.Insert(s.IndexOf(@"public static", StringComparison.InvariantCulture), @"public class A{");
            sb.Append(@"}");
            return sb.ToString();
        }

        public static void RunScript(Document doc, string script)
        {     
            var runScript = CSScript.LoadMethod(script, GetAssemblies()).GetStaticMethod("*.Main", doc);
            runScript(doc);
        }

        public static bool VerifyScript(string script, out string compileResult)
        {
            try
            {
                compileResult = CSScript.CompileCode(script, GetAssemblies());
                return true;
            }
            catch (Exception e)
            {
                compileResult = e.Message;
                return false;
            }
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

            if (result.HasValue && result.Value)
            {
                RunScript(commandData.Application.ActiveUIDocument.Document, vm.Script);
            }

            return Result.Succeeded;
        }

        private static string[] GetAssemblies()
        {
            string scdll = new Uri(System.Reflection.Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;
            var revitVersion = "2016";
            #if REVIT2017
                        revitVersion = "2017";
            #elif REVIT2018
                        revitVersion = "2018";
            #elif REVIT2019
                        revitVersion = "2019";
            #elif REVIT2020
                        revitVersion = "2020";
            #endif

            string[] assemblies = { @"C:\Program Files\Autodesk\Revit " + revitVersion + @"\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit " + revitVersion + @"\RevitAPIUI.dll", scdll };
            return assemblies;
        }
    }
}
