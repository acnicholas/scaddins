// (C) Copyright 2012-2014 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCexport.
//
// SCexport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCexport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCexport.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCexport
{
    using System.Collections.Generic;

    /// <summary>
    /// A file name (title block).
    /// </summary>
public class SheetName : List<SheetNameSegment>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SheetName" /> class.
    /// </summary>
    /// <param name="type"> The scheme type.</param>
    public SheetName(Scheme type)
    {
        if (type == SheetName.Scheme.SC_STANDARD) {
            this.Name = "YYYYMMDD-AD-NNN[R]";
            this.Add(new SheetNameSegment(
                        SheetNameSegment.SegmentType.ProjectNumber));
            this.Add(new SheetNameSegment(
                        SheetNameSegment.SegmentType.Hyphen));
            this.Add(new SheetNameSegment(
                        SheetNameSegment.SegmentType.Discipline, "AD"));
            this.Add(new SheetNameSegment(
                        SheetNameSegment.SegmentType.Hyphen));
            this.Add(new SheetNameSegment(
                        SheetNameSegment.SegmentType.SheetNumber));
            this.Add(new SheetNameSegment(
                        SheetNameSegment.SegmentType.Text, "["));
            this.Add(new SheetNameSegment(
                        SheetNameSegment.SegmentType.Revision));
            this.Add(new SheetNameSegment(
                        SheetNameSegment.SegmentType.Text, "]"));
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SheetName"/> class.
    /// </summary>
    public SheetName()
    {
        this.Name = "User Defined";
    }
    
    #region enums
    /// <summary>
    /// Filename Scheme.
    /// </summary>
    public enum Scheme
    {
        /// <summary>
        /// My standard scheme.
        /// </summary>
        SC_STANDARD = 0,

        /// <summary>
        /// A Basic scheme [sheetnumber]_[revision].
        /// </summary>
        SHORT
    }
    #endregion
    
    /// <summary>
    /// Gets or sets the name(Title).
    /// </summary>
    /// <value>The name is the Title of the sheet.</value>
    public string Name
    {
        get;
        set;
    }
   
    /// <summary>
    /// Adds the nodes from XML config file.
    /// </summary>
    /// <param name="reader"> The reader to access the XML file. </param>
    public void AddNodesFromXML(ref System.Xml.XmlTextReader reader)
    {
        do {
            reader.Read();
            if (reader.NodeType == System.Xml.XmlNodeType.Element) {
                switch (reader.Name) {
                case "ProjectNumber":
                    this.Add(SheetNameSegment.SegmentType.ProjectNumber);
                    break;
                case "SheetNumber":
                    this.Add(SheetNameSegment.SegmentType.SheetNumber);
                    break;
                case "Discipline":
                    this.Add(
                        SheetNameSegment.SegmentType.Discipline,
                        reader.ReadString());
                    break;
                case "Text":
                    reader.Read();
                    this.Add(
                        SheetNameSegment.SegmentType.Text,
                        reader.ReadString());
                    break;
                case "Revision":
                    this.Add(SheetNameSegment.SegmentType.Revision);
                    break;
                case "RevisionDescription":
                    this.Add(SheetNameSegment.SegmentType.RevisionDescription);
                    break;
                case "SheetName":
                    this.Add(SheetNameSegment.SegmentType.SheetName);
                    break;
                case "Underscore":
                    this.Add(SheetNameSegment.SegmentType.Underscore);
                    break;
                case "Hyphen":
                    this.Add(SheetNameSegment.SegmentType.Hyphen);
                    break;
                }
            }
        } while (!(reader.NodeType == System.Xml.XmlNodeType.EndElement
                    && reader.Name == "FilenameScheme"));
    }
    
        private void Add(SheetNameSegment.SegmentType s, string val)
    {
            this.Add(new SheetNameSegment(s, val));
    }

    private void Add(SheetNameSegment.SegmentType s)
    {
            this.Add(new SheetNameSegment(s));
    }
}
}

/* vim: set ts=4 sw=4 nu expandtab: */
