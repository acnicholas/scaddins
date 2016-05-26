// (C) Copyright 2016 by Andrew Nicholas
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

namespace SCaddins.SCasfar
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.DB.Architecture;
    
    public class RoomFilterItem
    {
       private LogicalOperators lo;
       private ComparisonOperators co;
       private Parameter parameter;
       private string test;
        
        public enum LogicalOperators
        {
            AND,
            OR
        }
        
        public enum ComparisonOperators
        {
            Equals,
            NotEqual,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual,
            Contains,
            Matches
        }
        
        public RoomFilterItem(LogicalOperators lo, ComparisonOperators co, Parameter parameter, string test)
        {
            this.lo = lo;
            this.co = co;
            this.parameter = parameter;
            this.test = test;
        }
        
        public bool PassesFilter()
        {
            return true;
        }
            
    }
    
}

