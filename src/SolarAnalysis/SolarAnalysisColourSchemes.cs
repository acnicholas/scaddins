// (C) Copyright 2018-2020 by Andrew Nicholas
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

namespace SCaddins.SolarAnalysis
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Analysis;

    public static class SolarAnalysisColourSchemes
    {
        public static List<AnalysisDisplayColorEntry> DefaultColours
        {
            get
            {
                List<AnalysisDisplayColorEntry> result = new List<AnalysisDisplayColorEntry>
                {
                    new AnalysisDisplayColorEntry(new Color(75, 107, 169), 0),
                    new AnalysisDisplayColorEntry(new Color(115, 147, 202)),
                    new AnalysisDisplayColorEntry(new Color(170, 200, 247)),
                    new AnalysisDisplayColorEntry(new Color(193, 213, 208)),
                    new AnalysisDisplayColorEntry(new Color(245, 239, 103)),
                    new AnalysisDisplayColorEntry(new Color(252, 230, 74)),
                    new AnalysisDisplayColorEntry(new Color(239, 156, 21)),
                    new AnalysisDisplayColorEntry(new Color(234, 123, 0)),
                    new AnalysisDisplayColorEntry(new Color(234, 74, 0)),
                    new AnalysisDisplayColorEntry(new Color(234, 38, 0), 7)
                };
                return result;
            }
        }

        public static ElementId CreateAnalysisScheme(List<AnalysisDisplayColorEntry> colours, Document doc, string name, bool showLegend)
        {
            var result = ElementId.InvalidElementId;

            var colouredSurfaceSettings = new AnalysisDisplayColoredSurfaceSettings();
            colouredSurfaceSettings.ShowContourLines = true;
            colouredSurfaceSettings.ShowGridLines = false;

            var colourSettings = new AnalysisDisplayColorSettings();
            colourSettings.MaxColor = colours.Last().Color;
            colourSettings.MinColor = colours.First().Color;
            colourSettings.SetIntermediateColors(colours);
            colourSettings.ColorSettingsType = AnalysisDisplayStyleColorSettingsType.GradientColor;

            if (AnalysisDisplayStyle.IsNameUnique(doc, name, null))
            {
                var ads = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(
                    doc,
                    name,
                    colouredSurfaceSettings,
                    colourSettings,
                    new AnalysisDisplayLegendSettings());
                if (!showLegend)
                {
                    ads.GetLegendSettings().ShowLegend = false;
                }
                result = ads.Id;
            }
            else
            {
                result = AnalysisDisplayStyle.FindByName(doc, name);
            }

            return result;
        }
    }
}
