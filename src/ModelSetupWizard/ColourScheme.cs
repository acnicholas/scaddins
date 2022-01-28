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

namespace SCaddins.ModelSetupWizard
{
    using System.Collections.Generic;

    public class ColourScheme
    {
        public ColourScheme(string config)
        {
            Config = config;
            var s = config.Split(';');
            Name = s[0];
            Colors = new List<System.Windows.Media.Color>(16);
            for (int i = 1; i < 17; i++)
            {
                Colors.Insert(i - 1, IniIO.ConvertStringToColor(s[i]));
            }
        }

        public string Config
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public List<System.Windows.Media.Color> Colors
        {
            get; private set;
        }
    }
}
