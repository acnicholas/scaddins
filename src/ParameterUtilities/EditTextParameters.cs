// (C) Copyright 2012-2014 by Andrew Nicholas
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

namespace SCaddins.ParameterUtils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class EditTextParameters : IExternalCommand
    {
        private static bool commit = true;
        private static ConversionMode mode = ConversionMode.UpperCase;
        private static string dryRunLogText = string.Empty;
        
        public static void ConvertAll(ConversionMode mode, ConversionTypes types, Document doc)
        {
            commit = true;
            EditTextParameters.mode = mode;
            using (var trans = new Transaction(doc)) {
                if (trans.Start("Convert all selected types to uppercase (SCulcase)") == TransactionStatus.Started) {
                    Convert(mode, types, doc);
                    trans.Commit();
                }
            }
        }

        public static void ConvertAllDryRun(ConversionMode mode, ConversionTypes types, Document doc)
        {
            commit = false; 
            dryRunLogText = string.Empty;
            Convert(mode, types, doc);
            using (var info = new SCulcaseInfoDialog()) {
                info.SetText(dryRunLogText);
                info.Show();
            }
        }

        public static void ConvertSelection(ConversionMode mode, Document doc, IList<ElementId> elements)
        {
            if (elements == null || doc == null) {
                return;
            }
            commit = true;
            EditTextParameters.mode = mode;
            using (var trans = new Transaction(doc)) {
                trans.Start("Convert selected elements to uppercase (SCulcase)");
                foreach (Autodesk.Revit.DB.ElementId eid in elements) {
                    Element e = doc.GetElement(eid);
                    Category category = e.Category;
                    var enumCategory = (BuiltInCategory)category.Id.IntegerValue;
                    switch (enumCategory) {
                        case BuiltInCategory.OST_Views:
                            var v = (View)e;
                            ConvertViewName(v);
                            break;
                        case BuiltInCategory.OST_TextNotes:
                            var text = (TextElement)e;
                            ConvertAnnotation(text);
                            break;
                        case BuiltInCategory.OST_Rooms:
                            var room = (Room)e;
                            ConvertRoom(room);
                            break;
                    }
                }
                trans.Commit();
            }
        }

        public Autodesk.Revit.UI.Result Execute(
           ExternalCommandData commandData,
           ref string message,
           Autodesk.Revit.DB.ElementSet elements) {
            if (commandData == null) {
                return Result.Failed;
            }

            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIApplication application = commandData.Application;
            UIDocument document = application.ActiveUIDocument;
            IList<ElementId> elems = document.Selection.GetElementIds().ToList<ElementId>();
            using (var t = new TransactionGroup(doc, "SCulcase")) {
                t.Start();
                if (elems.Count == 0) {
                    using (var form = new SCulcaseMainForm(doc)) {
                        System.Diagnostics.Debug.Print(form.DialogResult.ToString());
                    }
                } else {
                    EditTextParameters.ConvertSelection(ConversionMode.UpperCase, doc, elems);
                }
                t.Commit();
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        private static void Convert(ConversionMode mode, ConversionTypes types, Document doc)
        {
            EditTextParameters.mode = mode;
            if (types.HasFlag(ConversionTypes.Text)) {
                ConvertAllAnnotation(doc);
            }
            if (types.HasFlag(ConversionTypes.ViewNames)) {
                ConvertAllViewNames(doc);
            }
            if (types.HasFlag(ConversionTypes.RoomNames)) {  
                ConvertAllRooms(doc);
            }
            if (types.HasFlag(ConversionTypes.SheetNames)) {
                ConvertAllSheetNames(doc);
            }
            if (types.HasFlag(ConversionTypes.TitlesOnSheets)) {
                ConvertAllViewNamesOnSheet(doc);
            }
        }

        private static void ConvertViewName(View view)
        {
            string newName = NewString(view.Name, mode);
            if (commit && ValidRevitName(newName)) {
                view.Name = newName;
            } else {
                DryRunLog("VIEW NAME", view.Name);
            }
        }

        private static void ConvertAllViewNames(Document doc)
        {
            using (var f = new FilteredElementCollector(doc)) {
                f.OfCategory(BuiltInCategory.OST_Views);
                foreach (Element e in f) {
                    var v = (View)e;
                    ConvertViewName(v);
                }
            }
        }

        private static void ConvertViewNameOnSheet(View view)
        {
            Parameter p = view.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION);
            if (p.AsString().Length > 0) {
                if (commit) {
                    p.Set(NewString(p.AsString(), mode));
                } else {
                    DryRunLog("TITLE ON SHEET", p.AsString());
                }
            }
        }

        private static void ConvertAllViewNamesOnSheet(Document doc)
        {
            using (var f = new FilteredElementCollector(doc)) {
                f.OfCategory(BuiltInCategory.OST_Views);
                foreach (Element e in f) {
                    var view = (View)e;
                    ConvertViewNameOnSheet(view);
                }
            }
        }

        private static void ConvertSheetName(ViewSheet viewSheet)
        {
            if (commit) {
                viewSheet.Name = NewString(viewSheet.Name, mode);
            } else {
                DryRunLog("SHEET NAME", viewSheet.Name);
            }
        }

        private static void ConvertAllSheetNames(Document doc)
        {
            using (var f = new FilteredElementCollector(doc)) {
                f.OfCategory(BuiltInCategory.OST_Sheets);
                foreach (Element e in f) {
                    var viewSheet = (ViewSheet)e;
                    ConvertSheetName(viewSheet);
                }
            }
        }

        private static void ConvertAnnotation(TextElement text)
        {
            if (commit) {
                text.Text = NewString(text.Text, mode);
            } else {
                DryRunLog("GENERAL ANNOTATION", text.Text);
            }
        }

        private static void ConvertAllAnnotation(Document doc)
        {
            using (var f = new FilteredElementCollector(doc)) {
                f.OfCategory(BuiltInCategory.OST_TextNotes);
                foreach (Element e in f) {
                    var text = (TextElement)e;
                    ConvertAnnotation(text);
                }
            }
        }

        private static void ConvertRoom(Room room)
        {
            Parameter param = room.LookupParameter("Name");
            if (commit) {
                param.Set(NewString(param.AsString(), mode));
            } else {
                DryRunLog("ROOM NAME", param.AsString());
            }
        }

        private static void ConvertAllRooms(Document doc)
        {
            using (var f = new FilteredElementCollector(doc)) {
                f.OfCategory(BuiltInCategory.OST_Rooms);
                foreach (Element e in f) {
                    var room = (Room)e;
                    ConvertRoom(room);
                }
            }
        }

        private static string NewString(string oldString, ConversionMode mode)
        {
            switch (mode) {
                case ConversionMode.UpperCase:
                    return oldString.ToUpper(CultureInfo.CurrentCulture);
                case ConversionMode.LowerCase:
                    return oldString.ToLower(CultureInfo.CurrentCulture);
                case ConversionMode.TitleCase:
                    CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;
                    return textInfo.ToTitleCase(oldString.ToLower(CultureInfo.CurrentCulture));
                default:
                    return oldString.ToUpper(CultureInfo.CurrentCulture);
            }
        }

        private static void DryRunLog(string etype, string s)
        {
            dryRunLogText += etype + " --- " + s + " ---> " + NewString(s, mode);
            dryRunLogText += System.Environment.NewLine;
        }

        private static bool ValidRevitName(string s)
        {
            return !(s.Contains("{") || s.Contains("}"));
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
