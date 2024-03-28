// (C) Copyright 2017-2021 by Andrew Nicholas
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

namespace SCaddins.RenameUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Autodesk.Revit.DB;

    public class RenameManager
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "doc")]
        private Document doc;

        private List<ElementId> elements;
        private Caliburn.Micro.BindableCollection<RenameCandidate> renameCandidates;
        private RenameCommand renameCommand;
        private Caliburn.Micro.BindableCollection<RenameCommand> renameCommands;
        ////Constructors
        #region

        public RenameManager(Document doc, List<ElementId> elements) : this(doc)
        {
            this.elements = new List<ElementId>();
            this.elements.AddRange(elements);
        }

        public RenameManager(Document doc)
        {
            this.doc = doc;
            renameCandidates = new Caliburn.Micro.BindableCollection<RenameCandidate>();
            renameCommands = new Caliburn.Micro.BindableCollection<RenameCommand>();
            renameCommands.Add(new RenameCommand((a, c, b) => a, "None"));
            renameCommands.Add(new RenameCommand(RegexReplace, "Custom Replace", string.Empty, string.Empty));
            renameCommands.Add(new RenameCommand(Increment, "Increment Match", string.Empty, string.Empty) { ReplacementPatternHint = "Increment Ammount" });
            
            var lastRenameCommand = new RenameCommand(IncrementLast, "Increment Last", @"(^\D+)(\d+$)", string.Empty);
            lastRenameCommand.ReplacementPatternHint = "Increment Amount";
            renameCommands.Add(lastRenameCommand);
            
            renameCommands.Add(new RenameCommand((a, c, b) => a.ToLower(System.Globalization.CultureInfo.CurrentCulture), "Lowercase"));
            
            renameCommands.Add(new RenameCommand(
                PadMatch,
                "Pad Leading Zeros(Match)",
                @"^.*(\d+)$",
                "6")
                {
                    SearchPatternHint = "Search Filter", ReplacementPatternHint = "Length" 
                });
            
            renameCommands.Add(new RenameCommand(Reverse, "Reverse"));
            renameCommands.Add(new RenameCommand((a, c, b) => a.Replace(' ', '_'), "Spaces to Underscore"));
            renameCommands.Add(new RenameCommand((a, c, b) => a.Replace(' ', '-'), "Spaces to Hyphen"));
            renameCommands.Add(new RenameCommand(Streetify, "Streetify String", string.Empty, string.Empty) { SearchPatternHint = "Search Filter", ReplacementPatternHint = "Char Spacing" });
            renameCommands.Add(new RenameCommand((a, c, b) => a.ToUpper(System.Globalization.CultureInfo.CurrentCulture), "UpperCase"));
            renameCommand = renameCommands[0];
        }

        #endregion

        ////Properties
        #region

        public static Caliburn.Micro.BindableCollection<string> AvailableParameterTypes
        {
            get
            {
                List<string> result = new List<string>();
                result.Add("Areas");
                result.Add("Rooms");
                result.Add("Grids");
                result.Add("Levels");
                result.Add("Text");
                result.Add("Views");
                result.Add("View Filters");
                result.Add("View Templates");
                result.Add("Sheets");

                // result.Add("Walls");
                // result.Add("Windows");
                result.Add("Doors");

                // result.Add("Families");
                result.Add(@"Project Information");

                // result.Add(@"Model Groups");
                return new Caliburn.Micro.BindableCollection<string>(result.OrderBy(s => s));
            }
        }

        public RenameCommand ActiveRenameCommand
        {
            get => renameCommand;
            set => renameCommand = value;
        }

        public Document Document => doc;

        public Caliburn.Micro.BindableCollection<RenameCandidate> RenameCandidates => renameCandidates;

        public Caliburn.Micro.BindableCollection<RenameCommand> RenameModes => renameCommands;

        public RenameCommand SelectedRenameMode
        {
            get => renameCommand;

            set
            {
                renameCommand = value;
                Rename();
            }
        }

        public static Caliburn.Micro.BindableCollection<RenameParameter> GetParametersByCategoryName(string parameterCategory, Document doc)
        {
            if (parameterCategory == "Areas")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Areas, doc, RenameTypes.Area);
            }
            if (parameterCategory == "Rooms")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Rooms, doc, RenameTypes.Rooms);
            }
            if (parameterCategory == "Views")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Views, doc, RenameTypes.Views);
            }
            if (parameterCategory == "Grids")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Grids, doc, RenameTypes.Grids);
            }
            if (parameterCategory == "View Filters")
            {
                return GetViewFilterParameters(doc);
            }
            if (parameterCategory == "View Templates")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Views, doc, RenameTypes.ViewTemplate);
            }
            if (parameterCategory == "Sheets")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Sheets, doc, RenameTypes.Sheets);
            }

            // if (parameterCategory == "Walls")
            // {
            //    return GetParametersByCategory(BuiltInCategory.OST_Walls, doc, RenameTypes.Walls);
            // }
            if (parameterCategory == "Doors")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Doors, doc, RenameTypes.Doors);
            }

            // if (parameterCategory == "Windows")
            // {
            //    return GetParametersByCategory(BuiltInCategory.OST_Windows, doc, RenameTypes.Windows);
            // }
            if (parameterCategory == "Grids")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Grids, doc, RenameTypes.Grids);
            }
            if (parameterCategory == "Levels")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Levels, doc, RenameTypes.Level);
            }
            if (parameterCategory == "Floors")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Floors, doc, RenameTypes.Floors);
            }
            if (parameterCategory == @"Text")
            {
                return GetParametersByCategory(BuiltInCategory.OST_TextNotes, doc, RenameTypes.Text);
            }
            if (parameterCategory == "Families")
            {
                return RenameFamily.GetParameters(doc);
            }
            if (parameterCategory == @"Project Information")
            {
                return GetParametersByCategory(BuiltInCategory.OST_ProjectInformation, doc, RenameTypes.ProjectInformation);
            }

            // if (parameterCategory == @"Model Groups")
            // {
            //    return RenameGroups.GetParameters(doc);
            // }
            return new Caliburn.Micro.BindableCollection<RenameParameter>();
        }

        public static string Increment(string val, string search, string replace)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrEmpty(search))
            {
                return val;
            }
            var match = Regex.Match(val, search);
            if (match.Success)
            {
                var matchLength = match.Groups[1].Value.Length;
                if (int.TryParse(match.Groups[1].Value, out int n) && int.TryParse(replace, out int incVal))
                {
                    var i = n + incVal;
                    var firstPart = val.Substring(0, match.Groups[1].Index);
                    var secondPart = val.Substring(match.Groups[1].Index + match.Groups[1].Length);
                    var pad = string.Empty;
                    if (i > 0)
                    {
                        for (var j = (int)Math.Floor(Math.Log10(i)); j < (matchLength - 1); j++)
                        {
                            pad += "0";
                        }
                    }
                    return firstPart + pad + i + secondPart;
                }
            }
            return val;
        }

        public static string IncrementLast(string val, string search, string replace)
        {
            var match = Regex.Match(val, search);
            if (match.Success)
            {
                var matchLength = match.Groups[2].Value.Length;
                if (int.TryParse(match.Groups[2].Value, out int n) && int.TryParse(replace, out int incVal))
                {
                    var i = n + incVal;
                    string pad = string.Empty;
                    if (i > 0)
                    {
                        for (int j = (int)Math.Floor(Math.Log10(i)); j < (matchLength - 1); j++)
                        {
                            pad += "0";
                        }
                    }

                    return Regex.Replace(val, search, m => m.Groups[1].Value + pad + i);
                }
            }
            return val;
        }
        
        public static string PadMatch(string val, string search, string replace)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrEmpty(search))
            {
                return val;
            }
            var match = Regex.Match(val, search);
            if (match.Success)
            {
                var matchLength = match.Groups[1].Value.Length;
                if (int.TryParse(match.Groups[1].Value, out int originalNumber) && int.TryParse(replace, out int outputStringLength))
                {
                    var newString = match.Groups[1].Value.PadLeft(outputStringLength, '0');
                    var firstPart = val.Substring(0, match.Groups[1].Index);
                    var secondPart = val.Substring(match.Groups[1].Index + match.Groups[1].Length);
                    return firstPart + newString + secondPart;
                }
            }
            return val;
        }
        
        public static string RegexReplace(string val, string search, string replace)
        {
            return Regex.Replace(val, search, replace);
        }

        public static string Reverse(string val, string search, string replace)
        {
            var result = string.Empty;
            var e = val.Length - 1;
            if (e == 0)
            {
                return val;
            }
            while (e >= 0)
            {
                result += val[e];
                e--;
            }
            return result;
        }

        public static string Streetify(string val, string search, string replace)
        {
            if (!val.Contains(search))
            {
                return val;
            }
            var result = string.Empty;
            foreach (var c in val)
            {
                result += c + @" ";
                if (c == ' ')
                {
                    result += @"    ";
                }
            }
            return result;
        }

        public void CommitRename()
        {
            CommitRenameSelection(renameCandidates.ToList());
        }

        public void CommitRenameSelection(List<RenameCandidate> selectedCandiates)
        {
            // SCaddinsApp.WindowManager.ShowMessageBox(selectedCandiates.Count.ToString());
            int fails = 0;
            int successes = 0;
            using (var t = new Transaction(doc))
            {
                // SCaddinsApp.WindowManager.ShowMessageBox("IN T");
                if (t.Start("Bulk Rename") == TransactionStatus.Started)
                {
                    // SCaddinsApp.WindowManager.ShowMessageBox("T Started");
                    foreach (RenameCandidate candidate in selectedCandiates)
                    {
                        if (candidate.ValueChanged)
                        {
                            // SCaddinsApp.WindowManager.ShowMessageBox("Value Change");
                            if (candidate.Rename())
                            {
                                // SCaddinsApp.WindowManager.ShowMessageBox("Success");
                                successes++;
                            }
                            else
                            {
                                // SCaddinsApp.WindowManager.ShowMessageBox("Fail");
                                fails++;
                            }
                        }
                    }
                    t.Commit();
                    var pn = successes == 1 ? "parameter" : "parameters";
                    SCaddinsApp.WindowManager.ShowMessageBox(
                        @"Bulk Rename", successes + @" " + pn + @" successfully renamed, " + fails + @" errors.");
                }
                else
                {
                    SCaddinsApp.WindowManager.ShowMessageBox("Error", "Failed to start Bulk Rename Revit Transaction...");
                }
            }
        }

        /// <summary>
        /// Renames candidates without committing the change to Revit
        /// </summary>
        public void Rename()
        {
            foreach (RenameCandidate rc in renameCandidates)
            {
                rc.NewValue = renameCommand.Rename(rc.OldValue);
            }
        }

        public void SetCandidatesByParameter(
            RenameParameter renameParameter)
        {
            switch (renameParameter.RenameType)
            {
                case RenameTypes.Grids:
                    renameCandidates = RenameGrids.GetCandidates(doc);
                    break;
                case RenameTypes.Level:
                    renameCandidates = RenameLevels.GetCandidates(doc);
                    break;
                case RenameTypes.Text:
                    renameCandidates = RenameText.GetCandidates(doc);
                    break;
                case RenameTypes.Family:
                    renameCandidates = RenameFamily.GetCandidates(doc);
                    break;
                case RenameTypes.Group:
                    renameCandidates = RenameGroups.GetCandidates(doc);
                    break;
                case RenameTypes.ViewTemplate:
                    renameCandidates = RenameViewTemplate.GetCandidates(doc, renameParameter);
                    break;
                case RenameTypes.ViewFilter:
                    renameCandidates = RenameViewFilter.GetCandidates(doc);
                    break;
                default:
                    renameCandidates.Clear();
                    var list = new List<Element>();
                    FilteredElementCollector collector;

                    if (elements == null)
                    {
                        collector = new FilteredElementCollector(doc);
                    }
                    else
                    {
                        collector = new FilteredElementCollector(doc, elements);
                    }

                    collector.OfCategory(renameParameter.Category);
                    list = collector.ToList<Element>();

                    foreach (Element element in list)
                    {
                        var p = element.GetParameters(renameParameter.Parameter.Definition.Name);
                        if (p.Count > 0)
                        {
                            var rc = new RenameCategory(p[0]);
                            if (!string.IsNullOrEmpty(rc.OldValue))
                            {
                                rc.NewValue = renameCommand.Rename(rc.OldValue);
                                renameCandidates.Add(rc);
                            } else
                            {
                                if (rc.OldValue == string.Empty)
                                {
                                    rc.NewValue = renameCommand.Rename(rc.OldValue);
                                    renameCandidates.Add(rc);
                                }
                            }
                        }
                    }
                    break;
            }       
        }

        private static Caliburn.Micro.BindableCollection<RenameParameter> GetViewFilterParameters(Document doc)
        {
            Caliburn.Micro.BindableCollection<RenameParameter> parametersList = new Caliburn.Micro.BindableCollection<RenameParameter>();

            using (var f = new FilteredElementCollector(doc))
            {
                f.OfClass(typeof(ParameterFilterElement));
                var elem = f.FirstElement();
                var elem2 = f.ToElements()[f.GetElementCount() - 1];
                if (elem2.Parameters.Size > elem.Parameters.Size)
                {
                    elem = elem2;
                }
                parametersList.Add(new RenameParameter(null, BuiltInCategory.INVALID, elem, RenameTypes.ViewFilter));
                return parametersList;
            }
        }

        private static Caliburn.Micro.BindableCollection<RenameParameter> GetParametersByCategory(
            BuiltInCategory category,
            Document doc,
            RenameTypes renameType)
        {
            var parametersList = new Caliburn.Micro.BindableCollection<RenameParameter>();
            if (category == BuiltInCategory.OST_TextNotes)
            {
                parametersList.Add(new RenameParameter(null, category, null, RenameTypes.Text));
                parametersList.OrderBy(p => p.Name);
                return parametersList;
            }

            FilteredElementCollector collector;
            Element elem;
            Element elem2;

            collector = new FilteredElementCollector(doc);

            collector.OfCategory(category);
            elem = collector.FirstElement();
            elem2 = collector.ToElements()[collector.GetElementCount() - 1];

            if (elem2.Parameters.Size > elem.Parameters.Size)
            {
                elem = elem2;
            }

            if (category == BuiltInCategory.OST_Levels)
            {
                Parameter param = elem.LookupParameter("Name");
                parametersList.Add(new RenameParameter(param, BuiltInCategory.OST_Levels, null, RenameTypes.Level));
                parametersList.OrderBy(p => p.Name);
                return parametersList;
            }

            if (category == BuiltInCategory.OST_Grids)
            {
                Parameter param = elem.LookupParameter("Name");
                parametersList.Add(new RenameParameter(param, BuiltInCategory.OST_Grids, null, RenameTypes.Grids));
                parametersList.OrderBy(p => p.Name);
                return parametersList;
            }

            if (renameType == RenameTypes.ViewTemplate)
            {
                Parameter param = elem.LookupParameter("View Name");
                parametersList.Add(new RenameParameter(param, BuiltInCategory.OST_Views, null, RenameTypes.ViewTemplate));
                parametersList.OrderBy(p => p.Name);
                return parametersList;
            }

            if (category == BuiltInCategory.OST_TextNotes)
            {
                Parameter param = elem.LookupParameter("Text");
                parametersList.Add(new RenameParameter(param, BuiltInCategory.OST_TextNotes, null, RenameTypes.Text));
                parametersList.OrderBy(p => p.Name);
                return parametersList;
            }

            if (category == BuiltInCategory.OST_Doors)
            {
                return RenameDoors.GetParameters(doc);
            }

            foreach (Parameter param in elem.Parameters)
            {
                if (param.StorageType == StorageType.String && !param.IsReadOnly)
                {
                    parametersList.Add(new RenameParameter(param, category, null, RenameTypes.ByCategory));
                }
            }

            // parametersList = parametersList.OrderBy(p => p.Name);
            return parametersList;
        }

        // private static bool IsValidRevitName(string s)
        // {
        //     return !(s.Contains("{") || s.Contains("}"));
        // }
        #endregion

        // private void GetViewTemplateNames(RenameParameter renameParameter)
        // {
        //     renameCandidates.Clear();
        //     renameCandidates = RenameViewTemplate.GetCandidates(doc, renameParameter);
        // }
    }
}