namespace SCaddins.Common
{
    internal class MockDialogService : IDialogService
    {
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

        public bool? ShowMessageBox(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            return true;
        }

        public bool? ShowSaveAsDialog(string defaultFileName, string defaultExtension, string filter, out string savePath)
        {
            savePath = defaultFileName;
            return true;
        }
    }
}