// (C) Copyright 2021 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;

    public class NativeNamingRulesUtils
    {
#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
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
                if (GetIntVal(cid) == (int)BuiltInCategory.OST_ProjectInformation)
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
                        var p = param.First();
                        string paramValue;
                        try
                        {
                            paramValue = p.AsValueString();
                        } catch
                        {
                            paramValue = string.Empty;
                        }

                        if (p.Definition.Name == @"Current Revision" && paramValue.Length < 1)
                        {
                            paramValue = "Current Revision";
                        }
                        filenameTest += paramValue;
                    }
                }
                filenameTest += seg.Suffix;
                filenameTest += seg.Separator;
            }
            return filenameTest;
        }
#endif

#if REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025
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
            for (int i = 0; i < c.Length; i++)
            {
                prefix += c[i];

                if (i == c.Length - 1)
                {
                    if (scheme.Count > 0)
                    {
                        scheme[scheme.Count - 1].Suffix = prefix;
                    }
                }

                if (i > 0 && c[i] == '_' && c[i - 1] == '_')
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
                            seg.Prefix = prefix.Replace("_", string.Empty);
                            prefix = string.Empty;
                            scheme.Add(seg);
                        }
                    }
                    else
                    {
                        var seg = TableCellCombinedParameterData.Create();
                        prefix += c[i].ToString();
                        scheme.Add(seg);
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
                                        seg.Prefix = prefix.Replace("$", string.Empty);
                                        prefix = string.Empty;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetDescription":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_NAME);
                                        seg.Prefix = prefix.Replace("$", string.Empty);
                                        prefix = string.Empty;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetNumber":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_NUMBER);
                                        seg.Prefix = prefix.Replace("$", string.Empty);
                                        prefix = string.Empty;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetRevision":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_CURRENT_REVISION);
                                        seg.Prefix = prefix.Replace("$", string.Empty);
                                        prefix = string.Empty;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetRevisionDate":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_CURRENT_REVISION_DATE);
                                        seg.CategoryId = new ElementId(BuiltInCategory.OST_Revisions);
                                        if (Settings1.Default.ForceDateRevision || Settings1.Default.UseDateForEmptyRevisions)
                                        {
                                            seg.SampleValue = Common.MiscUtilities.GetDateString;
                                        }
                                        else
                                        {
                                            seg.SampleValue = string.Empty;
                                        }
                                        seg.Prefix = prefix.Replace("$", string.Empty);
                                        prefix = string.Empty;
                                        scheme.Add(seg);
                                        break;
                                    case "$sheetRevisionDescription":
                                        i += t.Length - 1;
                                        seg.ParamId = new ElementId(BuiltInParameter.SHEET_CURRENT_REVISION_DESCRIPTION);
                                        seg.Prefix = prefix.Replace("$", string.Empty);
                                        prefix = string.Empty;
                                        scheme.Add(seg);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return scheme;
        }
#endif
        private static int GetIntVal(ElementId id)
        {
#if REVIT2024 || REVIT2025
            return (int)id.Value;
#else
            return id.IntegerValue;
#endif
        }
    }
}
