using System.Collections;
using Autodesk.Revit.UI;

namespace SCaddins.ExportManager.ViewModels
{
    public class Controller
    {
        public void ShowCopySheets(UIDocument uidoc, IList sheets)
        {
            var manager = new SCaddins.SheetCopier.SheetCopierManager(uidoc);
            var window = new SCaddins.SheetCopier.MainForm(uidoc.Document, sheets, manager);
            window.Show();
        }

        public void ShowRenamer(UIDocument uidoc, IList sheets)
        {
            var manager = new SCaddins.SheetCopier.SheetCopierManager(uidoc);
            var window = new SCaddins.SheetCopier.MainForm(uidoc.Document, sheets, manager);
            window.ShowDialog();
        }
    }
}