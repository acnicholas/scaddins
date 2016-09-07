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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;
    using System.Globalization;

    public class RoomConversionCandidate : INotifyPropertyChanged
    {
        private Room room;
        private string destViewName;
        private string destSheetName; 
        private string destSheetNumber; 

        public RoomConversionCandidate(
                Room room,
                Document doc,
                Dictionary<string, View> existingSheets,
                Dictionary<string, View> existingViews)
        {
            this.room = room;
            this.destSheetName = GetDefaultSheetName();
            this.destSheetNumber = GetDefaultSheetNumber(doc, existingSheets);
            this.destViewName = GetDefaultViewName(doc, existingViews);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Room Room {
            get {
                return room;
            }
        }

        public string Number {
            get {
                string n = room.Number;
                return string.IsNullOrWhiteSpace(n) ? "-" : n;
            }
        }

        public string Name {
            get {
                if (string.IsNullOrWhiteSpace(room.Number)) {
                    return room.Name;
                } else {
                    string r = room.Name.Replace(room.Number, "").Trim();
                    return string.IsNullOrWhiteSpace(r) ? "-" : r;
                }
            }
        }

        public string DestinationViewName {
            get {
                return this.destViewName;
            }
            set {
                this.destViewName = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("DestinationViewName"));
                }
            }
        }

        public string DestinationSheetNumber {
            get {
                return this.destSheetNumber;
            }
            set {
                this.destSheetNumber = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("DestinationSheetNumber"));
                }
            }
        }

        public string DestinationSheetName {
            get {
                return this.destSheetName;
            }
            set {
                this.destSheetName = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("DestinationSheetName"));
                }
            }
        }

        public bool PassesFilter(RoomFilter filter)
        {
            return filter.PassesFilter(this.Room);
        }

        private string GetDefaultViewName(Document doc, Dictionary<string, View> existingViews)
        {
            string request = this.Number + " - " + this.Name;
            if(existingViews.ContainsKey(request)){
                return request + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) + @")";       
            } else {
                return request;
            }
        }

        private string GetDefaultSheetName()
        {
            //this is OK, sheets can cave duplicate names
            return this.Number + " - " + this.Name;
        }

        private string GetDefaultSheetNumber(Document doc, Dictionary<string, View> existingSheets)
        {
            string request = this.Number;
            if(existingSheets.ContainsKey(request)){
                return request + @"(" + (DateTime.Now.TimeOfDay.Ticks / 100000).ToString(CultureInfo.InvariantCulture) + @")";       
            } else {
                return request;
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */

