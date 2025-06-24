// (C) Copyright 2017-2025 by Andrew Nicholas
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

    public class RenameDesignOption : RenameCandidate
    {
        private Element designOption;

        public RenameDesignOption(Element designOption)
        {
            this.designOption = designOption;
            OldValue = designOption.Name.Replace(@"  <primary>","");
            NewValue = OldValue;
        }

        public static BindableCollection<RenameParameter> GetParameters(Document doc)
        {
            BindableCollection<RenameParameter> parametersList = new BindableCollection<RenameParameter>();
            parametersList.Add(new RenameParameter(null,BuiltInCategory.INVALID, null, RenameTypes.DesignOptions));
            return parametersList;
        }

        public static BindableCollection<RenameCandidate> GetCandidates(Document doc)
        {
            var result = new BindableCollection<RenameCandidate>();
            foreach (Element element in GetFilteredElementCollector(doc))
            {
                var designOption = (Element)element;
                if (designOption != null)
                {
                    var rc = new RenameDesignOption(designOption);
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
                    designOption.Name = NewValue;
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
            collector.OfCategory(BuiltInCategory.OST_DesignOptions);
            return collector;
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */