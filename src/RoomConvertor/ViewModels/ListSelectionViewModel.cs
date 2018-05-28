using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SCaddins.RoomConvertor.ViewModels
{
    class ListSelectionViewModel : Screen
    {
        public ListSelectionViewModel(List<string> items)
        {
            Items = items;
        }

        public List<string> Items
        {
            get; set;
        }

        public string SelectedItem
        {
            get; set;
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public void OK()
        {
            TryClose(true);
        }
    }
}
