namespace SCaddins.ExportManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Autodesk.Revit.DB;
    using SCaddins.Common;

    public class NativeNamingRulesUtils
    {
        #if REVIT2022
        public static string GetExportNameFromNamingRule(PDFExportOptions opts, ExportSheet vs)
        {
            var segs = opts.GetNamingRule();
            string filenameTest = string.Empty;
            foreach (var seg in segs)
            {
                filenameTest += seg.Prefix;
                var pid = seg.ParamId;
                var cid = seg.CategoryId;
                var cidString = cid.ToString();
                if (cid.IntegerValue == (int)BuiltInCategory.OST_ProjectInformation)
                {
                    var param = vs.Sheet.Document.ProjectInformation.Parameters.Cast<Parameter>().Where(p => p.Id == pid);
                    if (param.Count() > 0)
                    {
                        var paramValue = param.First().AsValueString();
                        filenameTest += paramValue;
                    }
                }
                else
                {
                    var param = vs.Sheet.Parameters.Cast<Parameter>().Where(p => p.Id == pid);
                    if (param.Count() > 0)
                    {
                        if (param.First().Definition.Name == "Current Revision" && vs.ForceDate)
                        {
                            filenameTest += MiscUtilities.GetDateString;
                        }
                        else
                        {
                            var paramValue = param.First().AsValueString();
                            if (paramValue.Length < 1 && vs.UseDateForEmptyRevisions)
                            {
                                filenameTest += MiscUtilities.GetDateString;
                            }
                            else
                            {
                                filenameTest += paramValue;
                            }
                        }
                    }
                }
                filenameTest += seg.Suffix;
                filenameTest += seg.Separator;
            }
            return filenameTest;
        }
        #endif

        #if REVIT2022
        public static List<TableCellCombinedParameterData> CreateNamingRuleFromFormatString(string filenameScheme, Document doc)
        {
            var fec = new FilteredElementCollector(doc);
            fec.OfClass(typeof(ViewSheet));
            var sheetParam = fec.First() as ViewSheet;

            string prefix = string.Empty;

            string[] slib = {
                "$height",
                "$width",
                "$fullExportName",
                "$fullExportPath",
                "$exportDir",
                "$pageSize",
                "$projectNumber",
                "$sheetDescription",
                "$sheetNumber",
                "$sheetRevisionDescription",
                "$sheetRevisionDate",
                "$sheetRevision"
             };

            var scheme = new List<TableCellCombinedParameterData>();

            string s = filenameScheme;

            char[] c = s.ToCharArray();
            for (int i = 0; i < c.Length - 1; i++)
            {
                if (c[i] != '_' && c[i] != '$')
                {
                    prefix += c[i];
                }

                if (c[i] == '_')
                {
                    i++;
                    if (c[i] == '_')
                    {
                        var n = s.Substring(i);
                        if (n.Contains(@"__"))
                        {
                            var ni = n.IndexOf(@"__");
                            i += ni;
                            i += 1;
                            var customParamName = n.Substring(1, ni - 1);
                            var seg = TableCellCombinedParameterData.Create();
                            var p = sheetParam.GetParameters(customParamName);
                            if (p.Count > 0)
                            {
                                seg.ParamId = p[0].Id;
                                seg.Prefix = prefix;
                                scheme.Add(seg);
                            }
                        }
                        prefix = string.Empty;
                    }
                }

                if (c[i] == '$')
                {
                    foreach (string t in slib)
                    {
                        if (t.Length + i <= s.Length)
                        {
                            if (s.Substring(i, t.Length) == t)
                            {
                                var seg = TableCellCombinedParameterData.Create();
                                switch (t)
                                {
                                    case "$height":
                                        break;
                                    case "$width":
                                        break;
                                    case "$fullExportName":
                                        break;
                                    case "$fullExportPath":
                                        break;
                                    case "$exportDir":
                                        break;
                                    case "$pageSize":
                                        break;
                                    case "$projectNumber":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.PROJECT_NUMBER);
                                        seg.CategoryId = new ElementId(BuiltInCategory.OST_ProjectInformation);
                                        seg.Prefix = prefix;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetDescription":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_NAME);
                                        seg.Prefix = prefix;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetNumber":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_NUMBER);
                                        seg.Prefix = prefix;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetRevision":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_CURRENT_REVISION);
                                        seg.Prefix = prefix;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetRevisionDate":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_CURRENT_REVISION_DATE);
                                        seg.CategoryId = new ElementId(BuiltInCategory.OST_Revisions);
                                        seg.Prefix = prefix;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetRevisionDescription":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_CURRENT_REVISION_DESCRIPTION);
                                        seg.Prefix = prefix;
                                        scheme.Add(seg);
                                        break;
                                }
                                prefix = string.Empty;
                            }
                        }
                    }
                }
            }
            return scheme;
        }
        #endif
    }
}
