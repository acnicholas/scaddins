// (C) Copyright 2016 by Andrew Nicholas
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

namespace SCaddins.SCasfar
{
    using System.Collections.ObjectModel;
    using Autodesk.Revit.DB.Architecture;

    public class RoomFilter
    {
        private Collection<RoomFilterItem> filters;

        public RoomFilter()
        {
            this.filters = new Collection<RoomFilterItem>();
            this.filters.Clear();
        }

        public void AddFilterItem(RoomFilterItem item)
        {
            this.filters.Add(item);
        }

        public void Clear()
        {
            this.filters.Clear();
        }

        public bool PassesFilter(Room room)
        {
            foreach(RoomFilterItem item in filters) {
                if (item.IsValid()) {
                    if(!item.PassesFilter(room)){
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
