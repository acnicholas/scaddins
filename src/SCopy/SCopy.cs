// (C) Copyright 2014-2015 by Andrew Nicholas
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

namespace SCaddins.SCopy
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    
    public class SCopy
    {
        private FamilyInstance sourceTitleBlock;
        private Document doc;
        private System.ComponentModel.BindingList<SCopySheet> sheets;
        private Dictionary<string, View> existingSheets =
            new Dictionary<string, View>();
        
        private Dictionary<string, View> existingViews =
            new Dictionary<string, View>();
        
        private Dictionary<string, View> viewTemplates =
            new Dictionary<string, View>();
        
        private Dictionary<string, Level> levels =
            new Dictionary<string, Level>();
        
        private List<string> sheetCategories = 
            new List<string>();
        
        private ElementId floorPlanViewFamilyTypeId = null;
           
        public SCopy(Document doc)
        {
            this.doc = doc;
            this.sheets = new System.ComponentModel.BindingList<SCopySheet>();
            this.GetViewTemplates();
            this.GetAllSheets();
            this.GetAllLevelsInModel();
            this.GetAllViewsInModel();
            this.GetFloorPlanViewFamilyTypeId();
            this.GetAllSheetCategories();
        }
               
        public enum ViewCreationMode
        {
            Copy,
            CopyAndModify,
            Replace,
            Place
        }
    
        #region properties

        public System.ComponentModel.BindingList<SCopySheet> Sheets {
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
        
        public List<string> SheetCategories {
            get {
                return this.sheetCategories;
            }    
        }
    
        #endregion

        #region public methods
        /// <summary>
        /// Cast a view to a viewsheet
        /// </summary>
        /// <param name="view">A Revit View</param>
        /// <returns>A Revit ViewSHeet, or null if the source view is not a ViewSheet</returns>
        public static ViewSheet ViewToViewSheet(View view)
        {
            return (view.ViewType != ViewType.DrawingSheet) ? null : view as ViewSheet;
        }
                  
        public bool CheckSheetNumberAvailability(string number)
        {
            foreach (SCopySheet s in this.sheets) {
                if (s.Number.ToUpper(CultureInfo.InvariantCulture).Equals(number.ToUpper(CultureInfo.InvariantCulture))) {
                    return false;
                }
            }
            return !this.existingSheets.ContainsKey(number);
        }

        public bool CheckViewNameAvailability(string title)
        {
            foreach (SCopySheet s in this.sheets) {
                foreach (SCopyViewOnSheet v in s.ViewsOnSheet) {
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
            var t = new Transaction(this.doc, "SCopy");
            t.Start();
            string summaryText = string.Empty;
            foreach (SCopySheet sheet in this.sheets) {
                this.CreateAndPopulateNewSheet(sheet, ref summaryText);
            }
            t.Commit();
            var td = new TaskDialog("SCopy - Summary");
            td.MainInstruction = "SCopy - Summary";
            td.MainContent = summaryText;
            td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            td.Show();           
        }
    
        public void AddSheet(ViewSheet sourceSheet)
        {
            string n = this.NextSheetNumber(sourceSheet.SheetNumber);
            string t = sourceSheet.Name + SCopyConstants.MenuItemCopy;
            this.sheets.Add(new SCopySheet(n, t, this, sourceSheet));
        }
        
        #endregion

        #region private methods
         
        private static XYZ ViewCenterFromTBBottomLeft(BoundingBoxXYZ viewBounds)
        {
            XYZ xyzPosition = (viewBounds.Max + viewBounds.Min) / 2.0;
            return xyzPosition;
        }
                      
        private static Dictionary<ElementId, BoundingBoxXYZ> GetVPDictionary(
            ViewSheet srcSheet, Document doc)
        {
            var result = new Dictionary<ElementId, BoundingBoxXYZ>();
            foreach (ElementId viewPortId in srcSheet.GetAllViewports()) {
                var viewPort = (Viewport)doc.GetElement(viewPortId);
                var viewPortBounds = viewPort.get_BoundingBox(srcSheet);
                result.Add(
                    viewPort.ViewId, viewPortBounds);
            }
            return result;
        }
		        
        private void GetViewTemplates()
        {
            this.viewTemplates.Clear();
            FilteredElementCollector c = new FilteredElementCollector(this.doc);
            c.OfCategory(BuiltInCategory.OST_Views);
            foreach (View view in c) {
                if (view.IsTemplate) {
                    this.viewTemplates.Add(view.Name, view);
                }
            }
        }
          
        private void GetAllSheetCategories()
        {
            this.sheetCategories.Clear();
            FilteredElementCollector c1 = new FilteredElementCollector(this.doc);
            c1.OfCategory(BuiltInCategory.OST_Sheets);
            foreach (View view in c1) {
                #if REVIT2015
                var viewCategoryParamList = view.GetParameters(SCopyConstants.SheetCategory);
                if (viewCategoryParamList != null && viewCategoryParamList.Count > 0) {
                    Parameter viewCategoryParam = viewCategoryParamList.First();
                    string s = viewCategoryParam.AsString();
                    if (!string.IsNullOrEmpty(s) && !this.sheetCategories.Contains(s)) {
                        this.sheetCategories.Add(s);
                    }
                } 
                #else
                var viewCategoryParam = view.get_Parameter(SCopyConstants.SheetCategory);
                if (viewCategoryParam != null) {
                    string s = viewCategoryParam.AsString();
                    if (!string.IsNullOrEmpty(s) && !this.sheetCategories.Contains(s)) {
                        this.sheetCategories.Add(s);
                    }
                } 
                #endif
            }
        }

        private void GetAllSheets()
        {
            this.existingSheets.Clear();
            FilteredElementCollector c1 = new FilteredElementCollector(this.doc);
            c1.OfCategory(BuiltInCategory.OST_Sheets);
            foreach (View view in c1) {
                ViewSheet vs = view as ViewSheet;
                this.existingSheets.Add(vs.SheetNumber, view);
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
        
        private void GetAllViewsInModel()
        {
            this.existingViews.Clear();
            FilteredElementCollector c = new FilteredElementCollector(this.doc);
            c.OfClass(typeof(Autodesk.Revit.DB.View));
            foreach (View view in c) {
                View v = view as View;
                View vv;
                if (!this.existingViews.TryGetValue(v.Name, out vv)) {
                    this.existingViews.Add(v.Name, view);
                }
            }
        }

        private void GetAllLevelsInModel()
        {
            this.levels.Clear();
            FilteredElementCollector c3 = new FilteredElementCollector(this.doc);
            c3.OfClass(typeof(Level));
            foreach (Level l in c3) {
                this.levels.Add(l.Name.ToString(), l);
            }
        }

        // this is where the action happens
        private bool CreateAndPopulateNewSheet(SCopySheet sheet, ref string summary)
        {
            this.sourceTitleBlock = this.GetTitleBlock(sheet.SourceSheet);
            
            // don't do sheets without a title.            
            if (this.sourceTitleBlock == null) {
                TaskDialog.Show("SCopy", "No Title Block, exiting now...");
                return false;
            }

            // create the "blank canvas"
            ViewSheet destSheet = this.AddEmptySheetToDocument(
                             sheet.Number,
                             sheet.Title,
                             sheet.SheetCategory);
 
            sheet.DestinationSheet = destSheet;
            if (sheet.DestinationSheet != null) {
                this.PlaceViewportsOnSheet(sheet);
            }
            
            this.CopyElementsBetweenSheets(sheet);
    
            // create a log...
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
            #if REVIT2015
            var viewCategoryParamList = result.GetParameters(SCopyConstants.SheetCategory);
            if (viewCategoryParamList.Count > 0) {
                Parameter viewCategoryParam = viewCategoryParamList.First();
                viewCategoryParam.Set(viewCategory);
            }
            #else
            var s = result.get_Parameter(SCopyConstants.SheetCategory);
            if (s != null) {
                s.Set(viewCategory);
            }
            #endif
            return result;
        }
        
        // put an existing view on a sheet.
        // just for Legends at the moment.
        private void PlaceExistingViewOnSheet(
            ViewSheet destSheet, View viewToPlace, XYZ srcViewCentre)
        {
            double destViewMidX = srcViewCentre.X;
            double destViewMidY = srcViewCentre.Y;
            var destViewCentre = new XYZ(destViewMidX, destViewMidY, 0);
            if (viewToPlace.ViewType == ViewType.Legend) {
                Viewport.Create(doc, destSheet.Id, viewToPlace.Id, srcViewCentre);
            }

        }

        private void PlaceViewOnSheet(
            ViewSheet destSheet, ElementId destViewId, XYZ srcViewCentre)
        {
            double destViewMidX = srcViewCentre.X;
            double destViewMidY = srcViewCentre.Y;
            var destViewCentre = new XYZ(destViewMidX, destViewMidY, 0);
            Viewport.Create(this.doc, destSheet.Id, destViewId, destViewCentre);
        }

        private string NextSheetNumber(string s)
        {
            int inc = 0;
            do {
                inc++;
            } while (!this.CheckSheetNumberAvailability(s + "-" + inc.ToString(CultureInfo.InvariantCulture)));
            return s + "-" + inc.ToString(CultureInfo.InvariantCulture);
        }
        
        private void PlaceNewView(
            SCopyViewOnSheet view, SCopySheet sheet, XYZ sourceViewCentre)
        {
            Level level = null;
            this.levels.TryGetValue(view.AssociatedLevelName, out level);
            if (level != null) {
                ViewPlan vp = ViewPlan.Create(this.doc, this.floorPlanViewFamilyTypeId, level.Id);
                vp.CropBox = view.OldView.CropBox;
                vp.CropBoxActive = view.OldView.CropBoxActive;
                vp.CropBoxVisible = view.OldView.CropBoxVisible;
                if (view.ViewTemplateName != SCopyConstants.MenuItemCopy) {
                    View vt = null;
                    if (this.viewTemplates.TryGetValue(view.ViewTemplateName, out vt)) {
                        vp.ViewTemplateId = vt.Id;
                    }
                }
                this.PlaceViewOnSheet(sheet.DestinationSheet, vp.Id, sourceViewCentre);
            }
        }
                  
        private void CopyElementsBetweenSheets(SCopySheet sheet)
        {
            IList<ElementId> list = new List<ElementId>();
            foreach (Element e in new FilteredElementCollector(this.doc).OwnedByView(sheet.SourceSheet.Id)) {
                if ( !(e is Viewport) ){
                    list.Add(e.Id);
                }
            }
            if (list.Count > 0) {
                ElementTransformUtils.CopyElements(sheet.SourceSheet, list, sheet.DestinationSheet, null, null);
            }
        }
             
        private void PlaceViewportsOnSheet(SCopySheet sheet)
        {
            Dictionary<ElementId, BoundingBoxXYZ> viewPorts =
                SCopy.GetVPDictionary(sheet.SourceSheet, this.doc);

            foreach (SCopyViewOnSheet view in sheet.ViewsOnSheet) {
                BoundingBoxXYZ srcViewBounds = null;
                if (!viewPorts.TryGetValue(view.OldId, out srcViewBounds)) {
                    TaskDialog.Show("SCopy", "Error...");
                    continue;
                }
            
                XYZ sourceViewCentre = SCopy.ViewCenterFromTBBottomLeft(srcViewBounds);
              
                switch (view.CreationMode) {
                    case ViewCreationMode.Copy:
                        this.DuplicateViewOntoSheet(view, sheet, sourceViewCentre);
                        break;
                    case ViewCreationMode.CopyAndModify:
                        this.PlaceNewView(view, sheet, sourceViewCentre);
                        break;     
                    case ViewCreationMode.Place:
                        this.PlaceExistingViewOnSheet(sheet.DestinationSheet, view.OldView, sourceViewCentre);
                        break;                 
                }
            }       
        }

        /// <summary>
        /// Copy a view and add it to a sheet
        /// </summary>
        /// <param name="view"></param>
        /// <param name="sheet"></param>
        /// <param name="sourceViewCentre"></param>
        private void DuplicateViewOntoSheet(
            SCopyViewOnSheet view, SCopySheet sheet, XYZ sourceViewCentre)
        {
            var d = view.DuplicateWithDetailing == true ? ViewDuplicateOption.WithDetailing : ViewDuplicateOption.Duplicate;          
            ElementId destViewId = view.OldView.Duplicate(d);
            string newName = sheet.GetNewViewName(view.OldView.Id);
            var v = this.doc.GetElement(destViewId) as View;
            if (newName != null) {
                v.Name = newName;
                if (view.ViewTemplateName != SCopyConstants.MenuItemCopy) {
                    View vt = null;
                    if (this.viewTemplates.TryGetValue(view.ViewTemplateName, out vt)) {
                        View dv = this.doc.GetElement(destViewId) as View;
                        dv.ViewTemplateId = vt.Id;
                    }    
                }
            }
            this.PlaceViewOnSheet(sheet.DestinationSheet, destViewId, sourceViewCentre);
        }

        /// <summary>
        /// Get the title block on a given ViewSheet.
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns>The title block or null if none/multiple are found</returns>
        private FamilyInstance GetTitleBlock(ViewSheet sheet)
        {
            var elemsOnSheet = new FilteredElementCollector(
                          this.doc, sheet.Id);
            elemsOnSheet.OfCategory(BuiltInCategory.OST_TitleBlocks);
            if (elemsOnSheet.Count() == 1) {
                var inst = (FamilyInstance)elemsOnSheet.ElementAt(0);
                return inst;
            } else {
                TaskDialog.Show("SCopy", "Multiple title blocks found...");
                return null;
            }
        }
        #endregion
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
