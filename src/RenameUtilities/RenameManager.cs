// (C) Copyright 2017 by Andrew Nicholas
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

using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace SCaddins.RenameUtilities
{
    public class RenameManager
    {
        public Caliburn.Micro.BindableCollection<SCaddins.RenameUtilities.RenameCandidate> renameCandidates;
        public Caliburn.Micro.BindableCollection<SCaddins.RenameUtilities.RenameCommand> renameCommands;
        public RenameCommand renameCommand;
        private Document doc;
        private List<ElementId> elements;

        //Constructors
        #region 

        public RenameManager(Document doc, List<ElementId> elements) : this(doc)
        {
            this.elements = new List<ElementId>();
            this.elements.AddRange(elements);  
        }

        public RenameManager(Document doc)
        {
            this.doc = doc;
            renameCandidates = new Caliburn.Micro.BindableCollection<SCaddins.RenameUtilities.RenameCandidate>();
            renameCommands = new Caliburn.Micro.BindableCollection<SCaddins.RenameUtilities.RenameCommand>();
            renameCommands.Add(new RenameUtilities.RenameCommand((a, c, b) => a, "None"));
            renameCommands.Add(new RenameUtilities.RenameCommand((a, c, b) => a.ToUpper(), "UpperCase"));
            renameCommands.Add(new RenameUtilities.RenameCommand((a, c, b) => a.ToLower(), "Lowercase"));
            renameCommands.Add(new RenameCommand((a, c, b) => a.Replace(' ', '_'), "Spaces to Underscore"));
            renameCommands.Add(new RenameCommand((a, c, b) => a.Replace(' ', '-'), "Spaces to Hyphen"));
            renameCommands.Add(new RenameCommand(RegexReplace, "Custom Replace", string.Empty, string.Empty));
            renameCommands.Add(new RenameCommand(IncrementOne, "Increment Match 1", string.Empty, string.Empty));
            renameCommands.Add(new RenameCommand(IncrementTwo, "Increment Match 2", string.Empty, string.Empty));
            renameCommand = renameCommands[0];
        }

        public static string RegexReplace(string val, string search, string replace)
        {
            return System.Text.RegularExpressions.Regex.Replace(val, search, replace);
        }

        public static string IncrementOne(string val, string search, string replace)
        {
            return "todo";
        }

        public static string IncrementTwo(string val, string search, string replace)
        {
            return "todo";
        }

        #endregion

        //Properties
        #region
        public Caliburn.Micro.BindableCollection<SCaddins.RenameUtilities.RenameCandidate> RenameCandidates
        {
            get { return renameCandidates; }
        }

        public Caliburn.Micro.BindableCollection<String> AvailableParameterTypes
        {
            get
            {
                Caliburn.Micro.BindableCollection<String> result = new Caliburn.Micro.BindableCollection<String>();
                result.Add("Rooms");
                result.Add("Text");
                result.Add("Views");
                result.Add("Sheets");
                result.Add("Walls");
                result.Add("Doors");
                result.Add(@"Model Groups");
                return result;
            }
        }

        public Caliburn.Micro.BindableCollection<RenameParameter> GetParametersByCategoryName(string parameterCategory)
        {
            if (parameterCategory == "Rooms") {
                return GetParametersByCategory(BuiltInCategory.OST_Rooms);
            }
            if (parameterCategory == "Views") {
                return GetParametersByCategory(BuiltInCategory.OST_Views);
            }
            if (parameterCategory == "Sheets") {
                return GetParametersByCategory(BuiltInCategory.OST_Sheets);
            }
            if (parameterCategory == "Walls") {
                return GetParametersByCategory(BuiltInCategory.OST_Walls);
            }
            if (parameterCategory == "Doors") {
                return GetParametersByCategory(BuiltInCategory.OST_Doors);
            }
            if (parameterCategory == "Windows") {
                return GetParametersByCategory(BuiltInCategory.OST_Windows);
            }
            if (parameterCategory == "Windows") {
                return GetParametersByCategory(BuiltInCategory.OST_Revisions);
            }
            if (parameterCategory == "Floors") {
                return GetParametersByCategory(BuiltInCategory.OST_Floors);
            }
            if (parameterCategory == @"Text") {
                return GetParametersByCategory(BuiltInCategory.OST_TextNotes);
            }
            if (parameterCategory == @"Model Groups") {
                return GetParametersByCategory(BuiltInCategory.OST_IOSModelGroups);
            }
            return new Caliburn.Micro.BindableCollection<RenameParameter>();
        }

        private Caliburn.Micro.BindableCollection<RenameParameter> GetParametersByCategory(BuiltInCategory category)
        {
            Caliburn.Micro.BindableCollection<RenameParameter> parametersList = new Caliburn.Micro.BindableCollection<RenameParameter>();
            if (category == BuiltInCategory.OST_TextNotes || category == BuiltInCategory.OST_IOSModelGroups) {
                parametersList.Add(new RenameParameter(category));
            }
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(category);
            var elem = collector.FirstElement();
            var elem2 = collector.ToElements()[collector.GetElementCount() -1];
            if (elem2.Parameters.Size > elem.Parameters.Size) {
                elem = elem2;
            }
            foreach (Parameter param in elem.Parameters) {
                if (param.StorageType == StorageType.String && !param.IsReadOnly) {
                    parametersList.Add(new RenameParameter(param, category));
                }
            }
            return parametersList;
        }

        public Caliburn.Micro.BindableCollection<RenameCommand> RenameModes
        {
            get
            {
                return renameCommands;
            }
        }

        public RenameCommand SelectedRenameMode
        {
            get
            {
                return renameCommand;
            }
            set
            {
                renameCommand = value;
                DryRename();
            }
        }

        #endregion

        public void DryRename()
        {
            foreach (RenameCandidate rc in renameCandidates) {
                rc.NewValue = renameCommand.Rename(rc.OldValue);
            }
        }

        private static bool IsValidRevitName(string s)
        {
            return !(s.Contains("{") || s.Contains("}"));
        }
       
        public Caliburn.Micro.BindableCollection<RenameCandidate> GetTextNoteValues(BuiltInCategory category){
            Caliburn.Micro.BindableCollection<RenameCandidate> candidates = new Caliburn.Micro.BindableCollection<RenameCandidate>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(category);
            foreach (Element element in collector) {
                var textNote = (TextElement)element;
                if(textNote != null) {
                    var rc = new RenameCandidate(textNote);
                    rc.NewValue = renameCommand.Rename(rc.OldValue);
                    candidates.Add(rc);
                }
            }
            return candidates;
        }
             
        public void SetCandidatesByParameter(Parameter parameter, BuiltInCategory category){
            if (category == BuiltInCategory.OST_TextNotes || category == BuiltInCategory.OST_IOSModelGroups) {
                renameCandidates = GetTextNoteValues(category);
                return;
            }
            renameCandidates.Clear();
            FilteredElementCollector collector;
            if (elements == null) {
                collector = new FilteredElementCollector(doc);
            } else {
                Autodesk.Revit.UI.TaskDialog.Show("xxx","xxx");
                collector = new FilteredElementCollector(doc, elements);
            }
            collector.OfCategory(category);
            foreach (Element element in collector) {
                var p = element.GetParameters(parameter.Definition.Name);
                if (p.Count > 0) {
                    var rc = new RenameCandidate(p[0]);
                    if (!string.IsNullOrEmpty(rc.OldValue)) {
                        rc.NewValue = renameCommand.Rename(rc.OldValue);
                        renameCandidates.Add(rc);
                    }
                }
            }
        }


    }
}
