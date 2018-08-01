// (C) Copyright 2014 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCaddins.
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

namespace SCaddins.PlaceCoordinate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Globalization;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private const double FeetToInches = 304.8;

        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData,
            ref string message,
            Autodesk.Revit.DB.ElementSet elements)
        {
            if (commandData == null) {
                return Result.Failed;
            }
            UIDocument udoc = commandData.Application.ActiveUIDocument;
            Document doc = udoc.Document;
            //PlaceMGA(doc,);

            var vm = new ViewModels.PlaceCoordinateViewModel(doc);
            SCaddinsApp.WindowManager.ShowDialog(vm, null, PlaceCoordinate.ViewModels.PlaceCoordinateViewModel.DefaultWindowSettings);

            return Result.Succeeded;
        }

        private static XYZ ToMGA(ProjectPosition projectPosition, double x, double y, double z)
        {
            double xp, yp;
            double ang = projectPosition.Angle;
            double nx, ny;
            xp = (x / FeetToInches) - projectPosition.EastWest;
            yp = (y / FeetToInches) - projectPosition.NorthSouth;
            nx = (xp * Math.Cos(-ang)) - (yp * Math.Sin(-ang));
            ny = (xp * Math.Sin(-ang)) + (yp * Math.Cos(-ang));
            return new XYZ(nx, ny, projectPosition.Elevation / FeetToInches);
        }

        public static List<FamilySymbol> GetAllFamilySymbols(Document doc)
        {
            List<FamilySymbol> result = new List<FamilySymbol>();
            using (var collector = new FilteredElementCollector(doc)) {
                collector.OfCategory(BuiltInCategory.OST_GenericModel);
                collector.OfClass(typeof(FamilySymbol));
                foreach(FamilySymbol fs in collector) {
                    result.Add(fs);
                }
            }
            return result;
        }

        public static FamilySymbol GetSpotCoordFamily(Document doc)
        {
            using (var collector = new FilteredElementCollector(doc)) {
                collector.OfCategory(BuiltInCategory.OST_GenericModel);
                collector.OfClass(typeof(FamilySymbol));
                foreach (FamilySymbol f in collector) {
                    if (f.Name.ToUpper(CultureInfo.InvariantCulture).Contains("SC-Survey_Point".ToUpper(CultureInfo.InvariantCulture))) {
                        return f;
                    }
                }
            }
            string version = doc.Application.VersionNumber;
            string family = SCaddins.Constants.FamilyDirectory +
                            version + @"\SC-Survey_Point.rfa";
            if (System.IO.File.Exists(family)) {
                Family fam;
                using (var loadFamily = new Transaction(doc, "Load Family")) {
                    loadFamily.Start();
                    doc.LoadFamily(family, out fam);
                    loadFamily.Commit();
                }
                System.Collections.Generic.ISet<ElementId> sids = fam.GetFamilySymbolIds();
                foreach (ElementId id in sids) {   
                    var f = doc.GetElement(id) as FamilySymbol;
                    if (f.Name.ToUpper(CultureInfo.InvariantCulture).Contains("SC-Survey_Point".ToUpper(CultureInfo.InvariantCulture))) {
                        return f;
                    }
                }
            }
            TaskDialog.Show("SCoord", "Family SC-Survey_Point not found.");
            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void PlaceMGA(Document doc, FamilySymbol family, XYZ location)
        {
            if (doc == null || family == null) {
                return;
            }

            ProjectLocation currentLocation = doc.ActiveProjectLocation;
            var origin = new XYZ(0, 0, 0);
            #if REVIT2018 || REVIT2019
            ProjectPosition projectPosition = currentLocation.GetProjectPosition(origin);
            #else
            ProjectPosition projectPosition = currentLocation.get_ProjectPosition(origin);
            #endif

            XYZ newLocation = ToMGA(projectPosition, location.X, location.Y, location.Z);

            using (var t = new Transaction(doc, "Place Family at Coordinate.")) {
                if (t.Start() == TransactionStatus.Started) {
                    FamilyInstance fi = doc.Create.NewFamilyInstance(
                                            newLocation,
                                            family,
                                            Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    t.Commit();
                }
            }
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
