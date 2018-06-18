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

        //public bool? ShowConfirmationDialog()
        //{

        //}

        public bool? ShowSaveAsDialog()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".text"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
            //// Show save file dialog box
            return dlg.ShowDialog();
        }
    }
}
