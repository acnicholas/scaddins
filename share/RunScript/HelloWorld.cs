using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

public static void Main(Document doc)
{
    TaskDialog.Show("Message","Hello World from Revit!");
    SCaddins.SCaddinsApp.WindowManager.ShowMessageBox("Hello World from SCaddins!");
}
