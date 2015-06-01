// (C) Copyright 2012-2014 by Andrew Nicholas
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

namespace SCaddins.SCexport
{
    using System;
    using System.IO;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    /// <summary>
    /// Generic file input and output functions.
    /// </summary>
    public static class FileUtilities
    {   
        public static bool ConfigFileExists(Document doc)
        {
             string config = SCexport.GetConfigFileName(doc);
             return System.IO.File.Exists(config);   
        }

        /// <summary>
        /// Use your systems defualt editor to edit the config file
        /// for the current revit model - if it exists.
        /// </summary>
        /// <param name="doc">The current Revit document.</param>
        public static void EditConfigFile(Document doc)
        {
            string config = SCexport.GetConfigFileName(doc);
            if (System.IO.File.Exists(config)) {
                System.Diagnostics.Process.Start(SCaddins.SCexport.Settings1.Default.TextEditor, config);
            } else {
                TaskDialog.Show("SCexport", "config file does not exist");
            }
        }

        /// <summary>
        /// Create a config file for the current revit model.
        /// </summary>
        /// <param name="doc">The current revit document.</param>
        public static void CreateConfigFile(Document doc)
        {
            string config = SCexport.GetConfigFileName(doc);
            TaskDialogResult overwrite = TaskDialogResult.Yes;
            if (System.IO.File.Exists(config)) {
                string msg = "config exists, do you want to overwrite?";
                overwrite = TaskDialog.Show(
                    "WARNING",
                    msg,
                    TaskDialogCommonButtons.No | TaskDialogCommonButtons.Yes);
            }

            if (overwrite == TaskDialogResult.Yes) {
                string example = SCaddins.Constants.InstallDir +
                    Path.DirectorySeparatorChar +
                    SCaddins.Constants.ShareDir +
                    Path.DirectorySeparatorChar +
                    Constants.ExampleConfigFileName;
                if (System.IO.File.Exists(example)) {
                    System.IO.File.Copy(example, config, true);
                }
            }
        }

        /// <summary>
        /// Determines if the givin string is a valid windows filename.
        /// </summary>
        /// <returns><c>true</c> if it is a valid filename; otherwise, <c>false</c>.</returns>
        /// <param name="fileName"> File name. </param>
        public static bool IsValidFileName(string fileName)
        {
           var valid = !string.IsNullOrEmpty(fileName) &&
              fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
           return valid;
        }

        /// <summary>
        /// Loop until a file is accessible.
        /// </summary>
        /// <param name="file"> The file to check.</param>
        public static void WaitForFileAccess(string file)
        {
           #if DEBUG
           string msg = "Waiting for file access: " +
                         System.Environment.NewLine +
                         file;
           System.Diagnostics.Debug.WriteLine(msg);
           #endif
           int i = 0;
           while (IsFileLocked(new FileInfo(file))) {
                System.Threading.Thread.Sleep(2000);
                i++;
                if (i > 30) {
                    return;
                }
           }
        }

        /// <summary>
        /// Gets the central filename.
        /// </summary>
        /// <returns>The central filename.</returns>
        /// <param name="doc">The current Revit document.</param>
        public static string GetCentralFileName(Document doc)
        {
            if (doc.IsWorkshared) {
            #if REVIT2012
                return doc.WorksharingCentralFilename;
            #else
                ModelPath mp = doc.GetWorksharingCentralModelPath();
                string s = ModelPathUtils.ConvertModelPathToUserVisiblePath(mp);
                return s;
            #endif
            } else {
                return doc.PathName;
            }
        }
            
        /// <summary>
        /// Determines if the specified file is locked.
        /// </summary>
        /// <returns><c>true</c> if the specified file is locked; otherwise, <c>false</c>.</returns>
        /// <param name="file"> The file to evaluate.</param>
        public static bool IsFileLocked(System.IO.FileInfo file)
        {
            #if DEBUG
            string msg = "File is locked: " +
                          System.Environment.NewLine +
                          file.FullName;
            System.Diagnostics.Debug.WriteLine(msg);
            #endif
            System.IO.FileStream stream = null;
            if (file.Exists == false) {
                return false;
            }
            try {
                stream = file.Open(
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None);
            } catch (IOException) {
                return true;
            } finally {
                if (stream != null) {
                    stream.Close();
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a file can be overwritten.
        /// </summary>
        /// <returns><c>true</c> if the specified filename can be overwritten; otherwise, <c>false</c>.</returns>
        /// <param name="filename"> The filename to check. </param>
        public static bool CanOverwriteFile(string fileName)
        {
            if (IsFileLocked(new FileInfo(fileName))) {
                TaskDialog td = new TaskDialog("File in use");
                td.MainContent = "The file: " + fileName + " appears to be in use." +
                    System.Environment.NewLine +
                    "please close it before continuing...";
                td.MainInstruction = "File in use";
                td.CommonButtons = TaskDialogCommonButtons.Ok;
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                td.Show();
                if (IsFileLocked(new FileInfo(fileName))) {
                    return false;
                }
            }
            if (!SCexport.ConfirmOverwrite) {
                return true;
            }
            if (System.IO.File.Exists(fileName)) {
                string s = fileName + " exists," + System.Environment.NewLine +
                    "do you want do overwrite the existing file?";
                using (var dialog = new ConfirmationDialog(s))
                {
                    dialog.StartPosition =
                        System.Windows.Forms.FormStartPosition.CenterParent;
                    dialog.TopMost = true;
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    return result == System.Windows.Forms.DialogResult.Yes ? true : false;
                }
            } else {
                return true;
            }
        }

        /// <summary>
        /// Get the full path of a file.
        /// Useful for binary executables that exist on the PATH variable.
        /// </summary>
        /// <param name="fileName">Exe file to query.</param>
        /// <returns>The full path as a string.</returns>
        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName)) {
                return Path.GetFullPath(fileName);
            }

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';')) {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath)) {
                    return fullPath;
                }
            }

            return null;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
