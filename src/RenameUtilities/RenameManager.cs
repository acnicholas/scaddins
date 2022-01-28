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
            renameCommands.Add(new RenameCommand((a, c, b) => a.ToUpper(System.Globalization.CultureInfo.CurrentCulture), "UpperCase"));
            renameCommands.Add(new RenameCommand((a, c, b) => a.ToLower(System.Globalization.CultureInfo.CurrentCulture), "Lowercase"));
            renameCommands.Add(new RenameCommand((a, c, b) => a.Replace(' ', '_'), "Spaces to Underscore"));
            renameCommands.Add(new RenameCommand((a, c, b) => a.Replace(' ', '-'), "Spaces to Hyphen"));
            renameCommands.Add(new RenameCommand(RegexReplace, "Custom Replace", string.Empty, string.Empty));
            renameCommands.Add(new RenameCommand(Increment, "Increment Match", string.Empty, string.Empty) { ReplacementPatternHint = "Increment Ammount" });
            renameCommands.Add(new RenameCommand(Streetify, "Streetify String", string.Empty, string.Empty) { SearchPatternHint = "Search Filter", ReplacementPatternHint = "Char Spacing" });

            ////inc last
            var lastRenameCommand = new RenameCommand(IncrementLast, "Increment Last", @"(^\D+)(\d+$)", string.Empty);
            lastRenameCommand.ReplacementPatternHint = "Increment Amount";
            renameCommands.Add(lastRenameCommand);

            renameCommand = renameCommands[0];
        }

        #endregion

        ////Properties
        #region

        public static Caliburn.Micro.BindableCollection<string> AvailableParameterTypes
        {
            get
            {
                Caliburn.Micro.BindableCollection<string> result = new Caliburn.Micro.BindableCollection<string>();
                result.Add("Areas");
                result.Add("Rooms");
                result.Add("Grids");
                result.Add("Levels");
                result.Add("Text");
                result.Add("Views");
                result.Add("Sheets");
                result.Add("Walls");
                result.Add("Windows");
                result.Add("Doors");
                result.Add("Families");
                result.Add(@"Project Information");
                result.Add(@"Model Groups");
                result.OrderBy(s => s);
                return result;
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
                return GetParametersByCategory(BuiltInCategory.OST_Areas, doc);
            }
            if (parameterCategory == "Rooms")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Rooms, doc);
            }
            if (parameterCategory == "Views")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Views, doc);
            }
            if (parameterCategory == "Sheets")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Sheets, doc);
            }
            if (parameterCategory == "Walls")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Walls, doc);
            }
            if (parameterCategory == "Doors")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Doors, doc);
            }
            if (parameterCategory == "Windows")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Windows, doc);
            }
            if (parameterCategory == "Grids")
            {
                return GetParametersByType(typeof(Grid), doc);
            }
            if (parameterCategory == "Levels")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Levels, doc);
            }
            if (parameterCategory == "Floors")
            {
                return GetParametersByCategory(BuiltInCategory.OST_Floors, doc);
            }
            if (parameterCategory == @"Text")
            {
                return GetParametersByCategory(BuiltInCategory.OST_TextNotes, doc);
            }
            if (parameterCategory == "Families")
            {
                return GetFamilyParameters(doc);
            }
            if (parameterCategory == @"Project Information")
            {
                return GetParametersByCategory(BuiltInCategory.OST_ProjectInformation, doc);
            }
            if (parameterCategory == @"Model Groups")
            {
                return GetModelGroupParameters(doc);
            }
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

        public static string RegexReplace(string val, string search, string replace)
        {
            return Regex.Replace(val, search, replace);
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
            int fails = 0;
            int successes = 0;

            using (var t = new Transaction(doc))
            {
                if (t.Start("Bulk Rename") == TransactionStatus.Started)
                {
                    foreach (RenameCandidate candidate in selectedCandiates)
                    {
                        if (candidate.ValueChanged)
                        {
                            if (candidate.Rename())
                            {
                                successes++;
                            }
                            else
                            {
                                fails++;
                            }
                        }
                    }
                    t.Commit();
                    SCaddinsApp.WindowManager.ShowMessageBox(
                        @"Bulk Rename", successes + @" parameters succesfully renames, " + fails + @" errors.");
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

        public void SetCandidatesByParameter(Parameter parameter, BuiltInCategory category, Type t, Family family, Autodesk.Revit.DB.GroupType group)
        {
            if (category == BuiltInCategory.OST_TextNotes || category == BuiltInCategory.OST_IOSModelGroups)
            {
                GetTextNoteValues(category);
                return;
            }
            if (family != null)
            {
                GetFamilyNames();
                return;
            }
            if (group != null)
            {
                GetGroupNames();
                return;
            }
            renameCandidates.Clear();
            FilteredElementCollector collector;
            if (elements == null)
            {
                collector = new FilteredElementCollector(doc);
            }
            else
            {
                collector = new FilteredElementCollector(doc, elements);
            }
            if (t != null)
            {
                collector.OfClass(t);
            }
            else
            {
                collector.OfCategory(category);
            }
            foreach (Element element in collector)
            {
                var p = element.GetParameters(parameter.Definition.Name);
                if (p.Count > 0)
                {
                    var rc = new RenameCandidate(p[0]);
                    if (!string.IsNullOrEmpty(rc.OldValue))
                    {
                        rc.NewValue = renameCommand.Rename(rc.OldValue);
                        renameCandidates.Add(rc);
                    }
                }
            }
        }

        private static Caliburn.Micro.BindableCollection<RenameParameter> GetParametersByCategory(BuiltInCategory category, Document doc)
        {
            Caliburn.Micro.BindableCollection<RenameParameter> parametersList = new Caliburn.Micro.BindableCollection<RenameParameter>();
            if (category == BuiltInCategory.OST_TextNotes)
            {
                parametersList.Add(new RenameParameter(category));
                return parametersList;
            }

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(category);
            var elem = collector.FirstElement();
            var elem2 = collector.ToElements()[collector.GetElementCount() - 1];
            if (elem2.Parameters.Size > elem.Parameters.Size)
            {
                elem = elem2;
            }

            if (category == BuiltInCategory.OST_Levels || category == BuiltInCategory.OST_Grids)
            {
                ////Parameter param = elem.GetParameters("Name").FirstOrDefault();
                Parameter param = elem.LookupParameter("Name");
                parametersList.Add(new RenameParameter(param, category));
                return parametersList;
            }

            foreach (Parameter param in elem.Parameters)
            {
                if (param.StorageType == StorageType.String && !param.IsReadOnly)
                {
                    parametersList.Add(new RenameParameter(param, category));
                }
            }
            return parametersList;
        }

        private static Caliburn.Micro.BindableCollection<RenameParameter> GetParametersByType(Type t, Document doc)
        {
            Caliburn.Micro.BindableCollection<RenameParameter> parametersList = new Caliburn.Micro.BindableCollection<RenameParameter>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(t);
            var elem = collector.FirstElement();
            var elem2 = collector.ToElements()[collector.GetElementCount() - 1];
            if (elem2.Parameters.Size > elem.Parameters.Size)
            {
                elem = elem2;
            }
            Parameter param = elem.LookupParameter("Name");
            parametersList.Add(new RenameParameter(param, t));
            return parametersList;
        }

        private static Caliburn.Micro.BindableCollection<RenameParameter> GetFamilyParameters(Document doc)
        {
            Caliburn.Micro.BindableCollection<RenameParameter> parametersList = new Caliburn.Micro.BindableCollection<RenameParameter>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Family));
            var elem = collector.FirstElement();
            var elem2 = collector.ToElements()[collector.GetElementCount() - 1];
            if (elem2.Parameters.Size > elem.Parameters.Size)
            {
                elem = elem2;
            }
            parametersList.Add(new RenameParameter(elem as Family));
            return parametersList;
        }

        private static Caliburn.Micro.BindableCollection<RenameParameter> GetModelGroupParameters(Document doc)
        {
            Caliburn.Micro.BindableCollection<RenameParameter> parametersList = new Caliburn.Micro.BindableCollection<RenameParameter>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Autodesk.Revit.DB.GroupType));
            var elem = collector.FirstElement();
            var elem2 = collector.ToElements()[collector.GetElementCount() - 1];
            if (elem2.Parameters.Size > elem.Parameters.Size)
            {
                elem = elem2;
            }
            parametersList.Add(new RenameParameter(elem as Autodesk.Revit.DB.GroupType));
            return parametersList;
        }

        // Possibly used by CM... TODO check this
        // ReSharper disable once UnusedMember.Local
        private static bool IsValidRevitName(string s)
        {
            return !(s.Contains("{") || s.Contains("}"));
        }
        #endregion
        private void GetTextNoteValues(BuiltInCategory category)
        {
            renameCandidates.Clear();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(category);
            foreach (Element element in collector)
            {
                var textNote = (TextElement)element;
                if (textNote != null)
                {
                    var rc = new RenameCandidate(textNote);
                    rc.NewValue = renameCommand.Rename(rc.OldValue);
                    renameCandidates.Add(rc);
                }
            }
        }

        private void GetFamilyNames()
        {
            renameCandidates.Clear();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Family));
            foreach (Element element in collector)
            {
                var family = (Family)element;
                if (family != null)
                {
                    var rc = new RenameCandidate(family);
                    rc.NewValue = renameCommand.Rename(rc.OldValue);
                    renameCandidates.Add(rc);
                }
            }
        }

        private void GetGroupNames()
        {
            SCaddinsApp.WindowManager.ShowMessageBox("adding groups");
            renameCandidates.Clear();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Autodesk.Revit.DB.GroupType));
            foreach (Element element in collector)
            {
                var group = (Autodesk.Revit.DB.GroupType)element;
                if (group != null)
                {
                    var rc = new RenameCandidate(group);
                    rc.NewValue = renameCommand.Rename(rc.OldValue);
                    renameCandidates.Add(rc);
                }
            }
        }
    }
}