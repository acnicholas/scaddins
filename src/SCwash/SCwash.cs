// (C) Copyright 2013-2014 by Andrew Nicholas andrewnicholas@iinet.net.au
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

namespace SCaddins.SCwash
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;

    public static class SCwash
    {
        public static List<SCwashTreeNode> Imports(Document doc, bool linked)
        {
            List<SCwashTreeNode> result = new List<SCwashTreeNode>();
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfClass(typeof(ImportInstance));
            string s = string.Empty;
            string name = string.Empty;
            foreach (ImportInstance ii in f) {
                if (ii.IsLinked == linked) {
                    s = string.Empty;
                    s += "View Specific - " + ii.ViewSpecific.ToString() + System.Environment.NewLine;
                    s += "Owner view id - " + ii.OwnerViewId + System.Environment.NewLine;
                    ParameterSet p = ii.Parameters;
                    foreach (Parameter param in p) {
                        s += param.Definition.Name + " - " + param.AsString() + System.Environment.NewLine;
                        if (param.Definition.Name == "Name") {
                            name = param.AsString();
                        }
                    }
                    s += "Element id - " + ii.Id;
                    var tn = new SCwashTreeNode(name);
                    tn.Id = ii.Id;
                    tn.Info = s;
                    result.Add(tn);
                }
            }
            return result;
        }

        public static List<SCwashTreeNode> Images(Document doc)
        {
            List<SCwashTreeNode> result = new List<SCwashTreeNode>();
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_RasterImages);         
            foreach (Element image in f) {
                string s = string.Empty;
                ParameterSet p = image.Parameters;
                foreach (Parameter param in p) {
                    if (param.HasValue) {
                        s += param.Definition.Name + " - " + param.AsString() + param.AsValueString() + System.Environment.NewLine;
                    }
                }
                var tn = new SCwashTreeNode(image.Name.ToString());
                tn.Info = "Name = " + image.Name.ToString() + System.Environment.NewLine +
                "id - " + image.Id.ToString();
                tn.Info += System.Environment.NewLine + s;
                tn.Id = image.Id;
                result.Add(tn);
            }
            return result;
        }
        
        public static List<SCwashTreeNode> Revisions(Document doc)
        {
            List<SCwashTreeNode> result = new List<SCwashTreeNode>();
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Revisions);         
            foreach (Element revision in f) {
                string s = string.Empty;
                ParameterSet p = revision.Parameters;
                foreach (Parameter param in p) {
                    if (param.HasValue) {
                        s += param.Definition.Name + " - " + param.AsString() + param.AsValueString() + System.Environment.NewLine;
                    }
                }
                var nodeName = revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DATE).AsString() + " - " +
                    revision.get_Parameter(BuiltInParameter.PROJECT_REVISION_REVISION_DESCRIPTION).AsString();
                var tn = new SCwashTreeNode(nodeName);
                tn.Info = "Name = " + revision.Name.ToString() + System.Environment.NewLine +
                "id - " + revision.Id.ToString();
                tn.Info += System.Environment.NewLine + s;
                tn.Id = revision.Id;
                result.Add(tn);
            }
            return result;
        }

        public static List<SCwashTreeNode> UnboundRooms(Document doc)
        {
            List<SCwashTreeNode> result = new List<SCwashTreeNode>();
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Rooms);         
            foreach (Element room in f) {
                string s = string.Empty;
                bool bound = false;
                ParameterSet p = room.Parameters;
                foreach (Parameter param in p) {
                    if (param.HasValue) {
                        s += param.Definition.Name + " - " + param.AsString() + param.AsValueString() + System.Environment.NewLine;
                    }
                    if (param.Definition.Name == "Area" && param.AsDouble() > 0) {
                        bound = true;
                    }
                }
                var tn = new SCwashTreeNode(room.Name.ToString());
                tn.Info = "Name = " + room.Name.ToString() + System.Environment.NewLine +
                "id - " + room.Id.ToString();
                tn.Info += System.Environment.NewLine + s;
                tn.Id = room.Id;
                if (!bound) {
                    result.Add(tn);
                }
            }
            return result;
        }

        public static void AddSheetNodes(Document doc, bool placedOnSheet, TreeNodeCollection nodes)
        {
            nodes.AddRange(SCwash.Views(doc, placedOnSheet, ViewType.DrawingSheet).ToArray<TreeNode>());
        }

        public static void AddViewNodes(Document doc, bool placedOnSheet, TreeNodeCollection nodes)
        {
            int i = 0;
            foreach (ViewType enumValue in Enum.GetValues(typeof(ViewType))) {
                if (enumValue != ViewType.DrawingSheet) {
                    nodes.Add(new SCwashTreeNode(enumValue.ToString()));
                    nodes[i].Nodes.AddRange(SCwash.Views(doc, placedOnSheet, enumValue).ToArray<TreeNode>());
                    if (nodes[i].Nodes.Count < 1) {
                        nodes.Remove(nodes[i]);
                    } else {
                        i++;
                    }
                }
            }
        }

        public static void RemoveElements(Document doc, ICollection<ElementId> elements)
        {
            if (elements.Count < 1) return;
            var t = new Transaction(doc, "Delete Elements"); 
            t.Start();
            ICollection<Autodesk.Revit.DB.ElementId> deletedIdSet = doc.Delete(elements);
            //if (0 == deletedIdSet.Count) {
            //    throw new Exception("Deleting the selected elements in Revit failed.");
            //}
            t.Commit();
        }

        // FIXME don't add view templates or project browser views
        private static List<SCwashTreeNode> Views(Document doc, bool placedOnSheet, ViewType type)
        {
            var result = new List<SCwashTreeNode>();
            var f = new FilteredElementCollector(doc);
            f.OfClass(typeof(Autodesk.Revit.DB.View));
            string s = string.Empty;
            foreach (Autodesk.Revit.DB.View view in f) {  
                if (view.ViewType == type) {
                    s = string.Empty;
                    string d = string.Empty;
                    string num = string.Empty;
                    bool os = false;
                    #if REVIT2014
                    Parameter p = view.get_Parameter("Dependency");
                    #else
                    Parameter p = view.LookupParameter("Dependency");
                    #endif
                    s += "Name - " + view.Name + System.Environment.NewLine;
                    if (p != null) {
                        d = p.AsString();
                        if (d == "Primary") {
                            os = true;
                        }
                        s += "Dependency - " + d + System.Environment.NewLine;
                    }
                    #if REVIT2014
                    Parameter p2 = view.get_Parameter("Sheet Number");
                    #else
                    Parameter p2 = view.LookupParameter("Sheet Number");
                    #endif
                    if (p2 != null) {
                        num = p2.AsString();
                        s += "Sheet Number - " + num + System.Environment.NewLine;
                        os = true;
                    } else {
                        s += @"Sheet Number - N/A" + System.Environment.NewLine;
                    }
                    s += "Element id - " + view.Id.ToString();
                    string n = string.Empty;
                    if (type == ViewType.DrawingSheet) {
                        n = num + " - " + view.Name;
                    } else {
                        n = view.Name;
                    }
                    var tn = new SCwashTreeNode(n);
                    tn.Info = s;
                    tn.Id = view.Id;
                    if (view.ViewType == ViewType.ProjectBrowser) {
                        continue;
                    }
                    if (view.ViewType == ViewType.SystemBrowser) {
                        continue;
                    }
                    if (view.ViewType != ViewType.Internal) {
                        if (os && placedOnSheet) {
                            result.Add(tn);
                        } else if (!placedOnSheet && !os) {
                            result.Add(tn);
                        }
                    }
                }
            }
            return result;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
