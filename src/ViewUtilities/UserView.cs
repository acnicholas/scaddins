// (C) Copyright 2014-2020 by Andrew Nicholas
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
    using System.Text.RegularExpressions;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Common;

    /// <summary>
    /// Copy a view; give it a user name, remove any view templates and
    /// categorize it nicely.
    /// </summary>
    public static class UserView
    {
        public static List<View> Create(View sourceView, UIDocument uidoc)
        {
            if (sourceView == null || uidoc == null || uidoc.Document == null)
            {
                return null;
            }

            if (sourceView.ViewType == ViewType.ProjectBrowser)
            {
                var views = new List<View>();
                var selection = uidoc.Selection.GetElementIds();
                foreach (var id in selection)
                {
                    var projectBrowserView = uidoc.Document.GetElement(id);
                    if (projectBrowserView is View)
                    {
                        var v = (View)projectBrowserView;
                        if (v.ViewType == ViewType.ProjectBrowser)
                        {
                            continue;
                        }
                        if (v is ViewSheet)
                        {
                            views.AddRange(Create(v as ViewSheet, uidoc.Document));
                            continue;
                        }
                        if (v is View)
                        {
                            views.Add(CreateView(v, uidoc.Document));
                        }
                    }
                }
                return views;
            }

            if (sourceView.ViewType == ViewType.DrawingSheet)
            {
                return Create(sourceView as ViewSheet, uidoc.Document);
            }

            if (ValidViewType(sourceView.ViewType))
            {
                return new List<View> { CreateView(sourceView, uidoc.Document) };
            }

            return null;
        }

        public static List<View> Create(ICollection<ExportManager.ExportSheet> sheets, Document doc)
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
                        foreach (ExportManager.ExportSheet sheet in sheets)
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
                        SCaddinsApp.WindowManager.ShowMessageBox("Error", "Could not start user view transaction");
                        return null;
                    }
                }
            }
            return result;
        }

        public static Parameter ParamFromString(string name, Element element)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (element == null) return null;
            if (element.GetParameters(name).Count > 0)
            {
                return element.GetParameters(name)[0];
            }
            return null;
        }

        public static void ShowSummaryDialog(List<View> newUserViews)
        {
            string message = string.Empty;
            if (newUserViews == null)
            {
                message = "No valid views found, User view not created." + Environment.NewLine
                + "\tValid views types are: " + Environment.NewLine
                + Environment.NewLine
                + "\t\tViewType.FloorPlan" + Environment.NewLine
                + "\t\tViewType.Elevation" + Environment.NewLine
                + "\t\tViewType.CeilingPlan" + Environment.NewLine
                + "\t\tViewType.Section" + Environment.NewLine
                + "\t\tViewType.AreaPlan" + Environment.NewLine
                + "\t\tViewType.ThreeD";
            }
            else
            {
                message += "Summary of users view created:" + Environment.NewLine;
                foreach (View view in newUserViews)
                {
                    message += view.Name + Environment.NewLine;
                }
            }
            SCaddinsApp.WindowManager.ShowMessageBox(message);
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
            ReplaceParameterValues(newView);
            return newView;
        }

        private static string GetNewViewName(Document doc, Element sourceView)
        {
            if (doc == null || sourceView == null)
            {
                return string.Empty;
            }

            string name = ViewUtilitiesSettings.Default.UserViewNameFormat;
            name = ReplacePatternMatches(sourceView, name);

            // FIXME move the below method somewhere else
            if (SolarAnalysis.SolarAnalysisManager.ViewNameIsAvailable(doc, name))
            {
                return name;
            }
            else
            {
                return SolarAnalysis.SolarAnalysisManager.GetNiceViewName(doc, name);
            }
        }

        private static void ReplaceParameterValues(Element element)
        {
            var p1 = ViewUtilitiesSettings.Default.FirstParamName;
            var p2 = ViewUtilitiesSettings.Default.SecondParamName;
            var p3 = ViewUtilitiesSettings.Default.ThirdParamName;
            var v1 = ViewUtilitiesSettings.Default.FirstParamValue;
            var v2 = ViewUtilitiesSettings.Default.SecondParamValue;
            var v3 = ViewUtilitiesSettings.Default.ThirdParamValue;
            ReplaceParameterValue(p1, v1, element);
            ReplaceParameterValue(p2, v2, element);
            ReplaceParameterValue(p3, v3, element);
        }

        private static string ReplacePatternMatches(Element element, string name)
        {
            string user = Environment.UserName;
            string date = MiscUtilities.GetDateString;

            try
            {
                name = name.Replace(@"$user", user);
                name = name.Replace(@"$date", date);
            }
            catch
            {
                //// FIXME
            }

            string pattern = @"(<<)(.*?)(>>)";
            name = Regex.Replace(
                    name,
                    pattern,
                    m => RoomConverter.RoomConversionCandidate.GetParamValueAsString(ParamFromString(m.Groups[2].Value, element)));

            // Revit wont allow { or } so replace them if they exist
            name = name.Replace(@"{", string.Empty).Replace(@"}", string.Empty);

            return name;
        }

        private static void ReplaceParameterValue(string paramName, string value, Element element)
        {
            var param = ParamFromString(paramName, element);
            if (param != null && !string.IsNullOrEmpty(value))
            {
                if (!param.IsReadOnly)
                {
                    value = ReplacePatternMatches(element, value);
                    param.Set(value);
                }
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