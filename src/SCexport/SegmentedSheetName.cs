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
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Naming", "CA1710", Justification = "This doesn't need to end in Collection", Scope = "Just for this Class")]
public class SegmentedSheetName : Collection<SheetNameSegment>
{
    public SegmentedSheetName(FilenameScheme type)
    {
        if (type == FilenameScheme.Standard) {
            this.Name = "YYYYMMDD-AD-NNN[R]";
            this.Add(new SheetNameSegment(
                        SegmentType.ProjectNumber));
            this.Add(new SheetNameSegment(
                        SegmentType.Hyphen));
            this.Add(new SheetNameSegment(
                        SegmentType.Discipline, "AD"));
            this.Add(new SheetNameSegment(
                        SegmentType.Hyphen));
            this.Add(new SheetNameSegment(
                        SegmentType.SheetNumber));
            this.Add(new SheetNameSegment(
                        SegmentType.Text, "["));
            this.Add(new SheetNameSegment(
                        SegmentType.Revision));
            this.Add(new SheetNameSegment(
                        SegmentType.Text, "]"));
        }
    }
    
    public SegmentedSheetName()
    {
        this.Name = "User Defined";
    }
     
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
    public void AddNodesFromXML(System.Xml.XmlReader reader)
    {
        do {
            reader.Read();
            if (reader.NodeType == System.Xml.XmlNodeType.Element) {
                switch (reader.Name) {
                case "ProjectNumber":
                    this.Add(SegmentType.ProjectNumber);
                    break;
                case "SheetNumber":
                    this.Add(SegmentType.SheetNumber);
                    break;
                case "Discipline":
                    this.Add(
                        SegmentType.Discipline,
                        reader.ReadString());
                    break;
                case "Text":
                    reader.Read();
                    this.Add(
                        SegmentType.Text,
                        reader.ReadString());
                    break;
                case "Revision":
                    this.Add(SegmentType.Revision);
                    break;
                case "RevisionDescription":
                    this.Add(SegmentType.RevisionDescription);
                    break;
                case "SheetName":
                    this.Add(SegmentType.SheetName);
                    break;
                case "Underscore":
                    this.Add(SegmentType.Underscore);
                    break;
                case "Hyphen":
                    this.Add(SegmentType.Hyphen);
                    break;
                }
            }
        } while (!(reader.NodeType == System.Xml.XmlNodeType.EndElement
                    && reader.Name == "FilenameScheme"));
    }
    
        private void Add(SegmentType s, string val)
    {
            this.Add(new SheetNameSegment(s, val));
    }

    private void Add(SegmentType s)
    {
            this.Add(new SheetNameSegment(s));
    }
}
}

/* vim: set ts=4 sw=4 nu expandtab: */
