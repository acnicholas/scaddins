// (C) Copyright 2019-2020 by Andrew Nicholas
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

namespace SCaddins.HatchEditor
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
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
                foreach (var element in f)
                {
                    var e = (FillPatternElement)element;
                    var h = new Hatch(e.GetFillPattern());
                    result.Add(h);
                }
            }
            result = result.OrderBy(l => l.Name).ToList();
            return result;
        }

        public static bool SaveToFile(string filePath, Hatch hatch)
        {
            try
            {
                File.WriteAllText(filePath, hatch.PatFileString, Encoding.ASCII);
            }
            catch
            {
                //// add proper exceptions here...
            }
            return true;
        }

        public static bool SaveToModel(Document doc, FillPattern pattern)
        {
            using (Transaction t = new Transaction(doc))
            {
                if (t.Start("Save pattern to Model") == TransactionStatus.Started)
                {
                    FillPatternElement.Create(doc, pattern);
                    t.Commit();
                }
                else
                {
                    t.RollBack();
                }
            }
            return true;
        }

        public static List<Hatch> ReadAllPatternsFromFile(string file)
        {
            if (File.Exists(file))
            {
                var fileLines = File.ReadAllLines(file);
                return GetPatternFromFile(0, fileLines);
            }
            else
            {
                return new List<Hatch>();
            }
        }

        public static List<Hatch> GetPatternFromFile(int startIndex, string[] array)
        {
            var result = new List<Hatch>();
            for (int i = startIndex; i < array.Length - 2; i++)
            {
                if (array[i].Length > 1 && array[i].Trim().StartsWith(@"*", System.StringComparison.InvariantCulture))
                {
                    var name = array[i].Substring(1).Trim();
                    i++;
                    var type = array[i].Trim();
                    var defs = new StringBuilder();
                    do
                    {
                        i++;
                        if (!array[i].StartsWith(@";", System.StringComparison.InvariantCulture))
                        {
                            defs.Append(array[i].Trim());
                            defs.Append(System.Environment.NewLine);
                        }
                    } while (i < (array.Length - 1) && !array[i + 1].Trim().StartsWith(@"*", System.StringComparison.InvariantCulture));
                    var hatch = new Hatch
                    {
                        Name = name,
                        HatchPattern =
                        {
                            Target = type.ToUpper(System.Globalization.CultureInfo.InvariantCulture).Contains("DRAFTING") ? FillPatternTarget.Drafting : FillPatternTarget.Model
                        },
                        Definition = defs.ToString()
                    };
                    result.Add(hatch);
                }
            }
            return result;
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
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
            settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            var vm = new ViewModels.HatchEditorViewModel(udoc.Document);
            SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, settings);
            return Result.Succeeded;
        }
    }
}