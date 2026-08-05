using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ColouredTabs {
    public class TabFilterRuleSetting {
        public string Colour { get; set; }
        public string TitleFilter { get; set; }
    }

    // Replaces pyRevit's user config ("tabcoloring" ini section read by tabs.py)
    public class TabSettings {
        public bool ColorizeDocTabs { get; set; } = true;
        public bool SortDocTabs { get; set; } = false;
        public int TabStyleIndex { get; set; } = TabColoringTheme.DefaultTabColoringStyleIndex;
        public int FamilyTabStyleIndex { get; set; } = TabColoringTheme.DefaultFamilyTabColoringStyleIndex;
        public List<string> TabColours { get; set; } = DefaultTabColours();
        public List<TabFilterRuleSetting> FilterRules { get; set; } = new List<TabFilterRuleSetting>();

        public static string SettingsPath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ColouredTabs",
                "ColouredTabs.ini");

        public static List<string> DefaultTabColours() {
            return TabColoringTheme.DefaultBrushes
                .OfType<SolidColorBrush>()
                .Select(ColourUtils.HexFromBrush)
                .ToList();
        }

        public static TabSettings Load() {
            var settings = new TabSettings();
            try {
                if (!File.Exists(SettingsPath)) {
                    settings.Save();
                    return settings;
                }

                var filterRules = new List<TabFilterRuleSetting>();
                foreach (string rawLine in File.ReadAllLines(SettingsPath)) {
                    string line = rawLine.Trim();
                    if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";") || line.StartsWith("["))
                        continue;

                    string[] parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length != 2)
                        continue;

                    string key = parts[0].Trim().ToLowerInvariant();
                    string value = parts[1].Trim();

                    switch (key) {
                        case "colorize_docs":
                            if (bool.TryParse(value, out bool colorize))
                                settings.ColorizeDocTabs = colorize;
                            break;
                        case "sort_colorize_docs":
                            if (bool.TryParse(value, out bool sort))
                                settings.SortDocTabs = sort;
                            break;
                        case "tabstyle_index":
                            if (int.TryParse(value, out int tabStyle))
                                settings.TabStyleIndex = tabStyle;
                            break;
                        case "family_tabstyle_index":
                            if (int.TryParse(value, out int famStyle))
                                settings.FamilyTabStyleIndex = famStyle;
                            break;
                        case "tab_colors":
                            var colours = value
                                .Split(',')
                                .Select(c => c.Trim())
                                .Where(c => c.Length > 0)
                                .ToList();
                            if (colours.Count > 0)
                                settings.TabColours = colours;
                            break;
                        default:
                            // filter_rule_<n> = <colour> :: <title regex>
                            if (key.StartsWith("filter_rule")) {
                                string[] rule = value.Split(new[] { "::" }, 2, StringSplitOptions.None);
                                if (rule.Length == 2)
                                    filterRules.Add(new TabFilterRuleSetting {
                                        Colour = rule[0].Trim(),
                                        TitleFilter = rule[1].Trim()
                                    });
                            }
                            break;
                    }
                }
                settings.FilterRules = filterRules;
            }
            catch (Exception ex) {
                // An unreadable/corrupt ini falls back to defaults rather than killing
                // startup, but leave a trace so the silent fallback is diagnosable.
                System.Diagnostics.Trace.TraceWarning("ColouredTabs: could not load {0}: {1}", SettingsPath, ex);
            }
            return settings;
        }

        public void Save() {
            var sb = new StringBuilder();
            sb.AppendLine("# ColouredTabs settings");
            sb.AppendLine("# Colours are #AARRGGBB or #RRGGBB hex, or any WPF colour name (e.g. OrangeRed).");
            sb.AppendLine("#");
            sb.AppendLine("# Style indices:");
            for (int i = 0; i < TabColoringTheme.AvailableStyles.Count; i++)
                sb.AppendLine($"#   {i} = {TabColoringTheme.AvailableStyles[i].Name}");
            sb.AppendLine();
            sb.AppendLine("[tabcoloring]");
            sb.AppendLine($"colorize_docs = {ColorizeDocTabs.ToString().ToLowerInvariant()}");
            sb.AppendLine($"sort_colorize_docs = {SortDocTabs.ToString().ToLowerInvariant()}");
            sb.AppendLine($"tabstyle_index = {TabStyleIndex}");
            sb.AppendLine($"family_tabstyle_index = {FamilyTabStyleIndex}");
            sb.AppendLine($"tab_colors = {string.Join(", ", TabColours)}");
            sb.AppendLine();
            sb.AppendLine("# Filter rules override order-based colours when a tab title matches the regex:");
            sb.AppendLine("#   filter_rule_1 = #FFFF0000 :: .*Template.*");
            int index = 1;
            foreach (TabFilterRuleSetting rule in FilterRules) {
                sb.AppendLine($"filter_rule_{index} = {rule.Colour} :: {rule.TitleFilter}");
                index++;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
            File.WriteAllText(SettingsPath, sb.ToString());
        }

        // Equivalent of tabs.py get_tabcoloring_theme()
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
                System.Diagnostics.Trace.TraceWarning("ColouredTabs: invalid colour '{0}' ignored", colourHex);
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
