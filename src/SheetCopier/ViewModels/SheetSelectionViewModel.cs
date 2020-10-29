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
    using System.ComponentModel;
    using System.Dynamic;
    using System.Linq;
    using System.Windows.Data;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

     public class SheetSelectionViewModel : Screen
    {
        private List<Autodesk.Revit.DB.View> views;
        private List<Autodesk.Revit.DB.View> selectedViews;
        private CollectionViewSource searchResults;
        private bool showSheets;
        private bool showViews;

        public SheetSelectionViewModel(SheetCopierManager manager)
        {
            views = manager.ExistingViews.Values.ToList().Cast<Autodesk.Revit.DB.View>().ToList();
            searchResults = new CollectionViewSource();
            searchResults.Source = views.ToList();
            SearchResults.Filter = vv => {
                Autodesk.Revit.DB.View fv = vv as Autodesk.Revit.DB.View;
                if (ShowSheets && !ShowViews) {
                    return fv.ViewType == ViewType.DrawingSheet;
                }
                if (!ShowSheets && ShowViews)
                {
                    return fv.ViewType != ViewType.DrawingSheet && fv.ViewType != ViewType.ProjectBrowser;
                }
                if (ShowSheets && ShowViews)
                {
                    return fv.ViewType != ViewType.ProjectBrowser;
                }
                return false;
            };

            selectedViews = new List<Autodesk.Revit.DB.View>();
            ShowSheets = true;
            ShowViews = false;
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

        public ICollectionView SearchResults
        {
            get { return searchResults.View; }
        }

        public List<Autodesk.Revit.DB.View> SelectedViews {
            get
            {
                return selectedViews;
            }
        }

        public bool ShowSheets
        {
            get
            {
                return showSheets;
            }

            set
            {
                showSheets = value;
                SearchResults.Refresh();
                NotifyOfPropertyChange(() => ShowSheets);
            }
        }

        public bool ShowViews
        {
            get
            {
                return showViews;
            }

            set
            {
                showViews = value;
                SearchResults.Refresh();
                NotifyOfPropertyChange(() => ShowViews);
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
