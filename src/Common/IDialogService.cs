namespace SCaddins.Common
{
    public interface IDialogService
    {
        bool? ShowConfirmationDialog(string message, bool? defaultCheckboxValue, out bool checkboxResult);

        bool? ShowDirectorySelectionDialog(string defaultDir, out string dirPath);

        bool? ShowMessageBox(string message);

        bool? ShowSaveAsDialog(string defaultFileName, string defaultExtension, string filter, out string savePath);
    }
}