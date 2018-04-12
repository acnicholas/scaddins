// (C) Copyright 2013-2014 by Andrew Nicholas andrewnicholas@iinet.net.au
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

namespace SCaddins.DestructivePurge
{
    using Autodesk.Revit.DB;
    using System.Drawing;

    public class DeletableItem
    {
        public DeletableItem(string name)
        {
            this.Info = "-";
            this.Id = null;
            this.Name = name;
            this.PreviewImage = null;
        }

        public string Info {
            get; set;
        }

        public ElementId Id {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public Bitmap PreviewImage
        {
            get; set;
        }

    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
