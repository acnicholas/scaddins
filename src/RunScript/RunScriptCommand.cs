// (C) Copyright 2019-2020 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.RunScript
{
    using System;
    using System.IO;
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

        System.Reflection.Assembly
        CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {

            string name = args.Name.Substring(0, args.Name.IndexOf(","));
            //SCaddinsApp.WindowManager.ShowMessageBox(name);

            string filename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //SCaddinsApp.WindowManager.ShowMessageBox(args.Name);


            filename = Path.Combine(filename, name + ".dll");
            if (File.Exists(filename))
            {
                return System.Reflection.Assembly.LoadFrom(filename);
            }

            return null;
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

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

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
            var scdll = new Uri(System.Reflection.Assembly.GetAssembly(typeof(SCaddinsApp)).CodeBase).LocalPath;
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

            // ReSharper disable once StringLiteralTypo
            string[] assemblies = { @"C:\Program Files\Autodesk\Revit " + revitVersion + @"\RevitAPI.dll", @"C:\Program Files\Autodesk\Revit " + revitVersion + @"\RevitAPIUI.dll", scdll };
            return assemblies;
        }
    }
}
