// (C) Copyright 2018 by Andrew Nicholas
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

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
            SCaddinsApp.WindowManager.ShowSaveFileDialog(@"C:\Temp\Clouds.xls", "*.xls", "Excel Workbook (.xls)|*.xls", out filePath);
            RevisionUtilities.ExportCloudInfo(doc, selectedRevisions, filePath);
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
            NotifyOfPropertyChange(() => RevisionClouds);
        }

        public void DeleteClouds()
        {
            RevisionUtilities.DeleteRevisionClouds(doc, selectedRevisionClouds);
            NotifyOfPropertyChange(() => RevisionClouds);
        }

    }
}
