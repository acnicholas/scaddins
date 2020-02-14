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
    public class StadiumSeatingTread
    {
        public double CValue
        {
            get; set;
        }

        public double EyeHeight
        {
            get; set;
        }

        public double EyeToFocusX
        {
            get; set;
        }

        public double Going
        {
            get; set;
        }

        public double HeightToFocus
        {
            get; set;
        }

        public double RiserHeight
        {
            get; set;
        }

        public void Initialize(
            double eyeToFocusX,
            double riserHeight,
            double heightToFocus,
            double going,
            double eyeHeight)
        {
            EyeToFocusX = eyeToFocusX;
            RiserHeight = riserHeight;
            HeightToFocus = heightToFocus;
            Going = going;
            EyeHeight = eyeHeight;
        }
    }
}