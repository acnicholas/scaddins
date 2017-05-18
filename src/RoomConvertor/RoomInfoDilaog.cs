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

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System.Globalization;
using SCaddins.Properties;

namespace SCaddins.RoomConvertor
{

    public partial class RoomInfoDialog : System.Windows.Forms.Form
    {
        public RoomInfoDialog()
        {
            InitializeComponent();
            listView1.View = System.Windows.Forms.View.Details;
            listView1.Columns.Add(Resources.Parameter);
            listView1.Columns.Add(Resources.Value);
            listView1.Columns.Add(Resources.Type);
            listView1.Columns[0].Width = 200;
            listView1.Columns[1].Width = 200;
            listView1.Columns[2].Width = 100;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void UpdateRoomInfo(Room room)
        {
            if (room == null) {
                return;
            }
            this.Text = room.Name;
            this.listView1.Items.Clear();
            foreach (Parameter p in room.Parameters) {  
                if (p.StorageType != StorageType.ElementId && p.StorageType != StorageType.None) {
                    listView1.Items.Add(new ListViewItem(new string[]{p.Definition.Name, GetParamValueAsString(p), p.StorageType.ToString()}));
                }
            }
        }

        private static string GetParamValueAsString(Parameter param)
        {
            switch (param.StorageType){
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
    }
}
