using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Autodesk.Revit.UI;

// AvalonDock also declares a DocumentClosedEventArgs; alias Revit's to keep it unambiguous
using RevitDocumentClosedEventArgs = Autodesk.Revit.DB.Events.DocumentClosedEventArgs;

using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;

namespace ColouredTabs {
    // Ported from pyRevit runtime EventHandling.cs (DocumentTabEventUtils)
    public static class DocumentTabEventUtils {
        public static UIApplication UIApp { get; private set; }

        public static bool IsUpdatingDocumentTabs { get; private set; }

        static readonly object UpdateLock = new object();

        static TabColoringTheme _tabColoringTheme;
        public static TabColoringTheme TabColoringTheme {
            get => _tabColoringTheme;
            set {
                // a newly applied theme adopts the previous theme's slots under its own rules
                if (value is TabColoringTheme && _tabColoringTheme != null)
                    value.InitSlots(_tabColoringTheme);
                _tabColoringTheme = value;
            }
        }

        public static DockingManager GetDockingManager(UIApplication uiapp) {
            var wndRoot = VisualTreeUtils.GetWindowRoot(uiapp);
            if (wndRoot != null)
                return VisualTreeUtils.FindFirstChild<DockingManager>(wndRoot);
            return null;
        }

        // LayoutUpdated fires on every layout pass of the main window — scrolling, panning,
        // resizing, all of it — so each tick has to be near-free. Resolving the tab group
        // means walking the visual tree from the window root, by far the most expensive
        // part of a tick; cache the control instead. AvalonDock only replaces it when the
        // docking layout is rebuilt, and then the old instance is detached from its
        // PresentationSource, which is a cheap O(1) probe — so a connected cached control
        // is always still the live one.
        static LayoutDocumentPaneGroupControl _docTabGroup;

        public static LayoutDocumentPaneGroupControl GetDocumentTabGroup(UIApplication uiapp) {
            if (_docTabGroup != null && PresentationSource.FromVisual(_docTabGroup) != null)
                return _docTabGroup;

            _docTabGroup = null;
            var wndRoot = VisualTreeUtils.GetWindowRoot(uiapp);
            if (wndRoot != null)
                _docTabGroup = VisualTreeUtils.FindFirstChild<LayoutDocumentPaneGroupControl>(wndRoot);
            return _docTabGroup;
        }

        public static IEnumerable<TabItem> GetDocumentTabs(LayoutDocumentPaneGroupControl docTabGroup) {
            var tabs = new List<TabItem>();
            if (docTabGroup != null)
                CollectDocumentTabs(docTabGroup, tabs);
            return tabs;
        }

        // Descends only through the docking scaffolding (grids, splitters, nested pane
        // groups) and stops at each LayoutDocumentPaneControl — a TabControl whose visual
        // subtree contains the entire content of the active view. Never walking below the
        // TabControl is what keeps a tick proportional to the number of tabs: the TabItem
        // containers come from the TabControl's ItemContainerGenerator (a lookup, not a
        // tree walk). Mid-churn a container may not be generated yet; the LayoutUpdated
        // that fires when generation completes picks it up.
        static void CollectDocumentTabs(DependencyObject parent, List<TabItem> tabs) {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++) {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is LayoutDocumentPaneControl pane) {
                    for (int item = 0; item < pane.Items.Count; item++) {
                        if (pane.ItemContainerGenerator.ContainerFromIndex(item) is TabItem tab)
                            tabs.Add(tab);
                    }
                }
                else {
                    CollectDocumentTabs(child, tabs);
                }
            }
        }

        public static void StartGroupingDocumentTabs(UIApplication uiapp) {
            lock (UpdateLock) {
                if (!IsUpdatingDocumentTabs) {
                    UIApp = uiapp;

                    DockingManager docMgr = GetDockingManager(UIApp);
                    if (docMgr is null)
                        return;

                    docMgr.LayoutUpdated += UpdateDockingManagerLayout;
                    // closing a document can tear the pane group down (last tab closing
                    // removes it entirely), so drop the cached control at that point
                    UIApp.Application.DocumentClosed += OnDocumentClosed;
                    IsUpdatingDocumentTabs = true;
                }
            }
            UpdateDocumentTabGroups();
        }

        public static void StopGroupingDocumentTabs() {
            lock (UpdateLock) {
                if (IsUpdatingDocumentTabs) {
                    DockingManager docMgr = GetDockingManager(UIApp);
                    if (docMgr != null)
                        docMgr.LayoutUpdated -= UpdateDockingManagerLayout;
                    UIApp.Application.DocumentClosed -= OnDocumentClosed;

                    ClearDocumentTabGroups();
                    _docTabGroup = null;

                    IsUpdatingDocumentTabs = false;
                }
            }
        }

        public static void ResetGroupingDocumentTabs() => _tabColoringTheme?.ResetSlots();

        static void UpdateDockingManagerLayout(object sender, EventArgs e) {
            UpdateDocumentTabGroups();
        }

        static void OnDocumentClosed(object sender, RevitDocumentClosedEventArgs e) {
            _docTabGroup = null;
        }

        static void ClearDocumentTabGroups() {
            var docTabGroup = GetDocumentTabGroup(UIApp);
            if (docTabGroup != null) {
                var docTabs = GetDocumentTabs(docTabGroup);
                if (!docTabs.Any())
                    return;

                if (TabColoringTheme is TabColoringTheme theme)
                    theme.ClearTheme(UIApp, docTabs);
            }
        }

        // Per-tick cost is: cached-control probe + generator lookups + SetTheme's state
        // hash, which bails before any document enumeration or styling when the tabs
        // haven't changed. The expensive work only runs on an actual tab change.
        static void UpdateDocumentTabGroups() {
            lock (UpdateLock) {
                if (IsUpdatingDocumentTabs) {
                    var docTabGroup = GetDocumentTabGroup(UIApp);
                    if (docTabGroup != null) {
                        var docTabs = GetDocumentTabs(docTabGroup).ToList();
                        if (docTabs.Count == 0)
                            return;

                        if (TabColoringTheme is TabColoringTheme theme)
                            theme.SetTheme(UIApp, docTabs);
                    }
                }
            }
        }
    }
}
