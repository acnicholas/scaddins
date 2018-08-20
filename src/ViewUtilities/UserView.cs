// (C) Copyright 2014-2017 by Andrew Nicholas
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
    using SCaddins.Properties;

    /// <summary>
    /// Copy a view; give it a user name, remove any view templates and
    /// categorize it nicely.
    /// </summary>
    public static class UserView
    {
        public static List<View> Create(View sourceView, Document doc)
        {
            if (sourceView == null || doc == null)
            {
                return null;
            }

            if (sourceView.ViewType == ViewType.DrawingSheet)
            {
                return Create(sourceView as ViewSheet, doc);
            }

            if (ValidViewType(sourceView.ViewType))
            {
                List<View> result = new List<View>();
                result.Add(CreateView(sourceView, doc));
                return result;
            }

            return null;
        }

        public static List<View> Create(ICollection<SCaddins.ExportManager.ExportSheet> sheets, Document doc)
        {
            List<View> result = new List<View>();
            if (sheets == null || doc == null)
            {
                return null;
            }
            else
            {
                using (var t = new Transaction(doc, "SCuv Copies User Views"))
                {
                    if (t.Start() == TransactionStatus.Started)
                    {
                        foreach (SCaddins.ExportManager.ExportSheet sheet in sheets)
                        {
                            var list = Create(sheet.Sheet, doc);
                            foreach (View v in list)
                            {
                                result.Add(v);
                            }
                        }
                        t.Commit();
                    }
                    else
                    {
                        TaskDialog.Show("Error", "Could not start user view transaction");
                        return null;
                    }
                }
            }
            return result;
        }

        public static void ShowSummaryDialog(List<View> newUserViews)
        {
            using (var td = new TaskDialog(Resources.CreateUserViews))
            {
                string message = string.Empty;
                if (newUserViews == null)
                {
                    message = "No valid views found, User view not created." + System.Environment.NewLine
                    + "\tValid views types are: " + System.Environment.NewLine
                    + System.Environment.NewLine
                    + "\t\tViewType.FloorPlan" + System.Environment.NewLine
                    + "\t\tViewType.Elevation" + System.Environment.NewLine
                    + "\t\tViewType.CeilingPlan" + System.Environment.NewLine
                    + "\t\tViewType.Section" + System.Environment.NewLine
                    + "\t\tViewType.AreaPlan" + System.Environment.NewLine
                    + "\t\tViewType.ThreeD";
                }
                else
                {
                    foreach (View view in newUserViews)
                    {
                        message += view.Name + System.Environment.NewLine;
                    }
                }
                td.MainIcon = TaskDialogIcon.TaskDialogIconNone;
                td.MainInstruction = "Summary of users view created:";
                td.MainContent = message;
                td.Show();
            }
        }

        private static List<View> Create(ViewSheet vs, Document doc)
        {
            List<View> result = new List<View>();
            foreach (ElementId id in vs.GetAllPlacedViews())
            {
                var v = (View)doc.GetElement(id);
                if (ValidViewType(v.ViewType))
                {
                    result.Add(CreateView(v, doc));
                }
            }
            return result;
        }

        private static View CreateView(View srcView, Document doc)
        {
            ElementId destViewId = srcView.Duplicate(ViewDuplicateOption.Duplicate);
            var newView = doc.GetElement(destViewId) as View;
            newView.Name = GetNewViewName(doc, srcView);
            newView.ViewTemplateId = ElementId.InvalidElementId;
            var p = newView.GetParameters("SC-View_Category");
            if (p.Count < 1)
            {
                return newView;
            }
            Parameter param = p[0];
            if (param == null)
            {
                return newView;
            }

            if (param.IsReadOnly)
            {
                TaskDialog.Show("SCuv Error", "SC-View_Category is read only!");
                return null;
            }
            else
            {
                if (!param.Set("User"))
                {
                    TaskDialog.Show("SCuv Error", "Error setting SC-View_Category parameter!");
                    return null;
                }
            }
            return newView;
        }

        private static string GetNewViewName(Document doc, Element sourceView)
        {
            if (doc == null || sourceView == null)
            {
                return string.Empty;
            }
            string name = sourceView.Name;

            // Revit wont allow { or } so replace them if they exist
            name = name.Replace(@"{", string.Empty).Replace(@"}", string.Empty);
            name = Environment.UserName + "-" + name + "-" + MiscUtilities.GetDateString;
            if (SolarUtilities.SolarViews.ViewNameIsAvailable(doc, name))
            {
                return name;
            }
            else
            {
                return SolarUtilities.SolarViews.GetNiceViewName(doc, name);
            }
        }

        private static bool ValidViewType(ViewType viewType)
        {
            switch (viewType)
            {
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
    }
}