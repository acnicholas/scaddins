using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.Common
{
    class BasicDialogService : IDialogService
    {
        public bool? ShowMessageBox(string message)
        {
            System.Windows.Forms.MessageBox.Show(message);
            return true;
        }

        public bool? ShowSaveAsDialog()
        {
            return true;
        }
    }
}
