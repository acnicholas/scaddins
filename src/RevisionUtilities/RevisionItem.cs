// (C) Copyright 2013-2016 by Andrew Nicholas
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

namespace SCaddins.RevisionUtilities
{
    using System;
    using Autodesk.Revit.DB;

    public class RevisionItem
    {
        private string date;
        private string description;
        private bool issued;
        private int sequence;

        public RevisionItem(Document doc, RevisionCloud revisionCloud)
        {
            if (revisionCloud == null)
            {
                throw new ArgumentNullException(nameof(revisionCloud));
            }
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            var revision = doc.GetElement(revisionCloud.RevisionId);
            Init(revision);
        }

        public RevisionItem(Revision revision)
        {
            Init(revision);
        }

        public string Date
        {
            get { return date; }
        }

        public string Description
        {
            get { return description; }
        }

        public bool Export
        {
            get;
            set;
        }

        public virtual ElementId Id
        {
            get; private set;
        }

        public bool Issued
        {
            get { return issued; }
        }

        public int Sequence
        {
            get { return sequence; }
        }

        private void Init(Element revision)
        {
            Id = revision.Id;
            description = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DESCRIPTION).AsString();
            date = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString();
            issued = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_ISSUED).AsInteger() == 1;
            sequence = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger();
        }
    }
}