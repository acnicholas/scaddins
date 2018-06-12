using System;
using System.Collections.Generic;
using System.Dynamic;
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

        public void ExportExcelSchedule()
        {
            string filePath = string.Empty;
            SCaddinsApp.WindowManager.ShowSaveFileDialog(ref filePath);
            //RevisionUtilities.ExportCloudInfo(doc, )
        }

        public void AssignRevision()
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 640;
            settings.Width = 480;
            settings.Title = "Select Revision to Assign";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            var revisionSelectionViewModel = new ExportManager.ViewModels.RevisionSelectionViewModel(doc);
            bool? result = SCaddinsApp.WindowManager.ShowDialog(revisionSelectionViewModel, null, settings);
            bool newBool = result.HasValue ? result.Value : false;
            if (newBool)
            {
                if (revisionSelectionViewModel.SelectedRevision != null)
                {
                    RevisionUtilities.AssignRevisionToClouds(doc, 
                        selectedRevisionClouds, 
                        revisionSelectionViewModel.SelectedRevision.Id);
                }
            }
        }

        public void DeleteClouds()
        {
            RevisionUtilities.DeleteRevisionClouds(doc, selectedRevisionClouds);
        }

    }
}
