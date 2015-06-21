// (C) Copyright 2013-2014 by Andrew Nicholas andrewnicholas@iinet.net.au
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

namespace SCaddins.SCwash
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Events;

    public partial class SCwashForm : System.Windows.Forms.Form
    {
        private Document doc;
        private UIDocument udoc;

        public SCwashForm(Document doc, UIDocument udoc)
        {
            this.doc = doc;
            this.udoc = udoc;
            this.InitializeComponent();
            treeView1.CheckBoxes = true;
            this.Init();
            textBox1.Text = "Select an item to show additional information";
        }
        
        public void CheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes) {
                node.Checked = true;
                this.CheckChildren(node, true);
            }
        }

        public void UncheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes) {
                node.Checked = false;
                this.CheckChildren(node, false);
            }
        }

        private void Init()
        {
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(new SCwashTreeNode("Views NOT on Sheets"));
            treeView1.Nodes.Add(new SCwashTreeNode("Views on Sheets"));
            treeView1.Nodes.Add(new SCwashTreeNode("Sheets"));
            treeView1.Nodes.Add(new SCwashTreeNode("Linked Files"));
            treeView1.Nodes.Add(new SCwashTreeNode("Imported Files"));
            treeView1.Nodes.Add(new SCwashTreeNode("Images"));
            treeView1.Nodes.Add(new SCwashTreeNode("Unbound Rooms"));
            treeView1.Nodes.Add(new SCwashTreeNode("Revisions"));
            SCwashUtilities.AddViewNodes(this.doc, false, treeView1.Nodes[0].Nodes);
            SCwashUtilities.AddViewNodes(this.doc, true, treeView1.Nodes[1].Nodes);
            SCwashUtilities.AddSheetNodes(this.doc, true, treeView1.Nodes[2].Nodes);
            treeView1.Nodes[3].Nodes.AddRange(SCwashUtilities.Imports(this.doc, true).ToArray<TreeNode>());
            treeView1.Nodes[4].Nodes.AddRange(SCwashUtilities.Imports(this.doc, false).ToArray<TreeNode>());
            treeView1.Nodes[5].Nodes.AddRange(SCwashUtilities.Images(this.doc).ToArray<TreeNode>());
            treeView1.Nodes[6].Nodes.AddRange(SCwashUtilities.UnboundRooms(this.doc).ToArray<TreeNode>());
            treeView1.Nodes[7].Nodes.AddRange(SCwashUtilities.Revisions(this.doc).ToArray<TreeNode>());
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode != null) {
                var t = (SCwashTreeNode)treeView1.SelectedNode;
                textBox1.Text = t.Info;
                if (t.Id != null) {
                    btnShowElement.Text = "Show" + System.Environment.NewLine + t.Id.ToString();
                    btnShowElement.Enabled = true;
                } else {
                    btnShowElement.Text = "Show Element";
                    btnShowElement.Enabled = false;
                }
            } else {
                textBox1.Text = "Select an item to show additional information";
            }
        }

        private void CheckChildren(TreeNode rootNode, bool selected)
        {
            foreach (TreeNode node in rootNode.Nodes) {
                this.CheckChildren(node, selected);
                node.Checked = selected;
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            this.CheckAllNodes(treeView1.Nodes);
        }

        private void BtnSelectNone_Click(object sender, EventArgs e)
        {
            this.UncheckAllNodes(treeView1.Nodes);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            // Make sure at least one revision remains
            int revisionsToStay = 0;
            foreach (SCwashTreeNode node in treeView1.Nodes[7].Nodes) {
                if (!node.Checked) {
                    // Ok, things shouldn't break here. move on.
                    revisionsToStay++;
                    break;
                }
            }
            
            // Un-mark the first revision because you can't delete them all.
            if (revisionsToStay == 0) {
                TaskDialog td = new TaskDialog("One last revision");
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                td.MainInstruction = "The project must have at least one revision!";
                td.MainContent = "Press OK for SCwash will keep the first revision for you." + System.Environment.NewLine +
                    System.Environment.NewLine +
                    "Press Cancel to select the revision you want to keep.";
                td.CommonButtons = TaskDialogCommonButtons.Cancel | TaskDialogCommonButtons.Ok;
                TaskDialogResult tr = td.Show();
                if (tr == TaskDialogResult.Cancel) {
                    return;
                }
                treeView1.Nodes[7].Nodes[0].Checked = false;    
            }
            
            ICollection<ElementId> elements = new List<ElementId>();
            foreach (SCwashTreeNode node in this.treeView1.Nodes) {
                if (node.Checked) {
                    this.AddSCChildrenToPurgeSet(ref elements, node);
                }
            }
            SCwashUtilities.RemoveElements(this.doc, elements);
            this.Init();
        }

        private void AddSCChildrenToPurgeSet(ref ICollection<ElementId> elements, SCwashTreeNode rootNode)
        {
            foreach (SCwashTreeNode node in rootNode.Nodes) {
                this.AddSCChildrenToPurgeSet(ref elements, node);
                if (node.Checked && node.Id != null) {
                    if (this.doc.GetElement(node.Id) != null && this.doc.ActiveView.Id != node.Id) {
                        elements.Add(node.Id);
                    }
                }
            }
        }

        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // treeView1.Nodes[7].Nodes[0].Checked = false;
            SCwashTreeNode tn = e.Node as SCwashTreeNode;
            foreach (SCwashTreeNode child in tn.Nodes) {
                if (!tn.Checked) {
                    child.ForeColor = System.Drawing.Color.LightGray;
                }
                if (tn.Checked) {
                    child.ForeColor = System.Drawing.Color.Black;
                }
                foreach (SCwashTreeNode child2 in child.Nodes) {
                    if (!tn.Checked) {
                        child2.ForeColor = System.Drawing.Color.LightGray;
                    }
                    if (tn.Checked) {
                        child2.ForeColor = System.Drawing.Color.Black;
                    }
                }
            }
        }

        private void SCwashForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TaskDialog.Show("Reminder", "Remember to purge!");
        }

        private void BtnShowElement_Click(object sender, EventArgs e)
        {
            // open view with selected id.
            UIApplication uiapp = new UIApplication(this.udoc.Application.Application);
            uiapp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(this.DismissOpenQuestion);
            SCwashTreeNode t = (SCwashTreeNode)treeView1.SelectedNode;
            if (t.Id != null) {
                this.udoc.ShowElements(t.Id);
            }
        }

        private void DismissOpenQuestion(object o, DialogBoxShowingEventArgs e)
        {
            TaskDialogShowingEventArgs t = e as TaskDialogShowingEventArgs;
            Debug.Print(t.Message);
            if (t != null && t.Message == "There is no open view that shows any of the highlighted elements.  Searching through the closed views to find a good view could take a long time.  Continue?") {
                e.OverrideResult((int)TaskDialogResult.Ok);
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
