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

namespace SCaddins.ViewUtilities
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using SCaddins.Common;

    /// <summary>
    /// Copy a view; give it a user name, remove any view templates and
    /// categorize it nicely.
    /// </summary>
    public static class UserView
    {     
        public static bool Create(View sourceView, Document doc)
        {
            if (sourceView == null || doc == null) {
                return false;
            }

            if (sourceView.ViewType == ViewType.DrawingSheet) {
                Create(sourceView as ViewSheet, doc);
                return true;
            }

            if (ValidViewType(sourceView.ViewType)) {
                return CreateView(sourceView, doc);
            }

            ShowErrorDialog(sourceView);
            return false;   
        }
               
        public static void Create(ICollection<SCaddins.ExportManager.ExportSheet> sheets, Document doc)
        {
            string message = string.Empty;
            if (sheets == null || doc == null) {
                message += "Could not create user view";
            } else {
                using (var t = new Transaction(doc, "SCuv Copies User Views")) {
                    if (t.Start() == TransactionStatus.Started) {
                        foreach (SCaddins.ExportManager.ExportSheet sheet in sheets) {
                            message += Create(sheet.Sheet, doc);
                        }
                        t.Commit();
                    } else {
                        TaskDialog.Show("Error", "Could not start user view transaction");
                    }
                }
            }
            ShowSummaryDialog(message);
    }
                    
        public static string GetNewViewName(Document doc, Element sourceView)
        { 
            if (doc == null || sourceView == null) {
                // FIXME add error message here
                return string.Empty;
            }
            string name = sourceView.Name;
            // Revit wont allow { or } so replace them if they exist
            name = name.Replace(@"{","").Replace(@"}","");
            name = Environment.UserName + "-" + name + "-" + MiscUtilities.GetDateString;
            if (SolarUtilities.Command.ViewNameIsAvailable(doc, name)) {
                return name;
            } else {
                return SolarUtilities.Command.GetNiceViewName(doc, name);
            }
        } 
        
        public static void ShowErrorDialog(Element sourceView)
        {
            if (sourceView == null) {
                // FIXME add a error message here
                return;
            }
            using (var td = new TaskDialog("SCuv - SCuv copies users views")) {
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                td.MainInstruction = "Error creating user view for view:";
                td.MainContent = sourceView.Name;
                td.Show();
            }
        }

        public static void ShowSummaryDialog(string message)
        {
            using (var td = new TaskDialog("SCuv - SCuv copies users views")) {
                td.MainIcon = TaskDialogIcon.TaskDialogIconNone;
                td.MainInstruction = "Summary of users view created:";
                td.MainContent = message;
                td.Show();
            } 
        }
        
        private static string Create(ViewSheet vs, Document doc)
        {
            string message = string.Empty;
            foreach (ElementId id in vs.GetAllPlacedViews()) {
                var v = (View)doc.GetElement(id);
                if (ValidViewType(v.ViewType)) {
                    CreateView(v, doc);
                    message += GetNewViewName(doc, v) + Environment.NewLine;
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
            var newView = doc.GetElement(destViewId) as View;
            newView.Name = GetNewViewName(doc, srcView); 
            newView.ViewTemplateId = ElementId.InvalidElementId;
            var p = newView.GetParameters("SC-View_Category");
            if (p.Count < 1) {
                return true;
            }
            Parameter param = p[0];
            if (param == null) {
                return true;
            }
            
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
