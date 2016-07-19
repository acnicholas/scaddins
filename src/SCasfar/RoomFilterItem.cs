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
       LogicalOperators lo;
       ComparisonOperators co;
       string parameterName;
       string test;

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
            //GreaterThanOrEqual,
            //LessThanOrEqual,
            Contains,
            //Matches
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
            #if REVIT2014
            return null;
            #else
            if(room.GetParameters(name).Count > 0) {
                return room.GetParameters(name)[0];
            }
            return null;
            #endif
        }

        private bool ParameterValueContainsString(Parameter param, string value)
        {
            if (!param.HasValue || string.IsNullOrWhiteSpace(value)){
                return false;
            }
            switch (param.StorageType){
                case StorageType.Double:
                        return false;
                case StorageType.String:
                        return param.AsString().Contains(value);
                case StorageType.Integer:
                        return false;
                case StorageType.ElementId:
                    return false;
                default:
                    return false;
            }
        }

        private int ParameterComparedToString(Parameter param, string value)
        {
            const int result = 441976;
            if (!param.HasValue || string.IsNullOrWhiteSpace(value)){
                return result;
            }
            switch (param.StorageType){
                case StorageType.Double:
                    double parse;
                    if (Double.TryParse(value, out parse)){
                        return param.AsDouble().CompareTo(parse);
                    } 
                    break;
                case StorageType.String:
                    int sparse, sparse2;
                    if (Int32.TryParse(value, out sparse) && Int32.TryParse(param.AsString(), out sparse2)){
                           return sparse2.CompareTo(sparse);
                    } else {
                        return param.AsString() == value ? 0 : result;
                    }
                case StorageType.Integer:
                    int iparse;
                    if (Int32.TryParse(value, out iparse)){
                           return param.AsInteger().CompareTo(iparse);
                    }
                    break;
                case StorageType.ElementId:
                    return result;
                default:
                    return result;
            }
            return result;
        }

        public bool PassesFilter(Room room)
        {
            Parameter param = ParamFromString(room, this.parameterName);
            if (param == null) {
                return false;
            }

            if (this.co == ComparisonOperators.Contains) {
                return ParameterValueContainsString(param, this.test);
            }

            int p = ParameterComparedToString(param, this.test);

            switch (this.co) {
                case ComparisonOperators.Equals:
                    return p == 0;
                case ComparisonOperators.LessThan:
                    return p < 0 && p != 441976;
                case ComparisonOperators.GreaterThan:
                    return p > 0 && p != 441976;
                case ComparisonOperators.NotEqual:
                    return p != 0 && p != 441976;
                default:
                    return false;
            }
        }
    }
}

