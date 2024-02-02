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

    public class RenameLevels : RenameCandidate
    {
        private Level level;

        public RenameLevels(Level level)
        {
            this.level = level;
            OldValue = level.Name;
            NewValue = OldValue;
        }

        public static BindableCollection<RenameCandidate> GetCandidates(Document doc)
        {
            var result = new BindableCollection<RenameCandidate>();
            foreach (Element element in GetFilteredElementCollector(doc))
            {
                var level = element as Level;
                if (level != null)
                {
                    var rc = new RenameLevels(level);
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
                    level.Name = NewValue;
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
            collector.OfCategory(BuiltInCategory.OST_Levels);
            return collector;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */