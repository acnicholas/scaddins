using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.Common
{
    class MockDialogService : IDialogService
    {
        public bool? ShowMessageBox(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            return true;
        }

        public bool? ShowConfirmationDialog()
        {
            return true;
        }

        public bool? ShowSaveAsDialog()
        {
            return true;
        }
    }
}
