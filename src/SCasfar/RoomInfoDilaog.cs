/*
 * Created by SharpDevelop.
 * User: derob
 * Date: 31/05/2016
 * Time: 8:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;

namespace SCaddins.SCasfar
{
    /// <summary>
    /// Description of RoomInfoDilaog.
    /// </summary>
    public partial class RoomInfoDilaog : System.Windows.Forms.Form
    {
        public RoomInfoDilaog()
        {
            InitializeComponent();
            listView1.View = System.Windows.Forms.View.Details;
            listView1.Columns.Add("Parameter");
            listView1.Columns.Add("Value");
            listView1.Columns.Add("Type");
        }
        
        public void UpdateRoomInfo(Room room)
        {
            this.Text = room.Name;
            this.listView1.Items.Clear();
            foreach (Parameter p in room.Parameters) {  
                if (p.StorageType != StorageType.ElementId && p.StorageType != StorageType.None) {
                    listView1.Items.Add(new ListViewItem(new string[]{p.Definition.Name, GetParamValueAsString(p), p.StorageType.ToString()}));
                }
            }
        }
        
        private string GetParamValueAsString(Parameter param)
        {
            switch (param.StorageType){
                case StorageType.Double:
                    return param.AsDouble().ToString() + @"(" + param.AsValueString() + @")";
                case StorageType.String:
                    return param.AsString();
                case StorageType.Integer:
                     return param.AsInteger().ToString() + @"(" + param.AsValueString() + @")";
                case StorageType.ElementId:
                    return string.Empty;
                default:
                    return string.Empty;
            }   
        }
        
    }
}
