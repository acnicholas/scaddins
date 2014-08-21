namespace SCaddins.SCulcase
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;

    public static class SCulcase
    {
        private static bool commit = true;
        private static ConversionMode mode = ConversionMode.UPPER_CASE;
        public static string DryRunLogText = string.Empty;

        /// <summary>
        /// Types of elements to convert
        /// </summary>
        [Flags]
        public enum ConversionTypes
        {
            None = 0,
            TEXT = 1,
            SHEET_NAMES = 2,
            VIEW_NAMES = 4,
            TITLES_ON_SHEETS = 8,
            ROOM_NAMES = 16
        }
        
        /// <summary>
        /// Conversion Type
        /// </summary>
        public enum ConversionMode
        {
            UPPER_CASE,
            LOWER_CASE,
            TITLE_CASE
        }
        
        public static void ConvertAll(ConversionMode mode, ConversionTypes types, ref Document doc)
        {
            commit = true;
            SCulcase.mode = mode;
            Transaction trans = new Transaction(doc);
            trans.Start("Convert all selected types uppercase (SCulcase)");
            Convert(mode, types, ref doc);
            trans.Commit();
        }
        
        private static void Convert(ConversionMode mode, ConversionTypes types, ref Document doc)
        {
            SCulcase.mode = mode;
            if (types.HasFlag(SCulcase.ConversionTypes.TEXT)) {
                ConvertAllAnnotation(ref doc);
            }
            if (types.HasFlag(SCulcase.ConversionTypes.VIEW_NAMES)) {
                ConvertAllViewNames(ref doc);
            }
            if (types.HasFlag(SCulcase.ConversionTypes.ROOM_NAMES)) {  
                ConvertAllRooms(ref doc);
            }
            if (types.HasFlag(SCulcase.ConversionTypes.SHEET_NAMES)) {
                ConvertAllSheetNames(ref doc);
            }
            if (types.HasFlag(SCulcase.ConversionTypes.TITLES_ON_SHEETS)) {
                ConvertAllViewNamesOnSheet(ref doc);
            }
        }
        
        public static void ConvertAllDryRun(ConversionMode mode, ConversionTypes types, ref Document doc)
        {
            commit = false; 
            DryRunLogText = string.Empty;
            Convert(mode, types, ref doc);
            SCulcaseInfoDialog info = new SCulcaseInfoDialog();
            info.SetText(DryRunLogText);
            info.Show();
        }
        
        public static void ConvertSelection(ConversionMode mode, ref Document doc, ref ElementSet elements)
        {
            commit = true;
            SCulcase.mode = mode;
            Transaction trans = new Transaction(doc);
            trans.Start("Convert selected elements to uppercase (SCulcase)");
            foreach (Autodesk.Revit.DB.Element e in elements) {      
                Category category = e.Category;
                BuiltInCategory enumCategory = (BuiltInCategory)category.Id.IntegerValue;
                switch (enumCategory) {
                    case BuiltInCategory.OST_Views:
                        View v = (View)e;
                        ConvertViewName(ref v);
                        break;
                    case BuiltInCategory.OST_TextNotes:
                        TextElement text = (TextElement)e;
                        ConvertAnnotation(ref text);
                        break;
                    case BuiltInCategory.OST_Rooms:
                        Room room = (Room)e;
                        ConvertRoom(ref room);
                        break;
                }
            }
            trans.Commit();
        }
                
        private static void ConvertViewName(ref View view)
        {
            string newName = NewString(view.Name, mode);
            if (commit && ValidRevitName(newName)) {
                view.Name = newName;
            } else {
                DryRunLog("VIEW NAME", view.Name);
            }
        }
        
        private static void ConvertAllViewNames(ref Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Views);
            foreach (Element e in f) {
                View v = (View)e;
                ConvertViewName(ref v);
            }
        }

        private static void ConvertViewNameOnSheet(ref View view)
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
        
        private static void ConvertAllViewNamesOnSheet(ref Document doc)
        {
            var f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Views);
            foreach (Element e in f) {
                View view = (View)e;
                ConvertViewNameOnSheet(ref view);
            }
        }
        
        private static void ConvertSheetName(ref ViewSheet viewSheet)
        {
            if (commit) {
                viewSheet.Name = NewString(viewSheet.Name, mode);
            } else {
                DryRunLog("SHEET NAME", viewSheet.Name);
            }
        }

        private static void ConvertAllSheetNames(ref Document doc)
        {
            var f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Sheets);
            foreach (Element e in f) {
                ViewSheet viewSheet = (ViewSheet)e;
                ConvertSheetName(ref viewSheet);
            }
        }
        
        private static void ConvertAnnotation(ref TextElement text)
        {
            if (commit) {
                text.Text = NewString(text.Text, mode);
            } else {
                DryRunLog("GENERAL ANNOTATION", text.Text);
            }
        }

        private static void ConvertAllAnnotation(ref Document doc)
        {
            var f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_TextNotes);
            foreach (Element e in f) {
                var text = (TextElement)e;
                ConvertAnnotation(ref text);
            }
        }
        
        private static void ConvertRoom(ref Room room)
        {
            Parameter param = room.get_Parameter("Name");
            if (commit) {
                param.Set(NewString(param.AsString(), mode));
            } else {
                DryRunLog("ROOM NAME", param.AsString());
            }
        }

        private static void ConvertAllRooms(ref Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Rooms);
            foreach (Element e in f) {
                Room room = (Room)e;
                ConvertRoom(ref room);
            }
        }

        // TODO
        private static void ConvertAllRevisionDescriptions(ref Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Revisions);
            foreach (Element e in f) {
                foreach (Parameter p in e.Parameters) {
                    if (p.Definition.Name.ToString().Equals("Revision Description")) {
                        if (!p.IsReadOnly) {
                            string v = NewString(p.AsString(), mode);
                            p.Set(v);
                        }
                    }
                }
            }
        }

        private static string NewString(string oldString, ConversionMode mode)
        {
            switch (mode) {
                case ConversionMode.UPPER_CASE:
                    return oldString.ToUpper();
                case ConversionMode.LOWER_CASE:
                    return oldString.ToLower();
                case ConversionMode.TITLE_CASE:
                    CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;
                    return textInfo.ToTitleCase(oldString.ToLower());
                default:
                    return oldString.ToUpper();
            }
        }
              
        private static void DryRunLog(string etype, string s)
        {
            DryRunLogText += etype + " --- " + s + " ---> " + NewString(s, mode);
            DryRunLogText += System.Environment.NewLine;
        }
        
        private static bool ValidRevitName(string s)
        {
            if (s.Contains("{") || s.Contains("}")) {
                return false;
            } else {
                return true;
            }
        }
    }
}
