// (C) Copyright 2018 by Andrew Nicholas
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

namespace SCaddins.SolarAnalysis
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Analysis;
    using Autodesk.Revit.UI;

    public class DirectSunTestFace
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "face")]
        private Face face;
        private string name;
        private IList<UV> pointsUV;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Parameter initialized by Revit", MessageId = "reference")]
        private Reference reference;
        private IList<ValueAtPoint> valList;

        public DirectSunTestFace(Reference reference, string name, Document doc)
        {
            this.reference = reference;
            face = FaceFromReferences(reference, doc);
            pointsUV = new List<UV>();
            valList = new List<ValueAtPoint>();
            this.name = name;
        }

        public FieldDomainPointsByUV Domain
        {
            get
            {
                return new FieldDomainPointsByUV(pointsUV);
            }
        }

        public Face Face
        {
            get
            {
                return face;
            }
        }

        public Reference Reference
        {
            get
            {
                return reference;
            }
        }

        public static SpatialFieldManager GetSpatialFieldManager(Document doc)
        {
            SpatialFieldManager sfm = SpatialFieldManager.GetSpatialFieldManager(doc.ActiveView);
            if (sfm == null) {
                sfm = SpatialFieldManager.CreateSpatialFieldManager(doc.ActiveView, 1);
            }
            return sfm;
        }

        public void AddValueAtPoint(UV uv, double value)
        {
            pointsUV.Add(uv);
            List<double> doubleList = new List<double>();
            doubleList.Add(value);
            valList.Add(new ValueAtPoint(doubleList));
        }

        public void CreateAnalysisSurface(UIDocument uiDoc, SpatialFieldManager sfm)
        {
            var idx = sfm.AddSpatialFieldPrimitive(Reference);
            var pnts = new FieldDomainPointsByUV(pointsUV);
            var vals = new FieldValues(valList);
            var resultSchema = new AnalysisResultSchema(name, name);
            var schemaIndex = sfm.RegisterResult(resultSchema);
            try {
                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, schemaIndex);
            } catch {
                ////FIXME. don't catch nothing...
            }
        }

        private static Face FaceFromReferences(Reference reference, Document doc)
        {
            Face f = (Face)doc.GetElement(reference).GetGeometryObjectFromReference(reference);
            return f;
        }
    }
}
