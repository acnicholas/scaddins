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
    using Autodesk.Revit.UI;
    using SCaddins.Common;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private SortableBindingListCollection<RevisionCloudItem> revisionClouds;

        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            this.revisionClouds = new SortableBindingListCollection<RevisionCloudItem>();
            //GetRevisions(doc, this.revisions);
            GetRevisionClouds(doc, this.revisionClouds);
            Form1 form = new Form1(doc, this.revisionClouds);
            form.ShowDialog();
            return Autodesk.Revit.UI.Result.Succeeded;
        }
        
        private static void GetRevisionClouds(Document doc, SortableBindingListCollection<RevisionCloudItem> revisionClouds)
        {
             #if !(REVIT2014)
            FilteredElementCollector a;
            a = new FilteredElementCollector(doc);
            a.OfCategory(BuiltInCategory.OST_RevisionClouds);
            a.OfClass(typeof(RevisionCloud));
            foreach (RevisionCloud e in a) {  
                //TaskDialog.Show("test",e.Name);
                revisionClouds.Add(new RevisionCloudItem(doc, e));
            }
            #endif
        }

//        private static void GetRevisions(Document doc, SortableBindingListCollection<RevisionItem> revisions)
//        {
//            FilteredElementCollector a;
//            a = new FilteredElementCollector(doc);
//            a.OfCategory(BuiltInCategory.OST_Revisions);
//            foreach (Element e in a) {
//                int seq = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger();
//                string date = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString();
//                int issued = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_ISSUED).AsInteger();
//                string description = e.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DESCRIPTION).AsString();
//                revisions.Add(new RevisionItem(date, description, issued == 1, seq));
//            }
//        }
    }  
}