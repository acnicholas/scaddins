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

namespace SCaddins.SheetCopier.ViewModels
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

     public class SheetSelectionViewModel : Screen
    {
        private List<ViewSheet> sheets;
        private List<ViewSheet> selectedSheets;

        public SheetSelectionViewModel(SheetCopierManager manager)
        {
            sheets = manager.ExistingSheets.Values.ToList().Cast<ViewSheet>().ToList();
            selectedSheets = new List<ViewSheet>();
        }

        public static dynamic DefaultWindowSettings {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 480;
                settings.Title = "Select Sheets for copying";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public List<ViewSheet> Sheets {
            get
            {
                return sheets;
            }
        }

        public List<ViewSheet> SelectedSheets {
            get
            {
                return selectedSheets;
            }
        }

        public void RowSheetSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs eventArgs)
        {
            try {
                selectedSheets.AddRange(eventArgs.AddedItems.Cast<ViewSheet>());
                eventArgs.RemovedItems.Cast<ViewSheet>().ToList().ForEach(w => selectedSheets.Remove(w));
            } catch {
            }
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public void OK()
        {
            TryClose(true);
        }
    }
}
