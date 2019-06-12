namespace SCaddins.HatchEditor
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Text;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public static List<Hatch> FillPatterns(Document doc)
        {
            var result = new List<Hatch>();
            using (var f = new FilteredElementCollector(doc))
            {
                f.OfClass(typeof(FillPatternElement));
                foreach (FillPatternElement e in f)
                {
                    var h = new Hatch(e.GetFillPattern());
                    result.Add(h);
                }
            }
            return result;
        }

        public static bool SaveToFile(string filePath, Hatch hatch)
        {
            try {
                File.WriteAllText(filePath, hatch.PatFileString);
            } catch {
                //// add proper exceptions here...
            }
            return true;
        }

        public static bool SaveToModel(Document doc, FillPattern pattern)
        {
            using (Transaction t = new Transaction(doc)) {
                if (t.Start("Save pattern to Model") == TransactionStatus.Started) {
                    FillPatternElement.Create(doc, pattern);
                    t.Commit();
                } else {
                    t.RollBack();
                }
            }
            return true;
        }

        public static List<Hatch> ReadAllPatternsFromFile(string file)
        {
            if (System.IO.File.Exists(file)) {
                var fileLines = System.IO.File.ReadAllLines(file);
                return GetPatternFromFile(0, fileLines);
            } else {
                return new List<Hatch>();
            }
        }

        public static List<Hatch> GetPatternFromFile(int startIndex, string[] array)
        {
            var result = new List<Hatch>();
            for (int i = startIndex; i < array.Length - 2; i++)
            {
                if (array[i].Trim().StartsWith(@"*", System.StringComparison.InvariantCulture)) {
                    var name = array[i].Trim();
                    i++;
                    var type = array[i].Trim();
                    var defs = new StringBuilder();
                    do
                    {
                        i++;
                        if (!array[i].StartsWith(@";", System.StringComparison.InvariantCulture)) {
                            defs.Append(array[i].Trim());
                            defs.Append(System.Environment.NewLine);
                        }
                    } while (i < (array.Length - 1) && !array[i + 1].Trim().StartsWith(@"*", System.StringComparison.InvariantCulture));
                    var hatch = new Hatch();
                    hatch.Name = name;
                    hatch.HatchPattern.Target = type.ToUpper(System.Globalization.CultureInfo.InvariantCulture).Contains("DRAFTING") ? FillPatternTarget.Drafting : FillPatternTarget.Model;
                    hatch.Definition = defs.ToString();
                    result.Add(hatch);
                }
            }
            return result;
        }

        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            if (commandData == null)
            {
                return Result.Failed;
            }

            UIDocument udoc = commandData.Application.ActiveUIDocument;

            dynamic settings = new ExpandoObject();
            settings.Height = 480;
            settings.Width = 768;
            settings.Title = "Hatch Editor - By Andrew Nicholas";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;

            var vm = new ViewModels.HatchEditorViewModel(udoc.Document);
            SCaddinsApp.WindowManager.ShowDialog(vm, null, settings);
            return Result.Succeeded;
        }
    }
}