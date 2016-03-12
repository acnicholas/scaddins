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

namespace SCaddins.SCexport
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public partial class RenameSheetForm : Form
    {
        private ICollection<ExportSheet> sheets;
        private Autodesk.Revit.DB.Document doc;

        public RenameSheetForm(ICollection<ExportSheet> sheets, Autodesk.Revit.DB.Document doc)
        {
            this.sheets = sheets;
            this.doc = doc;
            this.InitializeComponent();
            this.sheets = sheets;
            this.PopulateList();
        }

        private static string NewSheetValue(string s, string pattern, string replacement)
        {
            return Regex.Replace(s, pattern, replacement);
        }

        private void PopulateList()
        {
            listView1.Items.Clear();
            foreach (ExportSheet sheet in this.sheets) {
                var item = new ListViewItem();
                item.Text = sheet.SheetNumber;
                item.SubItems.Add(sheet.SheetDescription);
                item.SubItems.Add(this.NewSheetNumber(sheet.SheetNumber));
                item.SubItems.Add(this.NewSheetName(sheet.SheetDescription));
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
        
        private void removeRevivionsOnSheet() {
            
        }

        private void RenameSheets()
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
            
            foreach (ExportSheet sheet in this.sheets) {
                sheet.Sheet.Name = this.NewSheetName(sheet.SheetDescription);
                sheet.Sheet.SheetNumber = this.NewSheetNumber(sheet.SheetNumber);
                   
                if (removeRevisions) {  
                
                    //remove revisions on sheets
                    sheet.Sheet.SetAdditionalProjectRevisionIds(new List<Autodesk.Revit.DB.ElementId>());
              
                    Autodesk.Revit.DB.View view = sheet.Sheet as Autodesk.Revit.DB.View;
                    //Autodesk.Revit.DB.View view = sheet.Sheet;
                               
                    #if REVIT2014
                    SCopy.SheetCopy.deleteRevisionClouds(view.Id, this.doc); 
                    foreach (Autodesk.Revit.DB.View v in sheet.Sheet.Views) {
                        SCopy.SheetCopy.deleteRevisionClouds(v.Id, this.doc);
                    }
                    #else
                    List<Autodesk.Revit.DB.Revision> hiddenRevisionClouds = SCopy.SheetCopy.getAllHiddenRevisions(this.doc);
                    //turn on hidden revisions
                    foreach (Autodesk.Revit.DB.Revision rev in hiddenRevisionClouds) {
                        rev.Visibility = Autodesk.Revit.DB.RevisionVisibility.CloudAndTagVisible;
                    }
                    
                    doc.Regenerate();
                
                    //remove clouds on view
                    SCopy.SheetCopy.deleteRevisionClouds(view.Id, this.doc);     
                
                    //remove clouds in viewports                    
                    foreach (Autodesk.Revit.DB.ElementId id in sheet.Sheet.GetAllPlacedViews()) {
                        SCopy.SheetCopy.deleteRevisionClouds(id, this.doc);
                    }
                 
                    //re-hide hidden revisions
                    foreach (Autodesk.Revit.DB.Revision rev in hiddenRevisionClouds) {
                        rev.Visibility = Autodesk.Revit.DB.RevisionVisibility.Hidden;
                    }
                    #endif
                }
            }
            t.Commit();
        }

        private void TestRunButtonClick(object sender, EventArgs e)
        {
            this.PopulateList();
            this.listView1.Refresh();
        }

        private void RenameButtonClick(object sender, EventArgs e)
        {
            this.RenameSheets();
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
