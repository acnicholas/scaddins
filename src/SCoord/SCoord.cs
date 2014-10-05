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
            this.PlaceMGA(doc);
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        private XYZ ToMGA(ProjectPosition projectPosition, double x, double y, double z)
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

        private void PlaceMGA(Document doc)
        {
            Level levelZero = this.GetLevelZero(doc);
            FamilySymbol family = this.GetSpotCoordFamily(doc);
            if (levelZero == null || family == null) {
                return;
            }

            ProjectLocation currentLocation = doc.ActiveProjectLocation;
            XYZ origin = new XYZ(0, 0, 0);
            ProjectPosition projectPosition = currentLocation.get_ProjectPosition(origin);

            SCoordForm form = new SCoordForm();
            System.Windows.Forms.DialogResult r = form.ShowDialog();

            if (r == System.Windows.Forms.DialogResult.Cancel) {
                return;
            }

            double x = Convert.ToDouble(form.textBoxEW.Text);
            double y = Convert.ToDouble(form.textBoxNS.Text);
            double z = Convert.ToDouble(form.textBoxElev.Text);
            XYZ newLocation = this.ToMGA(projectPosition, x, y, z);

            Transaction t = new Transaction(doc, "Place SCoord");
            t.Start();
            FamilyInstance fi = doc.Create.NewFamilyInstance(
                                    newLocation,
                                    family,
                                    levelZero,
                                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            #if REVIT2014
            Parameter p = fi.get_Parameter("Z");
            #else
            Parameter p = fi.LookupParameter("Z");
            #endif
            p.Set(newLocation.Z);
            t.Commit();
        }

        private FamilySymbol GetSpotCoordFamily(Document doc)
        {
            var collector1 = new FilteredElementCollector(doc);
            collector1.OfCategory(BuiltInCategory.OST_GenericModel);
            collector1.OfClass(typeof(FamilySymbol));
            foreach (FamilySymbol f in collector1) {
                if (f.Name.ToUpper().Contains("SC-Survey_Point".ToUpper())) {
                    return f;
                }
            }
            string version = doc.Application.VersionNumber;
            string family = SCaddins.Constants.FamilyDir +
                            version + @"\SC-Survey_Point.rfa";
            if (System.IO.File.Exists(family)) {
                Transaction loadFamily = new Transaction(doc, "Load Family");
                loadFamily.Start();
                Family fam;
                doc.LoadFamily(family, out fam);
                loadFamily.Commit();
                #if REVIT2014
                foreach (FamilySymbol f in fam.Symbols) {
                    if (f.Name.ToUpper().Contains("SC-Survey_Point".ToUpper())) {
                        return f;
                    }
                }
                #else
                System.Collections.Generic.ISet<ElementId> sids = fam.GetFamilySymbolIds();
                foreach (ElementId id in sids) {   
                    FamilySymbol f = doc.GetElement(id) as FamilySymbol;
                    if (f.Name.ToUpper().Contains("SC-Survey_Point".ToUpper())) {
                        return f;
                    }
                }
                #endif
            }
            TaskDialog.Show("SCoord", "Family SC-Survey_Point not found.");
            return null;
        }

        private Level GetLevelZero(Document doc)
        {
            FilteredElementCollector collector1 = new FilteredElementCollector(doc);
            collector1.OfClass(typeof(Level));
            foreach (Level l in collector1) {
                if (l.Name.ToUpper().Contains("SEA")) {
                    return l;
                }
                if (l.Name.ToUpper().Contains("ZERO")) {
                    return l;
                }
            }
            TaskDialog.Show("SCoord", "Sea level not found.");
            return null;
        }
    }
}
/* vim: set ts=4 sw=4 nu expandtab: */
