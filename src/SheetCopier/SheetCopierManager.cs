
//test// (C) Copyright 2014-2016 by Andrew Nicholas
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
    
        #if !REVIT2014        
        private List<Revision> hiddenRevisionClouds = new List<Revision>();
        #endif
        
        private ElementId floorPlanViewFamilyTypeId = null;
           
        public SheetCopierManager(UIDocument uidoc)
        {
            this.doc = uidoc.Document;
            this.uidoc = uidoc;
            this.sheets = new System.ComponentModel.BindingList<SheetCopierSheet>();
            #if !REVIT2014   
            this.hiddenRevisionClouds = getAllHiddenRevisions(this.doc);
            #endif
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
            return (view.ViewType != ViewType.DrawingSheet) ? null : view as ViewSheet;
        }
                  
        public bool SheetNumberAvailable(string number)
        {
            foreach (SheetCopierSheet s in this.sheets) {
                if (s.Number.ToUpper(CultureInfo.InvariantCulture).Equals(number.ToUpper(CultureInfo.InvariantCulture))) {
                    return false;
                }
            }
            return !this.existingSheets.ContainsKey(number);
        }

        public bool ViewNameAvailable(string title)
        {
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
            if (this.sheets.Count < 1) {
                return;
            }
            
            int n = 0;
            string firstSheetNumber = string.Empty;
            string summaryText = string.Empty;
            
            using (Transaction  t = new Transaction(this.doc, "SCopy")) {
                t.Start("Copy Sheets");
                foreach (SheetCopierSheet sheet in this.sheets) {
                    n++;
                    if(this.CreateAndPopulateNewSheet(sheet, ref summaryText) && n == 1) {
                        firstSheetNumber = sheet.DestinationSheet.SheetNumber;
                    }
                }
                t.Commit();
            }
            
            //open first sheet
            if (!string.IsNullOrEmpty(firstSheetNumber)) {
                uidoc.ShowElements(SCaddins.ExportManager.ExportManager.TitleBlockInstanceFromSheetNumber(firstSheetNumber, doc));  
            }
            
            if(uidoc.GetOpenUIViews().Count > 0) {
                (uidoc.GetOpenUIViews())[0].ZoomToFit();
            }
            
            var td = new TaskDialog("SCopy - Summary");
            td.MainInstruction = "SCopy - Summary";
            td.MainContent = summaryText;
            td.MainIcon = TaskDialogIcon.TaskDialogIconNone;
            td.Show(); 
        }
    
        public void AddSheet(ViewSheet sourceSheet)
        {
            string n = this.GetNewSheetNumber(sourceSheet.SheetNumber);
            string t = sourceSheet.Name + SheetCopierConstants.MenuItemCopy;
            this.sheets.Add(new SheetCopierSheet(n, t, this, sourceSheet));
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
            var c = new FilteredElementCollector(this.doc).OfCategory(BuiltInCategory.OST_Views);
            foreach (View view in c) {
                if (view.IsTemplate) {
                    this.viewTemplates.Add(view.Name, view);
                }
            }
        }
          
        private void GetAllSheetCategories()
        {
            this.sheetCategories.Clear();
            var c1 = new FilteredElementCollector(this.doc).OfCategory(BuiltInCategory.OST_Sheets);
            foreach (View view in c1) {
                #if ( REVIT2015 || REVIT2016 || REVIT2017 )
                var viewCategoryParamList = view.GetParameters(SheetCopierConstants.SheetCategory);
                if (viewCategoryParamList != null && viewCategoryParamList.Count > 0) {
                    Parameter viewCategoryParam = viewCategoryParamList.First();
                    string s = viewCategoryParam.AsString();
                    if (!string.IsNullOrEmpty(s) && !this.sheetCategories.Contains(s)) {
                        this.sheetCategories.Add(s);
                    }
                } 
                #else
                var viewCategoryParam = view.get_Parameter(SheetCopierConstants.SheetCategory);
                if (viewCategoryParam != null) {
                    string s = viewCategoryParam.AsString();
                    if (!string.IsNullOrEmpty(s) && !this.sheetCategories.Contains(s)) {
                        this.sheetCategories.Add(s);
                    }
                } 
                #endif
            }
        }

        public static void GetAllSheets(Dictionary<string, View> existingSheets, Document doc)
        {
            existingSheets.Clear();
            var c1 = new FilteredElementCollector(doc);
            c1.OfCategory(BuiltInCategory.OST_Sheets);
            foreach (View view in c1) {
                var vs = view as ViewSheet;
                existingSheets.Add(vs.SheetNumber, view);
            }
        }

        private void GetFloorPlanViewFamilyTypeId()
        {
            foreach (ViewFamilyType vft in new FilteredElementCollector(this.doc).OfClass(typeof(ViewFamilyType))) {
                if (vft.ViewFamily == ViewFamily.FloorPlan) {
                    this.floorPlanViewFamilyTypeId = vft.Id;
                }
            }
        }
        
        public static void GetAllViewsInModel(Dictionary<string, View> existingViews, Document doc)
        {
            existingViews.Clear();
            FilteredElementCollector c = new FilteredElementCollector(doc).OfClass(typeof(Autodesk.Revit.DB.View));
            foreach (Element element in c) {
                var view = element as View;
                View tmpView;
                if (!existingViews.TryGetValue(view.Name, out tmpView)) {
                    existingViews.Add(view.Name, view);
                }
            }
        }

        private void GetAllLevelsInModel()
        {
            this.levels.Clear();
            var c3 = new FilteredElementCollector(this.doc).OfClass(typeof(Level));
            foreach (Element element in c3) {
                this.levels.Add(element.Name.ToString(), element as Level);
            }
        }

        // this is where the action happens
        public bool CreateAndPopulateNewSheet(SheetCopierSheet sheet, ref string summary)
        { 
            //turn on hidden revisions
            #if !REVIT2014
            foreach (Revision rev in this.hiddenRevisionClouds) {
                rev.Visibility = RevisionVisibility.CloudAndTagVisible;
            }
            #endif
            
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
            
            //re-hide hidden revision clouds
            #if !REVIT2014
            foreach (Revision rev in this.hiddenRevisionClouds) {
                rev.Visibility = RevisionVisibility.Hidden;
            }
            #endif

            var oldNumber = sheet.SourceSheet.SheetNumber;
            var msg = " Sheet: " + oldNumber + " copied to: " + sheet.Number;
            summary += msg + System.Environment.NewLine;

            return true;
        }
    
        // add an empty sheet to the doc.
        // this comes first before copying titleblock, views etc.
        private ViewSheet AddEmptySheetToDocument(
            string sheetNumber,
            string sheetTitle,
            string viewCategory)
        {
            ViewSheet result;
            result = ViewSheet.Create(this.doc, ElementId.InvalidElementId);           
            result.Name = sheetTitle;
            result.SheetNumber = sheetNumber;
            #if ( REVIT2015 || REVIT2016 || REVIT2017 )
            var viewCategoryParamList = result.GetParameters(SheetCopierConstants.SheetCategory);
            if (viewCategoryParamList.Count > 0) {
                Parameter viewCategoryParam = viewCategoryParamList.First();
                viewCategoryParam.Set(viewCategory);
            }
            #else
            var s = result.get_Parameter(SheetCopierConstants.SheetCategory);
            if (s != null) {
                s.Set(viewCategory);
            }
            #endif
            return result;
        }
        
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
                ViewPlan vp = ViewPlan.Create(this.doc, this.floorPlanViewFamilyTypeId, level.Id);
                vp.CropBox = view.OldView.CropBox;
                vp.CropBoxActive = view.OldView.CropBoxActive;
                vp.CropBoxVisible = view.OldView.CropBoxVisible;
                this.TryAssignViewTemplate(vp, view.ViewTemplateName);
                this.PlaceViewPortOnSheet(sheet.DestinationSheet, vp.Id, sourceViewCentre);
            }
        }
        
        #if !REVIT2014
        public static List<Revision> getAllHiddenRevisions(Document doc)
        {
            var revisions = new List<Revision>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);    
            collector.OfCategory(BuiltInCategory.OST_Revisions);
            foreach (Element e in collector) {
                Revision rev = e as Revision;
                if (rev.Visibility == RevisionVisibility.Hidden) {
                    revisions.Add(rev);
                }
            }
            return revisions;
        }
        #endif
        
        public static void deleteRevisionClouds(ElementId viewId, Document doc)
        {            
            FilteredElementCollector collector = new FilteredElementCollector(doc, viewId);
            collector.OfCategory(BuiltInCategory.OST_RevisionClouds);
            var clouds = new List<ElementId>();
            foreach (Element e in collector) {
                clouds.Add(e.Id);
            }
            doc.Delete(clouds);          
        }
        
        private void DuplicateViewOntoSheet(
            SheetCopierViewOnSheet view,
            SheetCopierSheet sheet,
            XYZ sourceViewCentre)
        {
            var d = view.DuplicateWithDetailing == true ? ViewDuplicateOption.WithDetailing : ViewDuplicateOption.Duplicate;          
            ElementId destViewId = view.OldView.Duplicate(d);
            deleteRevisionClouds(destViewId, this.doc);
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
            foreach (Element e in new FilteredElementCollector(this.doc).OwnedByView(sheet.SourceSheet.Id)) {
                if (!(e is Viewport)) {
                    Debug.WriteLine("adding " + e.GetType().ToString() + " to copy list(CopyElementsBetweenSheets).");
                    if (e is CurveElement) {
                        continue;
                    }
                    if (e.IsValidObject && e.ViewSpecific) {
                        list.Add(e.Id);
                    }
                }
            }             
            if (list.Count > 0) {
                Debug.WriteLine("Beggining element copy");
                ElementTransformUtils.CopyElements(
                    sheet.SourceSheet,
                    list,
                    sheet.DestinationSheet,
                    new Transform(ElementTransformUtils.GetTransformFromViewToView(sheet.SourceSheet, sheet.DestinationSheet)),
                    new CopyPasteOptions());
                deleteRevisionClouds(sheet.DestinationSheet.Id, this.doc);
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
