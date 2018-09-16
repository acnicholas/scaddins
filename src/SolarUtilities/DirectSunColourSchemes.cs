using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace SCaddins.SolarUtilities
{
    class DirectSunColourSchemes
    {

        public static List<AnalysisDisplayColorEntry> DefaultColours
        {
            get
            {
                List<AnalysisDisplayColorEntry> result = new List<AnalysisDisplayColorEntry>
                {
                    new AnalysisDisplayColorEntry(new Color(75, 107, 169),0),
                    new AnalysisDisplayColorEntry(new Color(115, 147, 202)),
                    new AnalysisDisplayColorEntry(new Color(170, 200, 247)),
                    new AnalysisDisplayColorEntry(new Color(193, 213, 208)),
                    new AnalysisDisplayColorEntry(new Color(245, 239, 103)),
                    new AnalysisDisplayColorEntry(new Color(252, 230, 74)),
                    new AnalysisDisplayColorEntry(new Color(239, 156, 21)),
                    new AnalysisDisplayColorEntry(new Color(234, 123, 0)),
                    new AnalysisDisplayColorEntry(new Color(234, 74, 0)),
                    new AnalysisDisplayColorEntry(new Color(234, 38, 0),7)
                };
                return result;
            }
        }

        public static void CreateAnalysisScheme(List<AnalysisDisplayColorEntry> colours, Document doc, string name)
        {
            var colouredSurfaceSettings = new AnalysisDisplayColoredSurfaceSettings();
            colouredSurfaceSettings.ShowContourLines = true;
            colouredSurfaceSettings.ShowGridLines = false;

            var colourSettings = new AnalysisDisplayColorSettings();
            colourSettings.MaxColor = colours.Last().Color;
            colourSettings.MinColor = colours.First().Color;
            colourSettings.SetIntermediateColors(colours);
            colourSettings.ColorSettingsType = AnalysisDisplayStyleColorSettingsType.GradientColor;

            if (AnalysisDisplayStyle.IsNameUnique(doc, name, null)) {
                var ads = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(
                    doc,
                    name,
                    colouredSurfaceSettings,
                    colourSettings,
                    new AnalysisDisplayLegendSettings()
                );
            }
        }
    }
}
