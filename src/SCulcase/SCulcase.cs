using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
using System.Globalization;

namespace SCaddins.SCulcase
{
    public static class SCulcase
    {
    	private static bool commit = true;
    	private static ConversionMode mode = ConversionMode.UPPER_CASE;
    	public static string DryRunLogText = "";
    	
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
            convert(mode, types, ref doc);
            trans.Commit();
        }
        
        private static void convert(ConversionMode mode, ConversionTypes types, ref Document doc)
        {
            SCulcase.mode = mode;
            if (types.HasFlag(SCulcase.ConversionTypes.TEXT)) ConvertAllAnnotation(ref doc);
            if (types.HasFlag(SCulcase.ConversionTypes.VIEW_NAMES)) ConvertAllViewNames(ref doc);
            if (types.HasFlag(SCulcase.ConversionTypes.ROOM_NAMES)) ConvertAllRooms(ref doc);
            if (types.HasFlag(SCulcase.ConversionTypes.SHEET_NAMES)) ConvertAllSheetNames(ref doc);
            if (types.HasFlag(SCulcase.ConversionTypes.TITLES_ON_SHEETS)) ConvertAllViewNamesOnSheet(ref doc);
        }
        
        public static void ConvertAllDryRun(ConversionMode mode, ConversionTypes types, ref Document doc)
        {
            commit = false; 
            DryRunLogText = "";
            convert(mode, types, ref doc);
            SCulcaseInfoDialog info = new SCulcaseInfoDialog();
            info.setText(DryRunLogText);
            info.Show();
        }
        
        public static void ConvertSelection(ConversionMode mode, ref Document doc, ref ElementSet elements )
        {
                commit = true;
                SCulcase.mode = mode;
                Transaction trans = new Transaction(doc);
                trans.Start("Convert selected elements to uppercase (SCulcase)");
                foreach (Autodesk.Revit.DB.Element  e in elements)
                {      
					 Category category = e.Category;
				     BuiltInCategory enumCategory = (BuiltInCategory)category.Id.IntegerValue;
				     switch(enumCategory){
				     	case BuiltInCategory.OST_Views:
				     		View v = (View)e;
                			convertViewName(ref v);	
                			break;
                        case BuiltInCategory.OST_TextNotes:
				     		TextElement text = (TextElement)e;
                			convertAnnotation(ref text);	
                			break;
                	    case BuiltInCategory.OST_Rooms:
				     		Room room = (Room)e;
                            convertRoom(ref room);
                			break;
				     }
                }
                trans.Commit();
        }
                
        private static void convertViewName(ref View view)
        {
                string newName = newString(view.Name,mode);
                if (commit && ValidRevitName(newName)){
                    view.Name = newName;
                }else{
                    dryRunLog("VIEW NAME", view.Name);
                }
        }
        
        private static void ConvertAllViewNames(ref Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Views);
            foreach (Element e in f) {
                View v = (View)e;
                convertViewName(ref v);
            }
        }

        private static void convertViewNameOnSheet(ref View view)
        {
                Parameter p = view.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION);
                if (p.AsString().Length > 0) {
                    if(commit){
                        p.Set(newString(p.AsString(),mode));
                    }else{
                        dryRunLog("TITLE ON SHEET",p.AsString());
                    }
                }
        }
        
        private static void ConvertAllViewNamesOnSheet(ref Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Views);
            foreach (Element e in f) {
                View view = (View)e;
                convertViewNameOnSheet(ref view);
            }
        }
        
        private static void convertSheetName(ref ViewSheet viewSheet)
        {
                if(commit){
                    viewSheet.Name = newString(viewSheet.Name,mode);
                }else{
                    dryRunLog("SHEET NAME", viewSheet.Name);
                }
        }

        private static void ConvertAllSheetNames(ref Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Sheets);
            foreach (Element e in f) {
                ViewSheet viewSheet = (ViewSheet)e;
                convertSheetName(ref viewSheet);
            }
        }
        
        private static void convertAnnotation(ref TextElement text)
        {
            if(commit){
                text.Text = newString(text.Text,mode);
            }else{
                dryRunLog("GENERAL ANNOTATION", text.Text);
            }
        }

        private static void ConvertAllAnnotation(ref Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_TextNotes);
            foreach (Element e in f) {
                TextElement text = (TextElement)e;
                convertAnnotation(ref text);
            }
        }
        
        private static void convertRoom(ref Room room)
        {
        	Parameter param = room.get_Parameter("Name");
            if(commit){
                //room.Name = newString(room.Name,mode);
                param.Set(newString(param.AsString(),mode));
            }else{
                dryRunLog("ROOM NAME",param.AsString());
            }
        }

        private static void ConvertAllRooms(ref Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Rooms);
            foreach (Element e in f) {
                Room room = (Room)e;
                convertRoom(ref room);
            }
        }

        //TODO
        private static void ConvertAllRevisionDescriptions(ref Document doc)
        {
            FilteredElementCollector f = new FilteredElementCollector(doc);
            f.OfCategory(BuiltInCategory.OST_Revisions);
            foreach (Element e in f) {
            	foreach (Parameter p in e.Parameters) {
            		if(p.Definition.Name.ToString().Equals("Revision Description")){
            				if (!p.IsReadOnly){
                                string v = newString(p.AsString(),mode);
            					p.Set(v);
            				}
            		}
            	}
            }
        }

        private static string newString(string oldString, ConversionMode mode)
        {
            switch(mode){
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
              
        private static void dryRunLog(string etype, string s)
        {
            DryRunLogText +=  etype + " --- " + s + " ---> " + newString(s,mode);
            DryRunLogText +=  System.Environment.NewLine;
        }
        
        private static bool ValidRevitName(String s)
        {
            if(s.Contains("{") || s.Contains("}")){
                return false;
            }else{
                return true;
            }
        }

    }
}
