// (C) Copyright 2016-2020 by Andrew Nicholas
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

namespace SCaddins.RoomConverter
{
    using System;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;

    public class RoomFilterItem
    {
        private ComparisonOperator co;
        private LogicalOperator lo;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "parameter")]
        private Parameter parameter;
        private string parameterName;
        private string test;

        public RoomFilterItem(LogicalOperator lo, ComparisonOperator co, string parameter, string test)
        {
            this.lo = lo;
            this.co = co;
            parameterName = parameter;
            this.parameter = null;
            this.test = test;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(test);
        }

        public bool PassesFilter(Room room)
        {
            parameter = ParamFromString(room, parameterName);

            // FIXME add OR option one day.
            if (lo != LogicalOperator.And)
            {
                return false;
            }

            if (parameter == null)
            {
                return false;
            }

            ////FIXME not sure how else to do this...
            if (parameterName == "Level")
            {
                return LevelPassesFilter(room);
            }

            if (co == ComparisonOperator.Contains)
            {
                return ParameterValueContainsString(parameter, test);
            }

            if (co == ComparisonOperator.DoesNotContain)
            {
                return !ParameterValueContainsString(parameter, test);
            }

            int p = ParameterComparedToString(parameter, test);

            switch (co)
            {
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

        private static int ParameterComparedToString(Parameter param, string value)
        {
            // ReSharper disable once InconsistentNaming
            const int Result = 441976;
            if (!param.HasValue || string.IsNullOrWhiteSpace(value))
            {
                return Result;
            }
            switch (param.StorageType)
            {
                case StorageType.Double:
                    if (double.TryParse(value, out var parse))
                    {
                        return param.AsDouble().CompareTo(parse);
                    }
                    break;

                case StorageType.String:
#pragma warning disable CA1307 // Specify StringComparison
                    return string.Compare(param.AsString(), value, StringComparison.Ordinal);
#pragma warning restore CA1307 // Specify StringComparison

                case StorageType.Integer:
                    if (int.TryParse(value, out var iparse))
                    {
                        return param.AsInteger().CompareTo(iparse);
                    }
                    break;

                case StorageType.ElementId:
                    return Result;

                default:
                    return Result;
            }
            return Result;
        }

        private static bool ParameterValueContainsString(Parameter param, string value)
        {
            if (!param.HasValue || string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            switch (param.StorageType)
            {
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

        private static Parameter ParamFromString(Element element, string name)
        {
            if (element.GetParameters(name).Count > 0)
            {
                return element.GetParameters(name)[0];
            }
            return null;
        }

        private bool LevelPassesFilter(Room room)
        {
            if (co == ComparisonOperator.Equals)
            {
                return room.Level.Name == test;
            }

            if (co == ComparisonOperator.Contains)
            {
                return room.Level.Name.Contains(test);
            }

            var collector = new FilteredElementCollector(room.Document)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .Where(lvl => lvl.Name == test);

            if (collector.Any())
            {
                double elev = collector.First().Elevation;
                {
                    int p = room.Level.Elevation.CompareTo(elev);
                    switch (co)
                    {
                        case ComparisonOperator.LessThan:
                            return p < 0;

                        case ComparisonOperator.GreaterThan:
                            return p > 0;

                        case ComparisonOperator.NotEqual:
                            return p != 0;
                    }
                }
            }
            return false;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
