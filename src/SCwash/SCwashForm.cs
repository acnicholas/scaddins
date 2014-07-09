//
// (C) Copyright 2013 by Andrew Nicholas andrewnicholas@iinet.net.au
//
// This file is part of SCwash.
//
// SCwash is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCwash is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCwash.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System.Diagnostics;

namespace SCaddins.SCwash
{
    public partial class SCwashForm : System.Windows.Forms.Form
    {
        Document doc;
        UIDocument udoc;

        public SCwashForm(Document doc, UIDocument udoc)
        {
            this.doc = doc;
            this.udoc = udoc;
            InitializeComponent();
            SetIcon();
            treeView1.CheckBoxes = true;
            init();
            textBox1.Text = "Select an item to show additional information";
        }

        private void init()
        {
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(new SCwashTreeNode("Views NOT on Sheets"));
            treeView1.Nodes.Add(new SCwashTreeNode("Views on Sheets"));
            treeView1.Nodes.Add(new SCwashTreeNode("Sheets"));
            treeView1.Nodes.Add(new SCwashTreeNode("Linked Files"));
            treeView1.Nodes.Add(new SCwashTreeNode("Imported Files"));
            treeView1.Nodes.Add(new SCwashTreeNode("Images"));
            treeView1.Nodes.Add(new SCwashTreeNode("Unbound Rooms"));
            SCwash.AddViewNodes(doc,false,treeView1.Nodes[0].Nodes);
            SCwash.AddViewNodes(doc,true,treeView1.Nodes[1].Nodes);
            SCwash.AddSheetNodes(doc,true,treeView1.Nodes[2].Nodes);
            treeView1.Nodes[3].Nodes.AddRange(SCwash.Imports(doc, true).ToArray<TreeNode>());
            treeView1.Nodes[4].Nodes.AddRange(SCwash.Imports(doc, false).ToArray<TreeNode>());
            treeView1.Nodes[5].Nodes.AddRange(SCwash.Images(doc).ToArray<TreeNode>());
            treeView1.Nodes[6].Nodes.AddRange(SCwash.UnboundRooms(doc).ToArray<TreeNode>());
        }

        private void SetIcon()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream st = a.GetManifestResourceStream("SCwash.Resources.scwash.ico");
            System.Drawing.Icon icnTask = new System.Drawing.Icon(st);
            this.Icon = icnTask;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode != null) {
                SCwashTreeNode t = (SCwashTreeNode)treeView1.SelectedNode;
                textBox1.Text = t.Info;
                try {
                    button4.Text = "Show" + System.Environment.NewLine + t.Id.ToString();
                    button4.Enabled = true;
                } catch {
                    button4.Text = "Show Element";
                    button4.Enabled = false;
                }
            } else {
                textBox1.Text = "Select an item to show additional information";
            }
        }

        public void CheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes) {
                node.Checked = true;
                CheckChildren(node, true);
            }
        }

        public void UncheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes) {
                node.Checked = false;
                CheckChildren(node, false);
            }
        }

        private void CheckChildren(TreeNode rootNode, bool isChecked)
        {
            foreach (TreeNode node in rootNode.Nodes) {
                CheckChildren(node, isChecked);
                node.Checked = isChecked;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CheckAllNodes(treeView1.Nodes);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UncheckAllNodes(treeView1.Nodes);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ICollection<ElementId> elements = new List<ElementId>();
            foreach (SCwashTreeNode node in this.treeView1.Nodes) {
                if (node.Checked) {
                    addSCChildrenToPurgeSet(ref elements, node);
                }
            }
            SCwash.RemoveElements(doc,elements);
            init();
        }

        private void addSCChildrenToPurgeSet(ref ICollection<ElementId> elements, SCwashTreeNode rootNode)
        {
            foreach (SCwashTreeNode node in rootNode.Nodes) {
                addSCChildrenToPurgeSet(ref elements, node);
                if (node.Checked && node.Id != null) {
//                    #if REVIT2014
                    if (doc.GetElement(node.Id) != null && doc.ActiveView.Id != node.Id) {
                        elements.Add(node.Id);
                    }
//                    #else
//                    if (doc.get_Element(node.Id) != null && doc.ActiveView.Id != node.Id) {
//                        elements.Add(node.Id);
//                    }
//                    #endif
                }
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            SCwashTreeNode tn = e.Node as SCwashTreeNode;
                foreach (SCwashTreeNode child in tn.Nodes) {
                    if(!tn.Checked)child.ForeColor = System.Drawing.Color.LightGray;
                    if (tn.Checked) child.ForeColor = System.Drawing.Color.Black;
                    foreach (SCwashTreeNode child2 in child.Nodes) {
                        if (!tn.Checked) child2.ForeColor = System.Drawing.Color.LightGray;
                        if (tn.Checked) child2.ForeColor = System.Drawing.Color.Black;
                    }
                }
        }

        private void SCwashForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TaskDialog.Show("Reminder", "Remember to purge!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //open view with selected id.
            UIApplication uiapp = new UIApplication(udoc.Application.Application);
            uiapp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(dismissOpenQuestion);
            SCwashTreeNode t = (SCwashTreeNode)treeView1.SelectedNode;
            if (t.Id != null) udoc.ShowElements(t.Id);
        }

        private void dismissOpenQuestion(object o, DialogBoxShowingEventArgs e)
        {
            TaskDialogShowingEventArgs t = e as TaskDialogShowingEventArgs;
            Debug.Print(t.Message);
            if (t != null && t.Message == "There is no open view that shows any of the highlighted elements.  Searching through the closed views to find a good view could take a long time.  Continue?") {
                e.OverrideResult((int)TaskDialogResult.Ok);
            }
        }

    }

}
