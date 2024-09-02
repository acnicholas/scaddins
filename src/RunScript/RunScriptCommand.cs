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
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using NLua;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class RunScriptCommand : IExternalCommand
    {
        public static object[] RunScript(string script, ExternalCommandData commandData, ElementSet elements, bool createTransaction)
        {
            if (createTransaction)
            {
                return null;
            }
            else
            {
                Lua state = new Lua();
                state.LoadCLRPackage();
                state["commandData"] = commandData;
                state["fecv"] = new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document, commandData.Application.ActiveUIDocument.ActiveView.Id);
                state["fec"] = new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document);
                try
                {
                    var r = state.DoString(script);
                    state.Dispose();
                    return r;
                }
                catch (NLua.Exceptions.LuaScriptException lse)
                {
                    object[] obj = new object[2];
                    obj[0] = lse.Message;
                    obj[1] = lse.Source;
                    state.Dispose();
                    return obj;
                }
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

            var vm = new ViewModels.RunScriptViewModel(commandData, elements);
            SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, ViewModels.RunScriptViewModel.DefaultViewSettings);
            return Result.Succeeded;
        }
    }
}
