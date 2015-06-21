/*
 * Created by SharpDevelop.
 * User: andrewn
 * Date: 18/06/15
 * Time: 1:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
namespace SCaddins.SCexport
{
    using System;
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
    
    /// <summary>
    /// Availabel types of Sheet Name Segments.
    /// </summary>
    public enum SegmentType
    {
        /// <summary>
        /// A text segment, can be user specified.
        /// </summary>
        Text = 0,

        /// <summary>
        /// The sheet number of the revivt sheet.
        /// This is the number on the title block.
        /// </summary>
        SheetNumber,

        /// <summary>
        /// The name of the Revit sheet.
        /// This is the title on the title block.
        /// </summary>
        SheetName,

        /// <summary>
        /// The project number of the Revit model.
        /// </summary>
        ProjectNumber,

        /// <summary>
        /// The discipline.
        /// </summary>
        Discipline,

        /// <summary>
        /// The revision of the sheet.
        /// </summary>
        Revision,

        /// <summary>
        /// The revision description.
        /// </summary>
        RevisionDescription,

        /// <summary>
        /// A hyphen '-'.
        /// </summary>
        Hyphen,

        /// <summary>
        /// An underscore '_'.
        /// </summary>
        Underscore
    }
        
    /// <summary>
    /// Filename Scheme.
    /// </summary>
    public enum FilenameScheme
    {
        /// <summary>
        /// My standard scheme.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// A Basic scheme [sheetnumber]_[revision].
        /// </summary>
        Short
    }
}
