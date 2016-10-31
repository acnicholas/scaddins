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

namespace SCaddins.ExportManager
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using SCaddins.SheetCopier;

    public partial class RenameSheetForm : Form
    {
        private ICollection<ExportSheet> exportSheets;
        private System.ComponentModel.BindingList<SheetCopierSheet> scopySheets;
        private Autodesk.Revit.DB.Document doc;

        public RenameSheetForm(ICollection<ExportSheet> sheets, Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
            this.scopySheets = null;
            this.exportSheets = sheets;
            this.InitializeComponent();
            this.PopulateList(this.exportSheets);
        }
        
        public RenameSheetForm(System.ComponentModel.BindingList<SheetCopierSheet> sheets, Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
            this.scopySheets = sheets;
            this.exportSheets = null;
            this.InitializeComponent();
            this.PopulateList(this.scopySheets);
        }

        private static string NewSheetValue(string s, string pattern, string replacement)
        {
            return Regex.Replace(s, pattern, replacement);
        }

        private void PopulateList(ICollection<ExportSheet> sheets)
        {
            listView1.Items.Clear();
            foreach (ExportSheet sheet in sheets) {
                var item = new ListViewItem();
                item.Text = sheet.SheetNumber;
                item.SubItems.Add(sheet.SheetDescription);
                item.SubItems.Add(this.NewSheetNumber(sheet.SheetNumber));
                item.SubItems.Add(this.NewSheetName(sheet.SheetDescription));
                this.listView1.Items.Add(item);
            }
        }
        
        private void PopulateList(System.ComponentModel.BindingList<SheetCopierSheet> sheets)
        {
              listView1.Items.Clear();
              foreach (SheetCopierSheet sheet in sheets) {
                  var item = new ListViewItem();
                  item.Text = sheet.Number;
                  item.SubItems.Add(sheet.Title);
                  item.SubItems.Add(this.NewSheetNumber(sheet.Number));
                  item.SubItems.Add(this.NewSheetName(sheet.Title));
                  this.listView1.Items.Add(item);
              }
        }

        private string NewSheetNumber(string number)
        {
            return NewSheetValue(
                number,
                this.textBoxNumberPattern.Text,
                this.textBoxNumberReplace.Text);
        }

        private string NewSheetName(string name)
        {
            return NewSheetValue(
                name,
                this.textBoxNamePattern.Text,
                this.textBoxNameReplace.Text);
        }
                
        private void RemoveRevisions(ExportSheet sheet)
        {
            //remove revisions on sheets
            #if REVIT2014
                    sheet.Sheet.SetAdditionalProjectRevisionIds(new List<Autodesk.Revit.DB.ElementId>());
            #else
            ICollection<Autodesk.Revit.DB.ElementId> revisions = sheet.Sheet.GetAdditionalRevisionIds();
            revisions.Clear();
            sheet.Sheet.SetAdditionalRevisionIds(revisions);
            #endif
              
            Autodesk.Revit.DB.View view = sheet.Sheet as Autodesk.Revit.DB.View;
            //Autodesk.Revit.DB.View view = sheet.Sheet;
                               
            #if REVIT2014
                    SheetCopierManager.DeleteRevisionClouds(view.Id, this.doc); 
                    foreach (Autodesk.Revit.DB.View v in sheet.Sheet.Views) {
                        SheetCopierManager.DeleteRevisionClouds(v.Id, this.doc);
                    }
            #else
            List<Autodesk.Revit.DB.Revision> hiddenRevisionClouds = SheetCopierManager.GetAllHiddenRevisions(this.doc);
            //turn on hidden revisions
            foreach (Autodesk.Revit.DB.Revision rev in hiddenRevisionClouds) {
                rev.Visibility = Autodesk.Revit.DB.RevisionVisibility.CloudAndTagVisible;
            }
                    
            doc.Regenerate();
                
            //remove clouds on view
            SheetCopierManager.DeleteRevisionClouds(view.Id, this.doc);     
                
            //remove clouds in viewports                    
            foreach (Autodesk.Revit.DB.ElementId id in sheet.Sheet.GetAllPlacedViews()) {
                SheetCopierManager.DeleteRevisionClouds(id, this.doc);
            }
                 
            //re-hide hidden revisions
            foreach (Autodesk.Revit.DB.Revision rev in hiddenRevisionClouds) {
                rev.Visibility = Autodesk.Revit.DB.RevisionVisibility.Hidden;
            }
            #endif    
        }
        
        private void RenameSheets(ICollection<ExportSheet> sheets)
        {
            bool removeRevisions = false;
            var t = new Autodesk.Revit.DB.Transaction(this.doc);
            t.Start("SCexport - Rename Sheets");
            Autodesk.Revit.UI.TaskDialog td = new Autodesk.Revit.UI.TaskDialog("Remove revisions?");
            td.MainIcon = Autodesk.Revit.UI.TaskDialogIcon.TaskDialogIconWarning;
            td.CommonButtons = Autodesk.Revit.UI.TaskDialogCommonButtons.No | Autodesk.Revit.UI.TaskDialogCommonButtons.Yes;
            td.MainInstruction = "Remove Revisions?";
            td.MainContent = "do you want to remove all revisions from the selected sheets after renaming them?";
            Autodesk.Revit.UI.TaskDialogResult tdr = td.Show();
            
            if (tdr == Autodesk.Revit.UI.TaskDialogResult.Yes) {
                removeRevisions = true;
            }
            
            foreach (ExportSheet sheet in sheets) {
                sheet.Sheet.Name = this.NewSheetName(sheet.SheetDescription);
                sheet.Sheet.SheetNumber = this.NewSheetNumber(sheet.SheetNumber);
                   
                if (removeRevisions) {  
                    RemoveRevisions(sheet);
                }
            }
            t.Commit();
        }

        private void RenameSheets(System.ComponentModel.BindingList<SheetCopierSheet> sheets)
        {
            //var t = new Autodesk.Revit.DB.Transaction(this.doc);
            //t.Start("SCexport - Rename Sheets");
            foreach (SheetCopierSheet sheet in sheets) {
                sheet.Title = this.NewSheetName(sheet.Title);
                sheet.Number = this.NewSheetNumber(sheet.Number);
            }
            //t.Commit();
        }

        private void TestRunButtonClick(object sender, EventArgs e)
        {
            if (this.scopySheets != null) {
                this.PopulateList(this.scopySheets);
            } else {
                this.PopulateList(this.exportSheets);    
            }
            this.listView1.Refresh();
        }

        private void RenameButtonClick(object sender, EventArgs e)
        {
            if (this.scopySheets != null) {
                this.RenameSheets(this.scopySheets);
            } else {
                this.RenameSheets(this.exportSheets);    
            }
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
