// (C) Copyright 2018-2020 by Andrew Nicholas
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
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

     public class SheetSelectionViewModel : Screen
    {
        private List<Autodesk.Revit.DB.View> views;
        private List<Autodesk.Revit.DB.View> selectedViews;

        public SheetSelectionViewModel(SheetCopierManager manager)
        {
            views = manager.ExistingSheets.Values.ToList().Cast<Autodesk.Revit.DB.View>().ToList();
            //// SCaddinsApp.WindowManager.ShowMessageBox(views.Count.ToString());
            selectedViews = new List<Autodesk.Revit.DB.View>();
        }

        public SheetSelectionViewModel(IEnumerable<Autodesk.Revit.DB.View> views)
        {
            this.views = views.ToList();
            //// SCaddinsApp.WindowManager.ShowMessageBox(views.Count.ToString());
            selectedViews = new List<Autodesk.Revit.DB.View>();
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

        public List<Autodesk.Revit.DB.View> Views {
            get
            {
                return views;
            }
        }

        public List<Autodesk.Revit.DB.View> SelectedViews {
            get
            {
                return selectedViews;
            }
        }

        public void RowSheetSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs eventArgs)
        {
            try {
                selectedViews.AddRange(eventArgs.AddedItems.Cast<Autodesk.Revit.DB.View>());
                eventArgs.RemovedItems.Cast<Autodesk.Revit.DB.View>().ToList().ForEach(w => selectedViews.Remove(w));
            }
            catch (ArgumentNullException argumentNullException) {
                Console.WriteLine(argumentNullException.Message);
            }
            catch (InvalidCastException invalidCastException) {
                Console.WriteLine(invalidCastException.Message);
            } catch (InvalidOperationException invalidOperationException) {
                Console.WriteLine(invalidOperationException.Message);
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
