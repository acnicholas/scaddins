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
    public class ProjectInformationReplacement
    {
        public ProjectInformationReplacement(string parameterName, string replacement)
        {
            ParamaterName = parameterName;
            ReplacementValue = replacement;
        }

        public ProjectInformationReplacement() : this(string.Empty, string.Empty)
        {
        }

        public string ParamaterName
        {
            get; set;
        }

        public string ReplacementFormat
        {
            get; set;
        }

        public string ReplacementValue
        {
            get; set;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2}", ParamaterName, ReplacementValue, ReplacementFormat);
        }
    }
}
