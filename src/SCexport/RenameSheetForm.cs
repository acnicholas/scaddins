// (C) Copyright 2014 by Andrew Nicholas
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

    /// <summary>
    /// Description of RenameSheetForm.
    /// </summary>
    public partial class RenameSheetForm : Form
    {
        private ICollection<SCexportSheet> sheets;
        private Autodesk.Revit.DB.Document doc;
        
        public RenameSheetForm(ICollection<SCexportSheet> sheets, Autodesk.Revit.DB.Document doc)
        {
            this.sheets = sheets;
            this.doc = doc;
            this.InitializeComponent();            
            this.sheets = sheets;
            this.PopulateList();
        }
                
        private void PopulateList()
        {
            listView1.Items.Clear();
            foreach (SCexportSheet sheet in this.sheets) {
                var item = new ListViewItem();
                item.Text = sheet.SheetNumber;
                item.SubItems.Add(sheet.SheetDescription);
                item.SubItems.Add(this.NewSheetNumber(sheet.SheetNumber));
                item.SubItems.Add(this.NewSheetName(sheet.SheetDescription));
                this.listView1.Items.Add(item);
            }
        }
        
        private static string NewSheetValue(string s, string pattern, string replacement)
        { 
            return Regex.Replace(s, pattern, replacement);
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
        
        private void RenameSheets()
        {
            var t = new Autodesk.Revit.DB.Transaction(this.doc);
            t.Start("SCexport - Rename Sheets");
            foreach (SCexportSheet sheet in this.sheets) {
                sheet.Sheet.Name = this.NewSheetName(sheet.SheetDescription);
                sheet.Sheet.SheetNumber = this.NewSheetNumber(sheet.SheetNumber);
            }
            t.Commit();
        }
        
        private void Button1Click(object sender, EventArgs e)
        {
            this.PopulateList();    
            this.listView1.Refresh();
        }
        
        private void OKClick(object sender, EventArgs e)
        {
            this.RenameSheets();                
        }      
    }
}
