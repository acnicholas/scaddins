// (C) Copyright 2024 by Andrew Nicholas
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

namespace SCaddins.HatchEditor.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Caliburn.Micro;

    public class SetTemplateParametersViewModel : Screen
    {
        private BindableCollection<TemplateParameter> templateParameters;

        public SetTemplateParametersViewModel(BindableCollection<TemplateParameter> templateParameters)
        {
            TemplateParameters = templateParameters;
        }

        public static System.Dynamic.ExpandoObject DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 160;
                settings.Width = 320;
                settings.Title = "Assign Parameter Vaiues";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                return settings;
            }
        }

        public BindableCollection<TemplateParameter> TemplateParameters
        {
            get => templateParameters;

            set
            {
                templateParameters = value;
                NotifyOfPropertyChange(() => TemplateParameters);
            }
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
