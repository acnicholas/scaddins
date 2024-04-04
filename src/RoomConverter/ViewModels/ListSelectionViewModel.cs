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

namespace SCaddins.RoomConverter.ViewModels
{
    using System.Collections.Generic;
    using Caliburn.Micro;

    public class ListSelectionViewModel : Screen
    {
        public ListSelectionViewModel(List<string> items)
        {
            Items = items;
        }

        public List<string> Items
        {
            get;
        }

        public string SelectedItem
        {
            get; set;
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void OK()
        {
            TryCloseAsync(true);
        }
    }
}
