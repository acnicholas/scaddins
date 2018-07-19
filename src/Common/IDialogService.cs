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
        bool? ShowConfirmationDialog(string message, bool? defaultCheckboxValue, out bool checkboxResult);
        bool? ShowSaveAsDialog(string defaultFileName, string defaultExtension, string filter, out string savePath);
        bool? ShowDirectorySelectionDialog(string defaultDir, out string dirPath);
    }
}
