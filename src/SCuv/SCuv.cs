// (C) Copyright 2014 by Andrew Nicholas
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
using Autodesk.Revit.DB;

namespace SCaddins.SCuv
{
    /// <summary>
    /// Copy a view; give it a user name, remove any view templates and
    /// categorize it nicely.
    /// </summary>
    public class SCuv
    {
        public SCuv()
        {
        }
        
        public static void CreateUserView(View srcView, Document doc)
        {
            ElementId destViewId = srcView.Duplicate(ViewDuplicateOption.Duplicate);
            View newView = doc.GetElement(destViewId) as View;  
            newView.Name = SetNewViewName(srcView);  
            
            // TODO test this really works
            newView.ViewTemplateId = ElementId.InvalidElementId;
        }
             
        private static string SetNewViewName(View srcView)
        { 
            return Environment.UserName + "-" + srcView.Name;           
        }       
    }
}
