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

namespace SCaddins.SCuv
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    /// <summary>
    /// Copy a view; give it a user name, remove any view templates and
    /// categorize it nicely.
    /// </summary>
    public static class SCuv
    {     
        public static bool CreateUserView(View srcView, Document doc)
        {
            if (srcView.ViewType == ViewType.DrawingSheet) {
                CreateUserViewsFromSheet(srcView as ViewSheet, doc);
                return true;
            }
            if (ValidViewType(srcView.ViewType)) {
                    return CreateView(srcView, doc);
            }
            ShowErrorDialog(srcView);
            return false;   
        }
               
        public static void CreateUserViews(ICollection<SCaddins.SCexport.SCexportSheet> sheets, Document doc)
        {
            string message = string.Empty;
            var t = new Transaction(doc, "SCuv Copies User Views");
            t.Start();
            foreach (SCaddins.SCexport.SCexportSheet sheet in sheets) {
                message += CreateUserViewsFromSheet(sheet.Sheet, doc);
            }
            t.Commit();
            ShowSummaryDialog(message);
    }
                    
        public static string GetNewViewName(View srcView)
        { 
            return Environment.UserName + "-" + srcView.Name + "-" + SCaddins.SCexport.SCexport.GetDateString();           
        } 
        
        public static void ShowErrorDialog(View srcView)
        {
            var td = new TaskDialog("SCuv - SCuv copies users views");
            td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            td.MainInstruction = "Error creating user view for view:";
            td.MainContent = srcView.Name;
            td.Show();   
        }

        public static void ShowSummaryDialog(string message)
        {
            var td = new TaskDialog("SCuv - SCuv copies users views");
            td.MainIcon = TaskDialogIcon.TaskDialogIconNone;
            td.MainInstruction = "Summary of users view created:";
            td.MainContent = message;
            td.Show();   
        }
        
        private static string CreateUserViewsFromSheet(ViewSheet vs, Document doc)
        {
            string message = string.Empty;
            foreach (View v in vs.Views) {
                if (ValidViewType(v.ViewType)) {
                    CreateView(v, doc);
                    message += GetNewViewName(v) + Environment.NewLine;
                }
            }  
            return message;          
        }
        
        private static bool ValidViewType(ViewType viewType)
        {
            switch (viewType) {
                case ViewType.FloorPlan:
                case ViewType.Elevation:
                case ViewType.CeilingPlan:
                case ViewType.Section:
                case ViewType.AreaPlan:
                case ViewType.ThreeD:
                    return true;
            }   
            return false;
        }
   
        private static bool CreateView(View srcView, Document doc)
        {
            ElementId destViewId = srcView.Duplicate(ViewDuplicateOption.Duplicate);
            View newView = doc.GetElement(destViewId) as View;
            newView.Name = GetNewViewName(srcView); 
            newView.ViewTemplateId = ElementId.InvalidElementId;
            
            Parameter param = newView.get_Parameter("SC-View_Category");
            if (param.IsReadOnly) {
                TaskDialog.Show("SCuv Error", "SC-View_Category is read only!");
                return false;
            } else {
                if (!param.Set("User")) {
                    TaskDialog.Show("SCuv Error", "Error setting SC-View_Category parameter!"); 
                    return false;    
                }
            } 
            return true;  
        }
    }
}
