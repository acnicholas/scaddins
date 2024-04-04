// (C) Copyright 2018 by Andrew Nicholas
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

namespace SCaddins.ExportManager.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using Caliburn.Micro;

    internal class ToggleSelectedSheetParametersViewModel : Screen
    {
        public ToggleSelectedSheetParametersViewModel(Autodesk.Revit.DB.Document doc, List<YesNoParameter> yesNoparameters)
        {
            YesNoParameters = new ObservableCollection<YesNoParameter>(yesNoparameters);
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 480;
                settings.Title = "Assign Yes/No Value to Selected Parameters";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public ObservableCollection<YesNoParameter> YesNoParameters
        {
            get; set;
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void OK()
        {
            TryCloseAsync(true);
        }
    }
}