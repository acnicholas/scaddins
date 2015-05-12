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
/// <summary>
/// Sheet name segment.
/// </summary>
public class SheetNameSegment
{ 
    private SegmentType type;
    private string text;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SheetNameSegment" /> class.
    /// </summary>
    /// <param name="type">The type of segment to create.</param>
    /// <param name="text">The text to display in the segment.</param>
    public SheetNameSegment(SegmentType type, string text)
    {
        this.Init(type, text);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SheetNameSegment" /> class.
    /// </summary>
    /// <param name="type">The type of segment to create.</param>
    public SheetNameSegment(SegmentType type)
    {
        this.Init(type, null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SheetNameSegment" /> class.
    /// </summary>
    /// <param name="text"> The text to display in this segmant.</param>
    public SheetNameSegment(string text)
    {
        this.Init(SegmentType.Text, text);
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
    /// Gets the type of segment.
    /// </summary>
    /// <value>The type of segment.</value>
    public SegmentType Type
    {
        get { return this.type; }
    }
    
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text displayed in the segment.</value>
    public string Text
    {
        get { return this.text; }
        set { this.text = SheetNameSegment.GetSafeFileName(value); }
    }
    
    public static string GetSafeFileName(string filename)
    {
        if (filename == null) {
            return null;
        }
        foreach (char c in System.IO.Path.GetInvalidFileNameChars()) {
            filename = filename.Replace(c, '_');
        }
        return filename;
    }

    private void Init(SegmentType segmentType, string value)
    {      
        this.type = segmentType;
        this.text = SheetNameSegment.GetSafeFileName(value);
    }
}
}
/* vim: set ts=4 sw=4 nu expandtab: */
