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

    public class RenameWindows : RenameCandidate
    {
        private FamilyInstance family;

        public RenameWindows(FamilyInstance family)
        {
            this.family = family;
            OldValue = family.Name;
            NewValue = OldValue;
        }

        public static BindableCollection<RenameCandidate> GetCandidates(Document doc)
        {
            var result = new BindableCollection<RenameCandidate>();
            foreach (Element element in GetFilteredElementCollector(doc))
            {
                var family = (FamilyInstance)element;
                if (family != null)
                {
                    var rc = new RenameDoors(family);
                    result.Add(rc);
                }
            }
            return result;
        }

        public static BindableCollection<RenameParameter> GetParameters(Document doc)
        {
            BindableCollection<RenameParameter> parametersList = new Caliburn.Micro.BindableCollection<RenameParameter>();
            var collector = GetFilteredElementCollector(doc);
            var elem = collector.FirstElement();
            foreach (Autodesk.Revit.DB.Parameter param in elem.Parameters)
            {
                if (param.StorageType == StorageType.String && !param.IsReadOnly)
                {
                    parametersList.Add(new RenameParameter(param, BuiltInCategory.OST_Windows, null, RenameTypes.Windows));
                }
            }
            return parametersList;
        }

        private static FilteredElementCollector GetFilteredElementCollector(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Windows);
            collector.OfClass(typeof(FamilyInstance));
            return collector;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
