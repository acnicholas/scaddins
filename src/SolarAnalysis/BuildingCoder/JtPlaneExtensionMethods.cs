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
    using System.Diagnostics;
    using Autodesk.Revit.DB;

    public static class JtPlaneExtensionMethods
    {
        /// <summary>
        /// Project given 3D XYZ point into plane,
        /// returning the UV coordinates of the result
        /// in the local 2D plane coordinate system.
        /// </summary>
        public static UV ProjectInto(
          this Plane plane,
          XYZ p)
        {
            XYZ q = plane.ProjectOnto(p);
            XYZ o = plane.Origin;
            XYZ d = q - o;
            double u = d.DotProduct(plane.XVec);
            double v = d.DotProduct(plane.YVec);
            return new UV(u, v);
        }

        /// <summary>
        /// Project given 3D XYZ point onto plane.
        /// </summary>
        public static XYZ ProjectOnto(
          this Plane plane,
          XYZ p)
        {
            double d = plane.SignedDistanceTo(p);

            ////XYZ q = p + d * plane.Normal; // wrong according to Ruslan Hanza and Alexander Pekshev in their comments http://thebuildingcoder.typepad.com/blog/2014/09/planes-projections-and-picking-points.html#comment-3765750464
            XYZ q = p - (d * plane.Normal);

            Debug.Assert(
              Util.IsZero(plane.SignedDistanceTo(q)),
              "expected point on plane to have zero distance to plane");

            return q;
        }

        /// <summary>
        /// Return the signed distance from
        /// a plane to a given point.
        /// </summary>
        public static double SignedDistanceTo(
          this Plane plane,
          XYZ p)
        {
            Debug.Assert(
              Util.IsEqual(plane.Normal.GetLength(), 1),
              "expected normalised plane normal");

            XYZ v = p - plane.Origin;

            return plane.Normal.DotProduct(v);
        }
    }
}