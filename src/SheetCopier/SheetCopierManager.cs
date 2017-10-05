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
        private Document doc;
        private UIDocument uidoc;
        private System.ComponentModel.BindingList<SheetCopierSheet> sheets;
        private Dictionary<string, View> existingSheets =
            new Dictionary<string, View>();
        
        private Dictionary<string, View> existingViews =
            new Dictionary<string, View>();
        
        private Dictionary<string, View> viewTemplates =
            new Dictionary<string, View>();
        
        private Dictionary<string, Level> levels =
            new Dictionary<string, Level>();
        
        private Collection<string> sheetCategories = 
            new Collection<string>();
        
        private StringBuilder summaryText; 
        private List<Revision> hiddenRevisionClouds = new List<Revision>();
        private ElementId floorPlanViewFamilyTypeId = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public SheetCopierManager(UIDocument uidoc)
        {
            this.summaryText = new StringBuilder();
            this.doc = uidoc.Document;
            this.uidoc = uidoc;
            this.sheets = new System.ComponentModel.BindingList<SheetCopierSheet>();
            this.hiddenRevisionClouds = GetAllHiddenRevisions(this.doc);
            this.GetViewTemplates();
            GetAllSheets(existingSheets, doc);
            this.GetAllLevelsInModel();
            GetAllViewsInModel(existingViews, doc);
            this.GetFloorPlanViewFamilyTypeId();
            this.GetAllSheetCategories();
        }
               
        #region properties

        public System.ComponentModel.BindingList<SheetCopierSheet> Sheets {
            get {
                return this.sheets;
            }
        }

        public Dictionary<string, View> ViewTemplates {
            get {
                return this.viewTemplates;
            }
        }

        public Dictionary<string, Level> Levels {
            get {
                return this.levels;
            }
        }
    
        public Dictionary<string, View> ExistingViews {
            get {
                return this.existingViews;
            }
        }
        
        public Collection<string> SheetCategories {
            get {
                return this.sheetCategories;
            }    
        }
    
        #endregion

        #region public methods

        public static ViewSheet ViewToViewSheet(View view)
        {
            if (view == null) {
                return null;
            }
            return (view.ViewType != ViewType.DrawingSheet) ? null : view as ViewSheet;
        }
                  
        public bool SheetNumberAvailable(string number)
        {
            if (string.IsNullOrEmpty(number)) {
                return false;
            }
            foreach (SheetCopierSheet s in this.sheets) {
                if (s.Number.ToUpper(CultureInfo.InvariantCulture).Equals(number.ToUpper(CultureInfo.InvariantCulture))) {
                    return false;
                }
            }
            return !this.existingSheets.ContainsKey(number);
        }

        public bool ViewNameAvailable(string title)
        {
            if (string.IsNullOrEmpty(title)) {
                return false;
            }
            foreach (SheetCopierSheet s in this.sheets) {
                foreach (SheetCopierViewOnSheet v in s.ViewsOnSheet) {
                    if (v.Title.ToUpper(CultureInfo.InvariantCulture).Equals(title.ToUpper(CultureInfo.InvariantCulture))) {
                        return false;
                    }
                }
            }
            return !this.existingViews.ContainsKey(title);
        }

        public void CreateSheets()
        {
            if (sheets.Count < 1) {
                return;
            }
            
            int n = 0;
            View firstSheet = null;
            summaryText.Clear();
            
            using (var t = new Transaction(this.doc, "Copy Sheets")) {
                if (t.Start() == TransactionStatus.Started) {
                    foreach (SheetCopierSheet sheet in sheets) {
                        n++;
                        if (CreateAndPopulateNewSheet(sheet, summaryText) && n == 1) {
                            firstSheet = sheet.DestinationSheet;
                        }
                    }
                    if (TransactionStatus.Committed != t.Commit()) {
                        TaskDialog.Show("Copy Sheets Failure", "Transaction could not be committed");
                    } else {
                        // try to open the first sheet created
                        if (firstSheet != null) {
                            var uiapp = new UIApplication(doc.Application);
                            uiapp.ActiveUIDocument.ActiveView = firstSheet;
                            
                        }
                    }
                }
            }

            using (var td = new TaskDialog("Copy Sheets - Summary")) {
                td.MainInstruction = "Copy Sheets - Summary";
                td.MainContent = summaryText.ToString();
                td.MainIcon = TaskDialogIcon.TaskDialogIconNone;
                td.Show();
            }
        }
    
        public void AddSheet(ViewSheet sourceSheet)
        {
            if (sourceSheet != null) {
                string n = this.GetNewSheetNumber(sourceSheet.SheetNumber);
                string t = sourceSheet.Name + SheetCopierConstants.MenuItemCopy;
                this.sheets.Add(new SheetCopierSheet(n, t, this, sourceSheet));
            }

            // FIXME add error message,
        }
        
        #endregion

        #region private methods
                              
        private static Dictionary<ElementId, XYZ> GetVPDictionary(
            ViewSheet srcSheet, Document doc)
        {
            var result = new Dictionary<ElementId, XYZ>();
            foreach (ElementId viewPortId in srcSheet.GetAllViewports()) {
                var viewPort = (Viewport)doc.GetElement(viewPortId);
                var viewPortCentre = viewPort.GetBoxCenter();
                result.Add(
                    viewPort.ViewId, viewPortCentre);
            }
            return result;
        }

        private void GetViewTemplates()
        {
            this.viewTemplates.Clear();
            using (var c = new FilteredElementCollector(this.doc)) {
                c.OfCategory(BuiltInCategory.OST_Views);
                foreach (View view in c) {
                    if (view.IsTemplate) {
                        this.viewTemplates.Add(view.Name, view);
                    }
                }
            }
        }
          
        private void GetAllSheetCategories()
        {
            this.sheetCategories.Clear();
            using (var c1 = new FilteredElementCollector(this.doc)) {
                c1.OfCategory(BuiltInCategory.OST_Sheets);
                foreach (View view in c1) {
                    var viewCategoryParamList = view.GetParameters(SheetCopierConstants.SheetCategory);
                    if (viewCategoryParamList != null && viewCategoryParamList.Count > 0) {
                        Parameter viewCategoryParam = viewCategoryParamList.First();
                        string s = viewCategoryParam.AsString();
                        if (!string.IsNullOrEmpty(s) && !this.sheetCategories.Contains(s)) {
                            this.sheetCategories.Add(s);
                        }
                    }
                }
            }
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
                foreach (View view in c1) {
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
                collector.OfClass(typeof(Autodesk.Revit.DB.View));
                foreach (Element element in collector) {
                    var view = element as View;
                    View tmpView;
                    if (!existingViews.TryGetValue(view.Name, out tmpView)) {
                        existingViews.Add(view.Name, view);
                    }
                }
            }
        }

        // this is where the action happens
        public bool CreateAndPopulateNewSheet(SheetCopierSheet sheet, StringBuilder summary)
        { 
            if (sheet == null) {
                return false;
            }

            // turn on hidden revisions
            foreach (Revision rev in this.hiddenRevisionClouds) {
                rev.Visibility = RevisionVisibility.CloudAndTagVisible;
            }
            
            sheet.DestinationSheet = this.AddEmptySheetToDocument(
                sheet.Number,
                sheet.Title,
                sheet.SheetCategory);
 
            if (sheet.DestinationSheet != null) {
                Debug.WriteLine(sheet.Number + " added to document.");
                this.CreateViewports(sheet);
            } else {
                return false;
            }
            
            try {
                this.CopyElementsBetweenSheets(sheet);
            } catch (InvalidOperationException e) {
                Debug.WriteLine(e.Message);
            } 
            
            foreach (Revision rev in this.hiddenRevisionClouds) {
                rev.Visibility = RevisionVisibility.Hidden;
            }

            var oldNumber = sheet.SourceSheet.SheetNumber;
            var msg = " Sheet: " + oldNumber + " copied to: " + sheet.Number;
            if (summary != null) {
                summary.Append(msg + System.Environment.NewLine);
            }

            return true;
        }

        private void GetFloorPlanViewFamilyTypeId() {
            using (var collector = new FilteredElementCollector(this.doc)) {
                collector.OfClass(typeof(ViewFamilyType));
                foreach (ViewFamilyType vft in collector) {
                    if (vft.ViewFamily == ViewFamily.FloorPlan) {
                        this.floorPlanViewFamilyTypeId = vft.Id;
                    }
                }
            }
        }

        private void GetAllLevelsInModel() {
            this.levels.Clear();
            using (var collector = new FilteredElementCollector(this.doc)) {
                collector.OfClass(typeof(Level));
                foreach (Element element in collector) {
                    this.levels.Add(element.Name.ToString(), element as Level);
                }
            }
        }

        // add an empty sheet to the doc.
        // this comes first before copying titleblock, views etc.
        private ViewSheet AddEmptySheetToDocument(
            string sheetNumber,
            string sheetTitle,
            string viewCategory)
        {
            var result = ViewSheet.Create(this.doc, ElementId.InvalidElementId);
                result.Name = sheetTitle;
                result.SheetNumber = sheetNumber;
                var viewCategoryParamList = result.GetParameters(SheetCopierConstants.SheetCategory);
                if (viewCategoryParamList.Count > 0) {
                    Parameter viewCategoryParam = viewCategoryParamList.First();
                    viewCategoryParam.Set(viewCategory);
                }
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void PlaceViewPortOnSheet(
            Element destSheet, ElementId destViewId, XYZ viewCentre)
        {
            Viewport.Create(this.doc, destSheet.Id, destViewId, viewCentre);
        }

        private string GetNewSheetNumber(string originalNumber)
        {
            int inc = 0;
            do {
                inc++;
            } while (!this.SheetNumberAvailable(originalNumber + "-" + inc.ToString(CultureInfo.InvariantCulture)));
            return originalNumber + "-" + inc.ToString(CultureInfo.InvariantCulture);
        }
        
        private void TryAssignViewTemplate(View view, string templateName)
        {
            if (templateName != SheetCopierConstants.MenuItemCopy) {
                View vt = null;
                if (this.viewTemplates.TryGetValue(templateName, out vt)) {
                    view.ViewTemplateId = vt.Id;
                }
            }   
        }
               
        private void PlaceNewViewOnSheet(
            SheetCopierViewOnSheet view,
            SheetCopierSheet sheet,
            XYZ sourceViewCentre)
        {
            Level level = null;
            this.levels.TryGetValue(view.AssociatedLevelName, out level);
            if (level != null) {
                using (ViewPlan vp = ViewPlan.Create(this.doc, this.floorPlanViewFamilyTypeId, level.Id)) {
                    vp.CropBox = view.OldView.CropBox;
                    vp.CropBoxActive = view.OldView.CropBoxActive;
                    vp.CropBoxVisible = view.OldView.CropBoxVisible;
                    this.TryAssignViewTemplate(vp, view.ViewTemplateName);
                    this.PlaceViewPortOnSheet(sheet.DestinationSheet, vp.Id, sourceViewCentre);
                }
            }
            level.Dispose();
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
        
        private void DuplicateViewOntoSheet(
            SheetCopierViewOnSheet view,
            SheetCopierSheet sheet,
            XYZ sourceViewCentre)
        {
            var d = view.DuplicateWithDetailing == true ? ViewDuplicateOption.WithDetailing : ViewDuplicateOption.Duplicate;          
            ElementId destViewId = view.OldView.Duplicate(d);
            DeleteRevisionClouds(destViewId, this.doc);
            string newName = sheet.GetNewViewName(view.OldView.Id);
            var v = this.doc.GetElement(destViewId) as View;
            if (newName != null) {
                v.Name = newName;
                var dv = this.doc.GetElement(destViewId) as View;  
                this.TryAssignViewTemplate(dv, view.ViewTemplateName);                
            }
            this.PlaceViewPortOnSheet(sheet.DestinationSheet, destViewId, sourceViewCentre);
        }

        private void CopyElementsBetweenSheets(SheetCopierSheet sheet)
        {
            IList<ElementId> list = new List<ElementId>();
            using (var collector = new FilteredElementCollector(this.doc)) {
                collector.OwnedByView(sheet.SourceSheet.Id);
                foreach (Element e in collector) {
                    if (!(e is Viewport)) {
                        // Debug.WriteLine("adding " + e.GetType().ToString() + " to copy list(CopyElementsBetweenSheets).");
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
                DeleteRevisionClouds(sheet.DestinationSheet.Id, this.doc);
                options.Dispose();
                transform.Dispose();
            }
        }
             
        private void CreateViewports(SheetCopierSheet sheet)
        {
            Dictionary<ElementId, XYZ> viewPorts =
                SheetCopierManager.GetVPDictionary(sheet.SourceSheet, this.doc);

            foreach (SheetCopierViewOnSheet view in sheet.ViewsOnSheet) {
                XYZ sourceViewPortCentre = null;
                if (!viewPorts.TryGetValue(view.OldId, out sourceViewPortCentre)) {
                    TaskDialog.Show("SCopy", "Error...");
                    continue;
                }
                             
                switch (view.CreationMode) {
                    case ViewPortPlacementMode.Copy:
                        this.DuplicateViewOntoSheet(view, sheet, sourceViewPortCentre);
                        break;
                    case ViewPortPlacementMode.New:
                        this.PlaceNewViewOnSheet(view, sheet, sourceViewPortCentre);
                        break;     
                    case ViewPortPlacementMode.Legend:
                        this.PlaceViewPortOnSheet(sheet.DestinationSheet, view.OldView.Id, sourceViewPortCentre);
                        break;                 
                }
            }       
        }
        #endregion
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
