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

namespace SCaddins.ExportManager
{
    using System;
    using System.IO;
    using System.Security;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    public static class FileUtilities
    {
        public static bool ConfigFileExists(Document doc)
        {
             string config = ExportManager.GetConfigFileName(doc);
             return System.IO.File.Exists(config);
        }

        public static void CreateConfigFile(Document doc)
        {
            string config = ExportManager.GetConfigFileName(doc);
            TaskDialogResult overwrite = TaskDialogResult.Yes;
            if (System.IO.File.Exists(config)) {
                const string FileExistsMessage = "config exists, do you want to overwrite?";
                overwrite = TaskDialog.Show(
                    "WARNING",
                    FileExistsMessage,
                    TaskDialogCommonButtons.No | TaskDialogCommonButtons.Yes);
            }

            if (overwrite == TaskDialogResult.Yes) {
                string example = SCaddins.Constants.InstallDirectory +
                    Path.DirectorySeparatorChar +
                    SCaddins.Constants.ShareDirectory +
                    Path.DirectorySeparatorChar +
                    Constants.ExampleConfigFileName;
                if (System.IO.File.Exists(example)) {
                    System.IO.File.Copy(example, config, true);
                }
            }
        }

        public static bool IsValidFileName(string fileName)
        {
           var valid = !string.IsNullOrEmpty(fileName) &&
              fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
           return valid;
        }

        ////FIXME - add delay param
        public static void WaitForFileAccess(string file)
        {
           int i = 0;
           while (IsFileLocked(new FileInfo(file))) {
                System.Threading.Thread.Sleep(2000);
                i++;
                if (i > 30) {
                    return;
                }
           }
        }

        public static string GetCentralFileName(Document doc)
        {
            if (doc == null) {
                return string.Empty;
            }
            if (doc.IsWorkshared) {
                ModelPath mp = doc.GetWorksharingCentralModelPath();
                string s = ModelPathUtils.ConvertModelPathToUserVisiblePath(mp);
                return s;
            } else {
                return doc.PathName;
            }
        }

        public static bool IsFileLocked(System.IO.FileInfo file)
        {
            System.IO.FileStream stream = null;
            if (file == null || file.Exists == false) {
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

        public static bool CanOverwriteFile(string fileName)
        {
            if (IsFileLocked(new FileInfo(fileName))) {
                using (var td = new TaskDialog("File in use")) {
                    td.MainContent = "The file: " + fileName + " appears to be in use." +
                        System.Environment.NewLine +
                        "please close it before continuing...";
                    td.MainInstruction = "File in use";
                    td.CommonButtons = TaskDialogCommonButtons.Ok;
                    td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                    td.Show();
                }
                if (IsFileLocked(new FileInfo(fileName))) {
                    return false;
                }
            }
            if (!ExportManager.ConfirmOverwrite) {
                return true;
            }
            if (System.IO.File.Exists(fileName)) {
                string s = fileName + " exists," + System.Environment.NewLine +
                    "do you want do overwrite the existing file?";
                //using (var dialog = new ConfirmationDialog(s))
                //{
                //    dialog.StartPosition =
                //        System.Windows.Forms.FormStartPosition.CenterParent;
                //    dialog.TopMost = true;
                //    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                //    return result == System.Windows.Forms.DialogResult.Yes ? true : false;
                //}
                return false;
            } else {
                return true;
            }
        }

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

        [SecurityCritical]
        internal static void EditConfigFile(Document doc) {
            string config = ExportManager.GetConfigFileName(doc);
            if (System.IO.File.Exists(config)) {
                System.Diagnostics.Process.Start(SCaddins.ExportManager.Settings1.Default.TextEditor, config);
            } else {
                TaskDialog.Show("SCexport", "config file does not exist");
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
