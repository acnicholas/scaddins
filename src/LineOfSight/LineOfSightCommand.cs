// (C) Copyright 2012-2020 by Andrew Nicholas
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

namespace SCaddins.LineOfSight
{
    using System.Dynamic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
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

            Document doc = commandData.Application.ActiveUIDocument.Document;

            dynamic settings = new ExpandoObject();
            settings.Height = 640;
            settings.Width = 360;
            settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                  new System.Uri("pack://application:,,,/SCaddins;component/Assets/scightlines.png"));
            settings.Title = "Stadium Line Of Sight - By A.Nicholas";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            var vm = new ViewModels.LineOfSightViewModel(doc);
            SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, settings);

            return Result.Succeeded;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
