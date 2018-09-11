//
// Util.cs - The Building Coder Revit API utility methods
//
// Copyright (C) 2008-2018 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

namespace BuildingCoder
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Autodesk.Revit.DB;

    public static class JtEdgeArrayExtensionMethods
    {
        /// <summary>
        /// Return a polygon as a list of XYZ points from
        /// an EdgeArray. If any of the edges are curved,
        /// we retrieve the tessellated points, i.e. an
        /// approximation determined by Revit.
        /// </summary>
        public static List<XYZ> GetPolygon(
          this EdgeArray ea)
        {
            int n = ea.Size;

            List<XYZ> polygon = new List<XYZ>(n);

            foreach (Edge e in ea) {
                IList<XYZ> pts = e.Tessellate();

                n = polygon.Count;

                if (n > 0) {
                    Debug.Assert(pts[0].IsAlmostEqualTo(polygon[n - 1]), "expected last edge end point to " + "equal next edge start point");
                    polygon.RemoveAt(n - 1);
                }
                polygon.AddRange(pts);
            }
            n = polygon.Count;
            Debug.Assert(polygon[0].IsAlmostEqualTo(polygon[n - 1]), "expected first edge start point to " + "equal last edge end point");
            polygon.RemoveAt(n - 1);
            return polygon;
        }
    }
}