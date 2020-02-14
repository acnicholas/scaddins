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
    using System.Linq;
    using Autodesk.Revit.DB;

    public class RevisionCloudItem : RevisionItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "cloud")]
        private RevisionCloud cloud;

        // for clouds not on sheets
        private ElementId id;

        public RevisionCloudItem(Document doc, RevisionCloud revisionCloud) : base(doc, revisionCloud)
        {
            Mark = revisionCloud.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
            Comments = revisionCloud.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
            id = revisionCloud.Id;
            Revision = string.Empty;
            cloud = revisionCloud;
            HostViewName = GetHostViewName(doc);
            UpdateSheetNameAndNumberStrings(doc);
        }

        public string Comments { get; }

        public string HostViewName { get; }

        public override ElementId Id => id;

        public string Mark { get; }

        public string Revision { get; private set; }

        public string SheetName { get; private set; }

        public string SheetNumber { get; private set; }

        public void SetCloudId(ElementId revisionId)
        {
            cloud.RevisionId = revisionId;
        }

        private string GetHostViewName(Document doc)
        {
            return doc.GetElement(cloud.OwnerViewId).Name;
        }

        private void UpdateSheetNameAndNumberStrings(Document doc)
        {
            SheetNumber = "-";
            SheetName = "-";
            if (cloud.GetSheetIds().Count == 1)
            {
                ElementId id2 = cloud.GetSheetIds().ToList().First();
                if (id2 != null)
                {
                    Element e2 = doc.GetElement(id2);
                    ViewSheet vs = (ViewSheet)e2;
                    if (vs != null)
                    {
                        SheetNumber = vs.SheetNumber;
                        Revision = vs.get_Parameter(BuiltInParameter.SHEET_CURRENT_REVISION).AsString();
                        SheetName = vs.Name;
                    }
                }
            }

            if (cloud.GetSheetIds().Count > 1)
            {
                SheetNumber = "Multiple";
                SheetName = "Multiple";
            }
        }
    }
}