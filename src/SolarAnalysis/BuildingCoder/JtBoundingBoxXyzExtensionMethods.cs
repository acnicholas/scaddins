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
    using System;
    using Autodesk.Revit.DB;

    public static class JtBoundingBoxXyzExtensionMethods
    {
        /// <summary>
        /// Expand the given bounding box to include
        /// and contain the given point.
        /// </summary>
        public static void ExpandToContain(
          this BoundingBoxXYZ bb,
          XYZ p)
        {
            bb.Min = new XYZ(Math.Min(bb.Min.X, p.X), Math.Min(bb.Min.Y, p.Y), Math.Min(bb.Min.Z, p.Z));
            bb.Max = new XYZ(Math.Max(bb.Max.X, p.X), Math.Max(bb.Max.Y, p.Y), Math.Max(bb.Max.Z, p.Z));
        }

        /// <summary>
        /// Expand the given bounding box to include
        /// and contain the given other one.
        /// </summary>
        public static void ExpandToContain(this BoundingBoxXYZ bb, BoundingBoxXYZ other)
        {
            bb.ExpandToContain(other.Min);
            bb.ExpandToContain(other.Max);
        }
    }
}