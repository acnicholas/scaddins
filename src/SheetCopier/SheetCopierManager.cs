// (C) Copyright 2014-2022 by Andrew Nicholas
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
        private List<Revision> hiddenRevisionClouds = new List<Revision>();
        private StringBuilder summaryText;
        private UIDocument uidoc;
        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public SheetCopierManager(UIDocument uidoc)
        {
            summaryText = new StringBuilder();
            doc = uidoc.Document;
            this.uidoc = uidoc;
            ViewHosts = new ObservableCollection<SheetCopierViewHost>();
            hiddenRevisionClouds = GetAllHiddenRevisions(doc);
            GetViewTemplates();
            GetAllSheets(ExistingSheets, doc);
            GetAllLevelsInModel();
            GetAllViewsInModel(ExistingViews, doc);
            CustomSheetParametersOne = new ObservableCollection<string>(GetAllParameterValuesInModel(Settings.Default.CustomSheetParameterOne));
            CustomSheetParametersTwo = new ObservableCollection<string>(GetAllParameterValuesInModel(Settings.Default.CustomSheetParameterTwo));
            CustomSheetParametersThree = new ObservableCollection<string>(GetAllParameterValuesInModel(Settings.Default.CustomSheetParameterThree));
        }
        
        public static string PrimaryCustomSheetParameterName => Settings.Default.CustomSheetParameterOne;

        public static string SecondaryCustomSheetParameterName => Settings.Default.CustomSheetParameterTwo;

        public static string TertiaryCustomSheetParameterName => Settings.Default.CustomSheetParameterThree;

        public ViewType ActiveViewType => doc.ActiveView.ViewType;

        public Document Doc => doc;

        public Dictionary<string, View> ExistingSheets { get; } = new Dictionary<string, View>();

        public Dictionary<string, View> ExistingViews { get; } = new Dictionary<string, View>();

        public int IndependentViewCount
        {
            get
            {
                var modelHost = ViewHosts.Where(v => v.Type == ViewHostType.Model);
                return modelHost.Count() > 0 ? modelHost.First().ChildViews.Count : 0;
            }
        }

        public Dictionary<string, Level> Levels { get; } = new Dictionary<string, Level>();

        public ObservableCollection<string> CustomSheetParametersOne { get; set; }

        public ObservableCollection<string> CustomSheetParametersTwo { get; set; }

        public ObservableCollection<string> CustomSheetParametersThree { get; set; }

        public ObservableCollection<SheetCopierViewHost> ViewHosts { get; }
        
        public Dictionary<string, View> ViewTemplates { get; } = new Dictionary<string, View>();

        public static void DeleteRevisionClouds(ElementId viewId, Document doc)
        {
            if (!Settings.Default.DeleteRevisionClouds)
            {
                return;
            }

            if (doc == null || viewId == null)
            {
                // FIXME add error message;
                return;
            }
            using (var collector = new FilteredElementCollector(doc, viewId))
            {
                collector.OfCategory(BuiltInCategory.OST_RevisionClouds);
                var clouds = new List<ElementId>();
                var issuedClouds = new List<Revision>();
                foreach (Element e in collector)
                {
                    var cloud = e as RevisionCloud;
                    var revisionId = cloud.RevisionId;
                    Revision revision = doc.GetElement(revisionId) as Revision;
                    if (revision == null)
                    {
                        continue;
                    }
                    if (revision.Issued)
                    {
                        revision.Issued = false;
                        issuedClouds.Add(revision);
                    }
                    clouds.Add(e.Id);
                }
                doc.Delete(clouds);
                foreach (Revision r in issuedClouds)
                {
                    r.Issued = true;
                }
            }
        }

        public static List<Revision> GetAllHiddenRevisions(Document doc)
        {
            var revisions = new List<Revision>();

            //// don't get revision if not needed
            //// just return an empty list.
            if (!Settings.Default.DeleteRevisionClouds)
            {
                return revisions;
            }

            using (FilteredElementCollector collector = new FilteredElementCollector(doc))
            {
                collector.OfCategory(BuiltInCategory.OST_Revisions);
                foreach (Element e in collector)
                {
                    Revision rev = e as Revision;
                    if (rev.Visibility == RevisionVisibility.Hidden)
                    {
                        revisions.Add(rev);
                    }
                }
            }
            return revisions;
        }

        public static void GetAllSheets(Dictionary<string, View> existingSheets, Document doc)
        {
            if (existingSheets == null)
            {
                return;
            }
            existingSheets.Clear();

            if (doc == null)
            {
                return;
            }
            using (var c1 = new FilteredElementCollector(doc))
            {
                c1.OfCategory(BuiltInCategory.OST_Sheets);
                foreach (var element in c1)
                {
                    var view = (View)element;
                    ViewSheet viewSheet = view as ViewSheet;
                    if (viewSheet == null)
                    {
                        continue;
                    }
                    existingSheets.Add(viewSheet.SheetNumber, view);
                }
            }
        }

        public static void GetAllViewsInModel(Dictionary<string, View> existingViews, Document doc)
        {
            if (existingViews == null)
            {
                return;
            }
            existingViews.Clear();

            if (doc == null)
            {
                return;
            }
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfClass(typeof(View));
                foreach (Element element in collector)
                {
                    var view = element as View;
                    if (view == null)
                    {
                        continue;
                    }
                    if (!existingViews.TryGetValue(view.Name, out _))
                    {
                        existingViews.Add(view.Name, view);
                    }
                }
            }
        }

        public static ElementId GetFloorPlanViewFamilyTypeId(Document doc, ViewType viewType)
        {
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfClass(typeof(ViewFamilyType));
                foreach (var element in collector)
                {
                    var vft = (ViewFamilyType)element;
                    if (viewType == ViewType.FloorPlan && vft.ViewFamily == ViewFamily.FloorPlan)
                    {
                        return vft.Id;
                    }
                    if (viewType == ViewType.CeilingPlan && vft.ViewFamily == ViewFamily.CeilingPlan)
                    {
                        return vft.Id;
                    }
                    if (viewType == ViewType.AreaPlan && vft.ViewFamily == ViewFamily.AreaPlan)
                    {
                        return vft.Id;
                    }
                }
            }
            return ElementId.InvalidElementId;
        }

        public bool AddCurrentView()
        {
            return AddView(doc.ActiveView);
        }

        // add an empty sheet to the doc.
        // this comes first before copying titleblock, views etc.
        public ViewSheet AddEmptySheetToDocument(
            string sheetNumber,
            string sheetTitle,
            string param1,
            string param2,
            string param3)
        {
            // Make a sheet without a title block, and copy the old titleblock later.
            // FIXME. make this work if there is more than one title block on a sheet.
            var result = ViewSheet.Create(doc, ElementId.InvalidElementId);
            if (result == null)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Error adding empty sheet");
                return null;
            }
            result.Name = sheetTitle;
            result.SheetNumber = sheetNumber;
            var viewCategoryParamList = result.GetParameters(Settings.Default.CustomSheetParameterOne);
            if (viewCategoryParamList.Count > 0)
            {
                Parameter param = viewCategoryParamList.First();
                param.Set(param1);
            }
            viewCategoryParamList.Clear();
            viewCategoryParamList = result.GetParameters(Settings.Default.CustomSheetParameterTwo);
            if (viewCategoryParamList.Count > 0)
            {
                Parameter param = viewCategoryParamList.First();
                param.Set(param2);
            }
            viewCategoryParamList.Clear();
            viewCategoryParamList = result.GetParameters(Settings.Default.CustomSheetParameterThree);
            if (viewCategoryParamList.Count > 0)
            {
                Parameter param = viewCategoryParamList.First();
                param.Set(param3);
            }
            return result;
        }

        public bool AddSheet(ViewSheet sourceSheet)
        {
            if (sourceSheet != null)
            {
                string n = GetNewSheetNumber(sourceSheet.SheetNumber);
                string t = sourceSheet.Name + SheetCopierConstants.MenuItemCopy;
                ViewHosts.Add(new SheetCopierViewHost(n, t, this, sourceSheet));
                return true;
            }
            return false;

            // FIXME add error message,
        }

        public bool AddView(View view)
        {
            if (view.ViewType == ViewType.ProjectBrowser)
            {
                var selection = uidoc.Selection.GetElementIds();
                foreach (var id in selection)
                {
                    var projectBrowserView = doc.GetElement(id);
                    if (projectBrowserView is View)
                    {
                        var v = (View)projectBrowserView;
                        if (v.ViewType == ViewType.ProjectBrowser)
                        {
                            continue;
                        }
                        AddView((View)projectBrowserView);
                    }
                }
                return true;
            }

            if (view.ViewType == ViewType.DrawingSheet)
            {
                if (view != null)
                {
                    var v = view as ViewSheet;
                    string n = GetNewSheetNumber(v.SheetNumber);
                    string t = v.Name + SheetCopierConstants.MenuItemCopy;
                    ViewHosts.Add(new SheetCopierViewHost(n, t, this, v));
                    return true;
                }
                return false;
            }
            else
            {
                var modelHost = GetFirstModelHost();
                modelHost.ChildViews.Add(new SheetCopierView(view.Name, view, this));
                modelHost.Title = "<" + modelHost.ChildViews.Count.ToString() + " Independent Views>";
                return true;
            }
        }

        public void CopyElementsBetweenSheets(SheetCopierViewHost sheet)
        {
            IList<ElementId> list = new List<ElementId>();
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OwnedByView(sheet.SourceSheet.Id);
                foreach (Element e in collector)
                {
                    if (!(e is Viewport))
                    {
                        if (e is CurveElement)
                        {
                            continue;
                        }
                        if (e.IsValidObject && e.ViewSpecific)
                        {
                            list.Add(e.Id);
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                Transform transform;
                CopyPasteOptions options;
                ElementTransformUtils.CopyElements(
                    sheet.SourceSheet,
                    list,
                    sheet.DestinationSheet,
                    transform = new Transform(ElementTransformUtils.GetTransformFromViewToView(sheet.SourceSheet, sheet.DestinationSheet)),
                    options = new CopyPasteOptions());
                if (Settings.Default.DeleteRevisionClouds)
                {
                    DeleteRevisionClouds(sheet.DestinationSheet.Id, doc);
                }
                options.Dispose();
                transform.Dispose();
            }
        }

        // this is where the action happens
        public bool CreateAndPopulateNewSheet(SheetCopierViewHost host, StringBuilder summaryText)
        {
            if (host == null)
            {
                return false;
            }

            // Run this is there is no host sheet
            // i.e. individual views are selected.
            if (host.Type == ViewHostType.Model)
            {
                CreateViews(host, summaryText);
                return true;
            }

            // turn on hidden revisions, if option set
            if (Settings.Default.DeleteRevisionClouds)
            {
                foreach (Revision rev in hiddenRevisionClouds)
                {
                    try
                    {
                        rev.Visibility = RevisionVisibility.CloudAndTagVisible;
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentOutOfRangeException ex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                    }
                }
            }

            try
            {
                host.DestinationSheet = AddEmptySheetToDocument(
                    host.Number,
                    host.Title,
                    host.PrimaryCustomSheetParameter,
                    host.SecondaryCustomSheetParameter,
                    host.TertiaryCustomSheetParameter);
            }
            catch (Exception ex)
            {
                SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                SCaddinsApp.WindowManager.ShowMessageBox(ex.StackTrace);
            }

            if (host.DestinationSheet != null)
            {
                CreateViews(host, summaryText);
            }
            else
            {
                return false;
            }

            try
            {
                CopyElementsBetweenSheets(host);
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.Message);
            }

            if (Settings.Default.DeleteRevisionClouds)
            {
                foreach (Revision rev in hiddenRevisionClouds)
                {
                    rev.Visibility = RevisionVisibility.Hidden;
                }
            }

            var oldNumber = host.SourceSheet.SheetNumber;
            var msg = " Sheet: " + oldNumber + " copied to: " + host.Number;
            if (summaryText != null)
            {
                summaryText.Append(msg + Environment.NewLine);
            }

            return true;
        }

        public void CreateSheets()
        {
            if (ViewHosts.Count < 1)
            {
                return;
            }

            int sheetCount = 0;
            int viewCount = 0;
            View firstView = null;
            summaryText.Clear();

            using (var t = new Transaction(doc, "Copy Sheets"))
            {
                if (t.Start() == TransactionStatus.Started)
                {
                    foreach (SheetCopierViewHost viewHost in ViewHosts)
                    {
                        if (viewHost.Type == ViewHostType.Sheet)
                        {
                            sheetCount++;
                        }
                        if (viewHost.Type == ViewHostType.Model)
                        {
                            viewCount++;
                        }

                        if (CreateAndPopulateNewSheet(viewHost, summaryText) && sheetCount == 1)
                        {
                            firstView = viewHost.Type == ViewHostType.Sheet ? viewHost.DestinationSheet : null;
                        }
                    }
                    if (TransactionStatus.Committed != t.Commit())
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox("Copy Sheets Failure", "Transaction could not be committed");
                    }
                    else
                    {
                        // try to open the first view created
                        if (firstView != null)
                        {
                            var uiapp = new UIApplication(doc.Application);
                            uiapp.ActiveUIDocument.ActiveView = firstView;
                        }
                    }
                }
            }

            SCaddinsApp.WindowManager.ShowMessageBox("Copy Sheets - Summary", summaryText.ToString());
        }

        public void CreateViews(SheetCopierViewHost host, StringBuilder summaryText)
        {
            Dictionary<ElementId, XYZ> viewPorts = null;
            if (host.Type == ViewHostType.Sheet)
            {
                viewPorts = GetViewportDictionary(host.SourceSheet, doc);
            }

            foreach (SheetCopierView view in host.ChildViews)
            {
                if (host.Type == ViewHostType.Model)
                {
                    var id = DuplicateView(view);
                    var elem = doc.GetElement(id);
                    if (elem != null)
                    {
                        var v = elem as View;
                        string newName = host.GetNewViewName(view.OldView.Id);
                        if (newName != null)
                        {
                            v.Name = newName;
                            var msg = " View: " + view.OldView.Name + " copied to: " + newName;
                            if (summaryText != null)
                            {
                                summaryText.Append(msg + Environment.NewLine);
                            }
                        }
                        else
                        {
                            SCaddinsApp.WindowManager.ShowMessageBox("ERROR", "New view name could not be set to: " + newName);
                        }
                    }
                }
                else
                {
                    XYZ sourceViewPortCentre = null;
                    if (!viewPorts.TryGetValue(view.OldId, out sourceViewPortCentre))
                    {
                    }
                    else
                    {
                        switch (view.CreationMode)
                        {
                            case ViewPortPlacementMode.Copy:
                                DuplicateViewOntoSheet(view, host, sourceViewPortCentre);
                                break;

                            case ViewPortPlacementMode.New:
                                PlaceNewViewOnSheet(view, host, sourceViewPortCentre);
                                break;

                            case ViewPortPlacementMode.Legend:
                                PlaceViewPortOnSheet(host.DestinationSheet, view.OldView.Id, sourceViewPortCentre);
                                break;
                        }
                    }
                }
            }
        }

        public ElementId DuplicateView(SheetCopierView view)
        {
            var d = view.DuplicateWithDetailing ? ViewDuplicateOption.WithDetailing : ViewDuplicateOption.Duplicate;

            ElementId destViewId = ElementId.InvalidElementId;
            if (view.OldView.CanViewBeDuplicated(d))
            {
                try
                {
                    destViewId = view.OldView.Duplicate(d);
                }
                catch (Autodesk.Revit.Exceptions.ArgumentOutOfRangeException arx)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox(arx.Message);
                    return ElementId.InvalidElementId;
                }
                catch (Autodesk.Revit.Exceptions.InvalidOperationException iox)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox(iox.Message);
                    return ElementId.InvalidElementId;
                }
            }
            else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("WARNING: CanViewBeDuplicated is returning false for view: " + view.OldView.Name);
                return ElementId.InvalidElementId;
            }

            if (destViewId == ElementId.InvalidElementId)
            {
                SCaddinsApp.WindowManager.ShowMessageBox("WARNING: could not create copy of view: " + view.OldView.Name);
                //// sometimes view.Duplicate seems to fail if the duplicate option is set to ViewDuplicateOption.WithDetailing
                //// try again with option set to ViewDuplicateOption.Duplicate
                if (d == ViewDuplicateOption.WithDetailing)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("Attempting to create view without detailing..." + view.OldView.Name);
                    view.DuplicateWithDetailing = false;
                    return DuplicateView(view);
                }
            }
            return destViewId;
        }

        public void DuplicateViewOntoSheet(
           SheetCopierView view,
           SheetCopierViewHost sheet,
           XYZ sourceViewCentre)
        {
            var destViewId = DuplicateView(view);

            if (Settings.Default.DeleteRevisionClouds)
            {
                DeleteRevisionClouds(destViewId, doc);
            }

            var elem = doc.GetElement(destViewId);
            if (elem == null)
            {
                return;
            }
            var v = elem as View;

            string newName = sheet.GetNewViewName(view.OldView.Id);

            if (newName != null)
            {
                v.Name = newName;
            }
            else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("ERROR", "New view name could not be set to: " + newName);
            }

            TryAssignViewTemplate(v, view.ViewTemplateName, view.OldView.ViewTemplateId);

            PlaceViewPortOnSheet(sheet.DestinationSheet, destViewId, sourceViewCentre);
        }

        public SheetCopierViewHost GetFirstModelHost()
        {
            var modelHost = ViewHosts.Where(s => s.Type == ViewHostType.Model);
            if (modelHost.Count() > 0)
            {
                return modelHost.First();
            }
            else
            {
                var newModelHost = new SheetCopierViewHost(this);
                ViewHosts.Add(newModelHost);
                return newModelHost;
            }
        }

        public string GetNewSheetNumber(string originalNumber)
        {
            if (string.IsNullOrEmpty(originalNumber))
            {
                return null;
            }
            int inc = 0;
            do
            {
                inc++;
            } while (!SheetNumberAvailable(originalNumber + "-" + inc.ToString(CultureInfo.InvariantCulture)));
            return originalNumber + "-" + inc.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Place a newly created view on a sheet.
        /// User this when copying a sheet AND assigning a new associated level.
        /// </summary>
        public void PlaceNewViewOnSheet(
            SheetCopierView view,
            SheetCopierViewHost sheet,
            XYZ sourceViewCentre)
        {
            Level level = null;
                try
                {
                    Levels.TryGetValue(view.AssociatedLevelName, out level);
                }
                catch
                {
                    SCaddinsApp.WindowManager.ShowErrorMessageBox("Error", "level not found");
                    return;
                }

                if (level != null)
                {
                    var floorPlanViewFamilyTypeId = GetFloorPlanViewFamilyTypeId(Doc, view.OldView.ViewType);
                    if (Autodesk.Revit.DB.ElementId.InvalidElementId == floorPlanViewFamilyTypeId)
                    {
                        SCaddinsApp.WindowManager.ShowErrorMessageBox("Error getting viewtype", "Error getting viewtype");
                        return;
                    }

                    using (ViewPlan vp = ViewPlan.Create(doc, floorPlanViewFamilyTypeId, level.Id))
                    {
                        try
                        {
                            vp.CropBox = view.OldView.CropBox;
                            vp.CropBoxActive = view.OldView.CropBoxActive;
                            vp.CropBoxVisible = view.OldView.CropBoxVisible;
                            vp.Name = view.Title;
                            var oldAnno = view.OldView.AreAnnotationCategoriesHidden;
                            vp.AreAnnotationCategoriesHidden = true;
                            TryAssignViewTemplate(vp, view.ViewTemplateName, view.OldView.ViewTemplateId);
                            PlaceViewPortOnSheet(sheet.DestinationSheet, vp.Id, sourceViewCentre);
                            vp.AreAnnotationCategoriesHidden = oldAnno;
                        }
                        catch (Exception ex)
                        {
                            SCaddinsApp.WindowManager.ShowErrorMessageBox("Error in PlaceNewViewOnSheet", ex.Message);
                        }
                    }
                }
                else
                {
                    SCaddinsApp.WindowManager.ShowErrorMessageBox("Error", "level not found");
                }
        }

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void PlaceViewPortOnSheet(
            Element destSheet, ElementId destViewId, XYZ viewCentre)
        {
            try
            {
                Viewport.Create(doc, destSheet.Id, destViewId, viewCentre);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
            }
            catch (Autodesk.Revit.Exceptions.ForbiddenForDynamicUpdateException fex)
            {
                SCaddinsApp.WindowManager.ShowMessageBox(fex.Message);
            }
            catch (Autodesk.Revit.Exceptions.ModificationForbiddenException mex)
            {
                SCaddinsApp.WindowManager.ShowMessageBox(mex.Message);
            }
        }

        public bool SheetNumberAvailable(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return false;
            }
            foreach (SheetCopierViewHost s in ViewHosts)
            {
                if (s.Number.ToUpper(CultureInfo.InvariantCulture).Equals(number.ToUpper(CultureInfo.InvariantCulture), StringComparison.InvariantCulture))
                {
                    return false;
                }
            }
            return !ExistingSheets.ContainsKey(number);
        }

        public void TryAssignViewTemplate(View view, string templateName, ElementId oldViewTemplateId)
        {
            // Try to copy the view  template, if one is assigned to the host view.
            if (templateName == SheetCopierConstants.MenuItemCopy && oldViewTemplateId != ElementId.InvalidElementId)              
            {         
                try
                {
                    view.ViewTemplateId = oldViewTemplateId;
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException ex)
                {
                    SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                }
                return;
            }

            // try to assign a view template to the new view.
            if (templateName != SheetCopierConstants.MenuItemCopy)
            {
                View vt = null;
                if (ViewTemplates.TryGetValue(templateName, out vt))
                {
                    try
                    {
                        view.ViewTemplateId = vt.Id;
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentException ex)
                    {
                        SCaddinsApp.WindowManager.ShowMessageBox(ex.Message);
                    }
                }
                else
                {
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
            foreach (SheetCopierViewHost s in ViewHosts)
            {
                foreach (SheetCopierView v in s.ChildViews)
                {
                    if (v.Title.ToUpper(CultureInfo.InvariantCulture).Equals(title.ToUpper(CultureInfo.InvariantCulture), StringComparison.CurrentCulture))
                    {
                        return false;
                    }
                }
            }
            return !ExistingViews.ContainsKey(title);
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
            Levels.Clear();
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfClass(typeof(Level));
                foreach (Element element in collector)
                {
                    Levels.Add(element.Name.ToString(CultureInfo.CurrentCulture), element as Level);
                }
            }
        }

        private List<string> GetAllParameterValuesInModel(string parameterName)
        {
            var result = new List<string>();
            result.Add(@"<None>");
            using (var collector = new FilteredElementCollector(doc))
            {
                collector.OfCategory(BuiltInCategory.OST_Sheets);
                foreach (var element in collector)
                {
                    var view = (View)element;
                    var sheetParameters = view.GetParameters(parameterName);
                    if (sheetParameters == null || sheetParameters.Count <= 0)
                    {
                        continue;
                    }
                    var sheetParameter = sheetParameters.First();
                    var s = sheetParameter.AsString();
                    if (!string.IsNullOrEmpty(s) && !result.Contains(s))
                    {
                        result.Add(s);
                    }
                }
            }
            return result;
        }

        private void GetViewTemplates()
        {
            ViewTemplates.Clear();
            using (var c = new FilteredElementCollector(doc))
            {
                c.OfCategory(BuiltInCategory.OST_Views);
                foreach (var element in c)
                {
                    var view = (View)element;
                    if (view.IsTemplate)
                    {
                        ViewTemplates.Add(view.Name, view);
                    }
                }
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
