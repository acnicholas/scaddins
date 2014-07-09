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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace SCaddins.SCwash
{
    class SCwashTreeNode : TreeNode
    {

        private string info;
        public string Info{
            get { return info; }
            set { info = value; }
        }

        private ElementId id;
        public ElementId Id
        {
            get { return id; }
            set { id = value; }
        }

        public SCwashTreeNode(string name) : base(name) {
            info = "-";
            id = null;
            this.Checked = true;
        }

    }
}
