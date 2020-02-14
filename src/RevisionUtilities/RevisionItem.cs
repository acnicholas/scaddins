// (C) Copyright 2013-2020 by Andrew Nicholas
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

        public string Date { get; private set; }

        public string Description { get; private set; }

        public bool Export
        {
            get;
            set;
        }

        public virtual ElementId Id
        {
            get; private set;
        }

        public bool Issued { get; private set; }

        public int Sequence { get; private set; }

        private void Init(Element revision)
        {
            Id = revision.Id;
            Description = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DESCRIPTION).AsString();
            Date = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString();
            Issued = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_ISSUED).AsInteger() == 1;
            Sequence = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger();
        }
    }
}