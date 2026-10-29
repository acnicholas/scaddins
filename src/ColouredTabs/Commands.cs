using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace SCaddins.ColouredTabs {
    // buttons stay clickable when no document is open — the colorizer is pure UI
    public class AlwaysAvailable : IExternalCommandAvailability {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories) => true;
    }

    // equivalent of tabs.py toggle_doc_colorizer()
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ToggleColourizerCommand : IExternalCommand {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            UIApplication uiapp = commandData.Application;

            TabSettings settings = TabSettings.Load();

            if (DocumentTabEventUtils.IsUpdatingDocumentTabs) {
                DocumentTabEventUtils.StopGroupingDocumentTabs();
            }
            else {
                DocumentTabEventUtils.TabColoringTheme = settings.CreateTheme();
                // DocumentTabEventUtils.UpdateDocumentTabGroups();
                DocumentTabEventUtils.StartGroupingDocumentTabs(uiapp);
            }

            settings.ColorizeDocTabs = DocumentTabEventUtils.IsUpdatingDocumentTabs;
            Settings1.Default.Save();

            return Result.Succeeded;
        }
    }
}
