// (C) Copyright 2013 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCexport.
//
// SCexport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCexport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCexport.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SCexport
{
    using System;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Events;
    
    /// <summary>
    /// Description of DialogHandler.
    /// </summary>
    public class DialogHandler
    {
        /// <summary>
        /// Create a new dialog handler.
        /// </summary>
        /// <param name="uiapp"></param>
        public DialogHandler(UIApplication uiapp)
        {
            uiapp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(this.DismissOpenQuestion);
        }
        
        private void DismissOpenQuestion(object o, DialogBoxShowingEventArgs e)
        {
            var t = e as TaskDialogShowingEventArgs;
            if (t != null && t.Message == "There is no open view that shows any of the highlighted elements.  Searching through the closed views to find a good view could take a long time.  Continue?") {
                e.OverrideResult((int)TaskDialogResult.Ok);
            }
        }
    }
}
