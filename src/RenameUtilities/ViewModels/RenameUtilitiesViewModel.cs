using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace SCaddins.RenameUtilities.ViewModels
{
    class RenameUtilitiesViewModel : Screen
    {
        private RenameManager manager;

        public RenameUtilitiesViewModel(RenameManager manager)
        {
            this.manager = manager;
        }
    }
}
