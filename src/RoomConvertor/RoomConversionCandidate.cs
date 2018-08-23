// (C) Copyright 2016-2018 by Andrew Nicholas
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;

    public class RoomConversionCandidate : INotifyPropertyChanged
    {
        private string destSheetName;
        private string destSheetNumber;
        private string destViewName;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter intialized by Revit", MessageId = "room")]
        private Room room;

        public RoomConversionCandidate(
                Room room,
                Dictionary<string, View> existingSheets,
                Dictionary<string, View> existingViews)
        {
            this.room = room;
            this.destSheetName = GetDefaultSheetName();
            this.destSheetNumber = GetDefaultSheetNumber(existingSheets);
            this.destViewName = GetDefaultViewName(existingViews);
            RoomParameters = new List<RoomParameter>();
            foreach (Parameter p in room.Parameters)
            {
                if (p.StorageType != StorageType.ElementId && p.StorageType != StorageType.None)
                {
                    RoomParameters.Add(new RoomParameter(p.Definition.Name, GetParamValueAsString(p), p.StorageType.ToString()));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string DestinationSheetName
        {
            get
            {
                return this.destSheetName;
            }

            set
            {
                this.destSheetName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(DestinationSheetName)));
                }
            }
        }

        public string DestinationSheetNumber
        {
            get
            {
                return this.destSheetNumber;
            }

            set
            {
                this.destSheetNumber = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(DestinationSheetNumber)));
                }
            }
        }

        public string DestinationViewName
        {
            get
            {
                return this.destViewName;
            }

            set
            {
                this.destViewName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(DestinationViewName)));
                }
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(room.Number))
                {
                    return room.Name;
                }
                else
                {
                    string r = room.Name.Replace(room.Number, string.Empty).Trim();
                    return string.IsNullOrWhiteSpace(r) ? "-" : r;
                }
            }
        }

        public string Number
        {
            get
            {
                string n = room.Number;
                return string.IsNullOrWhiteSpace(n) ? "-" : n;
            }
        }

        public Room Room
        {
            get
            {
                return room;
            }
        }

        public List<RoomParameter> RoomParameters
        {
            get; set;
        }

        ////FIXME put ths somewhere more useful.
        public static string GetParamValueAsString(Parameter param)
        {
            switch (param.StorageType)
            {
                case StorageType.Double:
                    return param.AsDouble().ToString(CultureInfo.CurrentCulture) + @"(" + param.AsValueString() + @")";

                case StorageType.String:
                    return param.AsString();

                case StorageType.Integer:
                    return param.AsInteger().ToString(CultureInfo.CurrentCulture) + @"(" + param.AsValueString() + @")";

                case StorageType.ElementId:
                    return string.Empty;

                default:
                    return string.Empty;
            }
        }

        public bool PassesFilter(RoomFilter filter)
        {
            return filter == null ? false : filter.PassesFilter(this.Room);
        }

        private string GetDefaultSheetName()
        {
            // this is OK, sheets can cave duplicate names
            return this.Number + " - " + this.Name;
        }

        private string GetDefaultSheetNumber(Dictionary<string, View> existingSheets)
        {
            string request = this.Number;
            if (existingSheets.ContainsKey(request))
            {
                return request + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) + @")";
            }
            else
            {
                return request;
            }
        }

        private string GetDefaultViewName(Dictionary<string, View> existingViews)
        {
            string request = this.Number + " - " + this.Name;
            if (existingViews.ContainsKey(request))
            {
                return request + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) + @")";
            }
            else
            {
                return request;
            }
        }
    }

    public class RoomParameter
    {
        public RoomParameter(string def, string value, string convertedValue)
        {
            Def = def;
            Value = value;
            ConvertedValue = convertedValue;
        }

        public string ConvertedValue { get; set; }

        public string Def { get; set; }

        public string Value { get; set; }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */