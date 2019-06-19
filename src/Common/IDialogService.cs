namespace SCaddins.Common
{
    public interface IDialogService
    {
        bool? ShowConfirmationDialog(string message, bool? defaultCheckboxValue, out bool checkboxResult);

        bool? ShowDirectorySelectionDialog(string defaultDir, out string dirPath);

        void ShowMessageBox(string message);

        void ShowMessageBox(string title, string message);

        void ShowErrorMessageBox(string title, string message);

        void ShowWarningMessageBox(string title, string message);

        bool ShowYesNoDialog(string title, string message, bool defaultValue);

        bool? ShowSaveAsDialog(string defaultFileName, string defaultExtension, string filter, out string savePath);

        bool? ShowOpenFileDialog(string defaultFileName, out string fileName);
    }
}