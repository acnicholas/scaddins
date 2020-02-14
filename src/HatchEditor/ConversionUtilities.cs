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

namespace SCaddins.HatchEditor
{
    public static class ConversionUtilities
    {
        public static double ToDeg(this double arg)
        {
            return arg * 180 / System.Math.PI;
        }

        public static double ToRad(this double arg)
        {
            return arg * System.Math.PI / 180;
        }

        public static double ToMM(this double arg)
        {
            return arg * 304.8;
        }

        public static double ToMM(this double arg, double scale)
        {
            return arg * 304.8 * scale;
        }

        public static double ToFeet(this double arg)
        {
            return arg / 304.8;
        }
    }
}
