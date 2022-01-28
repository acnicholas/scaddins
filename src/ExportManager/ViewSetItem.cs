// (C) Copyright 2018-2020 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System.Collections.Generic;

    public class ViewSetItem
    {
        public ViewSetItem(int id, string name, List<int> viewsIds)
        {
            Id = id;
            Name = name;
            DescriptiveName = name;
            ViewIds = viewsIds;
            NumberOfViews = viewsIds.Count;
            CreationDate = new System.DateTime();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string DescriptiveName { get; set; }

        public int NumberOfViews { get; set; }

        public System.DateTime CreationDate { get; set; }

        public List<int> ViewIds { get; }
    }
}
