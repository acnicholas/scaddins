// (C) Copyright 2019-2024 by Andrew Nicholas
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

using System.Threading.Tasks;

namespace SCaddins.RunScript.ViewModels
{
    using System;

    using System.Dynamic;

    using Caliburn.Micro;


    internal class OutputWindowViewModel : Screen
    {
        private string output;

        public OutputWindowViewModel()
        {
            output = string.Empty;
        }

        public static dynamic DefaultViewSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 840;
                settings.Title = "Output";
                settings.ShowInTaskbar = false;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/SCaddins;component/Assets/lua.png"));
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight; 
                return settings;
            }
        }

        public string Output
        {
            get => output;

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                output = value;
                NotifyOfPropertyChange(() => Output);
            }
        }

        public override async Task TryCloseAsync(bool? dialogResult = false)
        {
            await base.TryCloseAsync(dialogResult);
        }
    }
}
