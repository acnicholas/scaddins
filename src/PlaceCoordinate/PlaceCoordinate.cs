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

namespace SCaddins.SCoord
{
    using System;
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
            UIDocument udoc = commandData.Application.ActiveUIDocument;
            Document doc = udoc.Document;
            PlaceMGA(doc);
            return Autodesk.Revit.UI.Result.Succeeded;
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
            return new XYZ(nx, ny, z / FeetToInches);
        }

        private static FamilySymbol GetSpotCoordFamily(Document doc)
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
                var loadFamily = new Transaction(doc, "Load Family");
                loadFamily.Start();
                Family fam;
                doc.LoadFamily(family, out fam);
                loadFamily.Commit();
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

        private static Level GetLevelZero(Document doc)
        {
            var collector1 = new FilteredElementCollector(doc);
            collector1.OfClass(typeof(Level));
            foreach (Level l in collector1) {
                if (l.Name.ToUpper(CultureInfo.CurrentCulture).Contains("SEA")) {
                    return l;
                }
                if (l.Name.ToUpper(CultureInfo.CurrentCulture).Contains("ZERO")) {
                    return l;
                }
            }
            TaskDialog.Show("SCoord", "Sea level not found.");
            return null;
        }

        private static void PlaceMGA(Document doc)
        {
            Level levelZero = GetLevelZero(doc);
            FamilySymbol family = GetSpotCoordFamily(doc);
            if (levelZero == null || family == null) {
                return;
            }

            ProjectLocation currentLocation = doc.ActiveProjectLocation;
            var origin = new XYZ(0, 0, 0);
            #if REVIT2018
            ProjectPosition projectPosition = currentLocation.GetProjectPosition(origin);
            #else
            ProjectPosition projectPosition = currentLocation.get_ProjectPosition(origin);
            #endif

            var form = new SCoordForm();
            System.Windows.Forms.DialogResult r = form.ShowDialog();

            if (r == System.Windows.Forms.DialogResult.Cancel) {
                return;
            }

            double x = Convert.ToDouble(form.textBoxEW.Text, CultureInfo.CurrentCulture);
            double y = Convert.ToDouble(form.textBoxNS.Text, CultureInfo.CurrentCulture);
            double z = Convert.ToDouble(form.textBoxElevation.Text, CultureInfo.CurrentCulture);
            XYZ newLocation = ToMGA(projectPosition, x, y, z);

            var t = new Transaction(doc, "Place SCoord");
            t.Start();
            FamilyInstance fi = doc.Create.NewFamilyInstance(
                                    newLocation,
                                    family,
                                    levelZero,
                                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            Parameter p = fi.LookupParameter("Z");
            p.Set(newLocation.Z);
            t.Commit();
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
