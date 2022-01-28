// (C) Copyright 2019-2020 by Andrew Nicholas
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

namespace SCaddins.ModelSetupWizard
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;

    public static class ElementCollectors
    {
        public static IEnumerable<ProjectInformationParameter> GetProjectInformationParameters(Document doc)
        {
            FilteredElementCollector fec = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ProjectInformation);
            foreach (Element e in fec)
            {
                foreach (var param in e.GetOrderedParameters())
                {
                    yield return new ProjectInformationParameter(param);
                }
            }
        }

        public static IEnumerable<WorksetParameter> GetWorksetParameters(Document doc)
        {
            var worksets = new FilteredWorksetCollector(doc).ToWorksets().Where(w => w.Kind == WorksetKind.UserWorkset);
            foreach (var workset in worksets)
            {
                yield return new WorksetParameter(workset.Name, workset.IsVisibleByDefault, workset.Id.IntegerValue);
            }
        }
    }
}
