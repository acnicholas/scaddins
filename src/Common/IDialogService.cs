using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.Common
{
    public interface IDialogService
    {
        bool? ShowMessageBox(string message);
        bool? ShowSaveAsDialog();
    }
}
