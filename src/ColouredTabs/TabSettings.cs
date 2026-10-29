using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace SCaddins.ColouredTabs {
    public class TabFilterRuleSetting {
        public string Colour { get; set; }
        public string TitleFilter { get; set; }
    }

    // Replaces pyRevit's user config ("tabcoloring" ini section read by tabs.py)
    public class TabSettings {

        public bool ColorizeDocTabs
        {
            get
            {
                return Settings1.Default.ColorizeDocs;
            }
            set
            {
                Settings1.Default.ColorizeDocs = value;
                Settings1.Default.Save();
            }
        }
        public bool SortDocTabs
        {
            get
            {
                return Settings1.Default.SortColorizeDocs;
            }
            set
            {
                Settings1.Default.SortColorizeDocs = value;
                Settings1.Default.Save();
            }
        }
        public int TabStyleIndex
        {
            get
            {
                return Settings1.Default.TabstyleIndex;
            }
            set
            {
                Settings1.Default.TabstyleIndex = value;
                Settings1.Default.Save();
            }
        }
        public int FamilyTabStyleIndex
        {
            get
            {
                return Settings1.Default.FamilyTabstyleIndex;
            }
            set
            {
                Settings1.Default.FamilyTabstyleIndex = value;
                Settings1.Default.Save();
            }
        }

        public List<string> TabColours
        {
            get
            {
                return DefaultTabColours();
            }
        }

        public List<TabFilterRuleSetting> FilterRules { get; set; } = new List<TabFilterRuleSetting>();

        public static List<string> DefaultTabColours() {
            return TabColoringTheme.DefaultBrushes
                .OfType<SolidColorBrush>()
                .Select(ColourUtils.HexFromBrush)
                .ToList();
        }

        public static TabSettings Load() {
            var settings = new TabSettings();
            return settings;
        }

        public TabColoringTheme CreateTheme() {
            var styles = TabColoringTheme.AvailableStyles;

            var theme = new TabColoringTheme {
                SortDocTabs = SortDocTabs,
                TabStyle = styles[Clamp(TabStyleIndex, 0, styles.Count - 1)],
                FamilyTabStyle = styles[Clamp(FamilyTabStyleIndex, 0, styles.Count - 1)],
                TabOrderRules = TabColours
                    .Select(TryMakeBrush)
                    .Where(b => b != null)
                    .Select(b => new TabColoringRule(b))
                    .ToList(),
                TabFilterRules = FilterRules
                    .Select(f => new { Brush = TryMakeBrush(f.Colour), f.TitleFilter })
                    .Where(f => f.Brush != null)
                    .Select(f => new TabColoringRule(f.Brush, f.TitleFilter))
                    .ToList(),
            };

            if (theme.TabOrderRules.Count == 0)
                theme.TabOrderRules.Clear();
                theme.TabOrderRules = TabColoringTheme.DefaultBrushes
                    .OfType<SolidColorBrush>()
                    .Select(b => new TabColoringRule(b))
                    .ToList();

            return theme;
        }

        static SolidColorBrush TryMakeBrush(string colourHex) {
            try {
                return ColourUtils.HexToBrush(colourHex);
            }
            catch {
                // user-typed colour string from the ini that WPF can't parse — skip it
                System.Diagnostics.Trace.TraceWarning("SCaddins.ColouredTabs: invalid colour '{0}' ignored", colourHex);
                return null;
            }
        }

        static int Clamp(int value, int min, int max) {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
