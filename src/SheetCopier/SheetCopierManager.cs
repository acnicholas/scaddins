// (C) Copyright 2014-2016 by Andrew Nicholas
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

namespace SCaddins.SheetCopier
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    public class SheetCopierManager
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "doc")]
        private Document doc;
        private Dictionary<string, View> existingSheets =
            new Dictionary<string, View>();

        private Dictionary<string, View> existingViews =
            new Dictionary<string, View>();

        private ElementId floorPlanViewFamilyTypeId;
        private List<Revision> hiddenRevisionClouds = new List<Revision>();
        private Dictionary<string, Level> levels =
            new Dictionary<string, Level>();

        private ObservableCollection<string> sheetCategories =
            new ObservableCollection<string>();

        private ObservableCollection<SheetCopierSheet> sheets;
        private StringBuilder summaryText;

        private Dictionary<string, View> viewTemplates =
            new Dictionary<string, View>();

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public SheetCopierManager(UIDocument uidoc)
        {
            summaryText = new StringBuilder();
            doc = uidoc.Document;
            sheets = new ObservableCollection<SheetCopierSheet>();
            hiddenRevisionClouds = GetAllHiddenRevisions(doc);
            GetViewTemplates();
            GetAllSheets(existingSheets, doc);
            GetAllLevelsInModel();
            GetAllViewsInModel(existingViews, doc);
            GetFloorPlanViewFamilyTypeId();
            GetAllSheetCategories();
        }

        public Document Doc => doc;

        public Dictionary<string, View> ExistingViews => existingViews;

        public Dictionary<string, View> ExistingSheets => existingSheets;

        public Dictionary<string, Level> Levels => levels;

        public ObservableCollection<string> SheetCategories => sheetCategories;

        public ObservableCollection<SheetCopierSheet> Sheets => sheets;

        public Dictionary<string, View> ViewTemplates => viewTemplates;

        public static void DeleteRevisionClouds(ElementId viewId, Document doc)
        {
            if (doc == null || viewId == null) {
                // FIXME add error message;
                return;
            }
            using (var collector = new FilteredElementCollector(doc, viewId)) {
                collector.OfCategory(BuiltInCategory.OST_RevisionClouds);
                var clouds = new List<ElementId>();
                var issuedClouds = new List<Revision>();
                foreach (Element e in collector) {
                    var cloud = e as RevisionCloud;
                    var revisionId = cloud.RevisionId;
                    var revision = doc.GetElement(revisionId) as Revision;
                    if (revision == null) {
                        continue;
                    }
                    if (revision.Issued) {
                        revision.Issued = false;
                        issuedClouds.Add(revision);
                    }
                    clouds.Add(e.Id);
                }
                doc.Delete(clouds);
                foreach (Revision r in issuedClouds) {
                    r.Issued = true;
                }
            }
        }

        public static List<Revision> GetAllHiddenRevisions(Document doc)
        {
            var revisions = new List<Revision>();
            using (FilteredElementCollector collector = new FilteredElementCollector(doc)) {
                collector.OfCategory(BuiltInCategory.OST_Revisions);
                foreach (Element e in collector) {
                    Revision rev = e as Revision;
                    if (rev.Visibility == RevisionVisibility.Hidden) {
                        revisions.Add(rev);
                    }
                }
            }
            return revisions;
        }

        public static void GetAllSheets(Dictionary<string, View> existingSheets, Document doc)
        {
            if (existingSheets == null) {
                return;
            }
            existingSheets.Clear();

            if (doc == null) {
                return;
            }
            using (var c1 = new FilteredElementCollector(doc)) {
                c1.OfCategory(BuiltInCategory.OST_Sheets);
                foreach (var element in c1) {
                    var view = (View)element;
                    var vs = view as ViewSheet;
                    existingSheets.Add(vs.SheetNumber, view);
                }
            }
        }

        public static void GetAllViewsInModel(Dictionary<string, View> existingViews, Document doc)
        {
            if (existingViews == null) {
                return;
            }
            existingViews.Clear();

            if (doc == null) {
                return;
            }
            using (var collector = new FilteredElementCollector(doc)) {
                collector.OfClass(typeof(View));
                foreach (Element element in collector) {
                    var view = element as View;
                    if (view == null) {
                        continue;
                    }
                    if (!existingViews.TryGetValue(view.Name, out _)) {
                        existingViews.Add(view.Name, view);
                    }
                }
            }
        }

        public static ViewSheet ViewToViewSheet(View view)
        {
            if (view == null)
            {
                return null;
            }
            return (view.ViewType != ViewType.DrawingSheet) ? null : view as ViewSheet;
        }

        public bool AddCurrentSheet()
        {
            if (doc.ActiveView.ViewType == ViewType.DrawingSheet)
            {
                return AddSheet((ViewSheet)doc.ActiveView);
            }
            return false;
        }

        public void AddSheetCategory(string name)
        {
            sheetCategories.Add(name);
        }

        // add an empty sheet to the doc.
        // this comes first before copying titleblock, views etc.
        public ViewSheet AddEmptySheetToDocument(
            string sheetNumber,
            string sheetTitle,
            string viewCategory)
        {
            var result = ViewSheet.Create(doc, ElementId.InvalidElementId);
            result.Name = sheetTitle;
            result.SheetNumber = sheetNumber;
            var viewCategoryParamList = result.GetParameters(SheetCopierConstants.SheetCategory);
            if (viewCategoryParamList.Count > 0) {
                Parameter viewCategoryParam = viewCategoryParamList.First();
                viewCategoryParam.Set(viewCategory);
            }
            return result;
        }

        public bool AddSheet(ViewSheet sourceSheet)
        {
            if (sourceSheet != null)
            {
                string n = GetNewSheetNumber(sourceSheet.SheetNumber);
                string t = sourceSheet.Name + SheetCopierConstants.MenuItemCopy;
                sheets.Add(new SheetCopierSheet(n, t, this, sourceSheet));
                return true;
            }
            return false;

            // FIXME add error message,
        }

        public void CopyElementsBetweenSheets(SheetCopierSheet sheet)
        {
            IList<ElementId> list = new List<ElementId>();
            using (var collector = new FilteredElementCollector(doc)) {
                collector.OwnedByView(sheet.SourceSheet.Id);
                foreach (Element e in collector) {
                    if (!(e is Viewport)) {
                        if (e is CurveElement) {
                            continue;
                        }
                        if (e.IsValidObject && e.ViewSpecific) {
                            list.Add(e.Id);
                        }
                    }
                }
            }
            if (list.Count > 0) {
                Transform transform;
                CopyPasteOptions options;
                ElementTransformUtils.CopyElements(
                    sheet.SourceSheet,
                    list,
                    sheet.DestinationSheet,
                    transform = new Transform(ElementTransformUtils.GetTransformFromViewToView(sheet.SourceSheet, sheet.DestinationSheet)),
                    options = new CopyPasteOptions());
                DeleteRevisionClouds(sheet.DestinationSheet.Id, doc);
                options.Dispose();
                transform.Dispose();
            }
        }

        // this is where the action happens
        public bool CreateAndPopulateNewSheet(SheetCopierSheet sheet, StringBuilder summary)
        {
            if (sheet == null) {
                return false;
            }

            // turn on hidden revisions
            foreach (Revision rev in hiddenRevisionClouds) {
                try {
                    rev.Visibility = RevisionVisibility.CloudAndTagVisible;
                } catch (Autodesk.Revit.Exceptions.ArgumentOutOfRangeException ex) {
                    SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                }
            }

            sheet.DestinationSheet = AddEmptySheetToDocument(
                sheet.Number,
                sheet.Title,
                sheet.SheetCategory);

            if (sheet.DestinationSheet != null) {
                Debug.WriteLine(sheet.Number + " added to document.");
                CreateViewports(sheet);
            } else {
                Debug.WriteLine(sheet.Number + " Could not be added added to document.");
                return false;
            }

            try {
                CopyElementsBetweenSheets(sheet);
            } catch (InvalidOperationException e) {
                Debug.WriteLine(e.Message);
            }

            foreach (Revision rev in hiddenRevisionClouds) {
                rev.Visibility = RevisionVisibility.Hidden;
            }

            var oldNumber = sheet.SourceSheet.SheetNumber;
            var msg = " Sheet: " + oldNumber + " copied to: " + sheet.Number;
            if (summary != null) {
                summary.Append(msg + Environment.NewLine);
            }

            return true;
        }

        public void CreateSheets()
        {
            if (sheets.Count < 1)
            {
                return;
            }

            int n = 0;
            View firstSheet = null;
            summaryText.Clear();

            using (var t = new Transaction(doc, "Copy Sheets"))
            {
                if (t.Start() == TransactionStatus.Started)
                {
                    foreach (SheetCopierSheet sheet in sheets)
                    {
                        n++;
                        if (CreateAndPopulateNewSheet(sheet, summaryText) && n == 1)
                        {
                            firstSheet = sheet.DestinationSheet;
                        }
                    }
                    if (TransactionStatus.Committed != t.Commit())
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox("Copy Sheets Failure", "Transaction could not be committed");
                    }
                    else
                    {
                        // try to open the first sheet created
                        if (firstSheet != null)
                        {
                            var uiapp = new UIApplication(doc.Application);
                            uiapp.ActiveUIDocument.ActiveView = firstSheet;
                        }
                    }
                }
            }

            SCaddinsApp.WindowManager.ShowMessageBox("Copy Sheets - Summary", summaryText.ToString());
        }

        public void CreateViewports(SheetCopierSheet sheet)
        {
            Dictionary<ElementId, XYZ> viewPorts =
                GetViewportDictionary(sheet.SourceSheet, doc);

            foreach (SheetCopierViewOnSheet view in sheet.ViewsOnSheet) {
                XYZ sourceViewPortCentre = null;
                if (!viewPorts.TryGetValue(view.OldId, out sourceViewPortCentre)) {
                    SCaddinsApp.WindowManager.ShowMessageBox("SCopy", "Error...");
                    continue;
                }

                switch (view.CreationMode) {
                    case ViewPortPlacementMode.Copy:
                    DuplicateViewOntoSheet(view, sheet, sourceViewPortCentre);
                    break;

                    case ViewPortPlacementMode.New:
                    PlaceNewViewOnSheet(view, sheet, sourceViewPortCentre);
                    break;

                    case ViewPortPlacementMode.Legend:
                    PlaceViewPortOnSheet(sheet.DestinationSheet, view.OldView.Id, sourceViewPortCentre);
                    break;
                }
            }
        }

        public void DuplicateViewOntoSheet(
            SheetCopierViewOnSheet view,
            SheetCopierSheet sheet,
            XYZ sourceViewCentre)
        {
            var d = view.DuplicateWithDetailing ? ViewDuplicateOption.WithDetailing : ViewDuplicateOption.Duplicate;

            ElementId destViewId = ElementId.InvalidElementId;
            if (view.OldView.CanViewBeDuplicated(d)) {
                try {
                    destViewId = view.OldView.Duplicate(d);
                } catch (Autodesk.Revit.Exceptions.ArgumentOutOfRangeException arx) {
                    SCaddinsApp.WindowManager.ShowMessageBox(arx.Message);
                } catch (Autodesk.Revit.Exceptions.InvalidOperationException iox) {
                    SCaddinsApp.WindowManager.ShowMessageBox(iox.Message);
                } 
            } else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("WARNING: CanViewBeDuplicated is returning false for view: " + view.OldView.Name);
                return;
            }

            if (destViewId == ElementId.InvalidElementId) {
                SCaddinsApp.WindowManager.ShowMessageBox("WARNING: could not create copy of view: " + view.OldView.Name);
                //// sometimes view.Duplicate seems to fail if the duplicate option is set to ViewDuplicateOption.WithDetailing
                //// try again with option set to ViewDuplicateOption.Duplicate
                if (d == ViewDuplicateOption.WithDetailing)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("Attempting to create view without detailing..." + view.OldView.Name);
                    view.DuplicateWithDetailing = false;
                    DuplicateViewOntoSheet(view, sheet, sourceViewCentre);
                }
                return;
            }

            DeleteRevisionClouds(destViewId, doc);
       
            var elem = doc.GetElement(destViewId);
            if (elem == null) {
                return;
            }
            var v = elem as View;

            string newName = sheet.GetNewViewName(view.OldView.Id);

            if (newName != null) {
                v.Name = newName; 
            } else {
                SCaddinsApp.WindowManager.ShowMessageBox("ERROR", "New view name could not be set to: " + newName);
            }

            TryAssignViewTemplate(v, view.ViewTemplateName);

            PlaceViewPortOnSheet(sheet.DestinationSheet, destViewId, sourceViewCentre);
        }

        public string GetNewSheetNumber(string originalNumber)
        {
            if (string.IsNullOrEmpty(originalNumber)) {
                return null;
            }
            int inc = 0;
            do {
                inc++;
            } while (!SheetNumberAvailable(originalNumber + "-" + inc.ToString(CultureInfo.InvariantCulture)));
            return originalNumber + "-" + inc.ToString(CultureInfo.InvariantCulture);
        }

        public void PlaceNewViewOnSheet(
            SheetCopierViewOnSheet view,
            SheetCopierSheet sheet,
            XYZ sourceViewCentre)
        {
            Level level = null;
            levels.TryGetValue(view.AssociatedLevelName, out level);
            if (level != null) {
                using (ViewPlan vp = ViewPlan.Create(doc, floorPlanViewFamilyTypeId, level.Id)) {
                    vp.CropBox = view.OldView.CropBox;
                    vp.CropBoxActive = view.OldView.CropBoxActive;
                    vp.CropBoxVisible = view.OldView.CropBoxVisible;
                    TryAssignViewTemplate(vp, view.ViewTemplateName);
                    PlaceViewPortOnSheet(sheet.DestinationSheet, vp.Id, sourceViewCentre);
                }
            }
            level.Dispose();
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void PlaceViewPortOnSheet(
            Element destSheet, ElementId destViewId, XYZ viewCentre)
        {
            try {
                Viewport.Create(doc, destSheet.Id, destViewId, viewCentre);
            } catch (Autodesk.Revit.Exceptions.ArgumentException ex) {
                SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
            } catch (Autodesk.Revit.Exceptions.ForbiddenForDynamicUpdateException fex) {
                SCaddinsApp.WindowManager.ShowMessageBox(fex.Message);
            } catch (Autodesk.Revit.Exceptions.ModificationForbiddenException mex) {
                SCaddinsApp.WindowManager.ShowMessageBox(mex.Message);
            }
        }

        public bool SheetNumberAvailable(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return false;
            }
            foreach (SheetCopierSheet s in sheets)
            {
                if (s.Number.ToUpper(CultureInfo.InvariantCulture).Equals(number.ToUpper(CultureInfo.InvariantCulture), StringComparison.InvariantCulture))
                {
                    return false;
                }
            }
            return !existingSheets.ContainsKey(number);
        }

        public void TryAssignViewTemplate(View view, string templateName)
        {
            if (templateName != SheetCopierConstants.MenuItemCopy) {
                View vt = null;
                if (viewTemplates.TryGetValue(templateName, out vt)) {
                    try {
                        view.ViewTemplateId = vt.Id;
                    } catch (Autodesk.Revit.Exceptions.ArgumentException ex) {
                        SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                    }
                } else {
                    SCaddinsApp.WindowManager.ShowMessageBox("Warning: could not assign view template to view: " + view.Name);
                }
            }
        }

        public bool ViewNameAvailable(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return false;
            }
            foreach (SheetCopierSheet s in sheets)
            {
                foreach (SheetCopierViewOnSheet v in s.ViewsOnSheet)
                {
                    if (v.Title.ToUpper(CultureInfo.InvariantCulture).Equals(title.ToUpper(CultureInfo.InvariantCulture), StringComparison.CurrentCulture))
                    {
                        return false;
                    }
                }
            }
            return !existingViews.ContainsKey(title);
        }

        private static Dictionary<ElementId, XYZ> GetViewportDictionary(ViewSheet srcSheet, Document doc)
        {
            var result = new Dictionary<ElementId, XYZ>();
            foreach (ElementId viewPortId in srcSheet.GetAllViewports())
            {
                var viewPort = (Viewport)doc.GetElement(viewPortId);
                var viewPortCentre = viewPort.GetBoxCenter();
                result.Add(
                    viewPort.ViewId, viewPortCentre);
            }
            return result;
        }

        private void GetAllLevelsInModel()
        {
            levels.Clear();
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfClass(typeof(Level));
                foreach (Element element in collector)
                {
                    levels.Add(element.Name.ToString(CultureInfo.CurrentCulture), element as Level);
                }
            }
        }

        private void GetAllSheetCategories()
        {
            sheetCategories.Clear();
            sheetCategories.Add(@"<None>");
            using (var c1 = new FilteredElementCollector(doc))
            {
                c1.OfCategory(BuiltInCategory.OST_Sheets);
                foreach (var element in c1)
                {
                    var view = (View)element;
                    var viewCategoryParamList = view.GetParameters(SheetCopierConstants.SheetCategory);
                    if (viewCategoryParamList != null && viewCategoryParamList.Count > 0)
                    {
                        Parameter viewCategoryParam = viewCategoryParamList.First();
                        string s = viewCategoryParam.AsString();
                        if (!string.IsNullOrEmpty(s) && !sheetCategories.Contains(s))
                        {
                            sheetCategories.Add(s);
                        }
                    }
                }
            }
        }

        private void GetFloorPlanViewFamilyTypeId()
        {
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfClass(typeof(ViewFamilyType));
                foreach (var element in collector)
                {
                    var vft = (ViewFamilyType)element;
                    if (vft.ViewFamily == ViewFamily.FloorPlan)
                    {
                        floorPlanViewFamilyTypeId = vft.Id;
                    }
                }
            }
        }

        private void GetViewTemplates()
        {
            viewTemplates.Clear();
            using (var c = new FilteredElementCollector(doc))
            {
                c.OfCategory(BuiltInCategory.OST_Views);
                foreach (var element in c)
                {
                    var view = (View)element;
                    if (view.IsTemplate)
                    {
                        viewTemplates.Add(view.Name, view);
                    }
                }
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */