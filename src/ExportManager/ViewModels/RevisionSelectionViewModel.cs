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

using System.Dynamic;
using System.Collections.ObjectModel;
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
