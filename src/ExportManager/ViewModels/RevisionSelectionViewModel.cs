using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.UI;
using Caliburn.Micro;

namespace SCaddins.ExportManager.ViewModels
{
    class RevisionSelectionViewModel : Screen
    {
        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 480;
                settings.Title = "Select Revision to Assign";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public ObservableCollection<SCaddins.RevisionUtilities.RevisionItem> Revisions
        {
            get; set;
        }

        public RevisionUtilities.RevisionItem SelectedRevision
        {
            get; set;
        }

        public RevisionSelectionViewModel(Autodesk.Revit.DB.Document doc)
        {
            Revisions = new ObservableCollection<SCaddins.RevisionUtilities.RevisionItem>(RevisionUtilities.RevisionUtilities.GetRevisions(doc));
        }

        public void OK()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}
