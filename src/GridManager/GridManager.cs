// (C) Copyright 2024 by Andrew Nicholas
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


namespace SCaddins.GridManager
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class GridManager : IExternalCommand
    {
        public static int ShowBottomGridBubblesByView(View activeView, bool enable, List<ElementId> selection)
        {
            int result = 0;
            using (var transaction = new Transaction(activeView.Document, "Toggle Bottom Grids"))
            {
  
                transaction.Start();
                foreach (var grid in GetAllGridsInView(activeView, selection))
                {
                    // check if not a plan 
                    var viewDirection = activeView.ViewDirection;
                    if (viewDirection.Z == 0)
                    {
                        ToggleGridEnd(grid, enable, DatumEnds.End0, activeView);
                        result++;
                        continue;
                    }

                    var direction = GetGridDirection(grid); // get the direction of the current grid.
                    // check if it's within 45 degress(+-) of the left of the screen
                    var upwardDirection = activeView.UpDirection.Negate();
                    var crossProduct = direction.DotProduct(upwardDirection);

                    // .707 is 45 degress
                    if (Math.Abs(crossProduct) >= 0.707)
                    {
                        var datumEnd = crossProduct < 0 ? DatumEnds.End1 : DatumEnds.End0;
                        ToggleGridEnd(grid, enable, datumEnd, activeView);
                        result++;
                    }
                }
                transaction.Commit();
            }
            return result;
        }

        public static string GetViewInformation(View activeView, List<ElementId> selection)
        {
            var grids = GetAllGridsInView(activeView, selection);
            var levels = GetAllLevelsInView(activeView, selection);
            var s = (selection != null && selection.Count > 0) ? "selection" : "view";
            return "Grids in " + s + ": " + grids.Count + "; Levels in " + s + ": " + levels.Count;
        }

        public static int ShowLeftGridBubblesByView(View activeView, bool enable, List<ElementId> selection)
        {
            int result = 0;
            using (var transaction = new Transaction(activeView.Document, "Toggle Left Grids"))
            {
                transaction.Start();
                foreach (var grid in GetAllGridsInView(activeView, selection))
                {
                    var direction = GetGridDirection(grid); // get the direction of the current grid.
                    // check if it's within 45 degress(+-) of the left of the screen
                    var leftDirection = activeView.RightDirection.Negate();
                    var crossProduct = direction.DotProduct(leftDirection);

                    // .707 is 45 degress
                    if (Math.Abs(crossProduct) >= 0.707)
                    {
                        var datumEnd = crossProduct < 0 ? DatumEnds.End1 : DatumEnds.End0;
                        ToggleGridEnd(grid, enable, datumEnd, activeView);
                        result++;
                    }
                }
                transaction.Commit();
            }
            return result;
        }

        public static void ShowLeftLevelEndsByView(View activeView, bool enable, List<ElementId> selection)
        {
            using (var transaction = new Transaction(activeView.Document, "Toggle Left Level Ends"))
            {
                transaction.Start();
                foreach (var level in GetAllLevelsInView(activeView, selection))
                {
                    {
                        var levelCurves = level.GetCurvesInView(DatumExtentType.Model, activeView);
                        var ep = levelCurves.First().GetEndPoint(1);
                        var sp = levelCurves.First().GetEndPoint(0);
                        if (ep.X < sp.X || ep.Y < sp.Y)
                        {
                            ToggleLevelEnd(level, enable, DatumEnds.End1, activeView);
                        }
                        else
                        {
                            ToggleLevelEnd(level, enable, DatumEnds.End0, activeView);
                        }
                    }
                }
                transaction.Commit();
            }
        }

        public static int ShowRightGridBubblesByView(View activeView, bool enable, List<ElementId> selection)
        {
            int result = 0;
            using (var transaction = new Transaction(activeView.Document, "Toggle Right Grids"))
            {
                transaction.Start();
                foreach (var grid in GetAllGridsInView(activeView, selection))
                {
                    var direction = GetGridDirection(grid); // get the direction of the current grid.

                    // check if it's within 45 degress(+-) of the left of the screen
                    var rightDirection = activeView.RightDirection;
                    var crossProduct = direction.DotProduct(rightDirection);

                    // .707 is 45 degress
                    if (Math.Abs(crossProduct) >= 0.707)
                    {
                        var datumEnd = crossProduct < 0 ? DatumEnds.End1 : DatumEnds.End0;
                        ToggleGridEnd(grid, enable, datumEnd, activeView);
                        result++;
                    }
                }
                transaction.Commit();
            }
            return result;
        }

        public static int ShowRightLevelEndsByView(View activeView, bool enable, List<ElementId> selection)
        {
            int result = 0;
            using (var transaction = new Transaction(activeView.Document, "Toggle Right Level Ends"))
            {
                transaction.Start();
                foreach (var level in GetAllLevelsInView(activeView, selection))
                {
                    {
                        var levelCurves = level.GetCurvesInView(DatumExtentType.Model, activeView);
                        var ep = levelCurves.First().GetEndPoint(1);
                        var sp = levelCurves.First().GetEndPoint(0);
                        if (ep.X < sp.X || ep.Y < sp.Y)
                        {
                            ToggleLevelEnd(level, enable, DatumEnds.End0, activeView);
                            result++;
                        }
                        else
                        {
                            ToggleLevelEnd(level, enable, DatumEnds.End1, activeView);
                            result++;
                        }
                    }
                }
                transaction.Commit();
            }
            return result;
        }

        public static int ShowTopGridBubblesByView(View activeView, bool enable, List<ElementId> selection)
        {
            int result = 0;
            using (var transaction = new Transaction(activeView.Document, "Toggle Top Grids"))
            {
                transaction.Start();
                foreach (var grid in GetAllGridsInView(activeView, selection))
                {
                    // check if not a plan 
                    var viewDirection = activeView.ViewDirection;
                    if (viewDirection.Z == 0)
                    {
                        ToggleGridEnd(grid, enable, DatumEnds.End1, activeView);
                        result++;
                        continue;
                    }

                    var direction = GetGridDirection(grid); // get the direction of the current grid.

                    // check if it's within 45 degress(+-) of the left of the screen
                    var upwardDirection = activeView.UpDirection;
                    var crossProduct = direction.DotProduct(upwardDirection);

                    // .707 is 45 degress
                    if (Math.Abs(crossProduct) >= 0.707)
                    {
                        var datumEnd = crossProduct < 0 ? DatumEnds.End1 : DatumEnds.End0;
                        ToggleGridEnd(grid, enable, datumEnd, activeView);
                        result++;
                    }
                }
                transaction.Commit();
            }
            return result;
        }

        public static void Toggle2dGridsByView(View activeView, bool make2d, List<ElementId> selection)
        {
            using (var transaction = new Transaction(activeView.Document, "Make Grids 2d"))
            {
                transaction.Start();
                var datumExtendType = make2d ? DatumExtentType.ViewSpecific : DatumExtentType.Model;
                foreach (var grid in GetAllGridsInView(activeView, selection))
                {
                    grid.SetDatumExtentType(DatumEnds.End0, activeView, datumExtendType);
                    grid.SetDatumExtentType(DatumEnds.End1, activeView, datumExtendType);
                }

                transaction.Commit();
            }
        }

        public static void Toggle2dLevelsByView(View activeView, bool make2d, List<ElementId> selection)
        {
            using (var transaction = new Transaction(activeView.Document, "Make Levels 2d"))
            {
                transaction.Start();
                var datumExtendType = make2d ? DatumExtentType.ViewSpecific : DatumExtentType.Model;
                foreach (var level in GetAllLevelsInView(activeView, selection))
                {
                    level.SetDatumExtentType(DatumEnds.End0, activeView, datumExtendType);
                    level.SetDatumExtentType(DatumEnds.End1, activeView, datumExtendType);
                }

                transaction.Commit();
            }
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
            settings.Width = 640;
            settings.Title = "Grid and Level Tools - By Andrew Nicholas";
            settings.ShowInTaskbar = false;
            settings.SizeToContent = System.Windows.SizeToContent.Manual;
            settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            var selection = udoc.Selection.GetElementIds().ToList();
            var vm = new ViewModels.GridManagerViewModel(udoc.ActiveView, selection);
            SCaddinsApp.WindowManager.ShowDialogAsync(vm, null, settings);
            return Result.Succeeded;
        }

        private static List<Grid> GetAllGridsInSelection(View activeView, List<ElementId> selection)
        {
            var grids = new List<Grid>();
            using (var fec = new FilteredElementCollector(activeView.Document, selection))
            {
                fec.OfCategory(BuiltInCategory.OST_Grids);
                foreach (var element in fec)
                {
                    var grid = element as Grid;
                    grids.Add(grid);
                }
            }
            return grids;
        }

        public static List<Grid> GetAllGridsInView(View activeView, List<ElementId> selection)
        {
            if (selection != null && selection.Count > 0)
            {
                return GetAllGridsInSelection(activeView, selection);
            }

            var grids = new List<Grid>();
            using (var fec = new FilteredElementCollector(activeView.Document, activeView.Id))
            {
                fec.OfCategory(BuiltInCategory.OST_Grids);
                foreach (var element in fec)
                {
                    var grid = element as Grid;
                    grids.Add(grid);
                }
            }
            return grids;
        }

        private static List<Level> GetAllLevelsInSelection(View activeView, List<ElementId> selection)
        {
            var levels = new List<Level>();
            using (var fec = new FilteredElementCollector(activeView.Document, selection))
            {
                fec.OfCategory(BuiltInCategory.OST_Levels);
                foreach (var element in fec)
                {
                    var level = element as Level;
                    levels.Add(level);
                }
            }
            return levels;
        }

        public static List<Level> GetAllLevelsInView(View activeView, List<ElementId> selection)
        {
            if (selection != null && selection.Count > 0)
            {
                return GetAllLevelsInSelection(activeView, selection);
            }

            var levels = new List<Level>();
            using (var fec = new FilteredElementCollector(activeView.Document, activeView.Id))
            {
                fec.OfCategory(BuiltInCategory.OST_Levels);
                foreach (var element in fec)
                {
                    var level = element as Level;
                    levels.Add(level);
                }
            }
            return levels;
        }

        private static XYZ GetGridDirection(Grid grid)
        {
            var curve = grid.Curve;
            var direction = new XYZ();
            if (curve is Line)
            {
                direction = ((Line)curve).Direction;
            }

            return direction;
        }

        private static void ToggleGridEnd(Grid grid, bool show, DatumEnds end, View activeView)
        {
            if (show)
            {
                grid.ShowBubbleInView(end, activeView);
            }
            else
            {
                grid.HideBubbleInView(end, activeView);
            }
        }

        private static void ToggleLevelEnd(Level level, bool show, DatumEnds end, View activeView)
        {
            if (show)
            {
                level.ShowBubbleInView(end, activeView);
            }
            else
            {
                level.HideBubbleInView(end, activeView);
            }
        }
    }
}
