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
        public string Name
        {
            get;
            set;
        }

        public RenameParameter(Parameter parameter, BuiltInCategory category)
        {
            this.Parameter = parameter;
            this.Category = category;
            this.Name = parameter.Definition.Name;
        }
        
        public RenameParameter(bool textNote)
        {
            this.Parameter = null;
            this.Category = BuiltInCategory.OST_TextNotes;
            this.Name = "Text";
        }

        public Parameter Parameter {
            get;
            private set;
        } 
        
        public BuiltInCategory Category {
            get;
            private set;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */