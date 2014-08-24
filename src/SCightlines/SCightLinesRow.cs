// (C) Copyright 2012-2013 by Andrew Nicholas
//
// This file is part of SCightlines.
//
// SCightlines is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCightlines is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCightlines.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCightLines
{ 
    /// <summary>
    /// Class to hold row(tread) values
    /// </summary>
    /// <author>
    /// Andrew Nicholas
    /// </author>
    public class SCightLinesRow
    {
        public SCightLinesRow()
        {
        }
        
        /// <summary> the c value</summary> 
        public double CValue
        {
            get;
            set;
        }

        /// <summary> horizontal distane from eye to focus</summary> 
        public double EyeToFocusX
        {
            get;
            set;
        }

        /// <summary> the riser height</summary> 
        public double RiserHeight
        {
            get;
            set;
        }

        /// <summary> vertical height to the point of focus</summary> 
        public double HeightToFocus
        {
            get;
            set;
        }

        /// <summary> the seating row depth</summary> 
        public double Going
        {
            get;
            set;
        }

        /// <summary> the eye height</summary> 
        public double EyeHeight
        {
            get;
            set;
        }

        public void Initialize(
            double eyeToFocusX,
            double riserHeight,
            double heightToFocus,
            double going,
            double eyeHeight)
        {
            this.EyeToFocusX = eyeToFocusX;
            this.RiserHeight = riserHeight;
            this.HeightToFocus = heightToFocus;
            this.Going = going;
            this.EyeHeight = eyeHeight;
        }
    }
}
