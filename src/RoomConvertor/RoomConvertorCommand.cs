// (C) Copyright 2016-2018 by Andrew Nicholas
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

namespace SCaddins.RoomConvertor
{
    using System;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RoomConvertorCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(nameof(commandData));
            }

            Document doc = commandData.Application.ActiveUIDocument.Document;

            if (doc == null)
            {
                return Result.Failed;
            }

            var roomConversionManager = new RoomConversionManager(doc);

            if (roomConversionManager.Candidates.Count == 0)
            {
                using (TaskDialog td = new TaskDialog("Room Tools - Slight Problem..."))
                {
                    td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                    td.MainInstruction = "No Rooms Found in Model";
                    td.MainContent = "Room Tools will now exit as there's not much use continuing.";
                    td.Show();
                }
                return Result.Failed;
            }

            var vm = new ViewModels.RoomConvertorViewModel(roomConversionManager);
            SCaddinsApp.WindowManager.ShowDialog(vm, null, ViewModels.RoomConvertorViewModel.DefaultWindowSettings);

            return Result.Succeeded;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */