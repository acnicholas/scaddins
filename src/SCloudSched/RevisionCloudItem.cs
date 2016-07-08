// (C) Copyright 2013-2014 by Andrew Nicholas
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

namespace SCaddins.SCloudSChed
{
    using Autodesk.Revit.DB;
    
    public class RevisionCloudItem
    {
        private bool export;
        private string description;
        private string date;
        private string revision;
        private int sequence;
        private string sheetNumber;
        private string sheetName;
        private ElementId id;
        
        #if !(REVIT2014)
        private RevisionCloud cloud;
        #endif

        #if !(REVIT2014)
        public RevisionCloudItem(Document doc, RevisionCloud e)
        {
            this.sequence = e.get_Parameter(BuiltInParameter.REVISION_CLOUD_REVISION_NUM).AsInteger();
            Parameter p = e.get_Parameter(BuiltInParameter.REVISION_CLOUD_REVISION);
            //Revision r = p.AsValueString as Revision;
            this.revision = p.AsValueString();
            this.date = e.get_Parameter(BuiltInParameter.REVISION_CLOUD_REVISION_DATE).AsString();
            this.description = e.get_Parameter(BuiltInParameter.REVISION_CLOUD_REVISION_DESCRIPTION).AsString();
            this.id = e.Id;
            this.export = false;
            this.cloud = e;
            if(cloud.GetSheetIds().Count == 1) {
                ElementId id2 = null;
                foreach (ElementId id3 in cloud.GetSheetIds()) {
                    id2 = id3;
                    break;
                }
                if (id2 != null) {
                    Element e2 = doc.GetElement(id2);
                    ViewSheet vs = (ViewSheet)e2;
                    if(vs!= null){
                        this.sheetNumber = vs.SheetNumber;
                        this.sheetName = vs.Name;
                    } else {
                        this.sheetNumber = "Error";
                        this.sheetName = "Error";
                    }
                } else {
                    this.sheetNumber = "Error";
                    this.sheetName = "Error";    
                }
            }
            if(cloud.GetSheetIds().Count > 1) {
                this.sheetNumber = "Multiple";
                 this.sheetName = "Multiple";
            }
            if(cloud.GetSheetIds().Count < 1) {
                this.sheetNumber = "None";
                this.sheetName = "Multiple";
            }
            
        }
        #else
        public RevisionCloudItem(Element e)
        {
            this.sequence = e.get_Parameter(BuiltInParameter.REVISION_CLOUD_REVISION_NUM).AsInteger();
            Parameter p = e.get_Parameter(BuiltInParameter.REVISION_CLOUD_REVISION);
            //Revision r = p.AsValueString as Revision;
            this.revision = p.AsValueString();
            this.date = e.get_Parameter(BuiltInParameter.REVISION_CLOUD_REVISION_DATE).AsString();
            this.description = e.get_Parameter(BuiltInParameter.REVISION_CLOUD_REVISION_DESCRIPTION).AsString();
            this.id = e.Id;
            this.export = false;
        }
        #endif
        
        public bool Export {
            get { return this.export; }
            set { this.export = value; }
        }
        
        public string Revision {
            get { return this.revision; }
        }
        
        public string Description {
            get { return this.description; }
        }
        
        public string Date {
            get { return this.date; }
        }
        
        public string SheetNumber {
            get { return this.sheetNumber; }
        }
        
        public string SheetName {
            get { return this.sheetName; }
        }

        public int Sequence {
            get { return this.sequence; }
        }
           
        public ElementId Id {
            get { return this.id; }
        }
        
         #if !(REVIT2014)
        public RevisionCloud GetCloud()
        {
            return this.cloud;
        }
        #endif
        
        
        
        
    }
}
