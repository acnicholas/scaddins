using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCaddins.SheetCopier;
using Autodesk.Revit.UI;
using Caliburn.Micro;

namespace SCaddins.SheetCopier.ViewModels
{
    class SheetCopierViewModel : PropertyChangedBase
    {

        private SheetCopierManager copyManager;
        private ObservableCollection<SheetCopierSheet> sheets;

        public ObservableCollection<SheetCopierSheet> Sheets
        {
            get { return this.sheets; }
            set
            {
                if (sheets != value) {
                    this.sheets = value;
                }
            }
        }

        public SheetCopierViewModel(UIDocument uidoc)
        {
            copyManager = new SheetCopierManager(uidoc);
        }
    }
}
