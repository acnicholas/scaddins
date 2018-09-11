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
    using System.Linq;
    using Autodesk.Revit.DB;

    public static class JtFilteredElementCollectorExtensions
    {
        public static FilteredElementCollector OfClass<T>(
          this FilteredElementCollector collector)
            where T : Element
        {
            return collector.OfClass(typeof(T));
        }

        public static IEnumerable<T> OfType<T>(
          this FilteredElementCollector collector)
            where T : Element
        {
            return Enumerable.OfType<T>(
              collector.OfClass<T>());
        }
    }
}