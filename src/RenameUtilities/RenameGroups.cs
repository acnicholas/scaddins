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

    public class RenameGroups : RenameCandidate
    {
        private Group group;

        public RenameGroups(Group group)
        {
            this.group = group;
            OldValue = group.Name;
            NewValue = OldValue;
        }

        public static BindableCollection<RenameParameter> GetParameters(Document doc)
        {
            BindableCollection<RenameParameter> parametersList = new BindableCollection<RenameParameter>();
            var collector = GetFilteredElementCollector(doc);
            var elem = collector.FirstElement();
            var elem2 = collector.ToElements()[collector.GetElementCount() - 1];
            if (elem2.Parameters.Size > elem.Parameters.Size)
            {
                elem = elem2;
            }
            parametersList.Add(new RenameParameter(null, BuiltInCategory.INVALID, null, RenameTypes.Group));
            return parametersList;
        }

        public static BindableCollection<RenameCandidate> GetCandidates(Document doc)
        {
            var result = new BindableCollection<RenameCandidate>();
            foreach (Element element in GetFilteredElementCollector(doc))
            {
                var group = (Group)element;
                if (group != null)
                {
                    var rc = new RenameGroups(group);
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
                    group.Name = NewValue;
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private static FilteredElementCollector GetFilteredElementCollector(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(GroupType));
            return collector;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */