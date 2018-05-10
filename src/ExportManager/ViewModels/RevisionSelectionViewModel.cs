using System;
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
    class RevisionSelectionViewModel : PropertyChangedBase
    {

        public ObservableCollection<SCaddins.RevisionUtilities.RevisionItem> Revisions
        {
            set;
            get;
        }

        //public RevisionUtilities.RevisionItem SelectedRevision
        //{
        //    get; set;
        //}

        public RevisionSelectionViewModel(Autodesk.Revit.DB.Document doc)
        {
            Revisions = new ObservableCollection<SCaddins.RevisionUtilities.RevisionItem>(RevisionUtilities.RevisionUtilities.GetRevisions(doc));
        }
    }
}
