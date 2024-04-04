// (C) Copyright 2013-2021 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class OpenSheet : IExternalCommand
    {
        public static void OpenNextSheet(UIDocument udoc, ViewSheet view)
        {
            OpenSheetByOrder(udoc, view, -1);
        }

        public static void OpenPreviousSheet(UIDocument udoc, ViewSheet view)
        {
            OpenSheetByOrder(udoc, view, 1);
        }

        public static void OpenViews(System.Collections.IList views)
        {
            foreach (var item in views)
            {
                if (!(item is ExportSheet sheet) || sheet.Sheet == null)
                {
                    continue;
                }
                var uiApplication = new UIApplication(sheet.Sheet.Document.Application);
                uiApplication.ActiveUIDocument.ActiveView = sheet.Sheet;
            }
        }

        public static List<OpenableView> ViewsInModel(Document doc, bool includeViews)
        {
            var result = new List<OpenableView>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Sheets);
            foreach (var element in collector)
            {
                var view = (ViewSheet)element;
                result.Add(new OpenableView(view.Name, view.SheetNumber, view));
            }
            if (includeViews)
            {
                FilteredElementCollector collector2 = new FilteredElementCollector(doc);
                var views = collector2.OfCategory(BuiltInCategory.OST_Views).Cast<View>().Where(v => !v.IsTemplate);
                foreach (View view in views)
                {
                    result.Add(new OpenableView(view.Name, string.Empty, view));
                }
            }
            return result;
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (commandData == null)
            {
                return Result.Failed;
            }

            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            // FIXME. this is used a lot.
            // add to a utility class.
            if (doc.ActiveView.ViewType == ViewType.ProjectBrowser)
            {
                var views = new List<View>();
                var s = uidoc.Selection.GetElementIds();
                foreach (var id in s)
                {
                    var projectBrowserView = uidoc.Document.GetElement(id);
                    if (projectBrowserView is View)
                    {
                        var v = (View)projectBrowserView;
                        if (v.ViewType == ViewType.ProjectBrowser)
                        {
                            continue;
                        }
                        views.Add(v);
                    }
                }

                foreach (var view in views)
                {
                    if (view != null)
                    {
                        UIApplication uiApplication = new UIApplication(view.Document.Application);
                        uiApplication.ActiveUIDocument.ActiveView = view;
                    }
                }

                return Result.Succeeded;
            }

            var vm = new ViewModels.OpenSheetViewModel(doc);
            SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, ViewModels.OpenSheetViewModel.DefaultWindowSettings);
            return Result.Succeeded;
        }

        private static void OpenSheetByOrder(UIDocument udoc, ViewSheet view, int offset)
        {
            List<OpenableView> list = ViewsInModel(udoc.Document, false).OrderBy(o => o.SheetNumber).ToList();
            int index = list.IndexOf(list.Find(ov => ov.SheetNumber == view.SheetNumber));
            if (index > -1)
            {
                if (offset < 0 && index + offset < 0)
                {
                    return;
                }
                if (offset > 0 && index + offset >= list.Count)
                {
                    return;
                }
                list[index + offset].Open();
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
