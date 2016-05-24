/*
 * Created by SharpDevelop.
 * User: andrewn
 * Date: 24/05/16
 * Time: 4:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace SCaddins.SCasfar
{
    /// <summary>
    /// Description of SCasfarForm.
    /// </summary>
    public partial class SCasfarForm : System.Windows.Forms.Form
    {
        public SCasfarForm(Document doc)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            SpatialElement room = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).FirstElement() as SpatialElement;
            
            foreach (Parameter p in room.Parameters) {               
                ListViewItem item1 = new ListViewItem(p.Definition.Name);
                item1.SubItems.Add(p.GetType().ToString());
                item1.SubItems.Add(string.Empty);
                listView1.Items.Add(item1);
            }
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
    }
}
