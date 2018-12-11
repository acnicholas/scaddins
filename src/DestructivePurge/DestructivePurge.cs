// (C) Copyright 2013-2015 by Andrew Nicholas andrewnicholas@iinet.net.au
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

namespace SCaddins.DestructivePurge
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.Exceptions;

    public static class DestructivePurgeUtilitiles
    {
        public static List<DeletableItem> Images(Document doc)
        {
            var result = new List<DeletableItem>();
            using (var f = new FilteredElementCollector(doc))
            {
                f.OfCategory(BuiltInCategory.OST_RasterImages);
                foreach (Element image in f)
                {
                    string s = GetParameterList(image.Parameters);
                    var tn = new DeletableItem(image.Name.ToString());
                    tn.Info = "Name = " + image.Name.ToString() + System.Environment.NewLine +
                    "id - " + image.Id.ToString();
                    tn.Info += System.Environment.NewLine + s;
                    tn.Id = image.Id;

                    ElementId typeId = image.GetTypeId();
                    tn.ParentId = typeId;
                    ImageType type = doc.GetElement(typeId) as ImageType;
                    try
                    {
                        tn.PreviewImage = type.GetImage();
                        tn.HasParent = true;
                    }
                    catch
                    {
                        tn.PreviewImage = null;
                    }
                    result.Add(tn);
                }
            }
            return result;
        }

        public static List<DeletableItem> Imports(Document doc, bool linked)
        {
            var result = new List<DeletableItem>();
            using (var f = new FilteredElementCollector(doc))
            {
                f.OfClass(typeof(ImportInstance));
                string s = string.Empty;
                string name = string.Empty;
                foreach (ImportInstance ii in f)
                {
                    if (ii.IsLinked == linked)
                    {
                        s = string.Empty;
                        s += "View Specific - " + ii.ViewSpecific.ToString(System.Globalization.CultureInfo.CurrentCulture) + System.Environment.NewLine;
                        s += "Owner view id - " + ii.OwnerViewId + System.Environment.NewLine;
                        ParameterSet p = ii.Parameters;
                        foreach (Parameter param in p)
                        {
                            s += param.Definition.Name + " - " + param.AsString() + System.Environment.NewLine;
                            if (param.Definition.Name == "Name")
                            {
                                name = param.AsString();
                            }
                        }
                        s += "Element id - " + ii.Id;
                        var tn = new DeletableItem(name);
                        tn.Id = ii.Id;
                        tn.Info = s;
                        result.Add(tn);
                    }
                }
            }
            return result;
        }

        public static void RemoveElements(Document doc, List<DeletableItem> elements)
        {
            if (elements == null || doc == null) {
                return;
            }
            if (elements.Count < 1) {
                return;
            }

            using (var t = new Transaction(doc))
            {
                if (t.Start("Delete Elements") == TransactionStatus.Started) {
                    foreach (DeletableItem di in elements) {
                        if (di.Id == null) {
                            continue;
                        }
                        try {
                            if (!doc.GetElement(di.Id).IsValidObject) {
                                continue;
                            }
                        } catch {
                            continue;
                        }

                        if (doc.ActiveView.Id == di.Id) {
                            continue;
                        }
                        if (di.ParentId == null) {
                            continue;
                        }
                        if (di.HasParent && di.ParentId == ElementId.InvalidElementId) {
                            continue;
                        }
                        if (di.HasParent) {
                            try {
                                if (!doc.GetElement(di.ParentId).IsValidObject) {
                                    continue;
                                }
                            } catch {
                                continue;
                            }
                        }

                        try {
                                ICollection<Autodesk.Revit.DB.ElementId> deletedIdSet = doc.Delete(di.Id);
                        } catch (ArgumentNullException anex) {
                            Autodesk.Revit.UI.TaskDialog.Show("Failure", di.Id.ToString() + System.Environment.NewLine + anex.Message);
                        } catch (ModificationForbiddenException mfex) {
                            Autodesk.Revit.UI.TaskDialog.Show("Failure", di.Id.ToString() + System.Environment.NewLine + mfex.Message);
                        }
                    }

                    if (t.Commit() != TransactionStatus.Committed) {
                        Autodesk.Revit.UI.TaskDialog.Show("Failure", "Destructive Purge could not be run");
                    } 
                }
            }
        }

        public static List<DeletableItem> Revisions(Document doc)
        {
            var result = new List<DeletableItem>();
            using (var f = new FilteredElementCollector(doc))
            {
                f.OfCategory(BuiltInCategory.OST_Revisions);
                foreach (Element revision in f)
                {
                    string s = GetParameterList(revision.Parameters);
                    var nodeName = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString() + " - " +
                        revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DESCRIPTION).AsString();
                    var tn = new DeletableItem(nodeName);
                    tn.Info = "Name = " + revision.Name + System.Environment.NewLine +
                    "id - " + revision.Id.ToString();
                    tn.Info += System.Environment.NewLine + s;
                    tn.Id = revision.Id;
                    if (revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_SEQUENCE_NUM).AsInteger() > 1) {
                        result.Add(tn);
                    }
                }
            }
            return result;
        }

        public static List<DeletableItem> Sheets(Document doc, bool placedOnSheet)
        {
            var result = new List<DeletableItem>();
            result.AddRange(DestructivePurgeUtilitiles.Views(doc, placedOnSheet, ViewType.DrawingSheet));
            return result;
        }

        public static System.Windows.Media.Imaging.BitmapImage ToBitmapImage(this System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null) {
                return null;
            }
            var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            using (var memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        public static List<DeletableItem> UnboundRooms(Document doc)
        {
            var result = new List<DeletableItem>();
            using (var f = new FilteredElementCollector(doc))
            {
                f.OfCategory(BuiltInCategory.OST_Rooms);
                foreach (Element room in f)
                {
                    string s = string.Empty;
                    bool bound = false;
                    ParameterSet p = room.Parameters;
                    foreach (Parameter param in p)
                    {
                        if (param.HasValue)
                        {
                            s += param.Definition.Name + " - " + param.AsString() + param.AsValueString() + System.Environment.NewLine;
                        }
                        if (param.Definition.Name == "Area" && param.AsDouble() > 0)
                        {
                            bound = true;
                        }
                    }
                    var tn = new DeletableItem(room.Name);
                    tn.Info = "Name = " + room.Name + System.Environment.NewLine +
                    "id - " + room.Id.ToString();
                    tn.Info += System.Environment.NewLine + s;
                    tn.Id = room.Id;
                    if (!bound)
                    {
                        result.Add(tn);
                    }
                }
            }
            return result;
        }

        public static List<DeletableItem> UnusedViewFilters(Document doc)
        {
            Dictionary<ElementId, ElementId> usedFilters = new Dictionary<ElementId, ElementId>();
            using (var viewCollecter = new FilteredElementCollector(doc))
            {
                viewCollecter.OfClass(typeof(Autodesk.Revit.DB.View));
                foreach (Autodesk.Revit.DB.View view in viewCollecter)
                {
                    if (view.AreGraphicsOverridesAllowed())
                    {
                        foreach (ElementId id in view.GetFilters())
                        {
                            if (!usedFilters.ContainsKey(id))
                            {
                                usedFilters.Add(id, id);
                            }
                        }
                    }
                }
            }

            var result = new List<DeletableItem>();
            using (var f = new FilteredElementCollector(doc))
            {
                f.OfClass(typeof(Autodesk.Revit.DB.FilterElement));
                foreach (Element filter in f)
                {
                    if (!usedFilters.ContainsKey(filter.Id))
                    {
                        string s = GetParameterList(filter.Parameters);
                        var nodeName = filter.Name;
                        var tn = new DeletableItem(nodeName);
                        tn.Info = "Name = " + filter.Name + System.Environment.NewLine +
                        "id - " + filter.Id.ToString();
                        tn.Info += System.Environment.NewLine + s;
                        tn.Id = filter.Id;
                        result.Add(tn);
                    }
                }
            }
            return result;
        }

        // FIXME add view templates and project browser views to new category
        public static List<DeletableItem> Views(Document doc, bool placedOnSheet, ViewType type)
        {
            var result = new List<DeletableItem>();
            using (var f = new FilteredElementCollector(doc))
            {
                f.OfClass(typeof(Autodesk.Revit.DB.View));
                foreach (Autodesk.Revit.DB.View view in f)
                {
                    if (view.ViewType == type && !view.IsTemplate)
                    {
                        string s = string.Empty;
                        string d = string.Empty;
                        string num = string.Empty;
                        bool os = false;

                        Parameter p = GetParameterByName(view, "Dependency");

                        s += "Name - " + view.Name + System.Environment.NewLine;
                        if (p != null)
                        {
                            d = p.AsString();
                            if (d == "Primary")
                            {
                                s += "Dependency - " + d + " [May be safe to delete]" + System.Environment.NewLine;
                                os = true;
                            }
                            else
                            {
                                s += "Dependency - " + d + System.Environment.NewLine;
                            }
                        }

                        Parameter p2 = GetParameterByName(view, "Sheet Number");
                        if (p2 != null)
                        {
                            num = p2.AsString();
                            s += "Sheet Number - " + num + System.Environment.NewLine;
                            os |= num != "---" && !string.IsNullOrEmpty(num);
                        }
                        else
                        {
                            s += @"Sheet Number - N/A" + System.Environment.NewLine;
                        }
                        s += "Element id - " + view.Id.ToString() + System.Environment.NewLine;
                        s += System.Environment.NewLine + "[EXTENDED INFO]" + System.Environment.NewLine;
                        s += GetParameterList(view.Parameters);

                        string n = string.Empty;
                        if (type == ViewType.DrawingSheet)
                        {
                            n = num + " - " + view.Name;
                        }
                        else
                        {
                            n = view.Name;
                        }

                        var tn = new DeletableItem(n);
                        tn.Info = s;
                        tn.Id = view.Id;
                        tn.HasParent = s.Contains(@"Parent View");
                        if (tn.HasParent) {
                            Parameter parentId = view.GetParameters(@"Parent View")[0];
                            var pId = parentId.AsElementId();
                            tn.ParentId = pId;
                        }
                        if (view.ViewType == ViewType.ProjectBrowser || view.ViewType == ViewType.SystemBrowser)
                        {
                            continue;
                        }

                        if (view.ViewType != ViewType.Internal)
                        {
                            if (os && placedOnSheet)
                            {
                                result.Add(tn);
                            }
                            if (!os && !placedOnSheet)
                            {
                                result.Add(tn);
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static Parameter GetParameterByName(
            Autodesk.Revit.DB.Element view,
            string parameterName)
        {
            return view.LookupParameter(parameterName);
        }

        private static string GetParameterList(ParameterSet p)
        {
            string s = string.Empty;
            foreach (Parameter param in p)
            {
                if (param.HasValue)
                {
                    s += param.Definition.Name + " - " + param.AsString() + param.AsValueString() + System.Environment.NewLine;
                }
            }
            return s;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */