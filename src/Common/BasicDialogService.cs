using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.Common
{
    class BasicDialogService : IDialogService
    {
        public BasicDialogService()
        {

        }

        public bool? ShowMessageBox(string message)
        {
            System.Windows.MessageBox.Show(message);
            return true;
        }

        public bool? ShowConfirmationDialog()
        {
            var confirmOverwriteDialog = new ViewModels.ConfirmationDialogViewModel();
            confirmOverwriteDialog.Message = fileName + " exists," + Environment.NewLine +
                    "do you want do overwrite the existing file?";
            confirmOverwriteDialog.Value = true;
            bool? result = SCaddinsApp.WindowManager.ShowDialog(confirmOverwriteDialog, null, ViewModels.ConfirmationDialogViewModel.DefaultWindowSettings);
            bool newBool = result.HasValue ? result.Value : false;
            if (newBool) {
                ExportManager.ConfirmOverwrite = confirmOverwriteDialog.ValueAsBool;
                return confirmOverwriteDialog.ValueAsBool;
            }
            return false;
        }

        public bool? ShowSaveAsDialog(string defaultFileName, string defaultExtension, string filter)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = defaultFileName; // Default file name
            dlg.DefaultExt = defaultExtension; // Default file extension
            //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
            dlg.Filter = filter; // Filter files by extension
            //// Show save file dialog box
            return dlg.ShowDialog();
        }
    }
}
