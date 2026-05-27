// (C) Copyright 2019-2020 by Andrew Nicholas (andrewnicholas@iinet.net.au)
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

namespace SCaddins.SyncViews
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Events;
    using SCaddins;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            AttachIdleEventHandler(commandData);
            return Result.Succeeded;
        }

        public void AttachIdleEventHandler(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;
            uiApp.Idling += OnIdling;
        }

        public void RemoveIdleEventHandler(UIApplication uiApp)
        {
            uiApp.Idling -= OnIdling;
        }

        public void OnIdling(object sender, IdlingEventArgs e)
        {
            UIApplication uiApp = sender as UIApplication;
            Document doc = uiApp.ActiveUIDocument.Document;
            {
                Autodesk.Revit.UI.TaskDialog.Show("Idle action", "Idle action");
            }
        }


    }
}
