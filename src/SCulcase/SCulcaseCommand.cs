// (C) Copyright 2012-2014 by Andrew Nicholas
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

namespace SCaddins.SCulcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIApplication application = commandData.Application;
            UIDocument document = application.ActiveUIDocument;
            #if REVIT2014
            ElementSet eset = document.Selection.Elements;
            IList<ElementId> elems = new List<ElementId>();
            foreach (Element e in eset) {
                elems.Add(e.Id);
            }         
            #else
            IList<ElementId> elems = document.Selection.GetElementIds().ToList<ElementId>();
            #endif
            using (var t = new TransactionGroup(doc, "SCulcase")) {
                      t.Start();
                      if (elems.Count == 0) {
                          var form = new SCulcaseMainForm(doc);
                          System.Diagnostics.Debug.Print(form.DialogResult.ToString());
                      } else {
                          UppercaseUtilities.ConvertSelection(ConversionMode.UpperCase, doc, elems);
                      }
                      t.Commit();
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
