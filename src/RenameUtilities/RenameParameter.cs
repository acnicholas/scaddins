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

namespace SCaddins.RenameUtilities
{
    using System;
    using Autodesk.Revit.DB;

    public class RenameParameter
    {
        public RenameParameter(Parameter parameter, BuiltInCategory category)
        {
            Parameter = parameter;
            Category = category;
            Type = null;
            Name = parameter.Definition.Name;
        }

        public RenameParameter(Parameter parameter, Type t)
        {
            Parameter = parameter;
            Type = t;
            Category = BuiltInCategory.INVALID;
            Name = parameter.Definition.Name;
        }

        public RenameParameter(BuiltInCategory category)
        {
            Parameter = null;
            Category = category;
            Type = null;
            Name = "Text";
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