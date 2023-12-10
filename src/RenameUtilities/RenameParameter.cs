// (C) Copyright 2017-2020 by Andrew Nicholas
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
    using Autodesk.Revit.DB;

    public class RenameParameter
    {
        public RenameParameter(Parameter parameter, BuiltInCategory category, bool isViewTemplate)
        {
            this.Parameter = parameter;
            this.Category = category;
            this.Type = null;
            this.Family = null;
            this.Group = null;
            this.Name = parameter.Definition.Name;
            this.IsViewTemplate = isViewTemplate;
            this.ParameterFilterElement = null;
        }

        public RenameParameter(Parameter parameter, BuiltInCategory category)
        {
            this.Parameter = parameter;
            this.Category = category;
            this.Type = null;
            this.Family = null;
            this.Group = null;
            this.Name = parameter.Definition.Name;
            this.IsViewTemplate = false;
            this.ParameterFilterElement = null;
        }

        public RenameParameter(Parameter parameter, Type t)
        {
            this.Parameter = parameter;
            this.Type = t;
            this.Family = null;
            this.Group = null;
            this.Category = BuiltInCategory.INVALID;
            this.Name = parameter.Definition.Name;
            this.IsViewTemplate = false;
            this.ParameterFilterElement = null;
        }

        public RenameParameter(BuiltInCategory category)
        {
            this.Parameter = null;
            this.Category = category;
            this.Family = null;
            this.Group = null;
            this.Type = null;
            this.Name = "Text";
            this.IsViewTemplate = false;
            this.ParameterFilterElement = null;
        }

        public RenameParameter(Family family)
        {
            this.Parameter = null;
            this.Category = BuiltInCategory.INVALID;
            this.Type = null;
            this.Family = family;
            this.Group = null;
            this.Name = "Name";
            this.IsViewTemplate = false;
            this.ParameterFilterElement = null;
        }

        public RenameParameter(ParameterFilterElement pfe)
        {
            this.Parameter = null;
            this.Category = BuiltInCategory.INVALID;
            this.Type = null;
            this.Family = null;
            this.Group = null;
            this.Name = "Name";
            this.IsViewTemplate = false;
            this.ParameterFilterElement = pfe;
        }

        public RenameParameter(Autodesk.Revit.DB.GroupType group)
        {
            this.Parameter = null;
            this.Category = BuiltInCategory.INVALID;
            this.Type = null;
            this.Family = null;
            this.Group = group;
            this.Name = "Name";
            this.IsViewTemplate = false;
            this.ParameterFilterElement = null;
        }

        public BuiltInCategory Category
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public Family Family
        {
            get;
            private set;
        }

        public ParameterFilterElement ParameterFilterElement
        {
            get;
            private set;
        }

        public Autodesk.Revit.DB.GroupType Group
        {
            get;
            private set;
        }

        public bool IsViewTemplate
        {  
            get;
            private set; 
        }

        public Parameter Parameter
        {
            get;
            private set;
        }

        public Type Type
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
