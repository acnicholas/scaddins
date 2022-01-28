// (C) Copyright 2016-2020 by Andrew Nicholas
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

namespace SCaddins.RoomConverter
{
    using Autodesk.Revit.DB.Architecture;

    public class RoomFilter
    {
        private RoomFilterItem[] filters;

        public RoomFilter()
        {
            filters = new RoomFilterItem[3];
        }

        public static int Size
        {
            get { return 3; }
        }

        public void AddFilterItem(RoomFilterItem item, int index)
        {
            filters[index] = item;
        }

        public void Clear()
        {
            filters[0] = null;
            filters[1] = null;
            filters[2] = null;
        }

        public RoomFilterItem GetFiterItem(int index)
        {
            return filters[index];
        }

        public bool PassesFilter(Room room)
        {
            foreach (RoomFilterItem item in filters)
            {
                if (item != null && !item.PassesFilter(room))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
