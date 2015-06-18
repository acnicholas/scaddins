/*
 * Created by SharpDevelop.
 * User: andrewn
 * Date: 18/06/15
 * Time: 1:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SCaddins.SCexport
{
    /// <summary>
    /// Description of Enums.
    /// </summary>
    public static class Enums
    { 
         /// <summary>
        /// Type of export.
        /// </summary>
        [Flags]
        public enum ExportFlags
        {
            /// <summary>Export Nothing.</summary>
            None = 0,

            /// <summary>Export files using Adobe Acrobat.</summary>
            PDF = 1,

            /// <summary>Export a AutoCAD file.</summary>
            DWG = 2,

            /// <summary>Export A Microstation file.</summary>
            DGN = 4,

            /// <summary>Export a Autodesk dwf file.</summary>
            DWF = 8,

            /// <summary>
            /// Export files using Ghostscript to vreate pdf's.
            /// </summary>
            GhostscriptPDF = 16,

            /// <summary>Remove titleblock from sheet before exporting.
            /// </summary>
            NoTitle = 32,

            /// <summary>Tag pdf files with metadata.</summary>
            TagPDFExports = 64,
        }
        
        public enum LogType
        {
            Error,
            Warning,
            Normal
        }
        
    }
}
