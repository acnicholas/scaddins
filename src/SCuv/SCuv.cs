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
    public class SCuv
    {
        public SCuv()
        {
        }
        
        public static void CreateUserView(View srcView, Document doc)
        {
            ElementId destViewId = srcView.Duplicate(ViewDuplicateOption.Duplicate);
            View newView = doc.GetElement(destViewId) as View;
            newView.Name = SetNewViewName(srcView); 
            
            // TODO test this really works
            newView.ViewTemplateId = ElementId.InvalidElementId;
            
            Parameter param = newView.get_Parameter("SC-View_Category");
            if (param.IsReadOnly) {
                TaskDialog.Show("SCuv Error", "SC-View_Category is read only!");
            } else {
                if (!param.Set("User")) {
                    TaskDialog.Show("SCuv Error", "Error setting SC-View_Category parameter!");     
                }
            }
        }
        
        public static void CreateUserViews(ICollection<SCaddins.SCexport.SCexportSheet> sheets, Document doc)
        {
            Transaction t = new Transaction(doc, "SCuv Copies User Views");
            string message = string.Empty;
            t.Start();
            foreach (SCaddins.SCexport.SCexportSheet sheet in sheets) {
                foreach (View v in sheet.Sheet.Views) {
                    switch (v.ViewType) {
                        case ViewType.FloorPlan:
                        case ViewType.Elevation:
                        case ViewType.CeilingPlan:
                        case ViewType.Section:
                        case ViewType.AreaPlan:
                        case ViewType.ThreeD:
                            CreateUserView(v, doc);
                            message += SetNewViewName(v) + System.Environment.NewLine;
                            break;
                    }
                }
            }
            t.Commit();
            var td = new TaskDialog("SCuv - SCuv copies users views");
            td.MainIcon = TaskDialogIcon.TaskDialogIconNone;
            td.MainInstruction = "Summary of users view created:";
            td.MainContent = message;
            td.Show();   
        }
             
        private static string SetNewViewName(View srcView)
        { 
            return Environment.UserName + "-" + srcView.Name + "-" + SCaddins.SCexport.SCexport.GetDateString();           
        }       
    }
}
