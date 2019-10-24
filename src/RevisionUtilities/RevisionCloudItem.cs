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
    using System.Linq;
    using Autodesk.Revit.DB;

    public class RevisionCloudItem : RevisionItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "cloud")]
        private RevisionCloud cloud;
        private string comments;
        private string hostViewName;

        // for clouds not on sheets
        private ElementId id;

        private string mark;
        private string revision;
        private string sheetName;
        private string sheetNumber;

        public RevisionCloudItem(Document doc, RevisionCloud revisionCloud) : base(doc, revisionCloud)
        {
            mark = revisionCloud.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
            comments = revisionCloud.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
            id = revisionCloud.Id;
            revision = string.Empty;
            cloud = revisionCloud;
            hostViewName = GetHostViewName(doc);
            UpdateSheetNameAndNumberStrings(doc);
        }

        public string Comments
        {
            get { return comments; }
        }

        public string HostViewName
        {
            get { return hostViewName; }
        }

        public override ElementId Id
        {
            get { return id; }
        }

        public string Mark
        {
            get { return mark; }
        }

        public string Revision
        {
            get { return revision; }
        }

        public string SheetName
        {
            get { return sheetName; }
        }

        public string SheetNumber
        {
            get { return sheetNumber; }
        }

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
            sheetNumber = "-";
            sheetName = "-";
            if (cloud.GetSheetIds().Count == 1)
            {
                ElementId id2 = cloud.GetSheetIds().ToList().First();
                if (id2 != null)
                {
                    Element e2 = doc.GetElement(id2);
                    ViewSheet vs = (ViewSheet)e2;
                    if (vs != null)
                    {
                        sheetNumber = vs.SheetNumber;
                        revision = vs.get_Parameter(BuiltInParameter.SHEET_CURRENT_REVISION).AsString();
                        sheetName = vs.Name;
                    }
                }
            }

            if (cloud.GetSheetIds().Count > 1)
            {
                sheetNumber = "Multiple";
                sheetName = "Multiple";
            }
        }
    }
}