// (C) Copyright 2014-2020 by Andrew Nicholas
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

namespace SCaddins.WorksharingUtilities
{
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class WorksharingUtilitiesCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null)
            {
                return Result.Failed;
            }

            Document doc = commandData.Application.ActiveUIDocument.Document;
            var selection = commandData.Application.ActiveUIDocument.Selection;

            if (selection.GetElementIds().Count == 0)
            {
                SCaddinsApp.WindowManager.ShowErrorMessageBox(
                    "No Element Selected",
                    "No Element Selected. Please select an element before running.");
                return Result.Failed;
            }

            if (selection.GetElementIds().Count > 1)
            {
                SCaddinsApp.WindowManager.ShowWarningMessageBox(
                    "Multiple Elements Selected",
                    "Multiple Elements Selected. Only information for the first element will be displayed.");
                //// return Result.Failed;
            }

            var elem = doc.GetElement(selection.GetElementIds().First());
            ListInfo(elem, doc);

            return Result.Succeeded;
        }

        private void ListInfo(Element elem, Document doc)
        {
            var message = string.Empty;
            message += "Element Id: " + elem.Id;

            // The workset the element belongs to
            WorksetId worksetId = elem.WorksetId;
            message += "\nWorkset Id : " + worksetId.ToString();

            // Model Updates Status of the element
            ModelUpdatesStatus updateStatus = WorksharingUtils.GetModelUpdatesStatus(doc, elem.Id);
            message += "\nUpdate status : " + updateStatus.ToString();

            // Checkout Status of the element
            CheckoutStatus checkoutStatus = WorksharingUtils.GetCheckoutStatus(doc, elem.Id);
            message += "\nCheckout status : " + checkoutStatus.ToString();

            // Getting WorksharingTooltipInfo of a given element Id
            WorksharingTooltipInfo tooltipInfo = WorksharingUtils.GetWorksharingTooltipInfo(doc, elem.Id);
            message += "\nCreator : " + tooltipInfo.Creator;
            message += "\nCurrent Owner : " + tooltipInfo.Owner;
            message += "\nLast Changed by : " + tooltipInfo.LastChangedBy;

            SCaddinsApp.WindowManager.ShowMessageBox("Additional Element Information", message);
        }
    }
}
