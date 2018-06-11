using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Caliburn.Micro;

namespace SCaddins.RevisionUtilities.ViewModels
{
    class RevisionUtilitiesViewModel : Screen
    {
        private Document doc;
        private List<RevisionItem> selectedRevisions;
        private List<RevisionCloudItem> selectedRevisionClouds;

        public RevisionUtilitiesViewModel(Document doc)
        {
            this.doc = doc;
            selectedRevisions = new List<RevisionItem>();
            selectedRevisionClouds = new List<RevisionCloudItem>();
        }

        public List<RevisionItem> Revisions
        {
            get { return RevisionUtilities.GetRevisions(doc); }
        }

        public List<RevisionCloudItem> RevisionClouds
        {
            get { return RevisionUtilities.GetRevisionClouds(doc); }
        }

        public void RevisionSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            selectedRevisions.AddRange(obj.AddedItems.Cast<RevisionItem>());
            obj.RemovedItems.Cast<RevisionItem>().ToList().ForEach(w => selectedRevisions.Remove(w));
        }

        public void RevisionCloudSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            selectedRevisionClouds.AddRange(obj.AddedItems.Cast<RevisionCloudItem>());
            obj.RemovedItems.Cast<RevisionCloudItem>().ToList().ForEach(w => selectedRevisionClouds.Remove(w));
        }

        public void DeleteClouds()
        {
            RevisionUtilities.DeleteRevisionClouds(doc, selectedRevisionClouds);
        }

    }
}
