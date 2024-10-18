// (C) Copyright 2019-2020 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.Common
{
    public class MockDialogService : IDialogService
    {
        public bool? ShowColourChooser()
        {
            return true;
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

        public bool? ShowFileSelectionDialog(string defaultFile, out string filePath, string defaultExtension)
        {
            filePath = defaultFile;
            return true;
        }

        public void ShowMessageBox(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            System.Console.WriteLine(message);
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

        public bool ShowYesNoDialog(string title, string message, bool defaultValue)
        {
            return defaultValue;
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
