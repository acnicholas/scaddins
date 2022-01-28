// (C) Copyright 2012-2020 by Andrew Nicholas
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
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using System.Threading;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    public static class FileUtilities
    {
        public static bool CanOverwriteFile(string fileName)
        {
            if (IsFileLocked(new FileInfo(fileName)))
            {
                var mainContent = "The file: " + fileName + " appears to be in use." +
                                     Environment.NewLine +
                                     "please close it before continuing...";

                SCaddinsApp.WindowManager.ShowMessageBox("File in use", mainContent);

                if (IsFileLocked(new FileInfo(fileName)))
                {
                    return false;
                }
            }

            if (File.Exists(fileName) && Manager.ConfirmOverwrite)
            {
                var message = fileName + " exists," + Environment.NewLine + "do you want do overwrite the existing file?";
                var result = SCaddinsApp.WindowManager.ShowConfirmationDialog(message, Manager.ConfirmOverwrite, out var confirmOverwriteDialog);
                Manager.ConfirmOverwrite = confirmOverwriteDialog;
                return result.HasValue ? result.Value : false;
            }
            return true;
        }

        public static bool ConfigFileExists(Document doc)
        {
            var config = Manager.GetConfigFileName(doc);
            return File.Exists(config);
        }

        public static void CreateConfigFile(Document doc)
        {
            var config = Manager.GetConfigFileName(doc);
            var overwrite = TaskDialogResult.Yes;
            if (File.Exists(config))
            {
                overwrite = TaskDialog.Show(
                    "WARNING",
                    "config exists, do you want to overwrite?",
                    TaskDialogCommonButtons.No | TaskDialogCommonButtons.Yes);
            }

            if (overwrite == TaskDialogResult.Yes)
            {
                var example = SCaddins.Constants.InstallDirectory +
                              Path.DirectorySeparatorChar +
                              SCaddins.Constants.ShareDirectory +
                              Path.DirectorySeparatorChar +
                              Constants.ExampleConfigFileName;
                if (File.Exists(example))
                {
                    File.Copy(example, config, true);
                }
            }
        }

        public static string GetCentralFileName(Document doc)
        {
            if (doc == null)
            {
                return string.Empty;
            }
            if (doc.IsWorkshared)
            {
                var mp = doc.GetWorksharingCentralModelPath();
                var s = ModelPathUtils.ConvertModelPathToUserVisiblePath(mp);
                return s;
            }

            return doc.PathName;
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            if (file == null || file.Exists == false)
            {
                return false;
            }
            try
            {
                stream = file.Open(
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the file name and path is valid in windows
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsValidFileName(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }

        ////FIXME - add delay param
        public static void WaitForFileAccess(string file)
        {
            var i = 0;
            while (IsFileLocked(new FileInfo(file)))
            {
                Thread.Sleep(2000);
                i++;
                if (i > 30)
                {
                    return;
                }
            }
        }
        [SecurityCritical]
        internal static void EditConfigFile(Document doc)
        {
            var config = Manager.GetConfigFileName(doc);
            if (File.Exists(config))
            {
                var process = Process.Start(Settings1.Default.TextEditor, config);
                process.Dispose();
            }
            else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("SCexport", "config file does not exist");
            }
        }

        [SecurityCritical]
        internal static void EditConfigFileModal(Document doc)
        {
            var config = Manager.GetConfigFileName(doc);
            if (File.Exists(config))
            {
                var process = Process.Start(Settings1.Default.TextEditor, config);
                process.WaitForExit();
                process.Dispose();
            }
            else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("SCexport", "config file does not exist");
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
