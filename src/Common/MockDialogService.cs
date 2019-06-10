namespace SCaddins.Common
{
    public class MockDialogService : IDialogService
    {
        public MockDialogService()
        {
        }

        public bool? ShowConfirmationDialog(string message, bool? defaultCheckboxValue, out bool checkboxResult)
        {
            checkboxResult = true;
            return true;
        }

        public bool? ShowDirectorySelectionDialog(string defaultDir, out string dirPath)
        {
            dirPath = defaultDir;
            return true;
        }

        public bool? ShowFileSelectionDialog(string defaultFile, out string filePath)
        {
            filePath = defaultFile;
            return true;
        }

        public void ShowMessageBox(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void ShowMessageBox(string title, string message)
        {
            ShowMessageBox(message);
        }

        public void ShowErrorMessageBox(string title, string message)
        {
            ShowMessageBox(@"ERROR: " + message);
        }

        public void ShowWarningMessageBox(string title, string message)
        {
            ShowMessageBox(@"WARNING: " + message);
        }

        public bool? ShowSaveAsDialog(string defaultFileName, string defaultExtension, string filter, out string savePath)
        {
            savePath = defaultFileName;
            return true;
        }

        public bool? ShowOpenFileDialog(string defaultFileName, out string fileName)
        {
            fileName = defaultFileName;
            return true;
        }
    }
}