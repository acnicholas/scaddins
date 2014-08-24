namespace SCaddins.SCopy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    
    public class SCopy
    {
        private FamilyInstance sourceTitleBlock;
        private Document doc;
        private ViewSheet sourceSheet;
        private System.ComponentModel.BindingList<SCopySheet> sheets;
        private Dictionary<string, View> existingSheets =
            new Dictionary<string, View>();
        
        private Dictionary<string, View> existingViews =
            new Dictionary<string, View>();
        
        private Dictionary<string, View> viewTemplates =
            new Dictionary<string, View>();
        
        private Dictionary<string, Level> levels =
            new Dictionary<string, Level>();
        
        private ElementId floorPlanViewFamilyTypeId = null;
           
        public SCopy(Document doc, ViewSheet view)
        {
            this.doc = doc;
            this.sourceSheet = view;
            this.sheets = new System.ComponentModel.BindingList<SCopySheet>();
            this.GetViewTemplates();
            this.GetAllSheets();
            this.GetAllLevelsInModel();
            this.GetAllViewsInModel();
            this.GetFloorPlanViewFamilyTypeId();
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

        public ViewSheet SourceSheet {
            get {
                return this.sourceSheet;
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
    
        #endregion

        #region public methods
        public static ViewSheet ViewToViewSheet(View view)
        {
            if (view.ViewType != ViewType.DrawingSheet) {
                return null;
            } else {
                return view as ViewSheet;
            }
        }
        
        public bool CheckSheetNumberAvailability(string number)
        {
            foreach (SCopySheet s in this.sheets) {
                if (s.Number.ToUpper().Equals(number.ToUpper())) {
                    return false;
                }
            }
            return !this.existingSheets.ContainsKey(number);
        }

        public bool CheckViewNameAvailability(string title)
        {
            foreach (SCopySheet s in this.sheets) {
                foreach (SCopyViewOnSheet v in s.ViewsOnSheet) {
                    if (v.Title.ToUpper().Equals(title.ToUpper())) {
                        return false;
                    }
                }
            }
            return !this.existingViews.ContainsKey(title);
        }

        public void AddViewInfoToList(
            ref System.Windows.Forms.ListView list)
        {
            var colour = System.Drawing.Color.Gray;
            this.AddViewsToList(ref list, "Title", this.sourceSheet.Name, colour, 0);
            this.AddViewsToList(ref list, "Sheet Number", this.sourceSheet.SheetNumber, colour, 0);
            this.AddViewsToList(ref list, this.sourceSheet.Views);
        }
    
        public void CreateSheets()
        {
            string summaryText = string.Empty;
            foreach (SCopySheet sheet in this.sheets) {
                this.CreateSheet(sheet, ref summaryText);
            }
            TaskDialog td = new TaskDialog("SCopy - Summary");
            td.MainInstruction = "SCopy - Summary";
            td.MainContent = summaryText;
            td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            td.Show();
        }
    
        public void Add()
        {
            string n = this.NextSheetNumber(this.sourceSheet.SheetNumber);
            string t = this.sourceSheet.Name + SCopyConstants.MenuItemCopy;
            this.sheets.Add(new SCopySheet(n, t, this));
        }

        #endregion

        #region private methods
        private void AddViewsToList(
            ref System.Windows.Forms.ListView list,
            ViewSet views)
        {
            this.AddViewsToList(
                ref list,
                "Number of viewports",
                views.Size.ToString(),
                System.Drawing.Color.Gray,
                1);
            int i = 1;
            foreach (View view in views) {
                this.AddViewsToList(
                    ref list,
                    "View: " + i,
                    view.Name,
                    System.Drawing.Color.Black,
                    1);
                i++;
            }
        }

        private void AddViewsToList(
            ref System.Windows.Forms.ListView list,
            string title,
            string value,
            System.Drawing.Color colour,
            int group)
        {
            System.Windows.Forms.ListViewItem item;
            item = new System.Windows.Forms.ListViewItem(
                new[] { title, value }, list.Groups[group]);
            item.ForeColor = colour;
            list.Items.Add(item);
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

        private bool CreateSheet(SCopySheet sheet, ref string summary)
        {
            this.sourceTitleBlock = this.GetTitleBlock(this.sourceSheet);

            if (this.sourceTitleBlock == null) {
                TaskDialog.Show("SCopy", "No Title Block, exiting now...");
                return false;
            }

            // create a new sheet (the copy)
            ViewSheet destSheet = this.AddEmptySheetToDocument(
                                      this.sourceTitleBlock.Symbol, sheet.Number, sheet.Title);

            // copy ViewPorts from from the src sheet to dest sheet
            sheet.DestSheet = destSheet;
            if (sheet.DestSheet != null) {
                this.PlaceNewViews(sheet);
            }

            // add text to the summary dialog
            var oldNumber = this.sourceSheet.SheetNumber;
            var msg = " Sheet: " + oldNumber + " copied to: " + sheet.Number;
            summary += msg + System.Environment.NewLine;

            return true;
        }
    
        private ViewSheet AddEmptySheetToDocument(
            FamilySymbol titleBlock,
            string sheetNumber,
            string sheetTitle)
        {
            ViewSheet result;
            #if REVIT2013
        result = doc.Create.NewViewSheet(titleBlock);
            #else
            result = ViewSheet.Create(this.doc, titleBlock.Id);
            #endif
            result.Name = sheetTitle;
            result.SheetNumber = sheetNumber;
            return result;
        }

        private void PlaceViewOnSheet(
            ViewSheet destSheet, ElementId destViewId, XYZ srcViewCentre)
        {
            FamilyInstance destTB = this.GetTitleBlock(destSheet);
            BoundingBoxXYZ desTBBounds = destTB.get_BoundingBox(destSheet);
            double destViewMidX = desTBBounds.Min.X + srcViewCentre.X;
            double destViewMidY = desTBBounds.Min.Y + srcViewCentre.Y;
            XYZ destViewCentre = new XYZ(destViewMidX, destViewMidY, 0);
            Viewport.Create(this.doc, destSheet.Id, destViewId, destViewCentre);
        }

        private string NextSheetNumber(string s)
        {
            int inc = 0;
            do {
                inc++;
            } while (!this.CheckSheetNumberAvailability(s + "-" + inc.ToString()));
            return s + "-" + inc.ToString();
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
                this.PlaceViewOnSheet(sheet.DestSheet, vp.Id, sourceViewCentre);
            }
        }

        private void PlaceNewViews(SCopySheet sheet)
        {
            // creat a dictionary to hold all the ViewPorts on the src sheet
            Dictionary<ElementId, BoundingBoxXYZ> viewPorts =
                this.GetVPDictionary(this.sourceSheet, this.doc);

            // foreach(View viewOnSrcSheet in srcSheet.Views){
            foreach (SCopyViewOnSheet view in sheet.ViewsOnSheet) {
                // bounds of the view being copied/recreated.
                BoundingBoxXYZ srcViewBounds = null;
                if (!viewPorts.TryGetValue(view.OldId, out srcViewBounds)) {
                    TaskDialog.Show("SCopy", "Error...");
                    continue;
                }
            
                // the middle of the view being copied/recreated.
                XYZ sourceViewCentre = this.ViewCenterFromTBBottomLeft(
                                           this.sourceTitleBlock, srcViewBounds, this.sourceSheet);

                switch (view.CreationMode) {
                    case ViewCreationMode.Copy:
                        this.CopyViewToSheet(view, sheet, sourceViewCentre);
                        break;
                    case ViewCreationMode.CopyAndModify:
                        this.PlaceNewView(view, sheet, sourceViewCentre);
                        break;     
                    case ViewCreationMode.Place:
                        this.PlaceNewView(view, sheet, sourceViewCentre);
                        break;                 
                }
            }
        }
    
        private void CopyViewToSheet(
            SCopyViewOnSheet view, SCopySheet sheet, XYZ sourceViewCentre)
        {
            ElementId destViewId = view.OldView.Duplicate(
                                       ViewDuplicateOption.WithDetailing);
            string newName = sheet.GetNewViewName(view.OldView.Id);
            View v = this.doc.GetElement(destViewId) as View;
            if (newName != null) {
                v.Name = newName;
            }
            this.PlaceViewOnSheet(sheet.DestSheet, destViewId, sourceViewCentre);
        }

        private XYZ ViewCenterFromTBBottomLeft(
            FamilyInstance titleBlock, BoundingBoxXYZ viewBounds, View view)
        {
            BoundingBoxXYZ titleBounds = titleBlock.get_BoundingBox(view);
            double x =
                (viewBounds.Min.X - titleBounds.Min.X) +
                ((viewBounds.Max.X - viewBounds.Min.X) / 2);
            double y =
                (viewBounds.Min.Y - titleBounds.Min.Y) +
                ((viewBounds.Max.Y - viewBounds.Min.Y) / 2);
            return new XYZ(x, y, 0);
        }

        // TODO
        private void CopyAnnotationElements(
            ViewSheet src, ViewSheet dest)
        {
            FilteredElementCollector collector = new FilteredElementCollector(
                                                     this.doc, src.Id);
        }

        private FamilyInstance GetTitleBlock(ViewSheet sheet)
        {
            FilteredElementCollector elemsOnSheet = new FilteredElementCollector(
                                                        this.doc, sheet.Id);
            elemsOnSheet.OfCategory(BuiltInCategory.OST_TitleBlocks);
            if (elemsOnSheet.Count() == 1) {
                FamilyInstance inst = (FamilyInstance)elemsOnSheet.ElementAt(0);
                return inst;
            } else {
                TaskDialog.Show("SCopy", "Multiple title blocks found...");
                return null;
            }
        }

        private Dictionary<ElementId, BoundingBoxXYZ> GetVPDictionary(
            ViewSheet srcSheet, Document doc)
        {
            Dictionary<ElementId, BoundingBoxXYZ> result =
                new Dictionary<ElementId, BoundingBoxXYZ>();
            foreach (ElementId viewPortId in srcSheet.GetAllViewports()) {
                Viewport viewPort = (Viewport)doc.GetElement(viewPortId);
                var viewPortBounds = viewPort.get_BoundingBox(srcSheet);
                result.Add(
                    viewPort.ViewId, viewPortBounds);
            }
            return result;
        }

        private string BoundingBoxString(BoundingBoxXYZ bounds)
        {
            var x = bounds.Min.X * SCopyConstants.MMperFoot;
            var y = bounds.Min.Y * SCopyConstants.MMperFoot;
            var x2 = bounds.Max.X * SCopyConstants.MMperFoot;
            var y2 = bounds.Max.Y * SCopyConstants.MMperFoot;
            return x + " , " + y + " , " + x2 + " , " + y2;
        }
        #endregion
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
