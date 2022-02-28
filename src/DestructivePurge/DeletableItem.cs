// (C) Copyright 2013-2020 by Andrew Nicholas andrewnicholas@iinet.net.au
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
    using System.Drawing;
    using Autodesk.Revit.DB;

    public class DeletableItem
    {
        public DeletableItem(string name)
        {
            Info = "-";
            Id = null;
            Name = name;
            PreviewImage = null;
            HasParent = false;
            ParentId = ElementId.InvalidElementId;
        }

        public ElementId Id
        {
            get; set;
        }

        public bool HasParent
        {
            get; set;
        }

        public ElementId ParentId
        {
            get; set;
        }

        public string Info
        {
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
