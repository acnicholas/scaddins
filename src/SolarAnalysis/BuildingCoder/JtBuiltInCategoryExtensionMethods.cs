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

    public static class JtBuiltInCategoryExtensionMethods
    {
        /// <summary>
        /// Return a descriptive string for a built-in
        /// category by removing the trailing plural 's'
        /// and the OST_ prefix.
        /// </summary>
        public static string Description(
          this BuiltInCategory bic)
        {
            string s = bic.ToString().ToLower();
            s = s.Substring(4);
            Debug.Assert(s.EndsWith("s"), "expected plural suffix 's'");
            s = s.Substring(0, s.Length - 1);
            return s;
        }
    }
}