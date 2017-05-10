// (C) Copyright 2012-2015 by Andrew Nicholas
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

namespace SCaddins.ExportManager
{
    using System.Collections.ObjectModel;
    using System.Globalization;
    
    public class SegmentedSheetName
    {
        private Collection<string> hooks;
        
        public SegmentedSheetName()
        {
            this.hooks = new Collection<string>();
            this.Name = "YYYYMMDD-AD-NNN[R]";
        }
        
        public string Name {
            get; set;
        }
        
        public string NameFormat {
            get; set;
        }
    
        public Collection<string> Hooks {
            get { return this.hooks; }
        }
        
        public void AddHook(string hookName)
        {
            this.hooks.Add(hookName);
        }
        
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[SegmentedSheetName Hooks={0}, Name={1}, NameFormat={2}]", this.hooks, this.Name, this.NameFormat);
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
