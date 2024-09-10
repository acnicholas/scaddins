// (C) Copyright 2013-2023 by Andrew Nicholas
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

namespace SCaddins.SolarAnalysis
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using System.Threading.Tasks;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (commandData == null)
            {
                return Result.Failed;
            }

            UIDocument udoc = commandData.Application.ActiveUIDocument;

            if (udoc.Document.IsFamilyDocument)
            {
                SCaddinsApp.WindowManager.ShowErrorMessageBox("Families not supported", "Solar analysis tools will not work in a family environment.");
                return Result.Failed;
            }

            var vm = new ViewModels.SolarViewsViewModel(commandData.Application.ActiveUIDocument);
            Task<bool?> result = SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, ViewModels.SolarViewsViewModel.DefaultViewSettings);
            var r = result.Result.HasValue ? result.Result.Value : false;


            if (r && vm.CreateAnalysisView)
            {
#if REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
                var internalUnitsGridSize = UnitUtils.ConvertToInternalUnits(vm.AnalysisGridSize, UnitTypeId.Millimeters);
#else
                var internalUnitsGridSize = UnitUtils.ConvertToInternalUnits(vm.AnalysisGridSize, DisplayUnitType.DUT_MILLIMETERS);
#endif
                SolarAnalysisManager.CreateTestFaces(vm.FaceSelection, vm.MassSelection, internalUnitsGridSize, udoc, udoc.ActiveView);
            }
            if (r && vm.DrawSolarRay)
            {
                SolarAnalysisManager.DrawSolarRayAsModelLine(udoc, vm.SolarRayLength);
            }

            return Result.Succeeded;
        }
    }
}
