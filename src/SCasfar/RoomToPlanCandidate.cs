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
    
    public class RoomToPlanCandidate : INotifyPropertyChanged
    {  
        private string destViewName;
        private string destSheetName; 
        private string destSheetNumber; 
        
        public RoomToPlanCandidate(SpatialElement room, Document doc)
        {
            this.Room = room;
            this.DestSheetName = GetDefaultSheetName(doc);
            this.DestSheetNumber = Number;
            this.DestViewName = GetDefaultViewName(doc);
        }
               
        public event PropertyChangedEventHandler PropertyChanged;
               
        public SpatialElement Room {
            get; set;    
        }
        
        public string Number {
            get {
                return Room.Number;
            }
        }
        
        public string Name {
            get {
                return Room.Name;
            }
        }
       

        public string DestViewName {
            get {
                return this.destViewName;
            }
            
            set {
                this.destViewName = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("DestViewName"));
                }
            }
        }
        
        public string DestSheetNumber {
            get {
                return this.destSheetNumber;
            }
            
            set {
                this.destSheetNumber = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("DestSheetNumber"));
                }
            }
        }
        
        public string DestSheetName {
            get {
                return this.destSheetName;
            }
            
            set {
                 this.destSheetName = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("DestSheetName"));
                }
            }
        }
                
        private string GetDefaultViewName(Document doc)
        {
            string request = this.Number + " - " + this.Name;
            //FIXME move this to somewhere nicer than SCaos.Command
            return SCaddins.SCaos.Command.GetNiceViewName(doc, request);
        }
        
        private string GetDefaultSheetName(Document doc)
        {
            string request = this.Number + " - " + this.Name;
            //FIXME move this to somewhere nicer than SCaos.Command
            return SCaddins.SCaos.Command.GetNiceViewName(doc, request);
        }
        
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */

