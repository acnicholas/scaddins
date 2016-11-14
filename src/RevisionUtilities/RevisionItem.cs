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
    using Autodesk.Revit.DB;
    
    public class RevisionItem
    {
        public RevisionItem()
        {
        }
        
        public RevisionItem(Document doc, Revision revision)
        {
            this.Description = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DESCRIPTION).AsString();
            this.Date = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString();
            this.Issued = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_ISSUED).AsInteger() == 1;
            this.Sequence = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger();
        }

        public bool Export {
            get;
            set;
        }

        public string Description {
            get;
            set;
        }

        public string Date {
            get;
            set;
        }

        public bool Issued {
            get;
            set;
        }

        public int Sequence {
            get;
            set;
        }
    }
}