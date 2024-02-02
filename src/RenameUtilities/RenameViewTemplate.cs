// (C) Copyright 2017-2023 by Andrew Nicholas
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

namespace SCaddins.RenameUtilities
{
    using Autodesk.Revit.DB;
    using Caliburn.Micro;
    using View = Autodesk.Revit.DB.View;

    public class RenameViewTemplate : RenameCandidate
    {
        private Autodesk.Revit.DB.View view;

        public RenameViewTemplate(Autodesk.Revit.DB.View view)
        {
            this.view = view;
            OldValue = view.Name;
            NewValue = OldValue;
        }

        public static BindableCollection<RenameCandidate> GetCandidates(Document doc, RenameParameter renameParameter)
        {
            var result = new BindableCollection<RenameCandidate>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(View));
            foreach (Element element in collector)
            {
                var view = (Autodesk.Revit.DB.View)element;
                if (view != null && view.IsTemplate)
                {
                    var rc = new RenameViewTemplate(view);
                    result.Add(rc);
                }
            }
            return result;
        }

        public override bool Rename()
        {
            if (ValueChanged)
            {
                try
                {
                    view.Name = NewValue;
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */