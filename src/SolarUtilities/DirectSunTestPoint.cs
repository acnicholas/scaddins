using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;

namespace SCaddins.SolarUtilities
{
    public class DirectSunTestFace
    {
        private Face face;
        private Reference reference;
        private IList<UV> pointsUV;
        private IList<ValueAtPoint> valList;
        private string name;

        public DirectSunTestFace(Reference reference, string name, Document doc)
        {
            this.reference = reference;
            this.face = FaceFromReferences(reference, doc);
            pointsUV = new List<UV>();
            valList = new List<ValueAtPoint>();
            this.name = name;
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

        public FieldDomainPointsByUV Domain
        {
            get
            {
                return new FieldDomainPointsByUV(pointsUV);
            }
        }

        private static Face FaceFromReferences(Reference reference, Document doc)
        {
            Face f = (Face)doc.GetElement(reference).GetGeometryObjectFromReference(reference);
            return f;
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
            Document doc = uiDoc.Document;
            int idx = sfm.AddSpatialFieldPrimitive(Reference);
            FieldDomainPointsByUV pnts = new FieldDomainPointsByUV(pointsUV);
            FieldValues vals = new FieldValues(valList);
            AnalysisResultSchema resultSchema = new AnalysisResultSchema(name, name);
            int schemaIndex = sfm.RegisterResult(resultSchema);
            //if(pointsUV.Count != valList.Count) {
            //  return;
            //}
            try {
                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, schemaIndex);
            } catch { }
        }

        public static SpatialFieldManager GetSpatialFieldManager(Document doc)
        {
            SpatialFieldManager sfm = SpatialFieldManager.GetSpatialFieldManager(doc.ActiveView);
            if (sfm == null) {
                sfm = SpatialFieldManager.CreateSpatialFieldManager(doc.ActiveView, 1);
            }
            return sfm;
        }
    }
}
