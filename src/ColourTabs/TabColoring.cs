using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Xceed.Wpf.AvalonDock.Layout;

using Color = System.Windows.Media.Color;

namespace ColouredTabs {
    // Equivalents of tabs.py hex_to_brush / hex_from_brush
    public static class ColourUtils {
        public static SolidColorBrush HexToBrush(string colourHex) {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colourHex));
        }

        public static string HexFromBrush(SolidColorBrush brush) {
            Color c = brush.Color;
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
        }
    }

    // Ported from pyRevit runtime EventHandling.cs (TabColoringRule)
    public class TabColoringRule {
        public SolidColorBrush Brush { get; set; }
        public Regex TitleFilter { get; set; }

        public TabColoringRule(SolidColorBrush brush, string filter = null) {
            Brush = brush;
            try {
                if (filter is string regexFilter)
                    TitleFilter = new Regex(regexFilter);
            }
            catch (ArgumentException) {
                // Invalid user-typed regex in the ini — the rule just never matches,
                // but note it so a "why isn't my filter working" question is answerable.
                System.Diagnostics.Trace.TraceWarning("ColouredTabs: invalid filter regex '{0}' ignored", filter);
            }
        }

        public bool IsMatch(string tabTitle) {
            if (TitleFilter is Regex filter)
                return filter.IsMatch(tabTitle);
            return false;
        }
    }

    // Ported from pyRevit runtime EventHandling.cs (TabColoringStyle)
    public class TabColoringStyle {
        public string Name { get; private set; }

        public Thickness BorderThickness { get; set; } = new Thickness();
        public bool FillBackground { get; set; } = false;

        public TabColoringStyle(string name) => Name = name;

        public static readonly Thickness DefaultBorderThickness = new Thickness();
        public static readonly Brush DefaultBorderBrush = Brushes.White;
        public static readonly Brush DefaultBackground = Brushes.Transparent;
        public static readonly Brush DefaultSelectedBackground = Brushes.White;
        public static readonly Brush DefaultForeground = Brushes.Black;
        public static readonly Brush LightForeground = Brushes.White;

        // Styles created here are tagged with this resource key so they can be told apart
        // from Revit's own tab styles. AvalonDock drops and re-adds TabItems during layout
        // churn; without the tag, a re-adopted tab that is already coloured would have our
        // style captured as its "original", and restoring it would keep the colour forever.
        internal const string StyleMarkerKey = "ColouredTabsStyle";

        internal static bool IsOurStyle(Style style) =>
            style != null && style.Resources.Contains(StyleMarkerKey);

        // walk the BasedOn chain back to the first style we did not create
        internal static Style GetOriginalStyle(Style style) {
            while (IsOurStyle(style))
                style = style.BasedOn;
            return style;
        }

        public Style CreateStyle(TabItem ctrl, TabColoringRule rule) {
            // base on the tab's true original style, never on one of ours, so the
            // BasedOn chain stays one link deep and always unwinds to the original
            Style tabStyle = new Style(typeof(TabItem), GetOriginalStyle(ctrl.Style));
            tabStyle.Resources[StyleMarkerKey] = true;

            var hslColor = new HslColor(rule.Brush.Color);

            var triggerSelected = new Trigger {
                Property = TabItem.IsSelectedProperty,
                Value = true
            };
            var triggerMouseOver = new Trigger {
                Property = TabItem.IsMouseOverProperty,
                Value = true
            };

            if (FillBackground) {
                tabStyle.Setters.Add(
                    new Setter { Property = TabItem.BackgroundProperty, Value = rule.Brush }
                );

                var bgHighlightBrush = new SolidColorBrush(hslColor.Lighten(1.1).ToRgb());
                triggerMouseOver.Setters.Add(
                    new Setter { Property = TabItem.BackgroundProperty, Value = bgHighlightBrush }
                );
                triggerSelected.Setters.Add(
                    new Setter { Property = TabItem.BackgroundProperty, Value = bgHighlightBrush }
                );

                // pick readable text colour for the background
                var foreground = hslColor.Luminance > 127.0f ? DefaultForeground : LightForeground;
                tabStyle.Setters.Add(
                    new Setter { Property = TabItem.ForegroundProperty, Value = foreground }
                );
                // foreground of the tab's inner "close" button
                tabStyle.Resources["ClientAreaForegroundBrush"] = foreground;
            }

            tabStyle.Setters.Add(
                new Setter { Property = TabItem.BorderBrushProperty, Value = rule.Brush }
            );
            tabStyle.Setters.Add(
                new Setter { Property = TabItem.BorderThicknessProperty, Value = BorderThickness }
            );

            var borderHighlightBrush = new SolidColorBrush(hslColor.Lighten(0.9).ToRgb());
            // selected tab hides the bottom border
            var selectedThickness = new Thickness(BorderThickness.Left, BorderThickness.Top, BorderThickness.Right, 0);
            triggerSelected.Setters.Add(
                new Setter { Property = TabItem.BorderThicknessProperty, Value = FillBackground ? new Thickness(1, 1, 1, 0) : selectedThickness }
            );

            // border highlight is only visible over a filled background
            if (FillBackground) {
                triggerSelected.Setters.Add(
                    new Setter { Property = TabItem.BorderBrushProperty, Value = Brushes.White }
                );
            }
            else {
                triggerSelected.Setters.Add(
                    new Setter { Property = TabItem.BorderBrushProperty, Value = borderHighlightBrush }
                );
            }

            triggerMouseOver.Setters.Add(
                new Setter { Property = TabItem.BorderBrushProperty, Value = borderHighlightBrush }
            );

            tabStyle.Triggers.Add(triggerSelected);
            tabStyle.Triggers.Add(triggerMouseOver);

            return tabStyle;
        }
    }

    // Ported from pyRevit runtime EventHandling.cs (TabColoringTheme)
    public class TabColoringTheme {
        public class RuleSlot {
            public TabColoringRule Rule { get; private set; }

            public RuleSlot(TabColoringRule rule) => Rule = rule;

            public long Id { get; set; }
            public bool IsFamily { get; set; }

            public void Clear() {
                Id = -1;
                IsFamily = false;
            }
        }

        // stored for parity with pyRevit config; not used by the colorizer (pyRevit doesn't either)
        public bool SortDocTabs { get; set; } = false;

        public TabColoringStyle TabStyle { get; set; }
        public TabColoringStyle FamilyTabStyle { get; set; }

        List<TabColoringRule> _tabOrderRules;
        public List<TabColoringRule> TabOrderRules {
            get {
                if (_tabOrderRules is null)
                    _tabOrderRules = new List<TabColoringRule>();
                return _tabOrderRules;
            }
            set {
                if (value is null)
                    _tabOrderRules = new List<TabColoringRule>();
                else
                    _tabOrderRules = value;
            }
        }

        List<TabColoringRule> _tabFilterRules;
        public List<TabColoringRule> TabFilterRules {
            get {
                if (_tabFilterRules is null)
                    _tabFilterRules = new List<TabColoringRule>();
                return _tabFilterRules;
            }
            set {
                if (value is null)
                    _tabFilterRules = new List<TabColoringRule>();
                else
                    _tabFilterRules = value;
            }
        }

        // The Studio.SC supporting palette. Colours are handed out in this order as
        // documents open, so the order is not the brand sheet's — consecutive entries are
        // arranged to jump hue family (cool → warm → green → earth) and swing between light
        // and dark, so two tabs open side by side never read as the same colour. The
        // sequence also wraps: the last entry contrasts with the first, since slots are
        // recycled when documents close.
        public static readonly List<Brush> DefaultBrushes = new List<Brush> {
            ColourUtils.HexToBrush("#FF2869A0"),   // Lapis
            ColourUtils.HexToBrush("#FFE15549"),   // Vermillion
            ColourUtils.HexToBrush("#FFAACD98"),   // Pistachio
            ColourUtils.HexToBrush("#FF734632"),   // Russet
            ColourUtils.HexToBrush("#FF7BB8DA"),   // Powder
            ColourUtils.HexToBrush("#FFE5A04B"),   // Tangerine
            ColourUtils.HexToBrush("#FF347C5A"),   // Forest
            ColourUtils.HexToBrush("#FFD0C096"),   // Sandstone
            ColourUtils.HexToBrush("#FF909BC8"),   // Lavender
            ColourUtils.HexToBrush("#FFB48D61"),   // Tan
            ColourUtils.HexToBrush("#FF6CB989"),   // Jade
            ColourUtils.HexToBrush("#FFEBC86E"),   // Mustard
        };

        public static readonly int DefaultTabColoringStyleIndex = 8;       // Background Fill
        public static readonly int DefaultFamilyTabColoringStyleIndex = 5; // Border - Medium (2px)

        public static readonly List<TabColoringStyle> AvailableStyles = new List<TabColoringStyle> {
            new TabColoringStyle("Top Bar - Light") { BorderThickness = new Thickness(0, 1, 0, 0) },
            new TabColoringStyle("Top Bar - Medium") { BorderThickness = new Thickness(0, 2, 0, 0) },
            new TabColoringStyle("Top Bar - Heavy") { BorderThickness = new Thickness(0, 3, 0, 0) },
            new TabColoringStyle("Top Bar - Heavier") { BorderThickness = new Thickness(0, 4, 0, 0) },
            new TabColoringStyle("Border - Light") { BorderThickness = new Thickness(1) },
            new TabColoringStyle("Border - Medium") { BorderThickness = new Thickness(2) },
            new TabColoringStyle("Border - Heavy") { BorderThickness = new Thickness(3) },
            new TabColoringStyle("Border - Heavier") { BorderThickness = new Thickness(4) },
            new TabColoringStyle("Background Fill") { BorderThickness = new Thickness(2), FillBackground = true },
        };

        // unique hash of the open-tabs state so styling only refreshes when tabs change
        string _lastTabState = string.Empty;

        // original tab styles, restored when the colorizer is turned off
        readonly Dictionary<TabItem, Style> _tabOrigStyles = new Dictionary<TabItem, Style>();

        // used slots for coloring rules
        readonly List<RuleSlot> _ruleSlots = new List<RuleSlot>();

        public List<RuleSlot> StyledDocuments => _ruleSlots.ToList();

        static string GetTabTitle(TabItem tab) {
            return (tab.Header as LayoutContent)?.Title ?? string.Empty;
        }

        static string GetTabUniqueId(TabItem tab) {
            return $"{GetTabTitle(tab)}+{tab.GetHashCode()}+{tab.IsSelected}";
        }

        // pyRevit casts through UIFramework's MFCMDIChildFrameControl/MFCMDIFrameHost to reach
        // the MFC document pointer; reflection avoids referencing those internal assemblies
        // and survives type moves between Revit versions.
        static long GetTabDocumentId(TabItem tab) {
            try {
                object childFrame = (tab.Content as LayoutContent)?.Content;
                object frameHost = childFrame is ContentControl cc
                    ? cc.Content
                    : childFrame?.GetType().GetProperty("Content")?.GetValue(childFrame);
                if (frameHost is null)
                    return -1;

                Type hostType = frameHost.GetType();
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                object docPtr =
                    hostType.GetField("document", flags)?.GetValue(frameHost)
                    ?? hostType.GetProperty("document", flags)?.GetValue(frameHost);
                if (docPtr is IntPtr ptr)
                    return ptr.ToInt64();
            }
            catch { } // reflection into Revit's internal UIFramework types — shape varies by version
            return -1;
        }

        static long GetApiDocumentId(Document doc) {
            try {
                MethodInfo getMFCDocMethod = doc.GetType().GetMethod("getMFCDoc", BindingFlags.Instance | BindingFlags.NonPublic);
                object mfcDoc = getMFCDocMethod.Invoke(doc, new object[] { });
                MethodInfo ptrValMethod = mfcDoc.GetType().GetMethod("GetPointerValue", BindingFlags.Instance | BindingFlags.NonPublic);
                return ((IntPtr)ptrValMethod.Invoke(mfcDoc, new object[] { })).ToInt64();
            }
            catch {
                // reflection into Document's non-public getMFCDoc — may move between versions
                return -1;
            }
        }

        public void SetTheme(UIApplication uiApp, IEnumerable<TabItem> docTabs) {
            // dont do anything if it is the same tabs as before
            string newState = string.Join(";", docTabs.Select(t => GetTabUniqueId(t)));
            if (newState == _lastTabState)
                return;
            _lastTabState = newState;

            // collect ids of open documents, noting which are families
            var docIds = new List<long>();
            var familyDocIds = new List<long>();
            foreach (Document doc in uiApp.Application.Documents) {
                // linked docs don't have tabs
                if (doc.IsLinked)
                    continue;

                long docId = GetApiDocumentId(doc);
                docIds.Add(docId);
                if (doc.IsFamilyDocument)
                    familyDocIds.Add(docId);
            }

            // cleanup styling slots for docs that no longer exist
            // empty these before setting new styles so freed slots can be re-taken
            var removedDocs = _ruleSlots.Where(d => !docIds.Contains(d.Id)).ToList();
            foreach (RuleSlot rslot in removedDocs)
                rslot.Clear();

            // cleanup any recorded tabs that do not exist anymore
            var removedTabs = _tabOrigStyles.Keys.Where(t => !docTabs.Contains(t)).ToList();
            foreach (TabItem tab in removedTabs)
                _tabOrigStyles.Remove(tab);

            foreach (TabItem tab in docTabs) {
                long docId = GetTabDocumentId(tab);

                if (!_tabOrigStyles.ContainsKey(tab))
                    _tabOrigStyles[tab] = TabColoringStyle.GetOriginalStyle(tab.Style);

                Set(
                    tab: tab,
                    docId: docId,
                    isFamilyTab: familyDocIds.Contains(docId)
                );
            }
        }

        void Set(TabItem tab, long docId, bool isFamilyTab) {
            TabColoringStyle tstyle = isFamilyTab ? FamilyTabStyle : TabStyle;

            string title = GetTabTitle(tab);

            // filter rules take priority over order rules
            foreach (TabColoringRule rule in TabFilterRules) {
                if (!rule.IsMatch(title))
                    continue;

                tab.Style = tstyle.CreateStyle(tab, rule);
                return;
            }

            // otherwise apply colors by order; reuse this doc's slot if it has one
            var docSlot = _ruleSlots.FirstOrDefault(d => d.Id == docId);
            if (docSlot is RuleSlot) {
                tab.Style = tstyle.CreateStyle(tab, docSlot.Rule);
            }
            else {
                RuleSlot slot = null;

                if (_ruleSlots.Count >= 1) {
                    int nextRuleIndex = _ruleSlots.Count;
                    // all rules are slotted; reuse a slot whose doc has been closed
                    if (nextRuleIndex >= TabOrderRules.Count) {
                        var firstEmptySlot = _ruleSlots.FirstOrDefault(r => r.Id == -1);
                        if (firstEmptySlot is RuleSlot) {
                            slot = firstEmptySlot;
                            slot.Id = docId;
                            slot.IsFamily = isFamilyTab;
                        }
                    }
                    else {
                        slot = new RuleSlot(TabOrderRules[nextRuleIndex]) {
                            Id = docId,
                            IsFamily = isFamilyTab,
                        };
                        _ruleSlots.Add(slot);
                    }
                }
                else {
                    if (TabOrderRules.Count > 0) {
                        slot = new RuleSlot(TabOrderRules.First()) {
                            Id = docId,
                            IsFamily = isFamilyTab,
                        };
                        _ruleSlots.Add(slot);
                    }
                }

                if (slot is RuleSlot)
                    tab.Style = tstyle.CreateStyle(tab, slot.Rule);
            }
        }

        public void ClearTheme(UIApplication uiApp, IEnumerable<TabItem> docTabs) {
            foreach (TabItem tab in docTabs) {
                if (_tabOrigStyles.TryGetValue(tab, out Style tabStyle))
                    RestoreStyle(tab, tabStyle);
                // tab was dropped and re-added by AvalonDock while coloured, so it was
                // never recorded — unwind our style through its BasedOn chain instead
                else if (TabColoringStyle.IsOurStyle(tab.Style))
                    RestoreStyle(tab, TabColoringStyle.GetOriginalStyle(tab.Style));
            }
            _tabOrigStyles.Clear();
            _lastTabState = string.Empty;
        }

        // a null "original" means the tab had no local style; clearing the property
        // (rather than assigning null) lets WPF's implicit style lookup apply again
        static void RestoreStyle(TabItem tab, Style style) {
            if (style is null)
                tab.ClearValue(FrameworkElement.StyleProperty);
            else
                tab.Style = style;
        }

        internal void ResetSlots() {
            _ruleSlots.Clear();
            _lastTabState = string.Empty;
        }

        internal void InitSlots(TabColoringTheme theme) {
            // adopt the reserved slots of the previous theme under the new rules
            int ruleCount = TabOrderRules.Count;
            if (ruleCount > 0) {
                int index = 0;
                foreach (RuleSlot slot in theme._ruleSlots) {
                    if (index >= ruleCount)
                        break;

                    _ruleSlots.Add(
                        new RuleSlot(TabOrderRules[index]) {
                            Id = slot.Id,
                            IsFamily = slot.IsFamily
                        }
                    );
                    index++;
                }
            }
        }
    }
}
