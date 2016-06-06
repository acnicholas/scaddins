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
    using System;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;
    
    public class RoomFilterItem
    {
       private LogicalOperators lo;
       private ComparisonOperators co;
       private string parameterName;
       private string test;
        
        public enum LogicalOperators
        {
            AND
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
        
        public RoomFilterItem(string lo, string co, string parameter, string test)
        {
           this.lo = (RoomFilterItem.LogicalOperators) Enum.Parse(typeof(RoomFilterItem.LogicalOperators), lo);
           this.co = (RoomFilterItem.ComparisonOperators) Enum.Parse(typeof(RoomFilterItem.ComparisonOperators), co);
           this.parameterName = parameter;
           this.test = test;
        }
        
        public bool IsValid()
        {
            return true;
        }
        
        private Parameter ParamFromString(Room room, string name)
        {
            if(room.GetParameters(name).Count > 0) {
                return room.GetParameters(name)[0];
            }
            return null;
        }
        
        private bool ParameterValueMatchesString(Parameter param, string value)
        {
            if (!param.HasValue || string.IsNullOrWhiteSpace(value)){
                return false;
            }
            switch (param.StorageType){
                case StorageType.Double:
                    double parse;
                    if (Double.TryParse(value, out parse)){
                        return param.AsDouble().CompareTo(parse) == 0;
                    } else {
                        return false;
                    }
                case StorageType.String:
                    return param.AsString() == value;
                case StorageType.Integer:
                       int iparse;
                    if (Int32.TryParse(value, out iparse)){
                           return param.AsInteger().CompareTo(iparse) == 0;
                    } else {
                        return false;
                    }
                case StorageType.ElementId:
                    return false;
                default:
                    return false;
            }
        }
        
        private bool ParameterValueLessThanString(Parameter param, string value)
        {
            if (!param.HasValue || string.IsNullOrWhiteSpace(value)){
                return false;
            }
            switch (param.StorageType){
                case StorageType.Double:
                    double parse;
                    if (Double.TryParse(value, out parse)){
                        return param.AsDouble().CompareTo(parse) < 0;
                    } else {
                        return false;
                    }
                case StorageType.String:
                    return false;
                case StorageType.Integer:
                       int iparse;
                    if (Int32.TryParse(value, out iparse)){
                           return param.AsInteger().CompareTo(iparse) < 0;
                    } else {
                        return false;
                    }
                case StorageType.ElementId:
                    return false;
                default:
                    return false;
            }
        }
        
        public bool PassesFilter(Room room)
        {
            bool pass = true;
            Parameter param = ParamFromString(room, this.parameterName);
            if (param == null) {
                return false;
            }
            
            //switch (this.lo){
            //    case LogicalOperators.AND:
            switch (this.co) {
                case ComparisonOperators.Equals:
                    pass = ParameterValueMatchesString(param, this.test);
                    break;
                case ComparisonOperators.LessThan:
                    pass = ParameterValueLessThanString(param, this.test);
                    break;   
                case ComparisonOperators.Contains:
                    pass = ParameterValueGreaterThanString(param, this.test);    
            }
            //}
            return pass;
        }
            
    }
    
}

