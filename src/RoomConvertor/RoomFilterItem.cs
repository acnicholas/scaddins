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

namespace SCaddins.RoomConvertor
{
    using System;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;
    using System.Linq;

    public class RoomFilterItem
    {
       private LogicalOperator lo;
       private ComparisonOperator co;
       private Parameter parameter;
       private string parameterName;
       private bool paramIsString;
       private string test;

        public RoomFilterItem(LogicalOperator lo, ComparisonOperator co, string parameter, string test)
        {
            this.lo = lo;
            this.co = co;
            this.parameterName = parameter;
            this.parameter = null;
            this.test = test;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(test) ;
        }

        private static Parameter ParamFromString(Room room, string name)
        {
            if (room.GetParameters(name).Count > 0)
            {
                return room.GetParameters(name)[0];
            }
            return null;
        }

        private static bool ParameterValueContainsString(Parameter param, string value)
        {
            if (!param.HasValue || string.IsNullOrWhiteSpace(value)) {
                return false;
            }
            switch (param.StorageType) {
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

        private static int ParameterComparedToString(Parameter param, string value)
        {
            const int result = 441976;
            if (!param.HasValue || string.IsNullOrWhiteSpace(value)) {
                return result;
            }
            switch (param.StorageType) {
                case StorageType.Double:
                    double parse;
                    if (double.TryParse(value, out parse)) {
                        return param.AsDouble().CompareTo(parse);
                    } 
                    break;
                case StorageType.String:
                    return param.AsString().CompareTo(value);
                case StorageType.Integer:
                    int iparse;
                    if (int.TryParse(value, out iparse)) {
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

        private bool LevelPassesFilter(Room room)
        {
            Document doc = room.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Level));
            var levels = collector.Cast<Level>();
            var level = levels.Where<Level>(l => l.Name == test).ToList<Level>();
            return level[0] == room.Level;
            //return false;
        }

        public bool PassesFilter(Room room)
        {
            parameter = ParamFromString(room, parameterName);


            // FIXME add OR oprion one day.
            if (lo != LogicalOperator.And) {
                return false;
            }
            
            if (parameter == null) {
                return false;
            }

            //FIXME not sure how else to do this...
            if (parameterName == "Level")
            {
                return LevelPassesFilter(room);
            }

            if (co == ComparisonOperator.Contains) {
                return ParameterValueContainsString(parameter, test);
            }

            int p = ParameterComparedToString(parameter, test);

            switch (co) {
                case ComparisonOperator.Equals:
                    return p == 0;
                case ComparisonOperator.LessThan:
                    return p < 0 && p != 441976;
                case ComparisonOperator.GreaterThan:
                    return p > 0 && p != 441976;
                case ComparisonOperator.NotEqual:
                    return p != 0 && p != 441976;
                default:
                    return false;
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
