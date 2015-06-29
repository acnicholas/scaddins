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
    using System.Security;
    using Autodesk.Revit.UI;

    /// <summary>
    /// Generic file input and output functions.
    /// </summary>
    public static class PdfTools
    {
        /// <summary>
        /// Tags a PDF by creating a pdftk compatible meta file.
        /// </summary>
        /// <param name="file">The pdf file to add additional tags to.</param>
        /// <param name="name">This will populate the Title part of the pdf tag.</param>
        /// <param name="rev">This will populate the Keyword part of the pdf tag.</param>
        [SecurityCritical]
        internal static void TagPDF(string file, string name, string rev)
        {
            string s = Environment.GetFolderPath(
                Environment.SpecialFolder.ProgramFilesX86);
            string pdfmetafile = PdfTools.CreatePDFMetaFile(file);

            if (File.Exists(pdfmetafile)) {
                FileUtilities.WaitForFileAccess(pdfmetafile);
                PdfTools.PopulatePDFMetaFile(pdfmetafile, name, rev);
                FileUtilities.WaitForFileAccess(pdfmetafile);
                try {
                    string prog = s + @"\SCaddins\SCaddins\opt\pdftk.exe";
                    string cmd = "\"" + file + "\" update_info \"" + pdfmetafile +
                        "\" output \"" + file + "\".tmp dont_ask"; 
                    SCaddins.Common.ConsoleUtils.StartHiddenConsoleProg(prog, cmd);
                    FileUtilities.WaitForFileAccess(file);
                    File.Delete(file);
                    File.Move(file + ".tmp", file);
                    File.Delete(pdfmetafile);
                } catch {
                    TaskDialog.Show("Error", "Error tagging pdf file");
                    return;
                }
            } 
        }
        
         [SecurityCritical]
         private static string CreatePDFMetaFile(string file)
        {
            FileUtilities.WaitForFileAccess(file);
            string s = Environment.GetFolderPath(
                Environment.SpecialFolder.ProgramFilesX86);
            string pdfmetafile = file + ".scx";

            try {
                string prog = s + @"\SCaddins\SCaddins\opt\pdftk.exe";
                string args = "\"" + file + "\" dump_data output \"" +
                    pdfmetafile + "\"";
                SCaddins.Common.ConsoleUtils.StartHiddenConsoleProg(prog, args);
                return pdfmetafile;
            } catch {
                TaskDialog.Show("Error", "Error creating pdf meta file");
                return null;
            }
        }

        private static void PopulatePDFMetaFile(
            string file, string name, string rev)
        {
                try {
                string s = "InfoKey: Title" + Environment.NewLine +
                    "InfoValue: " + name + Environment.NewLine +
                    "InfoKey: Author" + Environment.NewLine +
                    "InfoValue: " + ExportManager.Author + System.Environment.NewLine +
                    "InfoKey: Keywords" + Environment.NewLine +
                    "InfoValue: " + rev;
                File.AppendAllText(file, s);
                } catch {
                    TaskDialog.Show("Error", "Error populating pdf meta file");
                    return;
                }
        }    
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
